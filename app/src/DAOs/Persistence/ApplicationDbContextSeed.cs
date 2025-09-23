using AutoMapper;
using BallerupKommune.DAOs.Identity;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Entities.Enums;
using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BallerupKommune.DAOs.Persistence.DefaultData;

namespace BallerupKommune.DAOs.Persistence
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

            if (!context.GlobalContentTypes.Any())
            {
                await context.GlobalContentTypes.AddAsync(new GlobalContentTypeEntity
                {
                    Name = "Vilkår og betingelser",
                    Type = GlobalContentType.TERMS_AND_CONDITIONS
                });
                await context.SaveChangesAsync();
            }

            if (!context.GlobalContents.Any())
            {
                var globalContentType = await context.GlobalContentTypes.SingleAsync();

                await context.GlobalContents.AddAsync(new GlobalContentEntity
                {
                    Content =
                        "Ved afgivelse af høringssvar accepterer du at svaret vil blive brugt i forbindelse med konklusionen på høringen",
                    Version = 1,
                    GlobalContentType = globalContentType,
                });
                await context.SaveChangesAsync();
            }

            if (!context.UserCapacities.Any())
            {
                await context.UserCapacities.AddRangeAsync(new List<UserCapacityEntity>
                {
                    new UserCapacityEntity
                    {
                        Capacity = UserCapacity.CITIZEN
                    },
                    new UserCapacityEntity
                    {
                        Capacity = UserCapacity.EMPLOYEE
                    },
                    new UserCapacityEntity
                    {
                        Capacity = UserCapacity.COMPANY
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
                        Name = "Novataris"
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
                        UserCapacityId = citizenCapacity.Id
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

            if (!context.HearingRoles.Any())
            {
                await context.HearingRoles.AddRangeAsync(new List<HearingRoleEntity>
                {
                    new HearingRoleEntity
                    {
                        Name = "Høringsejer",
                        Role = HearingRole.HEARING_OWNER
                    },
                    new HearingRoleEntity
                    {
                        Name = "Inviteret",
                        Role = HearingRole.HEARING_INVITEE
                    },
                    new HearingRoleEntity
                    {
                        Name = "Reviewer",
                        Role = HearingRole.HEARING_REVIEWER
                    },
                    new HearingRoleEntity
                    {
                        Name = "Besvarer",
                        Role = HearingRole.HEARING_RESPONDER
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.HearingStatus.Any())
            {
                await context.HearingStatus.AddRangeAsync(new List<HearingStatusEntity>
                {
                    new HearingStatusEntity
                    {
                        Name = "Oprettet",
                        Status = HearingStatus.CREATED
                    },
                    new HearingStatusEntity
                    {
                        Name = "Kladde",
                        Status = HearingStatus.DRAFT
                    },
                    new HearingStatusEntity
                    {
                        Name = "Afventer startdato",
                        Status = HearingStatus.AWAITING_STARTDATE
                    },
                    new HearingStatusEntity
                    {
                        Name = "Aktiv",
                        Status = HearingStatus.ACTIVE
                    },
                    new HearingStatusEntity
                    {
                        Name = "Afventer konklusion",
                        Status = HearingStatus.AWAITING_CONCLUSION
                    },
                    new HearingStatusEntity
                    {
                        Name = "Konkluderet",
                        Status = HearingStatus.CONCLUDED
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.FieldTypes.Any())
            {
                await context.FieldTypes.AddRangeAsync(new List<FieldTypeEntity>
                {
                    new FieldTypeEntity
                    {
                        Type = FieldType.TITLE
                    },
                    new FieldTypeEntity
                    {
                        Type = FieldType.ESDH_TITLE
                    },
                    new FieldTypeEntity
                    {
                        Type = FieldType.IMAGE
                    },
                    new FieldTypeEntity
                    {
                        Type = FieldType.SUMMARY
                    },
                    new FieldTypeEntity
                    {
                        Type = FieldType.BODYINFORMATION
                    },
                    new FieldTypeEntity
                    {
                        Type = FieldType.CONCLUSION
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.ContentTypes.Any())
            {
                await context.ContentTypes.AddRangeAsync(new List<ContentTypeEntity>
                {
                    new ContentTypeEntity
                    {
                        Type = ContentType.TEXT
                    },
                    new ContentTypeEntity
                    {
                        Type = ContentType.FILE
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.FieldTypeSpecifications.Any())
            {
                var fieldTypes = await context.FieldTypes.ToListAsync();
                var contentTypes = await context.ContentTypes.ToListAsync();

                await context.AddRangeAsync(new List<FieldTypeSpecificationEntity>
                {
                    new FieldTypeSpecificationEntity
                    {
                        FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.TITLE),
                        ContentType = contentTypes.First(contentType => contentType.Type == ContentType.TEXT)
                    },
                    new FieldTypeSpecificationEntity
                    {
                        FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.IMAGE),
                        ContentType = contentTypes.First(contentType => contentType.Type == ContentType.FILE)
                    },
                    new FieldTypeSpecificationEntity
                    {
                        FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.IMAGE),
                        ContentType = contentTypes.First(contentType => contentType.Type == ContentType.TEXT)
                    },
                    new FieldTypeSpecificationEntity
                    {
                        FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.SUMMARY),
                        ContentType = contentTypes.First(contentType => contentType.Type == ContentType.TEXT)
                    },
                    new FieldTypeSpecificationEntity
                    {
                        FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.BODYINFORMATION),
                        ContentType = contentTypes.First(contentType => contentType.Type == ContentType.TEXT)
                    },
                    new FieldTypeSpecificationEntity
                    {
                        FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.BODYINFORMATION),
                        ContentType = contentTypes.First(contentType => contentType.Type == ContentType.FILE)
                    },
                    new FieldTypeSpecificationEntity
                    {
                        FieldType = fieldTypes.First(fieldType => fieldType.Type == FieldType.CONCLUSION),
                        ContentType = contentTypes.First(contentType => contentType.Type == ContentType.TEXT)
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.HearingTemplates.Any())
            {
                await context.HearingTemplates.AddRangeAsync(new List<HearingTemplateEntity>
                {
                    new HearingTemplateEntity
                    {
                        Name = "Standard høringsskabelon"
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

            if (!context.ValidationRules.Any())
            {
                await context.AddRangeAsync(new List<ValidationRuleEntity>
                { 
                    // Titel
                    new ValidationRuleEntity
                    {
                        CanBeEmpty = false,
                        MaxLength = 60,
                        FieldType = FieldType.TITLE,
                    },
                    // Esdh Titel
                    new ValidationRuleEntity
                    {
                        CanBeEmpty = false,
                        MaxLength = 110,
                        FieldType = FieldType.ESDH_TITLE,
                    },
                    // Billede
                    new ValidationRuleEntity
                    {
                        AllowedFileTypes = new[]
                        {
                            "image/jpeg",
                            "image/png",
                            "image/svg"
                        },
                        MaxFileSize = 1000000,
                        CanBeEmpty = true,
                        FieldType = FieldType.IMAGE,
                    },
                    // Resumé
                    new ValidationRuleEntity
                    {
                        MaxLength = 500,
                        FieldType = FieldType.SUMMARY,
                    },
                    // Brød tekst
                    new ValidationRuleEntity
                    {
                        MaxFileSize = 100000000,
                        FieldType = FieldType.BODYINFORMATION,
                    },
                    new ValidationRuleEntity
                    {
                        MaxLength = 1000,
                        CanBeEmpty = false,
                        FieldType = Entities.Enums.FieldType.CONCLUSION,
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.Fields.Any())
            {
                var titleFieldType = await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.TITLE);
                var esdhTitleFieldType =
                    await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.ESDH_TITLE);
                var imageFieldType = await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.IMAGE);
                var summaryFieldType = await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.SUMMARY);
                var bodyinformationFieldType =
                    await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.BODYINFORMATION);
                var conclusionFieldType =
                    await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.CONCLUSION);

                var textFieldTemplate = await context.HearingTemplates.SingleAsync();
                var hearingType = await context.HearingTypes.FirstOrDefaultAsync(x => x.IsActive);

                var titleValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.TITLE);
                var esdhTitleValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.ESDH_TITLE);
                var imageValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.IMAGE);
                var summaryValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.SUMMARY);
                var bodyInformationValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.BODYINFORMATION);
                var conclusionValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.CONCLUSION);

                await context.Fields.AddRangeAsync(new List<FieldEntity>
                {
                    new FieldEntity
                    {
                        Name = "Titel",
                        AllowTemplates = true,
                        DisplayOrder = 100,
                        FieldType = titleFieldType,
                        HearingTemplate = textFieldTemplate,
                        FieldTemplates = new List<FieldTemplateEntity>
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
                        },
                        ValidationRule = titleValidationRule,
                    },
                    new FieldEntity
                    {
                        Name = "ESDH Titel",
                        AllowTemplates = true,
                        DisplayOrder = 200,
                        FieldType = esdhTitleFieldType,
                        HearingTemplate = textFieldTemplate,
                        FieldTemplates = new List<FieldTemplateEntity>
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
                        },
                        ValidationRule = esdhTitleValidationRule,
                    },
                    new FieldEntity
                    {
                        Name = "Billede",
                        AllowTemplates = false,
                        DisplayOrder = 300,
                        FieldType = imageFieldType,
                        HearingTemplate = textFieldTemplate,
                        ValidationRule = imageValidationRule,
                    },
                    new FieldEntity
                    {
                        Name = "Resumé",
                        AllowTemplates = false,
                        DisplayOrder = 400,
                        FieldType = summaryFieldType,
                        HearingTemplate = textFieldTemplate,
                        ValidationRule = summaryValidationRule
                    },
                    new FieldEntity
                    {
                        Name = "Brød tekst",
                        AllowTemplates = true,
                        DisplayOrder = 600,
                        FieldType = bodyinformationFieldType,
                        HearingTemplate = textFieldTemplate,
                        FieldTemplates = new List<FieldTemplateEntity>
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
                        },
                        ValidationRule = bodyInformationValidationRule
                    },
                    new FieldEntity
                    {
                        Name = "Konklusion",
                        AllowTemplates = true,
                        DisplayOrder = 500,
                        FieldType = conclusionFieldType,
                        HearingTemplate = textFieldTemplate,
                        FieldTemplates = new List<FieldTemplateEntity>
                        {
                            new FieldTemplateEntity
                            {
                                Name = "En god konklusion",
                                Text = "Denne konklusion er et godt udgangspunkt. [indsæt tekst]",
                                HearingType = hearingType
                            }
                        },
                        ValidationRule = conclusionValidationRule,
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.CommentTypes.Any())
            {
                await context.CommentTypes.AddRangeAsync(new List<CommentTypeEntity>
                {
                    new CommentTypeEntity
                    {
                        Type = CommentType.HEARING_RESPONSE
                    },
                    new CommentTypeEntity
                    {
                        Type = CommentType.HEARING_REVIEW
                    },
                    new CommentTypeEntity
                    {
                        Type = CommentType.HEARING_RESPONSE_REPLY
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.CommentStatuses.Any())
            {
                var hearingResponseCommentType =
                    await context.CommentTypes.FirstOrDefaultAsync(x => x.Type == CommentType.HEARING_RESPONSE);
                var hearingReviewCommentType =
                    await context.CommentTypes.FirstOrDefaultAsync(x => x.Type == CommentType.HEARING_REVIEW);
                var hearingResponseReplyCommentType =
                    await context.CommentTypes.FirstOrDefaultAsync(x => x.Type == CommentType.HEARING_RESPONSE_REPLY);

                await context.CommentStatuses.AddRangeAsync(new List<CommentStatusEntity>
                {
                    new CommentStatusEntity
                    {
                        CommentType = hearingResponseCommentType,
                        Status = CommentStatus.AWAITING_APPROVAL
                    },
                    new CommentStatusEntity
                    {
                        CommentType = hearingResponseCommentType,
                        Status = CommentStatus.APPROVED
                    },
                    new CommentStatusEntity
                    {
                        CommentType = hearingResponseCommentType,
                        Status = CommentStatus.NOT_APPROVED
                    },
                    new CommentStatusEntity
                    {
                        CommentType = hearingReviewCommentType,
                        Status = CommentStatus.NONE
                    },
                    new CommentStatusEntity
                    {
                        CommentType = hearingResponseReplyCommentType,
                        Status = CommentStatus.NONE
                    }
                });
                await context.SaveChangesAsync();
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

                var internalHearingType = await context.HearingTypes.FirstOrDefaultAsync(x => x.IsInternalHearing);
                var publicHearingType = await context.HearingTypes.FirstOrDefaultAsync(x => !x.IsInternalHearing);

                var activeSubjectArea = await context.SubjectAreas.FirstOrDefaultAsync(x => x.IsActive);
                var closedSubjectArea = await context.SubjectAreas.FirstOrDefaultAsync(x => !x.IsActive);

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
                        ShowComments = true
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
                        ShowComments = false
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
                        ShowComments = true
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
                        ShowComments = true
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
                        ShowComments = true
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
                        ShowComments = false
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
                        ShowComments = true
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
                        EsdhTitle = "Høring 8",
                        EsdhNumber = "K09-10.10.10-2020.19",
                        HearingType = publicHearingType,
                        SubjectArea = closedSubjectArea,
                        ShowComments = false
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
                        CommentDeclineInfo = new CommentDeclineInfoEntity(){
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
                        CommentDeclineInfo = new CommentDeclineInfoEntity(){
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
                        CommentDeclineInfo = new CommentDeclineInfoEntity() { 
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
                        CommentDeclineInfo = new CommentDeclineInfoEntity() { 
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
                });

                await context.SaveChangesAsync();

            }

            if (!context.Consents.Any())
            {
                var globalContent = await context.GlobalContents.FirstOrDefaultAsync();

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
                    }
                });
                await context.SaveChangesAsync();
            }

            if (!context.KleHierarchies.Any())
            {
                var kleHierarchiesModels = await kleService.GetKleHierarchies();

                var kleHierarchiesEntities = kleHierarchiesModels.Select(x => mapper.Map<KleHierarchyEntity>(x));

                await context.KleHierarchies.AddRangeAsync(kleHierarchiesEntities);
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

            if (!context.JournalizedStatuses.Any())
            {
                await context.JournalizedStatuses.AddRangeAsync(new List<JournalizedStatusEntity>
                {
                    new JournalizedStatusEntity
                    {
                        Status = JournalizedStatus.NOT_JOURNALIZED
                    },
                    new JournalizedStatusEntity
                    {
                        Status = JournalizedStatus.JOURNALIZED
                    },
                    new JournalizedStatusEntity
                    {
                        Status = JournalizedStatus.JOURNALIZED_WITH_ERROR
                    }
                });

                await context.SaveChangesAsync();
            }

            if (!context.NotificationTemplates.Any())
            {
                await context.NotificationTemplates.AddRangeAsync(new List<NotificationTemplateEntity>
                {
                    new NotificationTemplateEntity {
                        NotificationTemplateText =
                                @"<li>Reviewer: Der er tilføjet følgende reviewere: <ul><li>{{Reviewers}}</li></ul></li>",
                        SubjectTemplate = "Tilføjet reviewers"
                    },
                    new NotificationTemplateEntity
                    {
                        NotificationTemplateText =
                                @"<li>Høringsejer: Høringen har fået en ny høringsejer: <strong>{{HearingOwner}}.</strong></li>",
                        SubjectTemplate = "Ny høringsejer"
                    },
                    new NotificationTemplateEntity
                    {
                        NotificationTemplateText =
                                @"<li>Statusskifte: Høringen har skiftet status til: <strong>{{HearingStatus}}.</strong></li>",
                        SubjectTemplate = "Høringen har skiftet status"
                    },
                    new NotificationTemplateEntity
                    {
                        NotificationTemplateText =
                                @"<li>Høringssvar: Der er modtaget <strong>{{HearingResponseCount}}</strong> høringssvar.</li>",
                        SubjectTemplate = "Nye høringssvar"
                    },
                    new NotificationTemplateEntity
                    {
                        NotificationTemplateText =
                                @"<li>Høringskommentarer: Der er modtaget <strong>{{HearingReviewCount}}</strong> høringskommentarer.</li>",
                        SubjectTemplate = "Nye høringskommentarer"
                    },
                    new NotificationTemplateEntity
                    {
                        NotificationTemplateText =
                                @"{{HearingTitle}}{{NewLine}}{{NewLine}}Vi inviterer dig til at deltage i denne høring.{{NewLine}}Du kan se detaljerne om høringen og indsende dit høringssvar på dette link: {{LinkToHearing}}{{NewLine}}Vær opmærksom på at du skal logge ind med MitId.{{NewLine}}{{NewLine}}{{TermsAndConditions}}{{NewLine}}{{NewLine}}Med venlig hilsen{{NewLine}}Ballerup Kommune",
                        SubjectTemplate = "Offentlig høring"
                    },
                    new NotificationTemplateEntity
                    {
                        NotificationTemplateText =
                                @"Dit høringssvar{{CompanyResponder}} er blevet afvist på høringen: {{HearingTitle}}.{{NewLine}}{{NewLine}}Dette kan være sket fordi dit svar enten vurderes til ikke at høre til den aktuelle høring, eller at det indeholdt informationer og/eller persondata som vurderes ikke at være lovlige at offentliggøre i en høringssammenhæng.{{NewLine}}{{NewLine}}Nedenstående er høringsadministratorens begrundelse for afvisning af høringssvar {{CommentNumber}}:{{CommentDeclinedReason}}{{NewLine}}{{NewLine}}Det fulde svar indgår dog i den videre behandling.{{NewLine}}{{NewLine}}{{TermsAndConditions}}{{NewLine}}{{NewLine}}Med venlig hilsen{{NewLine}}Ballerup Kommune",
                        SubjectTemplate = "Høringssvar afvist"
                    },
                    new NotificationTemplateEntity
                    {
                        NotificationTemplateText =
                                @"Der foreligger nu en konklusion på høringen: {{HearingTitle}}, som du har svaret på.{{NewLine}}{{NewLine}}Du kan se konklusionen på høringen her: {{LinkToHearing}}{{NewLine}}{{NewLine}}Med venlig hilsen{{NewLine}}Ballerup Kommune",
                        SubjectTemplate = "Høring konkluderet"
                    },
                    new NotificationTemplateEntity
                    {
                        NotificationTemplateText =
                            @"Der er foretaget en ændring på høringen: {{HearingTitle}}, som du har svaret på.{{NewLine}}{{NewLine}}Der er tale om en mindre rettelse, men dette kan stadig have indflydelse på dit afgivne svar.{{NewLine}} Du kan se den opdaterede høringstekst samt rette dit svar her: {{LinkToHearing}}{{NewLine}}{{NewLine}}Med venlig hilsen{{NewLine}}Ballerup Kommune",
                        SubjectTemplate = "Høring opdateret"
                    },
                    new NotificationTemplateEntity
                    {
                        NotificationTemplateText =
                                @"Tak for dit høringssvar til høringen: {{HearingTitle}}.{{NewLine}}{{NewLine}}Svaret{{CompanyResponder}} indgår nu i det videre arbejde med høringen.{{NewLine}}Hvis dit svar indeholder følsomme personoplysninger, forbeholder vi os ret til ikke at publicere dit svar på høringsportalen.{{NewLine}}Det fulde svar indgår dog i den videre behandling.{{NewLine}}{{NewLine}}{{TermsAndConditions}}{{NewLine}}{{NewLine}}Med venlig hilsen{{NewLine}}Ballerup Kommune",
                        SubjectTemplate = "Kvittering for dit høringssvar"
                    }
                });

                await context.SaveChangesAsync();
            }

            if (!context.NotificationTypes.Any())
            {
                string addedReviewersString = "Tilføjet reviewers";
                string newHearingOwnerString = "Ny høringsejer";
                string hearingStatusChangeString = "Høringen har skiftet status";
                string newHearingAnswersString = "Nye høringssvar";
                string newHearingCommentsString = "Nye høringskommentarer";
                string publicHearingString = "Offentlig høring";
                string hearingAnswerDeclinedString = "Høringssvar afvist";
                string hearingAnswerConfirmationString = "Kvittering for dit høringssvar";
                string hearingConcludedString = "Høring konkluderet";
                string hearingUpdatedString = "Høring opdateret";

                NotificationTemplateEntity addedReviewersTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.SubjectTemplate == addedReviewersString);
                NotificationTemplateEntity newHearingOwnerTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.SubjectTemplate == newHearingOwnerString);
                NotificationTemplateEntity hearingStatusChangeTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.SubjectTemplate == hearingStatusChangeString);
                NotificationTemplateEntity newHearingAnswersTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.SubjectTemplate == newHearingAnswersString);
                NotificationTemplateEntity newHearingCommentsTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.SubjectTemplate == newHearingCommentsString);
                NotificationTemplateEntity publicHearingTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.SubjectTemplate == publicHearingString);
                NotificationTemplateEntity hearingAnswerDeclinedTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.SubjectTemplate == hearingAnswerDeclinedString);
                NotificationTemplateEntity hearingAnswerConfirmationTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.SubjectTemplate == hearingAnswerConfirmationString);
                NotificationTemplateEntity hearingConcludedTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.SubjectTemplate == hearingConcludedString);
                NotificationTemplateEntity hearingUpdatedTemplate = await context.NotificationTemplates.FirstOrDefaultAsync(x => x.SubjectTemplate == hearingUpdatedString);

                context.NotificationTypes.AddRange(new List<NotificationTypeEntity>
                {
                    new NotificationTypeEntity
                    {
                        Frequency = NotificationFrequency.DAILY,
                        Name = "Tilføjet som reviewer",
                        Type = NotificationType.ADDED_AS_REVIEWER,
                        NotificationTemplate = addedReviewersTemplate
                    },
                    new NotificationTypeEntity
                    {
                        Frequency = NotificationFrequency.DAILY,
                        Name = "Høringsejer skiftet",
                        Type = NotificationType.CHANGED_HEARING_OWNER,
                        NotificationTemplate = newHearingOwnerTemplate
                    },
                    new NotificationTypeEntity
                    {
                        Frequency = NotificationFrequency.DAILY,
                        Name = "Høringstatus skiftet",
                        Type = NotificationType.CHANGED_HEARING_STATUS,
                        NotificationTemplate = hearingStatusChangeTemplate
                    },
                    new NotificationTypeEntity
                    {
                        Frequency = NotificationFrequency.DAILY,
                        Name = "Høringssvar modtaget",
                        Type = NotificationType.HEARING_RESPONSE_RECEIVED,
                        NotificationTemplate = newHearingAnswersTemplate
                    },
                    new NotificationTypeEntity
                    {
                        Frequency = NotificationFrequency.DAILY,
                        Name = "Høringskommentar modtaget",
                        Type = NotificationType.HEARING_REVIEW_RECEIVED,
                        NotificationTemplate = newHearingCommentsTemplate
                    },
                    new NotificationTypeEntity
                    {
                        Frequency = NotificationFrequency.INSTANT,
                        Name = "Inviteret til høring",
                        Type = NotificationType.INVITED_TO_HEARING,
                        NotificationTemplate = publicHearingTemplate
                    },
                    new NotificationTypeEntity
                    {
                        Frequency = NotificationFrequency.INSTANT,
                        Name = "Kvittering for høringssvar",
                        Type = NotificationType.HEARING_ANSWER_RECEIPT,
                        NotificationTemplate = hearingAnswerConfirmationTemplate
                    },
                    new NotificationTypeEntity
                    {
                        Frequency = NotificationFrequency.INSTANT,
                        Name = "Høringskonklusion publiseret",
                        Type = NotificationType.HEARING_CONCLUSION_PUBLISHED,
                        NotificationTemplate = hearingConcludedTemplate
                    },
                    new NotificationTypeEntity
                    {
                        Frequency = NotificationFrequency.INSTANT,
                        Name = "Høring ændret",
                        Type = NotificationType.HEARING_CHANGED,
                        NotificationTemplate = hearingUpdatedTemplate
                    },
                    new NotificationTypeEntity
                    {
                        Frequency = NotificationFrequency.INSTANT,
                        Name = "Høringssvar afvist",
                        Type = NotificationType.HEARING_RESPONSE_DECLINED,
                        NotificationTemplate = hearingAnswerDeclinedTemplate
                    }
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

                var addedAsReviewerNotificationType =
                    await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.ADDED_AS_REVIEWER);
                var changeHearingOwnerNotificationType =
                    await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.CHANGED_HEARING_OWNER);
                var changeHearingStatusNotificationType =
                    await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.CHANGED_HEARING_STATUS);
                var hearingResponseNotificationType =
                    await context.NotificationTypes.FirstAsync(
                        x => x.Type == NotificationType.HEARING_RESPONSE_RECEIVED);
                var hearingReviewNotificationType =
                    await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.HEARING_REVIEW_RECEIVED);

                var invitedToHearingNotificationType =
                    await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.INVITED_TO_HEARING);
                var hearingAnswerReceiptNotificationType =
                    await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.HEARING_ANSWER_RECEIPT);
                var conclusionPublishedNotificationType =
                    await context.NotificationTypes.FirstAsync(x =>
                        x.Type == NotificationType.HEARING_CONCLUSION_PUBLISHED);
                var hearingChangedNotificationType =
                    await context.NotificationTypes.FirstAsync(x => x.Type == NotificationType.HEARING_CHANGED);

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
                        User = empTwoUser,
                        NotificationType = changeHearingOwnerNotificationType
                    },
                    new NotificationEntity
                    {
                        Hearing = activePublicHearing,
                        User = empTwoUser,
                        NotificationType = changeHearingStatusNotificationType
                    },
                    new NotificationEntity
                    {
                        Hearing = activePublicHearing,
                        User = empTwoUser,
                        NotificationType = hearingResponseNotificationType
                    },
                    new NotificationEntity
                    {
                        Hearing = activePublicHearing,
                        User = empTwoUser,
                        NotificationType = hearingReviewNotificationType
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
            await DefaultNotificationTemplates.SeedData(context);
            await DefaultNotificationType.SeedData(context);
            if (!context.KleHierarchies.Any())
            {
                var kleHierarchiesModels = await kleService.GetKleHierarchies();

                var kleHierarchiesEntities = kleHierarchiesModels.Select(mapper.Map<KleHierarchyEntity>);

                await context.KleHierarchies.AddRangeAsync(kleHierarchiesEntities);
                await context.SaveChangesAsync();
            }
        }
    }
}