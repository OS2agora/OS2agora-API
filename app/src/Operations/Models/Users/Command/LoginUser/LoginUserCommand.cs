using Agora.Models.Models;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models.Cpr;
using Agora.Operations.Authentication;
using Agora.Operations.Common.Interfaces.Cpr;
using Agora.Operations.Common.Interfaces.Cvr;

namespace Agora.Operations.Models.Users.Command.LoginUser
{
    public class LoginUserCommand : IRequest<User>
    {
        public TokenUser TokenUser { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsHearingCreator { get; set; }


        public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, User>
        {
            private readonly IUserDao _userDao;
            private readonly IUserCapacityDao _userCapacityDao;
            private readonly ICompanyDao _companyDao;
            private readonly ICprInformationService _cprInformationService;
            private readonly ICvrInformationService _cvrInformationService;

            public LoginUserCommandHandler(IUserDao userDao, IUserCapacityDao userCapacityDao, ICompanyDao companyDao, ICprInformationService cprInformationService, ICvrInformationService cvrInformationService)
            {
                _userDao = userDao;
                _userCapacityDao = userCapacityDao;
                _companyDao = companyDao;
                _cprInformationService = cprInformationService;
                _cvrInformationService = cvrInformationService;
            }

            public async Task<User> Handle(LoginUserCommand request, CancellationToken cancellationToken)
            {
                var defaultIncludes = IncludeProperties.Create<User>();
                var tokenUser = request.TokenUser;
                var user = await _userDao.FindUserByIdentifier(tokenUser.ApplicationUserId, defaultIncludes);

                if (user == null)
                {
                    // We might have a half-created user from an invitation that we need to populate
                    User possibleUser = null;
                    if (tokenUser.AuthMethod == AuthenticationMethod.AdfsEmployee && !string.IsNullOrEmpty(tokenUser.Email))
                    {
                        possibleUser = await _userDao.FindUserByEmail(tokenUser.Email, defaultIncludes);
                    }
                    else if (!string.IsNullOrEmpty(tokenUser.PersonalIdentifier))
                    {
                        possibleUser = await _userDao.FindUserByPersonalIdentifier(tokenUser.PersonalIdentifier, defaultIncludes);
                    }

                    if (possibleUser != null)
                    {
                        user = possibleUser;
                    }
                }

                var userCapacities = await _userCapacityDao.GetAllAsync();
                var correctUserCapacity = GetCorrectUserCapacity(tokenUser.AuthMethod, userCapacities);

                // Retrieve address information
                var addressInformation = await GetAddressInformation(tokenUser, correctUserCapacity);

                var newOrUpdatedUser = await GetUserObject(tokenUser, correctUserCapacity, request.IsAdministrator, request.IsHearingCreator, addressInformation);

                if (user == null)
                {
                    user = await _userDao.CreateAsync(newOrUpdatedUser);
                }
                else
                {
                    var updatedProperties = new List<string>
                    {
                        nameof(User.Identifier),
                        nameof(User.Name),
                        nameof(User.EmployeeDisplayName),
                        nameof(User.PersonalIdentifier),
                        nameof(User.Email),
                        nameof(User.Cpr),
                        nameof(User.Cvr),
                        nameof(User.CompanyId),
                        nameof(User.IsAdministrator),
                        nameof(User.IsHearingCreator),
                        nameof(User.UserCapacityId),
                        nameof(User.Address),
                        nameof(User.Municipality),
                        nameof(User.City),
                        nameof(User.PostalCode),
                        nameof(User.Country),
                        nameof(User.StreetName)
                    };
                    newOrUpdatedUser.PropertiesUpdated.AddRange(updatedProperties);
                    newOrUpdatedUser.Id = user.Id;
                    user = await _userDao.UpdateAsync(newOrUpdatedUser, defaultIncludes);
                }

                return user;
            }

            private async Task<AddressInformation> GetAddressInformation(TokenUser tokenUser, UserCapacity userCapacity)
            {
                if (userCapacity.Capacity == Agora.Models.Enums.UserCapacity.CITIZEN)
                {
                    var addressInformation = await _cprInformationService.GetAddressInformation(tokenUser.Cpr);
                    return addressInformation;
                }
                if (userCapacity.Capacity == Agora.Models.Enums.UserCapacity.COMPANY)
                {
                    var addressInformation = await _cvrInformationService.GetAddressInformation(tokenUser.Cvr);
                    return addressInformation;
                }

                return null;
            }

            private async Task<User> GetUserObject(TokenUser tokenUser, UserCapacity userCapacity, bool isAdmin, bool isHearingCreator, AddressInformation addressInformation)
            {

                var user = new User
                {
                    Identifier = tokenUser.ApplicationUserId,
                    Name = tokenUser.Name,
                    EmployeeDisplayName = null,
                    PersonalIdentifier = tokenUser.PersonalIdentifier,
                    Email = null,
                    Cpr = null,
                    Cvr = null,
                    CompanyId = null,
                    IsAdministrator = isAdmin,
                    IsHearingCreator = isHearingCreator,
                    CreatedBy = tokenUser.ApplicationUserId,
                    UserCapacityId = userCapacity.Id
                };

                switch (userCapacity.Capacity)
                {
                    case Agora.Models.Enums.UserCapacity.CITIZEN:
                        return GetCitizenUserObject(tokenUser, user, addressInformation);
                    case Agora.Models.Enums.UserCapacity.EMPLOYEE:
                        return GetEmployeeUserObject(tokenUser, user);
                    case Agora.Models.Enums.UserCapacity.COMPANY:
                        return await GetCompanyUserObject(tokenUser, user, addressInformation);
                    default:
                        throw new InvalidOperationException("Invalid UserCapacity was provided. Failed to login user.");
                }
            }

            private User GetCitizenUserObject(TokenUser tokenUser, User user, AddressInformation addressInformation)
            {
                user.Cpr = tokenUser.Cpr;

                if (addressInformation != null)
                {
                    user.PostalCode = addressInformation.PostalCode;
                    user.Address = addressInformation.Address;
                    user.City = addressInformation.City;
                    user.Municipality = addressInformation.Municipality;
                    user.Country = addressInformation.Country;
                    user.StreetName = addressInformation.StreetName;
                }
                return user;
            }

            private User GetEmployeeUserObject(TokenUser tokenUser, User user)
            {
                user.EmployeeDisplayName = tokenUser.EmployeeDisplayName;
                user.Email = tokenUser.Email;
                return user;
            }

            private async Task<User> GetCompanyUserObject(TokenUser tokenUser, User user, AddressInformation addressInformation)
            {
                var company = await _companyDao.GetCompanyByCvr(tokenUser.Cvr);

                if (company == null)
                {
                    company = await _companyDao.CreateAsync(new Company
                    {
                        Cvr = tokenUser.Cvr,
                        Name = tokenUser.CompanyName,
                        Address = addressInformation?.Address,
                        City = addressInformation?.City,
                        Municipality = addressInformation?.Municipality,
                        PostalCode = addressInformation?.PostalCode,
                        Country = addressInformation?.Country,
                        StreetName = addressInformation?.StreetName
                    });
                }
                else if (addressInformation != null && 
                         (company.Address != addressInformation.Address ||
                          company.City != addressInformation.City))
                {
                    company.Address = addressInformation.Address;
                    company.City = addressInformation.City;
                    company.PostalCode = addressInformation.PostalCode;
                    company.Country = addressInformation.Country;
                    company.Municipality = addressInformation.Municipality;
                    company.StreetName = addressInformation.StreetName;

                    company.PropertiesUpdated = new List<string>
                    {
                        nameof(Company.Address),
                        nameof(Company.City),
                        nameof(Company.Municipality),
                        nameof(Company.PostalCode),
                        nameof(Company.Country),
                        nameof(Company.StreetName)
                    };

                    company = await _companyDao.UpdateAsync(company);
                }

                user.Cpr = tokenUser.Cpr;
                user.Cvr = tokenUser.Cvr;
                user.CompanyId = company.Id;

                return user;
            }

            public UserCapacity GetCorrectUserCapacity(AuthenticationMethod authenticationMethod, List<UserCapacity> userCapacities)
            {
                switch (authenticationMethod)
                {
                    case AuthenticationMethod.MitIdCitizen:
                        return userCapacities.Single(userCapacity => userCapacity.Capacity == Agora.Models.Enums.UserCapacity.CITIZEN);
                    case AuthenticationMethod.AdfsEmployee:
                        return userCapacities.Single(userCapacity => userCapacity.Capacity == Agora.Models.Enums.UserCapacity.EMPLOYEE);
                    case AuthenticationMethod.MitIdErhverv:
                        return userCapacities.Single(userCapacity => userCapacity.Capacity == Agora.Models.Enums.UserCapacity.COMPANY);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(authenticationMethod), authenticationMethod, null);
                }
            }
        }
    }
}