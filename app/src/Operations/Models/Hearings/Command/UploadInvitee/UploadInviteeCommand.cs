using BallerupKommune.Models.Models;
using BallerupKommune.Models.Models.Csv;
using BallerupKommune.Models.Models.Files;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Common.Interfaces.Plugins;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Common.Exceptions;
using HearingRole = BallerupKommune.Models.Enums.HearingRole;
using HearingStatus = BallerupKommune.Models.Enums.HearingStatus;
using UserCapacityEnum = BallerupKommune.Models.Enums.UserCapacity;

namespace BallerupKommune.Operations.Models.Hearings.Command.UploadInvitee
{
    public class UploadInviteeCommand : IRequest<Hearing>
    {
        public int Id { get; set; }
        public File File { get; set; }
        public List<string> RequestIncludes { get; set; }

        public class UploadInviteeCommandHandler : IRequestHandler<UploadInviteeCommand, Hearing>
        {
            private readonly ICsvService _csvService;
            private readonly IUserDao _userDao;
            private readonly ICompanyDao _companyDao;
            private readonly IHearingRoleDao _hearingRoleDao;
            private readonly IHearingDao _hearingDao;
            private readonly IUserHearingRoleDao _userHearingRoleDao;
            private readonly ICompanyHearingRoleDao _companyHearingRoleDao;
            private readonly IPluginService _pluginService;
            private readonly IUserCapacityDao _userCapacityDao;

            public UploadInviteeCommandHandler(
                ICsvService csvService, 
                IUserDao userDao, 
                ICompanyDao companyDao,
                IHearingRoleDao hearingRoleDao, 
                IHearingDao hearingDao, 
                IUserHearingRoleDao userHearingRoleDao, 
                IPluginService pluginService, 
                ICompanyHearingRoleDao companyHearingRoleDao,
                IUserCapacityDao userCapacityDao)
            {
                _csvService = csvService;
                _userDao = userDao;
                _companyDao = companyDao;
                _hearingRoleDao = hearingRoleDao;
                _hearingDao = hearingDao;
                _userHearingRoleDao = userHearingRoleDao;
                _pluginService = pluginService;
                _companyHearingRoleDao = companyHearingRoleDao;
                _userCapacityDao = userCapacityDao;
            }

            public async Task<Hearing> Handle(UploadInviteeCommand request, CancellationToken cancellationToken)
            {
                var hearingIncludes = IncludeProperties.Create<Hearing>(request.RequestIncludes, new List<string>
                {
                    nameof(Hearing.UserHearingRoles),
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.HearingRole)}",
                    nameof(Hearing.CompanyHearingRoles),
                    $"{nameof(Hearing.CompanyHearingRoles)}.{nameof(CompanyHearingRole.HearingRole)}"
                });
                Hearing hearing = await _hearingDao.GetAsync(request.Id, hearingIncludes);

                var currentUserInvitees = hearing.UserHearingRoles.Where(x => x.HearingRole.Role == HearingRole.HEARING_INVITEE).ToList();
                var currentCompanyInvitees = hearing.CompanyHearingRoles
                    .Where(c => c.HearingRole.Role == HearingRole.HEARING_INVITEE).ToList();

                request.File = await _pluginService.BeforeFileUpload(request.File);
                if (request.File.MarkedByScanner)
                {
                    throw new FileUploadException(request.File.Name);
                }

                var uploadedInviteeIdentifierList = await _csvService.ParseCsv<InviteeIdentifiers>(request.File);

                await ReplaceInvitees(hearing, currentUserInvitees, currentCompanyInvitees, uploadedInviteeIdentifierList);

                var result = await _hearingDao.GetAsync(request.Id, hearingIncludes);

                return result;
            }

