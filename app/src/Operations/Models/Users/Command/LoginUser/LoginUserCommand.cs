using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Enums;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Authentication;
using InvalidOperationException = BallerupKommune.Operations.Common.Exceptions.InvalidOperationException;

namespace BallerupKommune.Operations.Models.Users.Command.LoginUser
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

            public LoginUserCommandHandler(IUserDao userDao, IUserCapacityDao userCapacityDao, ICompanyDao companyDao)
            {
                _userDao = userDao;
                _userCapacityDao = userCapacityDao;
                _companyDao = companyDao;
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
                    if (!string.IsNullOrEmpty(tokenUser.PersonalIdentifier))
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

                var newOrUpdatedUser = await GetUserObject(tokenUser, correctUserCapacity, request.IsAdministrator, request.IsHearingCreator);

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
                        nameof(User.UserCapacityId)
                    };
                    newOrUpdatedUser.PropertiesUpdated.AddRange(updatedProperties);
                    newOrUpdatedUser.Id = user.Id;
                    user = await _userDao.UpdateAsync(newOrUpdatedUser, defaultIncludes);
                }

                return user;
            }

            private async Task<User> GetUserObject(TokenUser tokenUser, UserCapacity userCapacity, bool isAdmin, bool isHearingCreator)
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
                    case BallerupKommune.Models.Enums.UserCapacity.CITIZEN:
                        return GetCitizenUserObject(tokenUser, user);
                    case BallerupKommune.Models.Enums.UserCapacity.EMPLOYEE:
                        return GetEmployeeUserObject(tokenUser, user);
                    case BallerupKommune.Models.Enums.UserCapacity.COMPANY:
                        return await GetCompanyUserObject(tokenUser, user);
                    default:
                        throw new InvalidOperationException("Invalid UserCapacity was provided. Failed to login user.");
                }
            }

            private User GetCitizenUserObject(TokenUser tokenUser, User user)
            {
                user.Cpr = tokenUser.Cpr;
                return user;
            }

            private User GetEmployeeUserObject(TokenUser tokenUser, User user)
            {
                user.EmployeeDisplayName = tokenUser.EmployeeDisplayName;
                user.Email = tokenUser.Email;
                return user;
            }

            private async Task<User> GetCompanyUserObject(TokenUser tokenUser, User user)
            {
                var company = await _companyDao.GetCompanyByCvr(tokenUser.Cvr);

                if (company == null)
                {
                    company = await _companyDao.CreateAsync(new Company
                    {
                        Cvr = tokenUser.Cvr,
                        Name = tokenUser.CompanyName
                    });
                }

                user.Cpr = tokenUser.Cpr;
                user.Cvr = tokenUser.Cvr;
                user.CompanyId = company.Id;

                return user;
            }


            private UserCapacity GetCorrectUserCapacity(AuthenticationMethod authenticationMethod, List<UserCapacity> userCapacities)
            {
                switch (authenticationMethod)
                {
                    case AuthenticationMethod.MitIdCitizen:
                        return userCapacities.Single(userCapacity => userCapacity.Capacity == BallerupKommune.Models.Enums.UserCapacity.CITIZEN);
                    case AuthenticationMethod.AdfsEmployee:
                        return userCapacities.Single(userCapacity => userCapacity.Capacity == BallerupKommune.Models.Enums.UserCapacity.EMPLOYEE);
                    case AuthenticationMethod.MitIdErhverv:
                        return userCapacities.Single(userCapacity => userCapacity.Capacity == BallerupKommune.Models.Enums.UserCapacity.COMPANY);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(authenticationMethod), authenticationMethod, null);
                }
            }
        }
    }
}