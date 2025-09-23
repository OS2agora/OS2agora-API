using BallerupKommune.Models.Enums;
using BallerupKommune.Models.Models;
using BallerupKommune.Models.Models.Multiparts;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Common.Interfaces.Plugins;
using BallerupKommune.Operations.Common.Interfaces.Security;
using MediatR;
using NovaSec.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using CommentStatus = BallerupKommune.Models.Models.CommentStatus;
using CommentType = BallerupKommune.Models.Enums.CommentType;
using ContentType = BallerupKommune.Models.Enums.ContentType;
using HearingRole = BallerupKommune.Models.Enums.HearingRole;
using HearingStatus = BallerupKommune.Models.Enums.HearingStatus;
using InvalidOperationException = BallerupKommune.Operations.Common.Exceptions.InvalidOperationException;
using UserCapacity = BallerupKommune.Models.Enums.UserCapacity;

namespace BallerupKommune.Operations.Models.Comments.Commands.CreateComment
{
    [PreAuthorize("HasAnyRole(['Citizen','Employee']) || @Security.IsHearingOwnerByHearingId(#request.HearingId) || @Security.IsHearingReviewerByHearingId(#request.HearingId)")]
    public class CreateCommentCommand : IRequest<Comment>
    {
        public int HearingId { get; set; }
        public string Text { get; set; }
        public string OnBehalfOf { get; set; }
        public int? CommentParrentId { get; set; }
        public CommentType CommentType { get; set; }
        public IEnumerable<FileOperation> FileOperations { get; set; }