            /// <summary>
            /// Creates new invitees based on the provided identifiers.
            /// If hearing is published, a notification will be created for the users invited.
            /// </summary>
            /// <param name="hearing">The hearing to which invitees are added.</param>
            /// <param name="currentUserInvitees">The current user invitees.</param>
            /// <param name="currentCompanyInvitees">The current company invitees.</param>
            /// <param name="uploadedInviteeIdentifierList">The uploaded invitee identifiers.</param>
            private async Task CreateInvitees(
                Hearing hearing,
                List<UserHearingRole> currentUserInvitees,
                List<CompanyHearingRole> currentCompanyInvitees,
                List<InviteeIdentifiers> uploadedInviteeIdentifierList)
            {
                List<InviteeIdentifiers> listOfInviteeIdentifiersToCreate = InviteesToCreate(currentUserInvitees, currentCompanyInvitees, uploadedInviteeIdentifierList);

                List<string> cvrIdentifiers = listOfInviteeIdentifiersToCreate.Where(inviteeIdentifier =>
                    !string.IsNullOrEmpty(inviteeIdentifier.CVR)).Select(identifier => identifier.CVR.ToLower()).Distinct().ToList();
                List<string> emailIdentifiers = listOfInviteeIdentifiersToCreate.Where(inviteeIdentifier =>
                    !string.IsNullOrEmpty(inviteeIdentifier.Email)).Select(identifier => identifier.Email.ToLower()).Distinct().ToList();
                List<string> cprIdentifiers = listOfInviteeIdentifiersToCreate.Where(inviteeIdentifier =>
                    !string.IsNullOrEmpty(inviteeIdentifier.CPR)).Select(identifier => identifier.CPR.ToLower()).Distinct().ToList();

                List<int> employeeIds = await GetOrCreateEmployees(emailIdentifiers);
                List<int> citizenIds = await GetOrCreateCitizens(cprIdentifiers);
                List<int> companyIds = await GetOrCreateCompanies(cvrIdentifiers);

                List<int> userIds = employeeIds.Concat(citizenIds).ToList();

                var hearingRoles = await _hearingRoleDao.GetAllAsync();
                var hearingInviteeRole = hearingRoles.Single(x => x.Role == HearingRole.HEARING_INVITEE);

                await CreateUserHearingRoles(hearingInviteeRole, userIds, hearing);
                await CreateCompanyHearingRoles(hearingInviteeRole, companyIds, hearing);

                var hearingIsPublished = hearing.HearingStatus.Status != HearingStatus.CREATED &&
                                         hearing.HearingStatus.Status != HearingStatus.DRAFT &&
                                         hearing.HearingStatus.Status != HearingStatus.AWAITING_STARTDATE;

                if (hearingIsPublished)
                {
                    await CreateNotifications(hearing.Id, userIds, companyIds);
                }
            }

            /// <summary>
            /// Deletes invitees based on the provided identifiers.
            /// </summary>
            /// <param name="currentUserInvitees">The current user invitees.</param>
            /// <param name="currentCompanyInvitees">The current company invitees.</param>
            /// <param name="uploadedInviteeIdentifierList">The uploaded invitee identifiers.</param>
            /// <param name="isReplacing">Indicates whether this is part of a replace operation.</param>
            private async Task DeleteInvitees(
                List<UserHearingRole> currentUserInvitees,
                List<CompanyHearingRole> currentCompanyInvitees,
                List<InviteeIdentifiers> uploadedInviteeIdentifierList,
                bool isReplacing = false)
            {
                var listOfUserHearingRolesToDelete = UserHearingRolesToDelete(currentUserInvitees, uploadedInviteeIdentifierList, isReplacing);
                var listOfCompanyHearingRolesToDelete =
                    CompanyHearingRolesToDelete(currentCompanyInvitees, uploadedInviteeIdentifierList, isReplacing);

                if (listOfUserHearingRolesToDelete.Any())
                {
                    await DeleteUserHearingRoles(listOfUserHearingRolesToDelete);
                }

                if (listOfCompanyHearingRolesToDelete.Any())
                {
                    await DeleteCompanyHearingRoles(listOfCompanyHearingRolesToDelete);
                }
            }

