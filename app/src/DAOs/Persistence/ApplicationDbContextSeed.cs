using Agora.DAOs.Identity;
using Agora.DAOs.Persistence.DefaultData;
using Agora.DAOs.Persistence.DefaultData.Ballerup;
using Agora.DAOs.Persistence.DefaultData.Copenhagen;
using Agora.Entities.Entities;
using Agora.Entities.Enums;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Interfaces;
using Agora.Primitives.Logic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommentStatus = Agora.Entities.Enums.CommentStatus;
using CommentType = Agora.Entities.Enums.CommentType;
using ContentType = Agora.Entities.Enums.ContentType;
using FieldType = Agora.Entities.Enums.FieldType;
using HearingRole = Agora.Entities.Enums.HearingRole;
using HearingStatus = Agora.Entities.Enums.HearingStatus;
using UserCapacity = Agora.Entities.Enums.UserCapacity;

namespace Agora.DAOs.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var defaultRoles = new List<IdentityRole>
            {
                new IdentityRole(JWT.Roles.Admin),
                new IdentityRole(JWT.Roles.HearingCreator)
            };

            foreach (var role in defaultRoles.Where(role =>
                roleManager.Roles.All(existingRole => existingRole.Name != role.Name)))
            {
                await roleManager.CreateAsync(role);
            }
        }

        public static async Task SeedDefaultUsers(UserManager<ApplicationUser> userManager)
        {
            var administratorRole = new IdentityRole(JWT.Roles.Admin);
            var administrator = new ApplicationUser
            { UserName = "administrator@localhost", Email = "administrator@localhost" };
            var existingApplicationUsers = userManager.Users.ToList();

            if (existingApplicationUsers.All(u => u.UserName != administrator.UserName))
            {
                await userManager.CreateAsync(administrator, "Administrator1!");
                await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, IKleService kleService, IMapper mapper)
        {
            var devIdentifier = userManager.Users.First().Id;
            var empOneIdentifier = "cb33b232-ba28-4779-97fe-bff05e99d5be";
            var empTwoIdentifier = "5b52fa39-ea90-4af0-8f0f-c37a146b45d1";
            var citIdentifier = "57541d87-ca77-43c3-be60-06656cecebfa";
            var companyOneIdentifier = "151dec4a-a40c-4e5d-930d-4e17256f6c64";
            var companyTwoIdentifier = "6630d700-e53f-430a-b890-4d4601d21f00";
            var reviewIdentifier = "b155ff95-1a6d-4f16-9f01-8bdd24876186";

            var companyCvr = "24555372";

            if (!context.SubjectAreas.Any())
            {
                await context.SubjectAreas.AddRangeAsync(new List<SubjectAreaEntity>
                {
                    new SubjectAreaEntity
                    {
                        Name = "Park og rekreation",
                        IsActive = true
                    },
                    new SubjectAreaEntity
                    {
                        Name = "Politik",
                        IsActive = false
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.CityAreas.Any())
            {
                await context.CityAreas.AddRangeAsync(new List<CityAreaEntity>
                {
                    new CityAreaEntity
                    {
                        Name = "Hele byen",
                        IsActive = true
                    },
                    new CityAreaEntity
                    {
                        Name = "Strandvejen",
                        IsActive = false
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.Companies.Any())
            {
                await context.Companies.AddRangeAsync(new List<CompanyEntity>
                {
                    new CompanyEntity
                    {
                        Cvr = companyCvr,
                        Name = "Novataris",
                        PostalCode = "1051",
                        City = "København K",
                        Address = "Nyhavn 43",
                        Country = "Danmark",
                        Municipality = "Københavns Kommune",
                        StreetName = "Nyhavn"
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.UsersDb.Any())
            {
                var citizenCapacity =
                    await context.UserCapacities.FirstOrDefaultAsync(capacity => capacity.Capacity == UserCapacity.CITIZEN);
                var employeeCapacity = await context.UserCapacities.FirstOrDefaultAsync(capacity => capacity.Capacity == UserCapacity.EMPLOYEE);
                var companyCapacity = await context.UserCapacities.FirstOrDefaultAsync(capacity => capacity.Capacity == UserCapacity.COMPANY);

                var companies = context.Companies.ToList();
                var company = companies.FirstOrDefault(c => c.Cvr == companyCvr);

                await context.UsersDb.AddRangeAsync(new List<UserEntity>
                {
                    new UserEntity
                    {
                        Identifier = devIdentifier,
                        EmployeeDisplayName = "LEV_NOVATARIS_DEV",
                        Name = "Devloper",
                        Email = "dev@novataris.com",
                        PersonalIdentifier = "dev@novataris.com",
                        UserCapacityId = employeeCapacity.Id
                    },
                    new UserEntity
                    {
                        Identifier = empOneIdentifier,
                        EmployeeDisplayName = "LEV_NOVATARIS_EMPLOYEE_ONE",
                        Name = "Employee One",
                        Email = "empone@novataris.com",
                        PersonalIdentifier = "empone@novataris.com",
                        UserCapacityId = employeeCapacity.Id
                    },
                    new UserEntity
                    {
                        Identifier = empTwoIdentifier,
                        EmployeeDisplayName = "LEV_NOVATARIS_EMPLOYEE_TWO",
                        Name = "Employee Two",
                        Email = "emptwo@novataris.com",
                        PersonalIdentifier = "emptwo@novataris.com",
                        UserCapacityId = employeeCapacity.Id
                    },
                    new UserEntity
                    {
                        Identifier = citIdentifier,
                        Name = "Citizen one",
                        PersonalIdentifier = "0306874438",
                        Cpr = "0306874438",
                        UserCapacityId = citizenCapacity.Id,
                        PostalCode = "6400",
                        City = "Sønderbronx",
                        Address = "Dybbølgade 1864",
                        Municipality = "Sønderbronx Kommune",
                        Country = "Danmark",
                        StreetName = "Dybbølgade"
                    },
                    new UserEntity
                    {
                        Identifier = companyOneIdentifier,
                        Name = "Company One",
                        PersonalIdentifier = "24555372-1206550593",
                        Cpr = "1206550593",
                        Cvr = companyCvr,
                        UserCapacityId = companyCapacity.Id,
                        CompanyId = company.Id
                    },
                    new UserEntity
                    {
                        Identifier = companyTwoIdentifier,
                        Name = "Company Two",
                        PersonalIdentifier = "24555372-1206550595",
                        Cpr = "1206550595",
                        Cvr = companyCvr,
                        UserCapacityId = companyCapacity.Id,
                        CompanyId = company.Id
                    },
                    new UserEntity
                    {
                        Identifier = reviewIdentifier,
                        EmployeeDisplayName = "LEV_NOVATARIS_REVIEWER",
                        Name = "Reviewer X",
                        Email = "rev@novataris.com",
                        PersonalIdentifier = "rev@novataris.com",
                        UserCapacityId = employeeCapacity.Id
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.HearingTypes.Any())
            {
                var defaultFieldTemplate = await context.HearingTemplates.SingleAsync();

                await context.HearingTypes.AddRangeAsync(new List<HearingTypeEntity>
                {
                    new HearingTypeEntity
                    {
                        IsActive = true,
                        HearingTemplate = defaultFieldTemplate,
                        IsInternalHearing = false,
                        Name = "Offentlig - Standard"
                    },
                    new HearingTypeEntity
                    {
                        IsActive = false,
                        HearingTemplate = defaultFieldTemplate,
                        IsInternalHearing = false,
                        Name = "Offentlig - Inaktiv"
                    },
                    new HearingTypeEntity
                    {
                        IsActive = true,
                        HearingTemplate = defaultFieldTemplate,
                        IsInternalHearing = true,
                        Name = "Intern - Standard"
                    },
                    new HearingTypeEntity
                    {
                        IsActive = false,
                        HearingTemplate = defaultFieldTemplate,
                        IsInternalHearing = true,
                        Name = "Intern - Inaktiv"
                    },
                });
                await context.SaveChangesAsync();
            }

            if (!context.FieldTemplates.Any())
            {
                var hearingType = await context.HearingTypes.FirstOrDefaultAsync(x => x.IsActive);

                var titleField = await context.Fields.FirstOrDefaultAsync(x => x.FieldType.Type == FieldType.TITLE);
                var esdhTitleField =
                    await context.Fields.FirstOrDefaultAsync(x => x.FieldType.Type == FieldType.ESDH_TITLE);
                var bodyinformationField =
                    await context.Fields.FirstOrDefaultAsync(x => x.FieldType.Type == FieldType.BODYINFORMATION);
                var conclusionField =
                    await context.Fields.FirstOrDefaultAsync(x => x.FieldType.Type == FieldType.CONCLUSION);

                titleField.FieldTemplates = new List<FieldTemplateEntity>
                {
                    new FieldTemplateEntity
                    {
                        Name = "En god høringstitel",
                        Text = "Høringer om virkeligt gode ting",
                        HearingType = hearingType
                    },
                    new FieldTemplateEntity
                    {
                        Name = "Høringer om svømmehallen",
                        Text = "Der mangler endnu engang vand i svømmehallen [indsæt dato]",
                        HearingType = hearingType
                    }
                };

                esdhTitleField.FieldTemplates = new List<FieldTemplateEntity>
                {
                    new FieldTemplateEntity
                    {
                        Name = "Normal ESDH titel",
                        Text = "Sag for høring",
                        HearingType = hearingType
                    },
                    new FieldTemplateEntity
                    {
                        Name = "Ekstraordiner ESDH titel",
                        Text = "Super spændende sag for høring",
                        HearingType = hearingType
                    }
                };

                bodyinformationField.FieldTemplates = new List<FieldTemplateEntity>
                {
                    new FieldTemplateEntity
                    {
                        Name = "En god høringstekst",
                        Text =
                            "Dette er en høringstekst, og du kan vælge at rette i denne vejledning som du ønsker",
                        HearingType = hearingType
                    },
                    new FieldTemplateEntity
                    {
                        Name = "Høringer om svømmehallen",
                        Text =
                            "Svømmehallen på [indsæt vej] mangler igen vand, fordi nogen tørstige dådyr har drukket det hele. Løsningen er potentielt [indsæt løsning]",
                        HearingType = hearingType
                    }
                };

                conclusionField.FieldTemplates = new List<FieldTemplateEntity>
                {
                    new FieldTemplateEntity
                    {
                        Name = "En god konklusion",
                        Text = "Denne konklusion er et godt udgangspunkt. [indsæt tekst]",
                        HearingType = hearingType
                    }
                };

                context.Fields.UpdateRange(new List<FieldEntity>
                {
                    titleField,
                    esdhTitleField,
                    bodyinformationField,
                    conclusionField
                });
            }

            if (!context.Hearings.Any())
            {
                var draftHearingStatus =
                    await context.HearingStatus.FirstOrDefaultAsync(x => x.Status == HearingStatus.DRAFT);
                var awaitingStartDateHearingStatus =
                    await context.HearingStatus.FirstOrDefaultAsync(x => x.Status == HearingStatus.AWAITING_STARTDATE);
                var activeHearingStatus =
                    await context.HearingStatus.FirstOrDefaultAsync(x => x.Status == HearingStatus.ACTIVE);
                var awaitingConclusionHearingStatus =
                    await context.HearingStatus.FirstOrDefaultAsync(x => x.Status == HearingStatus.AWAITING_CONCLUSION);
                var concludedHearingStatus =
                    await context.HearingStatus.FirstOrDefaultAsync(x => x.Status == HearingStatus.CONCLUDED);

                var journalized =
                    await context.JournalizedStatuses.FirstOrDefaultAsync(status =>
                        status.Status == JournalizedStatus.JOURNALIZED);

                var internalHearingType = await context.HearingTypes.FirstOrDefaultAsync(x => x.IsInternalHearing);
                var publicHearingType = await context.HearingTypes.FirstOrDefaultAsync(x => !x.IsInternalHearing);

                var activeSubjectArea = await context.SubjectAreas.FirstOrDefaultAsync(x => x.IsActive);
                var closedSubjectArea = await context.SubjectAreas.FirstOrDefaultAsync(x => !x.IsActive);

                var activeCityArea = await context.CityAreas.FirstOrDefaultAsync(x => x.IsActive);
                var closedCityArea = await context.CityAreas.FirstOrDefaultAsync(x => !x.IsActive);

                var kleHierarchies = await context.KleHierarchies.ToListAsync();

                await context.Hearings.AddRangeAsync(new List<HearingEntity>
                {
                    new HearingEntity
                    {
                        HearingStatus = draftHearingStatus,
                        ClosedHearing = true,
                        ContactPersonDepartmentName = "Afdeling Q",
                        ContactPersonName = "Carl Mørck",
                        ContactPersonEmail = "CarlMørck@q.dk",
                        ContactPersonPhoneNumber = "31435899",
                        Deadline = DateTime.Now.AddDays(44),
                        StartDate = DateTime.Now.AddDays(20),
                        EsdhTitle = "Høring 1",
                        EsdhNumber = "K01-10.10.10-2020.19",
                        HearingType = publicHearingType,
                        SubjectArea = activeSubjectArea,
                        CityArea = activeCityArea,
                        ShowComments = true,
                        KleHierarchyId = kleHierarchies.ElementAt(0).Id
                    },
                    new HearingEntity
                    {
                        HearingStatus = awaitingStartDateHearingStatus,
                        ClosedHearing = false,
                        ContactPersonDepartmentName = "Afdeling O",
                        ContactPersonName = "Tanya Lawrence",
                        ContactPersonEmail = "TanyaLawrence@o.dk",
                        ContactPersonPhoneNumber = "31435823",
                        Deadline = DateTime.Now.AddDays(10),
                        StartDate = DateTime.Now.AddDays(1),
                        EsdhTitle = "Høring 2",
                        EsdhNumber = "K03-10.10.10-2020.19",
                        HearingType = publicHearingType,
                        SubjectArea = activeSubjectArea,
                        CityArea = activeCityArea,
                        ShowComments = false,
                        KleHierarchyId = kleHierarchies.ElementAt(1).Id
                    },
                    new HearingEntity
                    {
                        HearingStatus = awaitingStartDateHearingStatus,
                        ClosedHearing = false,
                        ContactPersonDepartmentName = "Afdeling l",
                        ContactPersonName = "Lucas Pedersen",
                        ContactPersonEmail = "LucasPedersen@l.dk",
                        ContactPersonPhoneNumber = "31435822",
                        Deadline = DateTime.Now.AddDays(68),
                        StartDate = DateTime.Now.AddDays(55),
                        EsdhTitle = "Høring 3",
                        EsdhNumber = "K04-10.10.10-2020.19",
                        HearingType = internalHearingType,
                        SubjectArea = activeSubjectArea,
                        CityArea = activeCityArea,
                        ShowComments = true,
                        KleHierarchyId = kleHierarchies.ElementAt(2).Id
                    },
                    new HearingEntity
                    {
                        HearingStatus = activeHearingStatus,
                        ClosedHearing = false,
                        ContactPersonDepartmentName = "Afdeling P",
                        ContactPersonName = "Daniel Hansen",
                        ContactPersonEmail = "DanielHansen@p.dk",
                        ContactPersonPhoneNumber = "31435821",
                        Deadline = DateTime.Now.AddDays(9),
                        StartDate = DateTime.Now.AddDays(-3),
                        EsdhTitle = "Høring 4",
                        EsdhNumber = "K05-10.10.10-2020.19",
                        HearingType = publicHearingType,
                        SubjectArea = activeSubjectArea,
                        CityArea = activeCityArea,
                        ShowComments = true,
                        KleHierarchyId = kleHierarchies.ElementAt(0).Id
                    },
                    new HearingEntity
                    {
                        HearingStatus = activeHearingStatus,
                        ClosedHearing = false,
                        ContactPersonDepartmentName = "Afdeling J",
                        ContactPersonName = "Jacob Sørensen",
                        ContactPersonEmail = "JacobSørensen@p.dk",
                        ContactPersonPhoneNumber = "31435859",
                        Deadline = DateTime.Now.AddDays(1),
                        StartDate = DateTime.Now.AddDays(-6),
                        EsdhTitle = "Høring 5",
                        EsdhNumber = "K06-10.10.10-2020.19",
                        HearingType = internalHearingType,
                        SubjectArea = activeSubjectArea,
                        CityArea = activeCityArea,
                        ShowComments = true,
                        KleHierarchyId = kleHierarchies.ElementAt(1).Id
                    },
                    new HearingEntity
                    {
                        HearingStatus = activeHearingStatus,
                        ClosedHearing = false,
                        ContactPersonDepartmentName = "Afdeling S",
                        ContactPersonName = "Simon Jørgensen",
                        ContactPersonEmail = "SimonJørgensen@s.dk",
                        ContactPersonPhoneNumber = "31435826",
                        Deadline = DateTime.Now.AddDays(9),
                        StartDate = DateTime.Now.AddDays(-3),
                        EsdhTitle = "Høring 6",
                        EsdhNumber = "K07-10.10.10-2020.19",
                        HearingType = publicHearingType,
                        SubjectArea = closedSubjectArea,
                        CityArea = closedCityArea,
                        ShowComments = false,
                        KleHierarchyId = kleHierarchies.ElementAt(2).Id
                    },
                    new HearingEntity
                    {
                        HearingStatus = awaitingConclusionHearingStatus,
                        ClosedHearing = false,
                        ContactPersonDepartmentName = "Afdeling A",
                        ContactPersonName = "Sabrina Sommer",
                        ContactPersonEmail = "SabrinaSommer@a.dk",
                        ContactPersonPhoneNumber = "31435829",
                        Deadline = DateTime.Now.AddDays(-2),
                        StartDate = DateTime.Now.AddDays(-10),
                        EsdhTitle = "Høring 7",
                        EsdhNumber = "K08-10.10.10-2020.19",
                        HearingType = internalHearingType,
                        SubjectArea = activeSubjectArea,
                        CityArea = activeCityArea,
                        ShowComments = true,
                        KleHierarchyId = kleHierarchies.ElementAt(0).Id
                    },
                    new HearingEntity
                    {
                        HearingStatus = concludedHearingStatus,
                        ClosedHearing = true,
                        ContactPersonDepartmentName = "Afdeling W",
                        ContactPersonName = "Anne Winther",
                        ContactPersonEmail = "AnneWinther@w.dk",
                        ContactPersonPhoneNumber = "31435831",
                        Deadline = DateTime.Now.AddDays(-4),
                        StartDate = DateTime.Now.AddDays(-12),
                        ConcludedDate = DateTime.Now.AddDays(-1),
                        EsdhTitle = "Høring 8",
                        EsdhNumber = "K09-10.10.10-2020.19",
                        HearingType = publicHearingType,
                        SubjectArea = closedSubjectArea,
                        CityArea = closedCityArea,
                        ShowComments = false,
                        KleHierarchyId = kleHierarchies.ElementAt(1).Id
                    },
                    new HearingEntity
                    {
                        HearingStatus = concludedHearingStatus,
                        ClosedHearing = true,
                        ContactPersonDepartmentName = "Afdeling B",
                        ContactPersonName = "Bent Børgesen",
                        ContactPersonEmail = "bentthere.donethat@b.dk",
                        ContactPersonPhoneNumber = "31435114",
                        Deadline = DateTime.Now.AddDays(-800),
                        StartDate = DateTime.Now.AddDays(-820),
                        ConcludedDate = DateTime.Now.AddDays(-750),
                        EsdhTitle = "Høring 9",
                        EsdhNumber = "K10-10.10.10-2020.19",
                        HearingType = publicHearingType,
                        SubjectArea = closedSubjectArea,
                        CityArea = closedCityArea,
                        ShowComments = true,
                        JournalizedStatus = journalized,
                        JournalizedStatusId = journalized.Id,
                        KleHierarchyId = kleHierarchies.ElementAt(2).Id
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.Comments.Any())
            {
                var reviewCommentType =
                    await context.CommentTypes.FirstOrDefaultAsync(x => x.Type == CommentType.HEARING_REVIEW);
                var responseCommentType =
                    await context.CommentTypes.FirstOrDefaultAsync(x => x.Type == CommentType.HEARING_RESPONSE);

                var awaitingApprovalCommentStatus =
                    await context.CommentStatuses.FirstOrDefaultAsync(x => x.Status == CommentStatus.AWAITING_APPROVAL);
                var approvedCommentStatus =
                    await context.CommentStatuses.FirstOrDefaultAsync(x => x.Status == CommentStatus.APPROVED);
                var notApprovedCommentStatus =
                    await context.CommentStatuses.FirstOrDefaultAsync(x => x.Status == CommentStatus.NOT_APPROVED);
                var noneCommentStatus =
                    await context.CommentStatuses.FirstOrDefaultAsync(x => x.Status == CommentStatus.NONE);

                var empTwoUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == empTwoIdentifier);
                var companyUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == companyOneIdentifier);
                var revUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == reviewIdentifier);

                var awaitingStartDatePublicHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 2");
                var awaitingStartDateInternalHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 3");
                var activePublicHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 4");
                var activeInternalHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 5");
                var activeClosedSubjectHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 6");
                var awaitingConclusionHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 7");
                var concludedHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 8");
                var concludedOldHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 9");

                await context.Comments.AddRangeAsync(new List<CommentEntity>
                {
                    new CommentEntity
                    {
                        CommentStatus = awaitingApprovalCommentStatus,
                        CommentType = responseCommentType,
                        User = empTwoUser,
                        IsDeleted = true,
                        ContainsSensitiveInformation = false,
                        Hearing = activeInternalHearing
                    },
                    new CommentEntity
                    {
                        CommentStatus = approvedCommentStatus,
                        CommentType = responseCommentType,
                        User = empTwoUser,
                        IsDeleted = false,
                        ContainsSensitiveInformation = false,
                        Hearing = activeInternalHearing
                    },
                    new CommentEntity
                    {
                        CommentStatus = notApprovedCommentStatus,
                        CommentDeclineInfo = new CommentDeclineInfoEntity {
                            DeclineReason = "This comment contains sensitive information",
                            DeclinerInitials = "TDTB"
                        },
                        CommentType = responseCommentType,
                        User = empTwoUser,
                        IsDeleted = false,
                        ContainsSensitiveInformation = true,
                        Hearing = activeInternalHearing
                    },
                    new CommentEntity
                    {
                        CommentStatus = approvedCommentStatus,
                        CommentType = responseCommentType,
                        User = empTwoUser,
                        IsDeleted = false,
                        ContainsSensitiveInformation = true,
                        Hearing = activeInternalHearing
                    },
                    new CommentEntity
                    {
                        CommentStatus = awaitingApprovalCommentStatus,
                        CommentType = responseCommentType,
                        User = empTwoUser,
                        IsDeleted = false,
                        ContainsSensitiveInformation = false,
                        Hearing = awaitingConclusionHearing
                    },
                    new CommentEntity
                    {
                        CommentStatus = notApprovedCommentStatus,
                        CommentDeclineInfo = new CommentDeclineInfoEntity {
                            DeclineReason = "This comment contains sensitive information",
                            DeclinerInitials = "TBDB"
                        },
                        CommentType = responseCommentType,
                        User = empTwoUser,
                        IsDeleted = true,
                        ContainsSensitiveInformation = true,
                        Hearing = awaitingConclusionHearing
                    },
                    new CommentEntity
                    {
                        CommentStatus = awaitingApprovalCommentStatus,
                        CommentType = responseCommentType,
                        User = companyUser,
                        IsDeleted = true,
                        ContainsSensitiveInformation = false,
                        Hearing = activePublicHearing
                    },
                    new CommentEntity
                    {
                        CommentStatus = approvedCommentStatus,
                        CommentType = responseCommentType,
                        User = companyUser,
                        IsDeleted = true,
                        ContainsSensitiveInformation = false,
                        Hearing = activePublicHearing
                    },
                    new CommentEntity
                    {
                        CommentStatus = approvedCommentStatus,
                        CommentType = responseCommentType,
                        User = companyUser,
                        IsDeleted = false,
                        ContainsSensitiveInformation = false,
                        Hearing = activeClosedSubjectHearing,
                    },
                    new CommentEntity
                    {
                        CommentStatus = notApprovedCommentStatus,
                        CommentDeclineInfo = new CommentDeclineInfoEntity {
                            DeclineReason = "This comment contains sensitive information",
                            DeclinerInitials = "TDTB"
                        },
                        CommentType = responseCommentType,
                        User = companyUser,
                        IsDeleted = false,
                        ContainsSensitiveInformation = false,
                        Hearing = activeClosedSubjectHearing,
                    },
                    new CommentEntity
                    {
                        CommentStatus = notApprovedCommentStatus,
                        CommentDeclineInfo = new CommentDeclineInfoEntity {
                            DeclineReason = "This comment contains sensitive information",
                            DeclinerInitials = "TDTB"
                        },
                        CommentType = responseCommentType,
                        User = companyUser,
                        IsDeleted = true,
                        ContainsSensitiveInformation = true,
                        Hearing = concludedHearing
                    },
                    new CommentEntity
                    {
                        CommentStatus = approvedCommentStatus,
                        CommentType = responseCommentType,
                        User = companyUser,
                        IsDeleted = true,
                        ContainsSensitiveInformation = true,
                        Hearing = concludedHearing
                    },
                    new CommentEntity
                    {
                        CommentStatus = noneCommentStatus,
                        CommentType = reviewCommentType,
                        User = revUser,
                        Hearing = awaitingStartDateInternalHearing
                    },
                    new CommentEntity
                    {
                        CommentStatus = noneCommentStatus,
                        CommentType = reviewCommentType,
                        User = revUser,
                        Hearing = awaitingStartDatePublicHearing,
                    },
                    new CommentEntity
                    {
                        CommentStatus = notApprovedCommentStatus,
                        CommentDeclineInfo = new CommentDeclineInfoEntity {
                            DeclineReason = "This comment contains sensitive information",
                            DeclinerInitials = "XXYY"
                        },
                        CommentType = responseCommentType,
                        User = empTwoUser,
                        IsDeleted = false,
                        ContainsSensitiveInformation = true,
                        Hearing = concludedOldHearing,
                    },
                });
                await context.SaveChangesAsync();
            }

            if (!context.UserHearingRoles.Any())
            {
                var hearingOwnerRole =
                    await context.HearingRoles.FirstOrDefaultAsync(x => x.Role == HearingRole.HEARING_OWNER);
                var hearingInviteeRole =
                    await context.HearingRoles.FirstOrDefaultAsync(x => x.Role == HearingRole.HEARING_INVITEE);
                var hearingReviewerRole =
                    await context.HearingRoles.FirstOrDefaultAsync(x => x.Role == HearingRole.HEARING_REVIEWER);
                var hearingResponderRole =
                    await context.HearingRoles.FirstOrDefaultAsync(x => x.Role == HearingRole.HEARING_RESPONDER);

                var draftHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 1");
                var awaitingStartDatePublicHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 2");
                var awaitingStartDateInternalHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 3");
                var activePublicHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 4");
                var activeInternalHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 5");
                var activeClosedSubjectHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 6");
                var awaitingConclusionHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 7");
                var concludedHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 8");
                var concludedOldHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 9");

                var devUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == devIdentifier);
                var empOneUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == empOneIdentifier);
                var empTwoUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == empTwoIdentifier);
                var citOneUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == citIdentifier);
                var companyUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == companyOneIdentifier);
                var revUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == reviewIdentifier);

                await context.UserHearingRoles.AddRangeAsync(new List<UserHearingRoleEntity>
                {
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingOwnerRole,
                        Hearing = draftHearing,
                        User = devUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingOwnerRole,
                        Hearing = awaitingStartDatePublicHearing,
                        User = devUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingOwnerRole,
                        Hearing = awaitingStartDateInternalHearing,
                        User = devUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingOwnerRole,
                        Hearing = activePublicHearing,
                        User = devUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingOwnerRole,
                        Hearing = activeInternalHearing,
                        User = devUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingOwnerRole,
                        Hearing = activeClosedSubjectHearing,
                        User = devUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingOwnerRole,
                        Hearing = awaitingConclusionHearing,
                        User = devUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingOwnerRole,
                        Hearing = concludedHearing,
                        User = devUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingInviteeRole,
                        Hearing = activeInternalHearing,
                        User = empOneUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingInviteeRole,
                        Hearing = activeClosedSubjectHearing,
                        User = citOneUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingInviteeRole,
                        Hearing = activePublicHearing,
                        User = citOneUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingInviteeRole,
                        Hearing = awaitingConclusionHearing,
                        User = empOneUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingInviteeRole,
                        Hearing = concludedHearing,
                        User = citOneUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingReviewerRole,
                        Hearing = awaitingStartDateInternalHearing,
                        User = revUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingReviewerRole,
                        Hearing = awaitingStartDatePublicHearing,
                        User = revUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingReviewerRole,
                        Hearing = activePublicHearing,
                        User = revUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingReviewerRole,
                        Hearing = activeInternalHearing,
                        User = revUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingReviewerRole,
                        Hearing = activeClosedSubjectHearing,
                        User = revUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingResponderRole,
                        Hearing = awaitingStartDatePublicHearing,
                        User = revUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingResponderRole,
                        Hearing = awaitingStartDateInternalHearing,
                        User = revUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingResponderRole,
                        Hearing = activeInternalHearing,
                        User = empTwoUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingResponderRole,
                        Hearing = activeClosedSubjectHearing,
                        User = companyUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingResponderRole,
                        Hearing = awaitingConclusionHearing,
                        User = empTwoUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingResponderRole,
                        Hearing = concludedHearing,
                        User = companyUser
                    },
                    new UserHearingRoleEntity
                    {
                        HearingRole = hearingResponderRole,
                        Hearing = concludedOldHearing,
                        User = companyUser
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.CompanyHearingRoles.Any())
            {
                var companies = await context.Companies.ToListAsync();
                var companyOne = companies.FirstOrDefault(x => x.Cvr == companyCvr);

                var activePublicHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 4");
                var activeClosedSubjectHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 6");
                var concludedOldHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 9");

                var hearingInviteeRole =
                    await context.HearingRoles.FirstOrDefaultAsync(x => x.Role == HearingRole.HEARING_INVITEE);
                var hearingResponderRole =
                    await context.HearingRoles.FirstOrDefaultAsync(x => x.Role == HearingRole.HEARING_RESPONDER);

                await context.CompanyHearingRoles.AddRangeAsync(new List<CompanyHearingRoleEntity>
                {
                    new CompanyHearingRoleEntity
                    {
                        HearingRole = hearingResponderRole,
                        Hearing = activeClosedSubjectHearing,
                        Company = companyOne
                    },
                    new CompanyHearingRoleEntity
                    {
                        HearingRole = hearingInviteeRole,
                        Hearing = activePublicHearing,
                        Company = companyOne
                    },
                    new CompanyHearingRoleEntity
                    {
                        HearingRole = hearingInviteeRole,
                        Hearing = concludedOldHearing,
                        Company = companyOne
                    },
                });

                await context.SaveChangesAsync();

            }

            if (!context.Consents.Any())
            {
                var globalContent = await context.GlobalContents.FirstOrDefaultAsync();

                var concludedOldHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 9");

                var awaitingApprovalActiveInternalComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.AWAITING_APPROVAL &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && x.Hearing.HearingType.IsInternalHearing);
                var approvedActiveInternalComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && x.Hearing.HearingType.IsInternalHearing);
                var notApprovedActiveInternalComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.NOT_APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && x.Hearing.HearingType.IsInternalHearing);
                var approvedAwaitingConclusionComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.AWAITING_CONCLUSION);
                var awaitingApprovalAwaitingConclusionComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.AWAITING_APPROVAL &&
                    x.Hearing.HearingStatus.Status == HearingStatus.AWAITING_CONCLUSION);
                var notApprovedAwaitingConclusionComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.NOT_APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.AWAITING_CONCLUSION);
                var awaitingApprovalActivePublicComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.AWAITING_APPROVAL &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && !x.Hearing.HearingType.IsInternalHearing);
                var approvedActivePublicComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && !x.Hearing.HearingType.IsInternalHearing);
                var approvedInactiveSubjectComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && !x.Hearing.SubjectArea.IsActive);
                var notApprovedInactiveSubjectComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.NOT_APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && !x.Hearing.SubjectArea.IsActive);
                var notApprovedConcludedComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.NOT_APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.CONCLUDED);
                var approvedConcludedComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.CONCLUDED);
                var oldConcludedHearingComment =
                    await context.Comments.FirstOrDefaultAsync(
                        x => x.Hearing.Id == concludedOldHearing.Id);

                await context.Consents.AddRangeAsync(new List<ConsentEntity>
                {
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = awaitingApprovalActiveInternalComment
                    },
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = approvedActiveInternalComment
                    },
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = notApprovedActiveInternalComment
                    },
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = approvedAwaitingConclusionComment
                    },
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = awaitingApprovalAwaitingConclusionComment
                    },
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = notApprovedAwaitingConclusionComment
                    },
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = awaitingApprovalActivePublicComment
                    },
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = approvedActivePublicComment
                    },
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = approvedInactiveSubjectComment
                    },
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = notApprovedInactiveSubjectComment
                    },
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = notApprovedConcludedComment
                    },
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = approvedConcludedComment
                    },
                    new ConsentEntity
                    {
                        GlobalContent = globalContent,
                        Comment = oldConcludedHearingComment
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.Contents.Any())
            {
                var draftHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 1");
                var awaitingStartDatePublicHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 2");
                var awaitingStartDateInternalHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 3");
                var activePublicHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 4");
                var activeInternalHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 5");
                var activeClosedSubjectHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 6");
                var awaitingConclusionHearing =
                    await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 7");
                var concludedHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 8");
                var concludedOldHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 9");

                var awaitingApprovalActiveInternalComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.AWAITING_APPROVAL &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && x.Hearing.HearingType.IsInternalHearing);
                var approvedActiveInternalComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && x.Hearing.HearingType.IsInternalHearing);
                var notApprovedActiveInternalComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.NOT_APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && x.Hearing.HearingType.IsInternalHearing);
                var approvedAwaitingConclusionComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.AWAITING_CONCLUSION);
                var awaitingApprovalAwaitingConclusionComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.AWAITING_APPROVAL &&
                    x.Hearing.HearingStatus.Status == HearingStatus.AWAITING_CONCLUSION);
                var notApprovedAwaitingConclusionComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.NOT_APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.AWAITING_CONCLUSION);
                var awaitingApprovalActivePublicComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.AWAITING_APPROVAL &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && !x.Hearing.HearingType.IsInternalHearing);
                var approvedActivePublicComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && !x.Hearing.HearingType.IsInternalHearing);
                var approvedInactiveSubjectComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && !x.Hearing.SubjectArea.IsActive);
                var notApprovedInactiveSubjectComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.NOT_APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.ACTIVE && !x.Hearing.SubjectArea.IsActive);
                var notApprovedConcludedComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.NOT_APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.CONCLUDED);
                var approvedConcludedComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.APPROVED &&
                    x.Hearing.HearingStatus.Status == HearingStatus.CONCLUDED);
                var reviewInternalComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.NONE &&
                    x.Hearing.HearingType.IsInternalHearing);
                var reviewPublicComment = await context.Comments.FirstOrDefaultAsync(x =>
                    x.CommentStatus.Status == CommentStatus.NONE &&
                    !x.Hearing.HearingType.IsInternalHearing);
                var oldConcludedHearingComment =
                    await context.Comments.FirstOrDefaultAsync(
                        x => x.Hearing.Id == concludedOldHearing.Id);

                var fields = await context.Fields.Include(nameof(FieldEntity.FieldType)).ToListAsync();
                var contentTypes = await context.ContentTypes.ToListAsync();

                await context.Contents.AddRangeAsync(new List<ContentEntity>
                {
                    new ContentEntity
                    {
                        TextContent = "Mega sprød høringstitel for en høring i draft",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.TITLE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = draftHearing
                    },
                    new ContentEntity
                    {
                        FileName = "front_image1.png",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.IMAGE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.FILE),
                        Hearing = draftHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Dette er en opsummering vedrørerende handlingsforløbet af denne høring i draft",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.SUMMARY),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = draftHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Dette er et udkast til høringstekst i dens helhed",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.BODYINFORMATION),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = draftHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Mega sprød høringstitel for en høring som starter snart",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.TITLE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = awaitingStartDatePublicHearing
                    },
                    new ContentEntity
                    {
                        FileName = "front_image3.png",
                        FileContentType = "image/png",
                        FilePath = string.Empty,
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.IMAGE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.FILE),
                        Hearing = awaitingStartDatePublicHearing
                    },
                    new ContentEntity
                    {
                        TextContent =
                            "Dette er en opsummering vedrørerende handlingsforløbet af denne høring som starter om lidt",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.SUMMARY),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = awaitingStartDatePublicHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Dette er en høringstekst i dens helhed, snart aktuel",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.BODYINFORMATION),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = awaitingStartDatePublicHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Mega sprød høringstitel for en intern høring som starter snart",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.TITLE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = awaitingStartDateInternalHearing
                    },
                    new ContentEntity
                    {
                        FileName = "front_image4.png",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.IMAGE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.FILE),
                        Hearing = awaitingStartDateInternalHearing
                    },
                    new ContentEntity
                    {
                        TextContent =
                            "Dette er en opsummering vedrørerende handlingsforløbet af denne interne høring som starter om lidt",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.SUMMARY),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = awaitingStartDateInternalHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Dette er en intern høringstekst i dens helhed, snart aktuel",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.BODYINFORMATION),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = awaitingStartDateInternalHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Mega sprød høringstitel for en offentlig høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.TITLE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = activePublicHearing
                    },
                    new ContentEntity
                    {
                        FileName = "front_image5.png",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.IMAGE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.FILE),
                        Hearing = activePublicHearing
                    },
                    new ContentEntity
                    {
                        TextContent =
                            "Dette er en opsummering vedrørerende handlingsforløbet af denne offentlige høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.SUMMARY),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = activePublicHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Dette er en offentlig høringstekst i dens helhed",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.BODYINFORMATION),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = activePublicHearing
                    },
                    new ContentEntity
                    {
                        FileName = "fil_med_bilag.txt",
                        FileContentType = "text/plain",
                        FilePath = string.Empty,
                        Field = fields.FirstOrDefault(c => c.FieldType.Type == FieldType.BODYINFORMATION),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.FILE),
                        Hearing = activePublicHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Mega sprød høringstitel for en intern høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.TITLE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = activeInternalHearing
                    },
                    new ContentEntity
                    {
                        FileName = "front_image6.png",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.IMAGE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.FILE),
                        Hearing = activeInternalHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Dette er en opsummering vedrørerende handlingsforløbet af denne interne høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.SUMMARY),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = activeInternalHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Dette er en intern høringstekst i dens helhed",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.BODYINFORMATION),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = activeInternalHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Mega sprød høringstitel for en høring med et inaktivt emne",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.TITLE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = activeClosedSubjectHearing
                    },
                    new ContentEntity
                    {
                        FileName = "front_image7.png",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.IMAGE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.FILE),
                        Hearing = activeClosedSubjectHearing
                    },
                    new ContentEntity
                    {
                        TextContent =
                            "Dette er en opsummering vedrørerende handlingsforløbet af denne høring med et inaktivt emne",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.SUMMARY),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = activeClosedSubjectHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Dette er en høringstekst i dens helhed til en høring med et inaktivt emne",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.BODYINFORMATION),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = activeClosedSubjectHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Mega sprød høringstitel for en udløbet høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.TITLE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = awaitingConclusionHearing
                    },
                    new ContentEntity
                    {
                        FileName = "front_image8.png",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.IMAGE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.FILE),
                        Hearing = awaitingConclusionHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Dette er en opsummering vedrørerende handlingsforløbet af denne udløbne høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.SUMMARY),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = awaitingConclusionHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Dette er en høringstekst i dens helhed til en udløbet høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.BODYINFORMATION),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = awaitingConclusionHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Mega sprød høringstitel for en konkluderet høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.TITLE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = concludedHearing
                    },
                    new ContentEntity
                    {
                        FileName = "front_image9.png",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.IMAGE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.FILE),
                        Hearing = concludedHearing
                    },
                    new ContentEntity
                    {
                        TextContent =
                            "Dette er en opsummering vedrørerende handlingsforløbet af denne konkluderede høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.SUMMARY),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = concludedHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Dette er en høringstekst i dens helhed til en konkluderet høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.BODYINFORMATION),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = concludedHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "En præcis konklusion til en konkluderet høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.CONCLUSION),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = concludedHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "En høringstitel for en konkluderet og gammel høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.TITLE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = concludedOldHearing
                    },
                    new ContentEntity
                    {
                        FileName = "front_image9.png",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.IMAGE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.FILE),
                        Hearing = concludedOldHearing
                    },
                    new ContentEntity
                    {
                        TextContent =
                            "Dette er en opsummering vedrørerende handlingsforløbet af denne høring, der er konkluderet for længe siden.",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.SUMMARY),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = concludedOldHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "Dette er en høringstekst i dens helhed til en konkluderet og gammel høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.BODYINFORMATION),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = concludedOldHearing
                    },
                    new ContentEntity
                    {
                        TextContent = "En præcis konklusion til en konkluderet og gammel høring",
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.CONCLUSION),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        Hearing = concludedOldHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to an active internal hearing awaiting approval",
                        Comment = awaitingApprovalActiveInternalComment,
                        Hearing = activeInternalHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to an active internal hearing",
                        Comment = approvedActiveInternalComment,
                        Hearing = activeInternalHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to an active internal hearing that has not been approved",
                        Comment = notApprovedActiveInternalComment,
                        Hearing = activeInternalHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to a hearing awaiting a conclusion",
                        Comment = approvedAwaitingConclusionComment,
                        Hearing = awaitingConclusionHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to a hearing awaiting a conclusion, awaiting approval",
                        Comment = awaitingApprovalAwaitingConclusionComment,
                        Hearing = awaitingConclusionHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to a hearing awaiting a conclusion that has not been approved",
                        Comment = notApprovedAwaitingConclusionComment,
                        Hearing = awaitingConclusionHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to an active public hearing awaiting approval",
                        Comment = awaitingApprovalActivePublicComment,
                        Hearing = activePublicHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to an active public hearing",
                        Comment = approvedActivePublicComment,
                        Hearing = activePublicHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to an active hearing with a closed subject",
                        Comment = approvedInactiveSubjectComment,
                        Hearing = activeClosedSubjectHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to an active hearing with a closed subject that was not approved",
                        Comment = notApprovedInactiveSubjectComment,
                        Hearing = activeClosedSubjectHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to a hearing with a conclusion that was not approved",
                        Comment = notApprovedConcludedComment,
                        Hearing = concludedHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to a hearing with a conclusion",
                        Comment = approvedConcludedComment,
                        Hearing = concludedHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to an internal hearing awaiting its start date",
                        Comment = reviewInternalComment,
                        Hearing = awaitingStartDateInternalHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to an public hearing awaiting its start date",
                        Comment = reviewPublicComment,
                        Hearing = awaitingStartDatePublicHearing
                    },
                    new ContentEntity
                    {
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.TEXT),
                        TextContent = "A comment to an old concluded hearing",
                        Comment = oldConcludedHearingComment,
                        Hearing = concludedOldHearing,
                    },
                    new ContentEntity
                    {
                        FileName = "front_image3.png",
                        FileContentType = "image/png",
                        FilePath = string.Empty,
                        Field = fields.FirstOrDefault(f => f.FieldType.Type == FieldType.IMAGE),
                        ContentType = contentTypes.FirstOrDefault(c => c.Type == ContentType.FILE),
                        Hearing = concludedOldHearing
                    },
                });
                await context.SaveChangesAsync();
            }

            if (!context.KleMappings.Any())
            {
                var hearingTypes = await context.HearingTypes.ToListAsync();
                var kleHierarchies = await context.KleHierarchies.ToListAsync();

                if (kleHierarchies.Any())
                {
                    foreach (var hearingType in hearingTypes)
                    {
                        await context.KleMappings.AddRangeAsync(new List<KleMappingEntity>
                        {
                            new KleMappingEntity
                            {
                                HearingTypeId = hearingType.Id,
                                KleHierarchyId = kleHierarchies.ElementAt(0).Id
                            },
                            new KleMappingEntity
                            {
                                HearingTypeId = hearingType.Id,
                                KleHierarchyId = kleHierarchies.ElementAt(1).Id
                            },
                            new KleMappingEntity
                            {
                                HearingTypeId = hearingType.Id,
                                KleHierarchyId = kleHierarchies.ElementAt(2).Id
                            }
                        });
                    }

                    await context.SaveChangesAsync();
                }
            }

            if (!context.NotificationContentSpecifications.Any())
            {
                var draftHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 1");
                var awaitingStartDatePublicHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 2");
                var awaitingStartDateInternalHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 3");
                var activePublicHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 4");
                var activeInternalHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 5");
                var activeClosedSubjectHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 6");
                var awaitingConclusionHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 7");
                var concludedHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 8");
                var concludedOldHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 9");

                var subjectNotificationContentType = await context.NotificationContentTypes.FirstOrDefaultAsync(x => x.Type == NotificationContentType.SUBJECT);
                var headerNotificationContentType = await context.NotificationContentTypes.FirstOrDefaultAsync(x => x.Type == NotificationContentType.HEADER);
                var bodyNotificationContentType = await context.NotificationContentTypes.FirstOrDefaultAsync(x => x.Type == NotificationContentType.BODY);
                var footerNotificationContentType = await context.NotificationContentTypes.FirstOrDefaultAsync(x => x.Type == NotificationContentType.FOOTER);

                var invitationNotificationType = await context.NotificationTypes.FirstOrDefaultAsync(x => x.Type == NotificationType.INVITED_TO_HEARING);
                var conclusionNotificationType = await context.NotificationTypes.FirstOrDefaultAsync(x => x.Type == NotificationType.HEARING_CONCLUSION_PUBLISHED);

                var invitationSubjectTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.Id == invitationNotificationType.SubjectTemplateId);
                var invitationHeaderTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.Id == invitationNotificationType.HeaderTemplateId);
                var invitationBodyTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.Id == invitationNotificationType.BodyTemplateId);
                var invitationFooterTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.Id == invitationNotificationType.FooterTemplateId);

                var conclusionSubjectTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.Id == conclusionNotificationType.SubjectTemplateId);
                var conclusionHeaderTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.Id == conclusionNotificationType.HeaderTemplateId);
                var conclusionBodyTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.Id == conclusionNotificationType.BodyTemplateId);
                var conclusionFooterTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.Id == conclusionNotificationType.FooterTemplateId);

                await context.NotificationContentSpecifications.AddRangeAsync(
                    new List<NotificationContentSpecificationEntity>
                    {
                        new NotificationContentSpecificationEntity
                        {
                            NotificationType = invitationNotificationType,
                            Hearing = draftHearing,
                            SubjectContent = new NotificationContentEntity
                            {
                                TextContent = invitationSubjectTemplate.TextContent,
                                NotificationContentType = subjectNotificationContentType
                            },
                            HeaderContent = new NotificationContentEntity
                            {
                                TextContent = invitationHeaderTemplate.TextContent,
                                NotificationContentType = headerNotificationContentType
                            },
                            BodyContent = new NotificationContentEntity
                            {
                                TextContent = invitationBodyTemplate.TextContent,
                                NotificationContentType = bodyNotificationContentType
                            },
                            FooterContent = new NotificationContentEntity
                            {
                                TextContent = invitationFooterTemplate.TextContent,
                                NotificationContentType = footerNotificationContentType
                            }
                        },
                        new NotificationContentSpecificationEntity
                        {
                            NotificationType = invitationNotificationType,
                            Hearing = awaitingStartDatePublicHearing,
                            SubjectContent = new NotificationContentEntity
                            {
                                TextContent = invitationSubjectTemplate.TextContent,
                                NotificationContentType = subjectNotificationContentType
                            },
                            HeaderContent = new NotificationContentEntity
                            {
                                TextContent = invitationHeaderTemplate.TextContent,
                                NotificationContentType = headerNotificationContentType
                            },
                            BodyContent = new NotificationContentEntity
                            {
                                TextContent = invitationBodyTemplate.TextContent,
                                NotificationContentType = bodyNotificationContentType
                            },
                            FooterContent = new NotificationContentEntity
                            {
                                TextContent = invitationFooterTemplate.TextContent,
                                NotificationContentType = footerNotificationContentType
                            }
                        },
                        new NotificationContentSpecificationEntity
                        {
                            NotificationType = invitationNotificationType,
                            Hearing = awaitingStartDateInternalHearing,
                            SubjectContent = new NotificationContentEntity
                            {
                                TextContent = invitationSubjectTemplate.TextContent,
                                NotificationContentType = subjectNotificationContentType
                            },
                            HeaderContent = new NotificationContentEntity
                            {
                                TextContent = invitationHeaderTemplate.TextContent,
                                NotificationContentType = headerNotificationContentType
                            },
                            BodyContent = new NotificationContentEntity
                            {
                                TextContent = invitationBodyTemplate.TextContent,
                                NotificationContentType = bodyNotificationContentType
                            },
                            FooterContent = new NotificationContentEntity
                            {
                                TextContent = invitationFooterTemplate.TextContent,
                                NotificationContentType = footerNotificationContentType
                            }
                        },
                        new NotificationContentSpecificationEntity
                        {
                            NotificationType = invitationNotificationType,
                            Hearing = activePublicHearing,
                            SubjectContent = new NotificationContentEntity
                            {
                                TextContent = invitationSubjectTemplate.TextContent,
                                NotificationContentType = subjectNotificationContentType
                            },
                            HeaderContent = new NotificationContentEntity
                            {
                                TextContent = invitationHeaderTemplate.TextContent,
                                NotificationContentType = headerNotificationContentType
                            },
                            BodyContent = new NotificationContentEntity
                            {
                                TextContent = invitationBodyTemplate.TextContent,
                                NotificationContentType = bodyNotificationContentType
                            },
                            FooterContent = new NotificationContentEntity
                            {
                                TextContent = invitationFooterTemplate.TextContent,
                                NotificationContentType = footerNotificationContentType
                            }
                        },
                        new NotificationContentSpecificationEntity
                        {
                            NotificationType = invitationNotificationType,
                            Hearing = activeInternalHearing,
                            SubjectContent = new NotificationContentEntity
                            {
                                TextContent = invitationSubjectTemplate.TextContent,
                                NotificationContentType = subjectNotificationContentType
                            },
                            HeaderContent = new NotificationContentEntity
                            {
                                TextContent = invitationHeaderTemplate.TextContent,
                                NotificationContentType = headerNotificationContentType
                            },
                            BodyContent = new NotificationContentEntity
                            {
                                TextContent = invitationBodyTemplate.TextContent,
                                NotificationContentType = bodyNotificationContentType
                            },
                            FooterContent = new NotificationContentEntity
                            {
                                TextContent = invitationFooterTemplate.TextContent,
                                NotificationContentType = footerNotificationContentType
                            }
                        },
                        new NotificationContentSpecificationEntity
                        {
                            NotificationType = invitationNotificationType,
                            Hearing = activeClosedSubjectHearing,
                            SubjectContent = new NotificationContentEntity
                            {
                                TextContent = invitationSubjectTemplate.TextContent,
                                NotificationContentType = subjectNotificationContentType
                            },
                            HeaderContent = new NotificationContentEntity
                            {
                                TextContent = invitationHeaderTemplate.TextContent,
                                NotificationContentType = headerNotificationContentType
                            },
                            BodyContent = new NotificationContentEntity
                            {
                                TextContent = invitationBodyTemplate.TextContent,
                                NotificationContentType = bodyNotificationContentType
                            },
                            FooterContent = new NotificationContentEntity
                            {
                                TextContent = invitationFooterTemplate.TextContent,
                                NotificationContentType = footerNotificationContentType
                            }
                        },
                        new NotificationContentSpecificationEntity
                        {
                            NotificationType = invitationNotificationType,
                            Hearing = awaitingConclusionHearing,
                            SubjectContent = new NotificationContentEntity
                            {
                                TextContent = invitationSubjectTemplate.TextContent,
                                NotificationContentType = subjectNotificationContentType
                            },
                            HeaderContent = new NotificationContentEntity
                            {
                                TextContent = invitationHeaderTemplate.TextContent,
                                NotificationContentType = headerNotificationContentType
                            },
                            BodyContent = new NotificationContentEntity
                            {
                                TextContent = invitationBodyTemplate.TextContent,
                                NotificationContentType = bodyNotificationContentType
                            },
                            FooterContent = new NotificationContentEntity
                            {
                                TextContent = invitationFooterTemplate.TextContent,
                                NotificationContentType = footerNotificationContentType
                            }
                        },
                        new NotificationContentSpecificationEntity
                        {
                            NotificationType = invitationNotificationType,
                            Hearing = concludedHearing,
                            SubjectContent = new NotificationContentEntity
                            {
                                TextContent = invitationSubjectTemplate.TextContent,
                                NotificationContentType = subjectNotificationContentType
                            },
                            HeaderContent = new NotificationContentEntity
                            {
                                TextContent = invitationHeaderTemplate.TextContent,
                                NotificationContentType = headerNotificationContentType
                            },
                            BodyContent = new NotificationContentEntity
                            {
                                TextContent = invitationBodyTemplate.TextContent,
                                NotificationContentType = bodyNotificationContentType
                            },
                            FooterContent = new NotificationContentEntity
                            {
                                TextContent = invitationFooterTemplate.TextContent,
                                NotificationContentType = footerNotificationContentType
                            }
                        },
                        new NotificationContentSpecificationEntity
                        {
                            NotificationType = invitationNotificationType,
                            Hearing = concludedOldHearing,
                            SubjectContent = new NotificationContentEntity
                            {
                                TextContent = invitationSubjectTemplate.TextContent,
                                NotificationContentType = subjectNotificationContentType
                            },
                            HeaderContent = new NotificationContentEntity
                            {
                                TextContent = invitationHeaderTemplate.TextContent,
                                NotificationContentType = headerNotificationContentType
                            },
                            BodyContent = new NotificationContentEntity
                            {
                                TextContent = invitationBodyTemplate.TextContent,
                                NotificationContentType = bodyNotificationContentType
                            },
                            FooterContent = new NotificationContentEntity
                            {
                                TextContent = invitationFooterTemplate.TextContent,
                                NotificationContentType = footerNotificationContentType
                            }
                        },
                        new NotificationContentSpecificationEntity
                        {
                            NotificationType = conclusionNotificationType,
                            Hearing = awaitingConclusionHearing,
                            SubjectContent = new NotificationContentEntity
                            {
                                TextContent = conclusionSubjectTemplate.TextContent,
                                NotificationContentType = subjectNotificationContentType
                            },
                            HeaderContent = new NotificationContentEntity
                            {
                                TextContent = conclusionHeaderTemplate.TextContent,
                                NotificationContentType = headerNotificationContentType
                            },
                            BodyContent = new NotificationContentEntity
                            {
                                TextContent = conclusionBodyTemplate.TextContent,
                                NotificationContentType = bodyNotificationContentType
                            },
                            FooterContent = new NotificationContentEntity
                            {
                                TextContent = conclusionFooterTemplate.TextContent,
                                NotificationContentType = footerNotificationContentType
                            }
                        },
                        new NotificationContentSpecificationEntity
                        {
                            NotificationType = conclusionNotificationType,
                            Hearing = concludedHearing,
                            SubjectContent = new NotificationContentEntity
                            {
                                TextContent = conclusionSubjectTemplate.TextContent,
                                NotificationContentType = subjectNotificationContentType
                            },
                            HeaderContent = new NotificationContentEntity
                            {
                                TextContent = conclusionHeaderTemplate.TextContent,
                                NotificationContentType = headerNotificationContentType
                            },
                            BodyContent = new NotificationContentEntity
                            {
                                TextContent = conclusionBodyTemplate.TextContent,
                                NotificationContentType = bodyNotificationContentType
                            },
                            FooterContent = new NotificationContentEntity
                            {
                                TextContent = conclusionFooterTemplate.TextContent,
                                NotificationContentType = footerNotificationContentType
                            }
                        },
                        new NotificationContentSpecificationEntity
                        {
                            NotificationType = conclusionNotificationType,
                            Hearing = concludedOldHearing,
                            SubjectContent = new NotificationContentEntity
                            {
                                TextContent = conclusionSubjectTemplate.TextContent,
                                NotificationContentType = subjectNotificationContentType
                            },
                            HeaderContent = new NotificationContentEntity
                            {
                                TextContent = conclusionHeaderTemplate.TextContent,
                                NotificationContentType = headerNotificationContentType
                            },
                            BodyContent = new NotificationContentEntity
                            {
                                TextContent = conclusionBodyTemplate.TextContent,
                                NotificationContentType = bodyNotificationContentType
                            },
                            FooterContent = new NotificationContentEntity
                            {
                                TextContent = conclusionFooterTemplate.TextContent,
                                NotificationContentType = footerNotificationContentType
                            }
                        },
                    });
                await context.SaveChangesAsync();
            }

            if (!context.Notifications.Any())
            {
                var draftHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 1");
                var activePublicHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 4");
                var activeInternalHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 5");
                var empOneUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == empOneIdentifier);
                var empTwoUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == empTwoIdentifier);
                var citizen = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == citIdentifier);

                var addedAsReviewerNotificationType = await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.ADDED_AS_REVIEWER);
                var invitedToHearingNotificationType = await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.INVITED_TO_HEARING);
                var hearingAnswerReceiptNotificationType = await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.HEARING_ANSWER_RECEIPT);
                var conclusionPublishedNotificationType = await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.HEARING_CONCLUSION_PUBLISHED);
                var hearingChangedNotificationType = await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.HEARING_CHANGED);

                await context.Notifications.AddRangeAsync(new List<NotificationEntity>
                {
                     new NotificationEntity
                     {
                         Hearing = draftHearing,
                         User = empTwoUser,
                         NotificationType = addedAsReviewerNotificationType
                     },
                     new NotificationEntity
                     {
                         Hearing = activePublicHearing,
                         User = citizen,
                         NotificationType = invitedToHearingNotificationType
                     },
                     new NotificationEntity
                     {
                         Hearing = activeInternalHearing,
                         User = empTwoUser,
                         NotificationType = hearingAnswerReceiptNotificationType
                     },
                     new NotificationEntity
                     {
                         Hearing = activeInternalHearing,
                         User = empOneUser,
                         NotificationType = hearingAnswerReceiptNotificationType
                     },
                     new NotificationEntity
                     {
                         Hearing = activePublicHearing,
                         User = citizen,
                         NotificationType = hearingAnswerReceiptNotificationType
                     },
                     new NotificationEntity
                     {
                         Hearing = activePublicHearing,
                         User = citizen,
                         NotificationType = conclusionPublishedNotificationType
                     },
                     new NotificationEntity
                     {
                         Hearing = activePublicHearing,
                         User = citizen,
                         NotificationType = hearingChangedNotificationType
                     },
                 });

                await context.SaveChangesAsync();
            }

            if (!context.Events.Any())
            {
                var draftHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 1");
                var empOneUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == empOneIdentifier);

                var dailyStatusNotificationType = await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.DAILY_STATUS);

                await context.Events.AddRangeAsync(new List<EventEntity>
                {
                    new EventEntity
                    {
                        Hearing = draftHearing,
                        NotificationType = dailyStatusNotificationType,
                        User = empOneUser,
                        IsSentInNotification = false,
                        Type = EventType.HEARING_OWNER_CHANGED
                    }
                 });

                await context.SaveChangesAsync();
            }

            if (!context.InvitationGroups.Any())
            {
                await context.InvitationGroups.AddRangeAsync(new List<InvitationGroupEntity>
                {
                    new InvitationGroupEntity
                    {
                        Name = "Interne medarbejdere"
                    },
                    new InvitationGroupEntity
                    {
                        Name = "Bestyrelsen"
                    },
                    new InvitationGroupEntity
                    {
                        Name = "Husejere i lokalområde 21"
                    }
                });

                await context.SaveChangesAsync();
            }

            if (!context.InvitationKeys.Any())
            {
                var invitationGroupInternalEmployees = await context.InvitationGroups.FirstOrDefaultAsync(x => x.Name == "Interne medarbejdere");
                var invitationGroupBoardMembers = await context.InvitationGroups.FirstOrDefaultAsync(x => x.Name == "Bestyrelsen");
                var invitationGroupLocalArea = await context.InvitationGroups.FirstOrDefaultAsync(x => x.Name == "Husejere i lokalområde 21");

                await context.InvitationKeys.AddRangeAsync(new List<InvitationKeyEntity>
                {
                    new InvitationKeyEntity
                    {
                        InvitationGroup = invitationGroupInternalEmployees,
                        Email = "empone@novataris.com"
                    },
                    new InvitationKeyEntity
                    {
                        InvitationGroup = invitationGroupBoardMembers,
                        Email = "dev@novataris.com"
                    },
                    new InvitationKeyEntity
                    {
                        InvitationGroup = invitationGroupBoardMembers,
                        Cpr = "0306874438"
                    },
                    new InvitationKeyEntity
                    {
                        InvitationGroup = invitationGroupBoardMembers,
                        Cvr = "24555372"
                    },
                    new InvitationKeyEntity
                    {
                        InvitationGroup = invitationGroupLocalArea,
                        Cpr = "0306874438"
                    },
                    new InvitationKeyEntity
                    {
                        InvitationGroup = invitationGroupLocalArea,
                        Cpr = "0306874438"
                    }
                });

                await context.SaveChangesAsync();
            }

            if (!context.InvitationGroupMappings.Any())
            {
                var invitationGroupInternalEmployees = await context.InvitationGroups.FirstOrDefaultAsync(x => x.Name == "Interne medarbejdere");
                var invitationGroupBoardMembers = await context.InvitationGroups.FirstOrDefaultAsync(x => x.Name == "Bestyrelsen");
                var invitationGroupLocalArea = await context.InvitationGroups.FirstOrDefaultAsync(x => x.Name == "Husejere i lokalområde 21");

                var hearingTypeInternalStandard = await context.HearingTypes.FirstOrDefaultAsync(x => x.Name == "Intern - Standard");
                var hearingTypePublicStandard = await context.HearingTypes.FirstOrDefaultAsync(x => x.Name == "Offentlig - Standard");
                var hearingTypePublicInactive = await context.HearingTypes.FirstOrDefaultAsync(x => x.Name == "Offentlig - Inaktiv");

                await context.InvitationGroupMappings.AddRangeAsync(new List<InvitationGroupMappingEntity>
                {
                    new InvitationGroupMappingEntity
                    {
                        InvitationGroup = invitationGroupInternalEmployees,
                        HearingType = hearingTypeInternalStandard
                    },
                    new InvitationGroupMappingEntity
                    {
                        InvitationGroup = invitationGroupBoardMembers,
                        HearingType = hearingTypeInternalStandard
                    },
                    new InvitationGroupMappingEntity
                    {
                        InvitationGroup = invitationGroupBoardMembers,
                        HearingType = hearingTypePublicInactive
                    },
                    new InvitationGroupMappingEntity
                    {
                        InvitationGroup = invitationGroupLocalArea,
                        HearingType = hearingTypePublicStandard
                    }
                });

                await context.SaveChangesAsync();
            }

            if (!context.InvitationSourceMappings.Any())
            {
                var personalInvitationSource = await context.InvitationSources.FirstOrDefaultAsync(x => x.InvitationSourceType == InvitationSourceType.PERSONAL);

                var companies = await context.Companies.ToListAsync();
                var companyOne = companies.FirstOrDefault(x => x.Cvr == companyCvr);

                var activePublicHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 4");
                var activeInternalHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 5");
                var activeClosedSubjectHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 6");
                var awaitingConclusionHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 7");
                var concludedHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 8");
                var concludedOldHearing = await context.Hearings.FirstOrDefaultAsync(x => x.EsdhTitle == "Høring 9");

                var empOneUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == empOneIdentifier);
                var citOneUser = await context.UsersDb.FirstOrDefaultAsync(x => x.Identifier == citIdentifier);

                var userHearingRoleActiveInternalHearingEmpOneUser = await context.UserHearingRoles.FirstOrDefaultAsync(x => x.HearingId == activeInternalHearing.Id && x.UserId == empOneUser.Id);
                var userHearingRoleActiveClosedSubjectHearingCitOneUser = await context.UserHearingRoles.FirstOrDefaultAsync(x => x.HearingId == activeClosedSubjectHearing.Id && x.UserId == citOneUser.Id);
                var userHearingRoleActivePublicHearingCitOneUser = await context.UserHearingRoles.FirstOrDefaultAsync(x => x.HearingId == activePublicHearing.Id && x.UserId == citOneUser.Id);
                var userHearingRoleAwaitingConclusionHearingEmpOneUser = await context.UserHearingRoles.FirstOrDefaultAsync(x => x.HearingId == awaitingConclusionHearing.Id && x.UserId == empOneUser.Id);
                var userHearingRoleConcludedHearingCitOneUser = await context.UserHearingRoles.FirstOrDefaultAsync(x => x.HearingId == concludedHearing.Id && x.UserId == citOneUser.Id);
                var companyHearingRoleActivePublishedHearingCompanyOne = await context.CompanyHearingRoles.FirstOrDefaultAsync(x => x.HearingId == activePublicHearing.Id && x.CompanyId == companyOne.Id);
                var companyHearingRoleConcludedOldHearingCompanyOne = await context.CompanyHearingRoles.FirstOrDefaultAsync(x => x.HearingId == concludedOldHearing.Id && x.CompanyId == companyOne.Id);

                await context.InvitationSourceMappings.AddRangeAsync(new List<InvitationSourceMappingEntity>
                {
                    new InvitationSourceMappingEntity
                    {
                        SourceName = personalInvitationSource.Name,
                        InvitationSourceId = personalInvitationSource.Id,
                        UserHearingRoleId = userHearingRoleActiveInternalHearingEmpOneUser.Id
                    },
                    new InvitationSourceMappingEntity
                    {
                        SourceName = personalInvitationSource.Name,
                        InvitationSourceId = personalInvitationSource.Id,
                        UserHearingRoleId = userHearingRoleActiveClosedSubjectHearingCitOneUser.Id
                    },
                    new InvitationSourceMappingEntity
                    {
                        SourceName = personalInvitationSource.Name,
                        InvitationSourceId = personalInvitationSource.Id,
                        UserHearingRoleId = userHearingRoleActivePublicHearingCitOneUser.Id
                    },
                    new InvitationSourceMappingEntity
                    {
                        SourceName = personalInvitationSource.Name,
                        InvitationSourceId = personalInvitationSource.Id,
                        UserHearingRoleId = userHearingRoleAwaitingConclusionHearingEmpOneUser.Id
                    },
                    new InvitationSourceMappingEntity
                    {
                        SourceName = personalInvitationSource.Name,
                        InvitationSourceId = personalInvitationSource.Id,
                        UserHearingRoleId = userHearingRoleConcludedHearingCitOneUser.Id
                    },
                    new InvitationSourceMappingEntity
                    {
                        SourceName = personalInvitationSource.Name,
                        InvitationSourceId = personalInvitationSource.Id,
                        CompanyHearingRoleId = companyHearingRoleActivePublishedHearingCompanyOne.Id
                    },
                    new InvitationSourceMappingEntity
                    {
                        SourceName = personalInvitationSource.Name,
                        InvitationSourceId = personalInvitationSource.Id,
                        CompanyHearingRoleId = companyHearingRoleConcludedOldHearingCompanyOne.Id
                    }
                });

                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedDefaultDataAsync(ApplicationDbContext context, IKleService kleService,
            IMapper mapper)
        {
            await DefaultGlobalContentType.SeedData(context);
            await DefaultGlobalContent.SeedData(context);
            await DefaultHearingRoles.SeedData(context);
            await DefaultUserCapacity.SeedData(context);
            await DefaultHearingStatus.SeedData(context);
            await DefaultFieldType.SeedData(context);
            await DefaultContentType.SeedData(context);
            await DefaultFieldTypeSpecification.SeedData(context);
            await DefaultHearingTemplate.SeedData(context);
            await DefaultValidationRules.SeedData(context);
            await DefaultFields.SeedData(context);
            await DefaultCommentType.SeedData(context);
            await DefaultCommentStatus.SeedData(context);
            await DefaultJournalizedStatus.SeedData(context);
            await DefaultNotificationContentType.SeedData(context);
            await DefaultNotificationTemplates.SeedData(context);
            await DefaultNotificationType.SeedData(context);
            await DefaultInvitationSources.SeedData(context);

            // Seed Municipality specific default data:
            if (MunicipalityProfile.IsBallerupMunicipalityProfile())
            {
                await SeedBallerupDefaultDataAsync(context);
            }

            if (MunicipalityProfile.IsCopenhagenMunicipalityProfile())
            {
                await SeedCopenhagenDefaultDataAsync(context);
            }

            if (!context.KleHierarchies.Any())
            {
                var kleHierarchiesModels = await kleService.GetKleHierarchies();

                var kleHierarchiesEntities = kleHierarchiesModels.Select(mapper.Map<KleHierarchyEntity>);

                await context.KleHierarchies.AddRangeAsync(kleHierarchiesEntities);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedCopenhagenDefaultDataAsync(ApplicationDbContext context)
        {
            await DefaultValidationRules.SeedData(context, CopenhagenValidationRules.GetEntities());
            await DefaultInvitationSources.SeedData(context, CopenhagenInvitationSources.GetEntities());
        }

        public static async Task SeedBallerupDefaultDataAsync(ApplicationDbContext context)
        {
            await DefaultValidationRules.SeedData(context, BallerupValidationRules.GetEntities());
            var ballerupFields = await BallerupFields.GetEntities(context);
            await DefaultFields.SeedData(context, ballerupFields);
            var ballerupNotificationTemplates = await BallerupNotificationTemplates.GetEntities(context);
            await DefaultNotificationTemplates.SeedData(context, ballerupNotificationTemplates);
        }
    }
}