        public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Comment>
        {
            private readonly ICommentTypeDao _commentTypeDao;
            private readonly ICommentStatusDao _commentStatusDao;
            private readonly IHearingDao _hearingDao;
            private readonly ICurrentUserService _currentUserService;
            private readonly IUserDao _userDao;
            private readonly ICommentDao _commentDao;
            private readonly IContentDao _contentDao;
            private readonly IContentTypeDao _contentTypeDao;
            private readonly IFileService _fileService;
            private readonly ISecurityExpressions _securityExpressions;
            private readonly IGlobalContentDao _globalContentDao;
            private readonly IConsentDao _consentDao;
            private readonly IHearingRoleDao _hearingRoleDao;
            private readonly IUserHearingRoleDao _userHearingRoleDao;
            private readonly ICompanyDao _companyDao;
            private readonly ICompanyHearingRoleDao _companyHearingRoleDao;
            private readonly IPluginService _pluginService;

            public CreateCommentCommandHandler(
                ICommentTypeDao commentTypeDao, 
                ICommentStatusDao commentStatusDao, 
                IHearingDao hearingDao, ICurrentUserService currentUserService, 
                IUserDao userDao, ICommentDao commentDao, IContentDao contentDao, 
                IContentTypeDao contentTypeDao, IFileService fileService, 
                IGlobalContentDao globalContentDao, IConsentDao consentDao,
                IHearingRoleDao hearingRoleDao, 
                IUserHearingRoleDao userHearingRoleDao, 
                ISecurityExpressions securityExpressions, 
                IPluginService pluginService,
                ICompanyDao companyDao,
                ICompanyHearingRoleDao companyHearingRoleDao)
            {
                _commentTypeDao = commentTypeDao;
                _commentStatusDao = commentStatusDao;
                _hearingDao = hearingDao;
                _currentUserService = currentUserService;
                _userDao = userDao;
                _commentDao = commentDao;
                _contentDao = contentDao;
                _contentTypeDao = contentTypeDao;
                _fileService = fileService;
                _globalContentDao = globalContentDao;
                _consentDao = consentDao;
                _hearingRoleDao = hearingRoleDao;
                _userHearingRoleDao = userHearingRoleDao;
                _securityExpressions = securityExpressions;
                _pluginService = pluginService;
                _companyDao = companyDao;
                _companyHearingRoleDao = companyHearingRoleDao;

            }

            public async Task<Comment> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
            {
                var hearingIncludes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    nameof(Hearing.HearingStatus), 
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.CommentType)}"
                });

                var allCommentTypes = await _commentTypeDao.GetAllAsync();
                var correctCommentType = allCommentTypes.Single(commentType => commentType.Type == request.CommentType);

                var commentStatusIncludes = IncludeProperties.Create<CommentStatus>();
                var allCommentStatus = await _commentStatusDao.GetAllAsync(commentStatusIncludes);
                var correctCommentStatus = SelectCorrectCommentStatus(allCommentStatus, correctCommentType);

                var currentHearing = await _hearingDao.GetAsync(request.HearingId, hearingIncludes);

                if (currentHearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.HearingId);
                }

                var userIncludes = IncludeProperties.Create<User>(null, new List<string>
                {
                    nameof(User.Company)
                });
                var loggedInUserId = _currentUserService.UserId;
                var loggedInUser = await _userDao.FindUserByIdentifier(loggedInUserId, userIncludes);

                if (loggedInUser == null)
                {
                    throw new NotFoundException(nameof(User), loggedInUserId);
                }

                var allContentTypes = await _contentTypeDao.GetAllAsync();
                var textContentType = allContentTypes.Single(contentType => contentType.Type == ContentType.TEXT);
                var fileContentType = allContentTypes.Single(contentType => contentType.Type == ContentType.FILE);

                var isCommentValid = ValidateNewComment(currentHearing.Id, correctCommentType.Type,
                    currentHearing.HearingStatus.Status, request.CommentParrentId);

                if (!isCommentValid)
                {
                    throw new InvalidOperationException("Invalid comment type.");
                }

                var globalContent = await _globalContentDao.GetLatestVersionOfTypeAsync(BallerupKommune.Models.Enums.GlobalContentType.TERMS_AND_CONDITIONS);

                if (globalContent == null)
                {
                    throw new NotFoundException($"Latest version of entity: {nameof(GlobalContent)} with type: {BallerupKommune.Models.Enums.GlobalContentType.TERMS_AND_CONDITIONS} was not found");
                }

                request.FileOperations = request.FileOperations != null ? await _pluginService.BeforeFileOperation(request.FileOperations) : null;
                if (request.FileOperations?.Any(fileOperation => fileOperation.MarkedByScanner) ?? false)
                {
                    var errors = request.FileOperations.Where(fileOperation => fileOperation.MarkedByScanner)
                        .Select(fileOperation => fileOperation.File.Name);
                    throw new FileUploadException(errors);
                }

                var isHearingResponse = correctCommentType.Type == CommentType.HEARING_RESPONSE;
                var isHearingResponseReply = correctCommentType.Type == CommentType.HEARING_RESPONSE_REPLY;

                Consent createdConsent = null;
                Comment commentParent = null;

                if (isHearingResponse)
                {
                    var newConsent = new Consent
                    {
                        GlobalContentId = globalContent.Id
                    };
                    createdConsent = await _consentDao.CreateAsync(newConsent);
                }

                if (isHearingResponseReply)
                {
                    var commentParentIncludes =
                        IncludeProperties.Create<Comment>(null, new List<string> {nameof(Comment.CommentChildren)});

                    commentParent = await _commentDao.GetAsync(request.CommentParrentId ?? 0, commentParentIncludes);
                    
                    if (commentParent == null)
                    {
                        throw new NotFoundException(nameof(Comment), request.CommentParrentId);
                    }
                    if (commentParent.CommentChildren.Any())
                    {
                        throw new InvalidOperationException("Cannot add a comment child to a comment that already has a child");
                    }
                }

                var numberOfHearingResponses = currentHearing.Comments.Count(comment => comment.CommentType.Type == CommentType.HEARING_RESPONSE);

                var newComment = new Comment
                {
                    OnBehalfOf = request.OnBehalfOf,
                    ConsentId = createdConsent?.Id,
                    CommentTypeId = correctCommentType.Id,
                    CommentStatusId = correctCommentStatus.Id,
                    ContainsSensitiveInformation = false,
                    HearingId = currentHearing.Id,
                    UserId = loggedInUser.Id,
                    CommentParrentId = commentParent?.Id,
                    Number = isHearingResponse ? numberOfHearingResponses + 1 : 0
                };

                var createdComment = await _commentDao.CreateAsync(newComment);

                var createdTextContent = await _contentDao.CreateAsync(new Content
                {
                    ContentTypeId = textContentType.Id,
                    CommentId = createdComment.Id,
                    HearingId = currentHearing.Id,
                    TextContent = request.Text
                });

                await _pluginService.AfterCommentTextContentCreate(createdComment.Id, createdTextContent.Id);

                if (request.FileOperations != null)
                {
                    foreach (var fileOperation in request.FileOperations.Where(x => x.Operation == FileOperationEnum.ADD))
                    {
                        var filePath = await _fileService.SaveCommentFileToDisk(fileOperation.File.Content, currentHearing.Id, createdComment.Id);

                        var newFileContent = await _contentDao.CreateAsync(new Content
                        {
                            ContentTypeId = fileContentType.Id,
                            CommentId = createdComment.Id,
                            HearingId = currentHearing.Id,
                            FileName = $"{CommentCommandUtils.FormatFileName(fileOperation.File.Name)}",
                            FileContentType = fileOperation.File.ContentType,
                            FilePath = filePath
                        });

                        await _pluginService.AfterCommentFileContentCreate(createdComment.Id, newFileContent.Id);
                    }
                }

                if (isHearingResponse)
                {
                    await CreateResponderHearingRole(loggedInUser, currentHearing);
                }

                if (isHearingResponse)
                {
                    await _pluginService.NotifyAfterHearingResponse(currentHearing.Id, loggedInUser.Id);
                }
                else if (isHearingResponseReply)
                {
                    // do nothing for now.
                }
                else
                {
                    await _pluginService.NotifyAfterHearingReview(currentHearing.Id);
                }

                // Round-trip to database to get the relationships
                var defaultIncludes = IncludeProperties.Create<Comment>();
                var result = await _commentDao.GetAsync(createdComment.Id, defaultIncludes);

                return result;
            }

            private async Task CreateResponderHearingRole(User user, Hearing hearing)
            {
                var allHearingRoles = await _hearingRoleDao.GetAllAsync();
                var hearingResponderHearingRole = allHearingRoles.Single(hearingRole => hearingRole.Role == HearingRole.HEARING_RESPONDER);
                var newUserHearingRole = new UserHearingRole
                {
                    HearingRoleId = hearingResponderHearingRole.Id,
                    HearingId = hearing.Id,
                    UserId = user.Id
                };
                await _userHearingRoleDao.CreateAsync(newUserHearingRole);

                if (user.UserCapacity.Capacity == UserCapacity.COMPANY)
                {
                    if (user.Company == null)
                    {
                        throw new InvalidOperationException(
                            $"Cannot create Responder CompanyHearingRole on hearing with Id {hearing.Id}. User with Id {user.Id} has UserCapacity COMPANY, but is not connected to a Company");
                    }

                    var newCompanyHearingRole = new CompanyHearingRole
                    {
                        HearingRoleId = hearingResponderHearingRole.Id,
                        HearingId = hearing.Id,
                        CompanyId = user.Company.Id
                    };

                    await _companyHearingRoleDao.CreateAsync(newCompanyHearingRole);
                }
            }

            private CommentStatus SelectCorrectCommentStatus(List<CommentStatus> commentStatuses, BallerupKommune.Models.Models.CommentType commentType)
            {
                IEnumerable<CommentStatus> candidates;
                switch (commentType.Type)
                {
                    case CommentType.HEARING_RESPONSE:
                        candidates = commentStatuses.Where(x => x.CommentType.Type == CommentType.HEARING_RESPONSE);
                        return candidates.Single(x => x.Status == BallerupKommune.Models.Enums.CommentStatus.AWAITING_APPROVAL);
                    case CommentType.HEARING_REVIEW:
                        candidates = commentStatuses.Where(x => x.CommentType.Type == CommentType.HEARING_REVIEW);
                        return candidates.Single(x => x.Status == BallerupKommune.Models.Enums.CommentStatus.NONE);
                    case CommentType.HEARING_RESPONSE_REPLY:
                        candidates =
                            commentStatuses.Where(x => x.CommentType.Type == CommentType.HEARING_RESPONSE_REPLY);
                        return candidates.Single(x => x.Status == BallerupKommune.Models.Enums.CommentStatus.NONE);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            private bool ValidateNewComment(int hearingId, CommentType commentType, HearingStatus hearingStatus, int? commentParrentId)
            {
                var isHearingOwner = _securityExpressions.IsHearingOwnerByHearingId(hearingId);

                if (commentType == CommentType.HEARING_RESPONSE)
                {
                    if (isHearingOwner)
                    {
                        return hearingStatus == HearingStatus.ACTIVE ||
                               hearingStatus == HearingStatus.AWAITING_CONCLUSION;
                    }
                    return hearingStatus == HearingStatus.ACTIVE;
                }

                if (commentType == CommentType.HEARING_REVIEW)
                {
                    var isHearingReviewer = _securityExpressions.IsHearingReviewerByHearingId(hearingId);

                    if (isHearingOwner || isHearingReviewer)
                    {
                        return hearingStatus == HearingStatus.DRAFT;
                    }
                }

                if (commentType == CommentType.HEARING_RESPONSE_REPLY)
                {
                    if (isHearingOwner)
                    {
                        return commentParrentId.HasValue && commentParrentId != 0;
                    }
                }
                
                return false;
            }
        }
    }
}