            /// <summary>
            /// Replaces the invitees for a hearing with the ones from the uploaded file.
            /// </summary>
            /// <param name="hearing">The hearing to update.</param>
            /// <param name="currentUserInvitees">The current user invitees in the hearing.</param>
            /// <param name="currentCompanyInvitees">The current company invitees in the hearing.</param>
            /// <param name="uploadedInviteeIdentifierList">The new list of invitee identifiers.</param>
            private async Task ReplaceInvitees(
                Hearing hearing,
                List<UserHearingRole> currentUserInvitees,
                List<CompanyHearingRole> currentCompanyInvitees,
                List<InviteeIdentifiers> uploadedInviteeIdentifierList)
            {
                await CreateInvitees(hearing, currentUserInvitees, currentCompanyInvitees, uploadedInviteeIdentifierList);
                await DeleteInvitees(currentUserInvitees, currentCompanyInvitees, uploadedInviteeIdentifierList, true);
            }

            /// <summary>
            /// Determines which invitees should be created based on current and uploaded invitee data.
            /// </summary>
            /// <param name="currentUserHearingRoles">The current user hearing roles.</param>
            /// <param name="currentCompanyHearingRoles">The current company hearing roles.</param>
            /// <param name="uploadedInviteeIdentifiersList">The uploaded list of invitee identifiers.</param>
            /// <returns>A list of invitee identifiers to create.</returns>
            private List<InviteeIdentifiers> InviteesToCreate(
                List<UserHearingRole> currentUserHearingRoles, 
                List<CompanyHearingRole> currentCompanyHearingRoles, 
                List<InviteeIdentifiers> uploadedInviteeIdentifiersList)
            {
                var inviteesToCreate = new List<InviteeIdentifiers>();

                foreach (var uploadedInviteeIdentifiers in uploadedInviteeIdentifiersList)
                {
                    var emailIsInCurrentUserHearingRoles = uploadedInviteeIdentifiers.Email != null &&
                                                           currentUserHearingRoles.Any(x =>
                                                               string.Equals(x.User.Email, uploadedInviteeIdentifiers.Email, StringComparison.CurrentCultureIgnoreCase));
                    var cprIsInCurrentUserHearingRoles = uploadedInviteeIdentifiers.CPR != null &&
                                                           currentUserHearingRoles.Any(x => 
                                                               string.Equals(x.User.PersonalIdentifier, uploadedInviteeIdentifiers.CPR, StringComparison.CurrentCultureIgnoreCase));
                    var cvrIsInCurrentUserHearingRoles = uploadedInviteeIdentifiers.CVR != null &&
                                                           currentCompanyHearingRoles.Any(x => 
                                                               string.Equals(x.Company.Cvr, uploadedInviteeIdentifiers.CVR, StringComparison.CurrentCultureIgnoreCase));

                    if (!emailIsInCurrentUserHearingRoles && !cprIsInCurrentUserHearingRoles &&
                        !cvrIsInCurrentUserHearingRoles)
                    {
                        inviteesToCreate.Add(uploadedInviteeIdentifiers);
                    }
                }

                return inviteesToCreate;
            }

