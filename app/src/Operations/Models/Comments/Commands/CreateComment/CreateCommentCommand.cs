using Agora.Models.Enums;
using Agora.Models.Models;
using Agora.Models.Models.Multiparts;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Plugins;
using Agora.Operations.Common.Interfaces.Security;
using MediatR;
using NovaSec.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Operations.ApplicationOptions.OperationsOptions;
using Microsoft.Extensions.Options;
using CommentStatus = Agora.Models.Models.CommentStatus;
using CommentType = Agora.Models.Enums.CommentType;
using ContentType = Agora.Models.Enums.ContentType;
using HearingRole = Agora.Models.Enums.HearingRole;
using HearingStatus = Agora.Models.Enums.HearingStatus;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;
using UserCapacity = Agora.Models.Enums.UserCapacity;
using Agora.Operations.Common.Interfaces.Files;

namespace Agora.Operations.Models.Comments.Commands.CreateComment
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
            private readonly ICompanyHearingRoleDao _companyHearingRoleDao;
            private readonly IPluginService _pluginService;
            private readonly IOptions<CommentOperationsOptions> _options;

            public CreateCommentCommandHandler(
                ICommentTypeDao commentTypeDao, 
                ICommentStatusDao commentStatusDao, 
                IHearingDao hearingDao, 
                ICurrentUserService currentUserService, 
                IUserDao userDao, ICommentDao commentDao, 
                IContentDao contentDao, 
                IContentTypeDao contentTypeDao, 
                IFileService fileService, 
                IGlobalContentDao globalContentDao, 
                IConsentDao consentDao, 
                IHearingRoleDao hearingRoleDao, 
                IUserHearingRoleDao userHearingRoleDao,
                ICompanyHearingRoleDao companyHearingRoleDao,
                ISecurityExpressions securityExpressions, 
                IPluginService pluginService, 
                IOptions<CommentOperationsOptions> options)
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
                _companyHearingRoleDao = companyHearingRoleDao;
                _securityExpressions = securityExpressions;
                _pluginService = pluginService;
                _options = options;
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

                var currentHearing = await _hearingDao.GetAsync(request.HearingId, hearingIncludes);

                if (currentHearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.HearingId);
                }

                var correctCommentStatus = SelectCorrectCommentStatus(allCommentStatus, correctCommentType, currentHearing.AutoApproveComments);

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

                var isCommentCountValid = ValidateCommentLimits(currentHearing, correctCommentType.Type, loggedInUser.Id);

                if (!isCommentCountValid)
                {
                    throw new InvalidOperationException(
                        $"New comment would exceed comment limitation for user (Id: {loggedInUserId}) on hearing (Id: {currentHearing.Id})");
                }
                

                var globalContent = await _globalContentDao.GetLatestVersionOfTypeAsync(Agora.Models.Enums.GlobalContentType.TERMS_AND_CONDITIONS);

                if (globalContent == null)
                {
                    throw new NotFoundException($"Latest version of entity: {nameof(GlobalContent)} with type: {Agora.Models.Enums.GlobalContentType.TERMS_AND_CONDITIONS} was not found");
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

                var result = await EvaluateStatusAfterScan(createdComment.Id, allCommentStatus, isHearingResponse);

                return result;
            }

            private async Task<Comment> EvaluateStatusAfterScan(int commentId,
                List<CommentStatus> allCommentStatus, bool isHearingResponse)
            {
                // Round-trip to database to get the relationships
                var defaultIncludes = IncludeProperties.Create<Comment>();
                var result = await _commentDao.GetAsync(commentId, defaultIncludes);

                if (isHearingResponse && result.ContainsSensitiveInformation)
                {
                    var awaitingApprovalStatus = allCommentStatus.Single(status =>
                        status.Status == Agora.Models.Enums.CommentStatus.AWAITING_APPROVAL);
                    result.CommentStatusId = awaitingApprovalStatus.Id;
                    result.CommentStatus = awaitingApprovalStatus;
                    result.PropertiesUpdated = new List<string> { nameof(Comment.CommentStatusId), nameof(Comment.CommentStatus) };
                    result = await _commentDao.UpdateAsync(result);
                }

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

            private CommentStatus SelectCorrectCommentStatus(List<CommentStatus> commentStatuses, Agora.Models.Models.CommentType commentType, bool autoAcceptResponse)
            {
                IEnumerable<CommentStatus> candidates;
                switch (commentType.Type)
                {
                    case CommentType.HEARING_RESPONSE:
                        candidates = commentStatuses.Where(x => x.CommentType.Type == CommentType.HEARING_RESPONSE);
                        if (autoAcceptResponse)
                        {
                            return candidates.Single(x => x.Status == Agora.Models.Enums.CommentStatus.APPROVED);
                        }
                        return candidates.Single(x => x.Status == Agora.Models.Enums.CommentStatus.AWAITING_APPROVAL);
                    case CommentType.HEARING_REVIEW:
                        candidates = commentStatuses.Where(x => x.CommentType.Type == CommentType.HEARING_REVIEW);
                        return candidates.Single(x => x.Status == Agora.Models.Enums.CommentStatus.NONE);
                    case CommentType.HEARING_RESPONSE_REPLY:
                        candidates =
                            commentStatuses.Where(x => x.CommentType.Type == CommentType.HEARING_RESPONSE_REPLY);
                        return candidates.Single(x => x.Status == Agora.Models.Enums.CommentStatus.NONE);
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

            private bool ValidateCommentLimits(Hearing hearing, CommentType commentType, int currentUserId)
            {
                if (commentType != CommentType.HEARING_RESPONSE)
                {
                    return true;
                }

                var currentUserCommentCount = hearing.Comments
                    .Where(comment => comment.CommentType.Type == CommentType.HEARING_RESPONSE && !comment.IsDeleted)
                    .Count(comment => comment.UserId == currentUserId);

                var isHearingOwner = _securityExpressions.IsHearingOwnerByHearingId(hearing.Id);

                if (isHearingOwner)
                {
                    var hearingOwnerResponseLimit = _options.Value.CreateComment.HearingOwnerResponseLimit;
                    if (hearingOwnerResponseLimit <= 0)
                    {
                        return true;
                    }
                    var newCommentExceedsLimits = currentUserCommentCount + 1 > hearingOwnerResponseLimit;
                    return !newCommentExceedsLimits;
                }

                var responseLimit = _options.Value.CreateComment.ResponseLimit;
                if (responseLimit > 0)
                {
                    var newCommentExceedsLimits = currentUserCommentCount + 1 > responseLimit;
                    return !newCommentExceedsLimits;
                }
                
                return true;
            }
        }
    }
}