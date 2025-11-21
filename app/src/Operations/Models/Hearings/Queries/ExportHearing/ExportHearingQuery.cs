using Agora.Models.Common;
using Agora.Models.Enums;
using Agora.Models.Models;
using Agora.Models.Models.Records;
using Agora.Operations.Common.Extensions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Files.Excel;
using Agora.Operations.Common.Interfaces.Files.Pdf;
using Agora.Operations.Common.TextResolverKeys;
using Agora.Operations.Resolvers;
using MediatR;
using Microsoft.Extensions.Logging;
using NovaSec.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Operations.Common.Interfaces.Files;
using CommentType = Agora.Models.Enums.CommentType;
using ContentType = Agora.Models.Enums.ContentType;
using FieldType = Agora.Models.Enums.FieldType;
using HearingRole = Agora.Models.Enums.HearingRole;
using UserCapacity = Agora.Models.Enums.UserCapacity;
using Agora.Operations.Common.Interfaces.ExternalProcesses.Services;
using Agora.Operations.ApplicationOptions.OperationsOptions;
using Microsoft.Extensions.Options;

namespace Agora.Operations.Models.Hearings.Queries.ExportHearing
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.Id)")]
    public class ExportHearingQuery : IRequest<FileDownload>
    {
        public int Id { get; set; }
        public ExportFormat Format { get; set; }

        public class ExportHearingsQueryHandler : IRequestHandler<ExportHearingQuery, FileDownload>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IFieldSystemResolver _fieldSystemResolver;
            private readonly IPdfService _pdfService;
            private readonly IExcelService _excelService;
            private readonly IFileService _fileService;
            private readonly ITextResolver _textResolver;
            private readonly IHearingRoleResolver _hearingRoleResolver;
            private readonly IExternalPdfService _externalPdfService;
            private readonly ILogger<ExportHearingsQueryHandler> _logger;
            private readonly HearingOperationsOptions _options;

            public ExportHearingsQueryHandler(IHearingDao hearingDao, IFieldSystemResolver fieldSystemResolver, IPdfService pdfService, IExternalPdfService externalPdfService,
                IExcelService excelService, IFileService fileService, ITextResolver textResolver, IHearingRoleResolver hearingRoleResolver, ILogger<ExportHearingsQueryHandler> logger,
                IOptions<HearingOperationsOptions> options)
            {
                _hearingDao = hearingDao;
                _fieldSystemResolver = fieldSystemResolver;
                _pdfService = pdfService;
                _excelService = excelService;
                _fileService = fileService;
                _textResolver = textResolver;
                _hearingRoleResolver = hearingRoleResolver;
                _logger = logger;
                _externalPdfService = externalPdfService;
                _options = options.Value;
            }

            public async Task<FileDownload> Handle(ExportHearingQuery request, CancellationToken cancellationToken)
            {
                _logger.LogInformation("Exporting hearing with id={hearingId} in format={format}", request.Id, Enum.GetName(typeof(ExportFormat), request.Format));

                var hearing = await GetHearingAsync(request.Id);

                var titleContent = await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.TITLE);
                var titlePrefix = _textResolver.GetText(GroupKey.FileGeneration, TextKey.TitlePrefix);
                var title = $"{titlePrefix}{titleContent.TextContent}_{System.DateTime.Now:dd_MM_yyyy}";

                switch (request.Format)
                {
                    case ExportFormat.PDF:
                        {
                            var hearingReportPdf = await GeneratePdfReport(hearing);
                            await _fileService.SaveExportFileToDisk(hearingReportPdf, request.Id, ExportFormat.PDF, $"{title}.pdf");
                            return new FileDownload
                            {
                                ContentType = "application/pdf",
                                FileName = $"Rapport: {title}.pdf",
                                Content = hearingReportPdf
                            };
                        }
                    case ExportFormat.EXCEL:
                        {
                            var excelContent = await GenerateExcelReport(hearing);
                            await _fileService.SaveExportFileToDisk(excelContent, request.Id, ExportFormat.EXCEL, $"{title}.xlsx");
                            return new FileDownload
                            {
                                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                FileName = $"Rapport: {title}.xlsx",
                                Content = excelContent
                            };
                        }
                    case ExportFormat.FULL_PDF:
                        {
                            var fullHearingReportPdf = await GenerateFullHearingReport(hearing);
                            await _fileService.SaveExportFileToDisk(fullHearingReportPdf, request.Id, ExportFormat.FULL_PDF, $"{title}.pdf");
                            return new FileDownload
                            {
                                ContentType = "application/pdf",
                                FileName = $"Rapport: {title}.pdf",
                                Content = fullHearingReportPdf
                            };
                        }
                    case ExportFormat.USER_REPORT_EXCEL:
                        {
                            var excelUserReport = GenerateExcelHearingUserReport(hearing);
                            await _fileService.SaveExportFileToDisk(excelUserReport, request.Id, ExportFormat.USER_REPORT_EXCEL, $"{title}.xlsx");
                            return new FileDownload
                            {
                                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                FileName = $"Brugerdata: {title}.xlsx",
                                Content = excelUserReport
                            };
                        }
                    case ExportFormat.RESPONSE_REPORT_EXCEL:
                        {
                            var excelResponseReport = GenerateExcelResponseReport(hearing);
                            await _fileService.SaveExportFileToDisk(excelResponseReport, request.Id, ExportFormat.RESPONSE_REPORT_EXCEL, $"{title}.xlsx");
                            return new FileDownload
                            {
                                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                FileName = $"Bruger- og Svarrapport: {title}.xlsx",
                                Content = excelResponseReport
                            };
                        }
                    default:
                        return new FileDownload();
                }
            }

            private async Task<byte[]> GeneratePdfReport(Hearing hearing)
            {
                var hearingRecord = await GetBaseHearingRecord(hearing);

                if (_options.ExportHearing.UseExternalPdfService)
                {
                    return await _externalPdfService.GenerateReportAsync(ExportFormat.PDF, hearingRecord);
                }
                
                return _pdfService.CreateHearingReport(hearingRecord);
            }

            private async Task<byte[]> GenerateExcelReport(Hearing hearing)
            {
                var commentRecords = await GetCommentRecords(hearing);
                return _excelService.CreateHearingReport(commentRecords);
            }

            private byte[] GenerateExcelHearingUserReport(Hearing hearing)
            {
                var userRecords = GetUserRecords(hearing);
                return _excelService.CreateHearingUserReport(userRecords);
            }

            private byte[] GenerateExcelResponseReport(Hearing hearing)
            {
                var commentRecords = GetHearingResponseRecords(hearing);
                return _excelService.CreateHearingResponseReport(commentRecords);
            }

            private async Task<byte[]> GenerateFullHearingReport(Hearing hearing)
            {
                var hearingRecord = await GetFullHearingRecord(hearing);

                if (_options.ExportHearing.UseExternalPdfService)
                {
                    return await _externalPdfService.GenerateReportAsync(ExportFormat.FULL_PDF, hearingRecord);
                }

                return _pdfService.CreateFullHearingReport(hearingRecord);
            }

            private async Task<HearingRecord> GetBaseHearingRecord(Hearing hearing)
            {
                var titleContent = await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.TITLE);
                var commentRecords = await GetCommentRecords(hearing);

                return new HearingRecord
                {
                    BaseData = new HearingBaseData
                    {
                        Title = titleContent.TextContent,
                        EsdhNumber = hearing.EsdhNumber,
                    },
                    CommentRecords = commentRecords
                };
            }

            private async Task<HearingRecord> GetFullHearingRecord(Hearing hearing)
            {
                var titleContent = await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.TITLE);
                var summaryContent = await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.SUMMARY);
                var bodyTextContent = await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.BODYINFORMATION);
                var conclusionTextContent = await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.CONCLUSION);

                var bodyFileContent = await hearing.GetFileContentsOfFieldType(_fieldSystemResolver, FieldType.BODYINFORMATION);
                var attachments = bodyFileContent.Select(fileContent => new FileRecord
                {
                    Extension = fileContent.FileContentType,
                    FileName = fileContent.FileName,
                    FilePath = fileContent.FilePath
                }).ToList();

                var hearingImageContent = (await hearing.GetFileContentsOfFieldType(_fieldSystemResolver, FieldType.IMAGE)).FirstOrDefault();
                var hearingImageFile = hearingImageContent != null
                    ? new FileRecord
                    {
                        Extension = hearingImageContent.FileContentType,
                        FileName = hearingImageContent.FileName,
                        FilePath = hearingImageContent.FilePath
                    }
                    : null;

                var conclusionFileContent = await hearing.GetFileContentsOfFieldType(_fieldSystemResolver, FieldType.CONCLUSION);
                var conclusionFiles = conclusionFileContent.Select(fileContent => new FileRecord
                {
                    Extension = fileContent.FileContentType,
                    FileName = fileContent.FileName,
                    FilePath = fileContent.FilePath
                }).ToList();

                var commentRecords = await GetCommentRecords(hearing);
                var commentStats = GetCommentStats(commentRecords);

                var baseData = new HearingBaseData
                {
                    Id = hearing.Id,
                    SubjectArea = hearing.SubjectArea?.Name ?? string.Empty,
                    CityArea = hearing.CityArea?.Name ?? string.Empty,
                    StartDate = hearing.StartDate.Value,
                    Deadline = hearing.Deadline.Value,
                    ClosedHearing = hearing.ClosedHearing,
                    HearingType = hearing.HearingType.Name,
                    EsdhNumber = hearing.EsdhNumber,
                    Title = titleContent.TextContent,
                    Summary = summaryContent.TextContent,
                    BodyText = bodyTextContent.TextContent,
                    Conclusion = conclusionTextContent?.TextContent,
                    Image = hearingImageFile,
                };

                return new HearingRecord
                {
                    BaseData = baseData,
                    CommentStats = commentStats,
                    Attachments = attachments,
                    ConclusionAttachments = conclusionFiles,
                    CommentRecords = commentRecords,
                };
            }

            private CommentStats GetCommentStats(List<CommentRecord> commentRecords)
            {
                var responses = commentRecords.Where(record => record.Type == CommentType.HEARING_RESPONSE).ToList();
                var totalResponses = responses.Count;
                var onBehalfOfResponses = responses.Count(record => !string.IsNullOrEmpty(record.OnBehalfOf));
                var citizenResponses = responses.Count(record => record.ResponderCapacity == Agora.Models.Enums.UserCapacity.CITIZEN);
                var companyResponses = responses.Count(record => record.ResponderCapacity == Agora.Models.Enums.UserCapacity.COMPANY);
                var employeeResponses = responses.Count(record => record.ResponderCapacity == Agora.Models.Enums.UserCapacity.EMPLOYEE);

                return new CommentStats
                {
                    TotalResponses = totalResponses,
                    OnBehalfOfResponses = onBehalfOfResponses,
                    EmployeeResponses = employeeResponses,
                    CitizenResponses = citizenResponses,
                    CompanyResponses = companyResponses
                };
            }

            private static List<CommentRecord> GetHearingResponseRecords(Hearing hearing)
            {
                var hearingResponseRecords = new List<CommentRecord>();
                var hearingResponses =
                    hearing.Comments.Where(comment => comment.CommentType.Type == CommentType.HEARING_RESPONSE && !comment.IsDeleted);
                var userInvitees =
                    hearing.UserHearingRoles.Where(uhr => uhr.HearingRole.Role == HearingRole.HEARING_INVITEE).ToList();
                var companyInvitees = hearing.CompanyHearingRoles
                    .Where(chr => chr.HearingRole.Role == HearingRole.HEARING_INVITEE).ToList();

                foreach (var response in hearingResponses)
                {
                    Content commentTextContent = response.Contents.SingleOrDefault(content =>
                        content.ContentType.Type == ContentType.TEXT) ?? new Content { TextContent = "Tom kommentar" };

                    var commentText = commentTextContent.TextContent;

                    var responder = response.User;
                    var company = responder.Company;

                    var isCompany = response.User.UserCapacity.Capacity == UserCapacity.COMPANY;
                    var isUserInvited = userInvitees.Any(uhr => uhr.UserId == response.UserId);
                    var isCompanyInvited = companyInvitees.Any(chr => chr.CompanyId == response.User.CompanyId);

                    var companyRecord = !isCompany || company == null ? null
                        : new CompanyRecord()
                        {
                            Name = company.Name,
                            Address = company.Address,
                            PostalCode = company.PostalCode,
                            City = company.City,
                            Cvr = company.Cvr
                        };

                    var responderRecord = new UserRecord
                    {
                        IsCompany = isCompany,
                        IsInvitee = isUserInvited || isCompanyInvited,
                        Name = responder.Name,
                        Address = responder.Address,
                        PostalCode = responder.PostalCode,
                        City = responder.City,
                        Email = responder.Email,
                        Cpr = responder.Cpr,
                        Company = companyRecord
                    };

                    hearingResponseRecords.Add(new CommentRecord
                    {
                        Number = response.Number,
                        Status = response.CommentStatus.Status,
                        CommentText = commentText,
                        Responder = responderRecord,
                        ResponderCapacity = response.User.UserCapacity.Capacity,
                        Company = companyRecord,
                        OnBehalfOf = response.OnBehalfOf,
                        CommentDeclineInfo = response.CommentDeclineInfo,
                        ResponderName = response.User.Name
                    });
                }

                return hearingResponseRecords;
            }

            private async Task<List<CommentRecord>> GetCommentRecords(Hearing hearing)
            {
                var commentRecords = new List<CommentRecord>();

                var commentsOnHearing = hearing.Comments;
                var hearingOwner = await hearing.GetHearingOwner(_hearingRoleResolver);
                var hearingOwnerDisplayName = hearingOwner.EmployeeDisplayName;

                foreach (var comment in commentsOnHearing)
                {
                    Content commentTextContent = comment.Contents.SingleOrDefault(content =>
                        content.ContentType.Type == ContentType.TEXT) ?? new Content { TextContent = "Tom kommentar" };

                    var commentText = commentTextContent.TextContent;

                    var commentFileRecords = GetFilesForComment(comment);
                    var fileNamesOnComment = commentFileRecords.Select(fileRecord => fileRecord.FileName).ToList();

                    var commentAnswersToComment = commentsOnHearing.Where(localComment =>
                        localComment.CommentParrentId == comment.Id &&
                        localComment.CommentType.Type == CommentType.HEARING_RESPONSE_REPLY &&
                        localComment.Contents.Any(y => !string.IsNullOrEmpty(y.TextContent)));

                    var commentAnswersAsText = commentAnswersToComment
                        .Select(localComment =>
                            localComment.Contents.SingleOrDefault(content =>
                                content.ContentType.Type == ContentType.TEXT))
                        .Where(content => content != null).Select(content => content.TextContent).ToList();

                    CompanyRecord companyData = null;
                    UserRecord userData = null;

                    if (comment.User.UserCapacity.Capacity == UserCapacity.COMPANY)
                    {
                        var company = comment.User.Company;
                        companyData = new CompanyRecord
                        {
                            Name = company.Name,
                            PostalCode = company.PostalCode,
                            City = company.City,
                            StreetName = company.StreetName,
                        };
                    }
                    else
                    {
                        var user = comment.User;
                        userData = new UserRecord
                        {
                            Name = user.Name,
                            City = user.City,
                            StreetName = user.StreetName,
                            PostalCode = user.PostalCode,
                        };
                    }

                    // If comment is created after the hearing deadline, then use the hearing deadline as the created date.
                    var clampedCreatedDate = (!hearing.Deadline.HasValue || comment.Created < hearing.Deadline) ? comment.Created : hearing.Deadline.Value;

                    commentRecords.Add(new CommentRecord
                    {
                        Number = comment.Number,
                        Status = comment.CommentStatus.Status,
                        CommentText = commentText,
                        Responder = userData,
                        ResponderCapacity = comment.User.UserCapacity.Capacity,
                        Company = companyData,
                        OnBehalfOf = comment.OnBehalfOf,
                        AnswersToComment = commentAnswersAsText,
                        FileNames = fileNamesOnComment,
                        Files = commentFileRecords,
                        IsDeleted = comment.IsDeleted,
                        Type = comment.CommentType.Type,
                        Created = clampedCreatedDate,
                        HearingOwnerDisplayName = hearingOwnerDisplayName,
                        CommentDeclineInfo = comment.CommentDeclineInfo,
                        ResponderName = comment.User.Name
                    });

                }

                return commentRecords;
            }

            private static List<FileRecord> GetFilesForComment(Comment comment)
            {
                var commentFileRecords = comment.Contents
                    .Where(content => content.ContentType.Type == ContentType.FILE)
                    .Select(content => new FileRecord
                    {
                        FileName = content.FileName,
                        FilePath = content.FilePath,
                        Extension = content.FileContentType
                    });
                return commentFileRecords.ToList();
            }

            private async Task<Hearing> GetHearingAsync(int id)
            {
                var includes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    $"{nameof(Hearing.SubjectArea)}",
                    $"{nameof(Hearing.CityArea)}",
                    $"{nameof(Hearing.Contents)}",
                    $"{nameof(Hearing.Contents)}.{nameof(Content.ContentType)}",
                    $"{nameof(Hearing.Contents)}.{nameof(Content.Field)}.{nameof(Field.FieldType)}",
                    $"{nameof(Hearing.Comments)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.User)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.User)}.{nameof(User.UserCapacity)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.User)}.{nameof(User.Company)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.CommentType)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.CommentStatus)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.Contents)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.Contents)}.{nameof(Content.ContentType)}",
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.HearingRole)}",
                    $"{nameof(Hearing.CompanyHearingRoles)}.{nameof(CompanyHearingRole.HearingRole)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.CommentDeclineInfo)}"
                });

                return await _hearingDao.GetAsync(id, includes, asNoTracking: true);
            }

            private static List<UserRecord> GetUserRecords(Hearing hearing)
            {
                // Find users who was invited and/or has responded to the hearing
                var groupedUserHearingRoles = hearing.UserHearingRoles.Where(uhr =>
                        uhr.HearingRole.Role == HearingRole.HEARING_INVITEE ||
                        uhr.HearingRole.Role == HearingRole.HEARING_RESPONDER)
                    .GroupBy(uhr => uhr.UserId);
                var users = new List<User>();
                foreach (var userHearingRoles in groupedUserHearingRoles)
                {
                    var currentUser = userHearingRoles.FirstOrDefault()?.User;
                    if (currentUser == null) continue;
                    currentUser.UserHearingRoles = userHearingRoles.ToList();
                    users.Add(currentUser);
                }

                // Find companies that were invited to the hearing.
                // Companies that have responded will be included through the users above
                var companies = hearing.CompanyHearingRoles.Where(chr =>
                    chr.HearingRole.Role == HearingRole.HEARING_INVITEE).Select(chr => chr.Company);


                var userRecords = new List<UserRecord>();

                foreach (var user in users)
                {
                    var isCompany = user.Company?.Id != null;

                    var isResponder =
                        user.UserHearingRoles.Any(uhr => uhr.HearingRole?.Role == HearingRole.HEARING_RESPONDER);

                    var isInviteeAsUser =
                        user.UserHearingRoles.Any(uhr => uhr.HearingRole?.Role == HearingRole.HEARING_INVITEE);
                    var isInviteeAsCompany =
                        user.Company?.CompanyHearingRoles?.Any(chr =>
                            chr.HearingRole?.Role == HearingRole.HEARING_INVITEE) ?? false;


                    var userRecord = new UserRecord
                    {
                        Name = user.Name,
                        Cpr = user.Cpr,
                        Email = user.Email,
                        Address = user.Address,
                        City = user.City,
                        PostalCode = user.PostalCode,
                        IsInvitee = isInviteeAsUser || isInviteeAsCompany,
                        IsResponder = isResponder,
                        IsCompany = isCompany,
                        Company = !isCompany ? null : new CompanyRecord
                        {
                            Name = user.Company.Name,
                            Cvr = user.Company.Cvr,
                            PostalCode = user.Company.PostalCode,
                            City = user.Company.City,
                            Address = user.Company.Address
                        }
                    };
                    userRecords.Add(userRecord);
                }

                foreach (var company in companies)
                {
                    var userRecord = new UserRecord
                    {
                        IsCompany = true,
                        IsInvitee = true,
                        Company = new CompanyRecord
                        {
                            Name = company.Name,
                            Cvr = company.Cvr,
                            City = company.City,
                            Address = company.Address,
                            PostalCode = company.PostalCode
                        }
                    };
                    userRecords.Add(userRecord);
                }

                return userRecords;
            }
        }
    }
}