            /// <summary>
            /// Identifies the user hearing roles that should be deleted.
            /// </summary>
            /// <param name="currentUserHearingRoles">The current user hearing roles.</param>
            /// <param name="uploadedInviteeIdentifiersList">The uploaded list of invitee identifiers.</param>
            /// <param name="isReplacing">Indicates whether this is part of a replace operation.</param>
            /// <returns>A list of user hearing roles to delete.</returns>
            private List<UserHearingRole> UserHearingRolesToDelete(List<UserHearingRole> currentUserHearingRoles, List<InviteeIdentifiers> uploadedInviteeIdentifiersList, bool isReplacing)
            {
                var userHearingRolesToDelete = new List<UserHearingRole>();

                foreach (var currentUserHearingRole in currentUserHearingRoles)
                {
                    var emailExistsInUploadedList = currentUserHearingRole.User.Email != null
                                                    && uploadedInviteeIdentifiersList.Any(x => 
                                                        string.Equals(x.Email, currentUserHearingRole.User.Email, StringComparison.CurrentCultureIgnoreCase));
                    var cprExistsInUploadedList = currentUserHearingRole.User.PersonalIdentifier != null
                                                  && uploadedInviteeIdentifiersList.Any(x => 
                                                      string.Equals(x.CPR, currentUserHearingRole.User.PersonalIdentifier, StringComparison.CurrentCultureIgnoreCase));
                    var existInUploadedList = emailExistsInUploadedList || cprExistsInUploadedList;

                    if ((isReplacing && !existInUploadedList) || (!isReplacing && existInUploadedList))
                    {
                        userHearingRolesToDelete.Add(currentUserHearingRole);
                    }
                }

                return userHearingRolesToDelete;
            }

            /// <summary>
            /// Identifies the company hearing roles that should be deleted.
            /// </summary>
            /// <param name="currentCompanyHearingRoles">The current company hearing roles.</param>
            /// <param name="uploadedInviteeIdentifiersList">The uploaded list of invitee identifiers.</param>
            /// <param name="isReplacing">Indicates whether this is part of a replace operation.</param>
            /// <returns>A list of company hearing roles to delete.</returns>
            private List<CompanyHearingRole> CompanyHearingRolesToDelete(
                List<CompanyHearingRole> currentCompanyHearingRoles,
                List<InviteeIdentifiers> uploadedInviteeIdentifiersList,
                bool isReplacing)
            {
                var companyHearingRolesToDelete = new List<CompanyHearingRole>();
                foreach (var currentCompanyHearingRole in currentCompanyHearingRoles)
                {
                    var cvrExistsInUploadedList = currentCompanyHearingRole.Company.Cvr != null
                                                  && uploadedInviteeIdentifiersList.Any(x => 
                                                      string.Equals(x.CVR, currentCompanyHearingRole.Company.Cvr, StringComparison.CurrentCultureIgnoreCase));
                    if ((isReplacing && !cvrExistsInUploadedList) || (!isReplacing && cvrExistsInUploadedList))
                    {
                        companyHearingRolesToDelete.Add(currentCompanyHearingRole);
                    }
                }

                return companyHearingRolesToDelete;
            }

            /// <summary>
            /// Deletes the specified user hearing roles from the database.
            /// </summary>
            /// <param name="userHearingRolesToDelete">The user hearing roles to delete.</param>
            private async Task DeleteUserHearingRoles(List<UserHearingRole> userHearingRolesToDelete)
            {
                var userHearingRoleIdsToDelete = userHearingRolesToDelete.Select(uhr => uhr.Id).ToArray();
                await _userHearingRoleDao.DeleteRangeAsync(userHearingRoleIdsToDelete);
            }

            /// <summary>
            /// Deletes the specified company hearing roles from the database.
            /// </summary>
            /// <param name="companyHearingRolesToDelete">The company hearing roles to delete.</param>
            private async Task DeleteCompanyHearingRoles(List<CompanyHearingRole> companyHearingRolesToDelete)
            {
                var companyHearingRoleIdsToDelete = companyHearingRolesToDelete.Select(chr => chr.Id).ToArray();
                await _companyHearingRoleDao.DeleteRangeAsync(companyHearingRoleIdsToDelete);
            }

            /// <summary>
            /// Creates notifications for users and companies after being invited to a hearing.
            /// </summary>
            /// <param name="hearingId">The ID of the hearing.</param>
            /// <param name="userIds">The IDs of the users to notify.</param>
            /// <param name="companyIds">The IDs of the companies to notify.</param>
            private async Task CreateNotifications(int hearingId, List<int> userIds, List<int> companyIds)
            {
                await _pluginService.NotifyUsersAfterInvitedToHearing(hearingId, userIds);
                await _pluginService.NotifyCompaniesAfterInvitedToHearing(hearingId, companyIds);
            }

            /// <summary>
            /// Retrieves or creates employee users based on provided email identifiers.
            /// </summary>
            /// <param name="emailIdentifiers">The email identifiers of the employees.</param>
            /// <returns>A list of employee user IDs.</returns>
            private async Task<List<int>> GetOrCreateEmployees(List<string> emailIdentifiers)
            {
                var currentUsers = await _userDao.GetAllAsync();
                var currentEmails =
                    new HashSet<string>(currentUsers.Where(user => !string.IsNullOrEmpty(user.Email)).Select(user => user.Email.ToLower()));

                var employeeIds = new List<int>();
                var employeesToCreate = new List<User>();

                var employeeUserCapacity = await GetUserCapacity(UserCapacityEnum.EMPLOYEE);

                foreach (var email in emailIdentifiers)
                {
                    if (currentEmails.Contains(email))
                    {
                        var employee = currentUsers.First(user => string.Equals(user.Email, email, StringComparison.CurrentCultureIgnoreCase));
                        employeeIds.Add(employee.Id);
                    }
                    else
                    {
                        employeesToCreate.Add(new User
                        {
                            Email = email,
                            UserCapacityId = employeeUserCapacity.Id
                        });
                    }
                }

                if (employeesToCreate.Any())
                {
                    var newUsers = await _userDao.CreateRangeAsync(employeesToCreate);
                    var newUserIds = GetNewUserIdsFromEmail(newUsers, currentEmails);
                    employeeIds.AddRange(newUserIds);
                }

                return employeeIds;
            }

            private async Task<UserCapacity> GetUserCapacity(UserCapacityEnum userCapacity)
            {
                var allUserCapacities = await _userCapacityDao.GetAllAsync();
                return allUserCapacities.Single(capacity => capacity.Capacity == userCapacity);
            }

            /// <summary>
            /// Retrieves or creates citizen users based on provided CPR identifiers.
            /// </summary>
            /// <param name="cprIdentifiers">The CPR identifiers of the citizens.</param>
            /// <returns>A list of citizen user IDs.</returns>
            private async Task<List<int>> GetOrCreateCitizens(List<string> cprIdentifiers)
            {
                var currentUsers = await _userDao.GetAllAsync();
                var currentPersonalIdentifiers =
                    new HashSet<string>(currentUsers.Where(user => !string.IsNullOrEmpty(user.PersonalIdentifier)).Select(user => user.PersonalIdentifier.ToLower()));

                var citizenIds = new List<int>();
                var citizensToCreate = new List<User>();

                var citizenUserCapacity = await GetUserCapacity(UserCapacityEnum.CITIZEN);

                foreach (var cpr in cprIdentifiers)
                {
                    if (currentPersonalIdentifiers.Contains(cpr))
                    {
                        var citizen = currentUsers.First(user => string.Equals(user.PersonalIdentifier, cpr, StringComparison.CurrentCultureIgnoreCase));
                        citizenIds.Add(citizen.Id);
                    }
                    else
                    {
                        citizensToCreate.Add(new User
                        {
                            PersonalIdentifier = cpr,
                            Cpr = cpr,
                            UserCapacityId = citizenUserCapacity.Id
                        });
                    }
                }

                if (citizensToCreate.Any())
                {
                    var newUsers = await _userDao.CreateRangeAsync(citizensToCreate);
                    var newUserIds = GetNewUserIdsFromPersonalIdentifier(newUsers, currentPersonalIdentifiers);
                    citizenIds.AddRange(newUserIds);
                }

                return citizenIds;
            }

            /// <summary>
            /// Retrieves or creates company entities based on provided CVR identifiers.
            /// </summary>
            /// <param name="cvrIdentifiers">The CVR identifiers of the companies.</param>
            /// <returns>A list of company IDs.</returns>
            private async Task<List<int>> GetOrCreateCompanies(List<string> cvrIdentifiers)
            {
                var currentCompanies = await _companyDao.GetAllAsync();
                var currentCVRs = new HashSet<string>(currentCompanies.Where(company => !string.IsNullOrEmpty(company.Cvr)).Select(company => company.Cvr.ToLower()));

                var companyIds = new List<int>();
                var companiesToCreate = new List<Company>();

                foreach (var cvr in cvrIdentifiers)
                {
                    if (currentCVRs.Contains(cvr))
                    {
                        var company = currentCompanies.First(company => string.Equals(company.Cvr, cvr, StringComparison.CurrentCultureIgnoreCase));
                        companyIds.Add(company.Id);
                    }
                    else
                    {
                        companiesToCreate.Add(new Company
                        {
                            Cvr = cvr
                        });
                    }
                }

                if (companiesToCreate.Any())
                {
                    var newCompanies = await _companyDao.CreateRangeAsync(companiesToCreate);
                    var newCompanyIds = GetNewCompanyIdsFromCvrIdentifier(newCompanies, currentCVRs);
                    companyIds.AddRange(newCompanyIds);
                }

                return companyIds;
            }

            private List<int> GetNewUserIdsFromPersonalIdentifier(List<User> users, HashSet<string> currentPersonalIdentifiers)
            {
                return users.Where(user => !currentPersonalIdentifiers.Contains(user.PersonalIdentifier))
                    .Select(user => user.Id).ToList();
            }

            private List<int> GetNewCompanyIdsFromCvrIdentifier(List<Company> companies, HashSet<string> currentCvrs)
            {
                return companies.Where(company => !currentCvrs.Contains(company.Cvr))
                    .Select(company => company.Id).ToList();
            }

            private List<int> GetNewUserIdsFromEmail(List<User> users, HashSet<string> currentEmails)
            {
                return users.Where(user => !currentEmails.Contains(user.Email))
                    .Select(user => user.Id).ToList();
            }

            /// <summary>
            /// Adds user hearing roles to the hearing for the specified users.
            /// </summary>
            /// <param name="hearingRole">The hearing role to assign.</param>
            /// <param name="userIds">The user IDs to assign the role to.</param>
            /// <param name="hearing">The hearing to which the roles are assigned.</param>
            private async Task CreateUserHearingRoles(BallerupKommune.Models.Models.HearingRole hearingRole, List<int> userIds, Hearing hearing)
            {
                var userHearingRolesToCreate = new List<UserHearingRole>();

                foreach (int userId in userIds)
                {
                    userHearingRolesToCreate.Add(new UserHearingRole
                    {
                        HearingRoleId = hearingRole.Id,
                        UserId = userId,
                        HearingId = hearing.Id
                    });
                }

                if (userHearingRolesToCreate.Any())
                {
                    await _userHearingRoleDao.CreateRangeAsync(userHearingRolesToCreate);
                }
            }

            /// <summary>
            /// Adds company hearing roles to the hearing for the specified companies.
            /// </summary>
            /// <param name="hearingRole">The hearing role to assign.</param>
            /// <param name="companyIds">The company IDs to assign the role to.</param>
            /// <param name="hearing">The hearing to which the roles are assigned.</param>
            private async Task CreateCompanyHearingRoles(BallerupKommune.Models.Models.HearingRole hearingRole,
                List<int> companyIds, Hearing hearing)
            {
                var companyHearingRolesToCreate = new List<CompanyHearingRole>();

                foreach (var companyId in companyIds)
                {
                    companyHearingRolesToCreate.Add(new CompanyHearingRole
                    {
                        HearingRoleId = hearingRole.Id,
                        CompanyId = companyId,
                        HearingId = hearing.Id
                    });    
                }

                if (companyHearingRolesToCreate.Any())
                {
                    await _companyHearingRoleDao.CreateRangeAsync(companyHearingRolesToCreate);
                }
            }
        }
    }
}