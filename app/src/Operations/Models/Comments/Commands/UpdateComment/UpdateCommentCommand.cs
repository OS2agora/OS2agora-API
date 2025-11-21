using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Models.Models.Multiparts;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Common.Interfaces.Plugins;
using Agora.Operations.Common.Interfaces.Security;
using MediatR;
using NovaSec.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommentStatusEnum = Agora.Models.Enums.CommentStatus;
using CommentType = Agora.Models.Enums.CommentType;
using ContentType = Agora.Models.Enums.ContentType;
using FileOperationEnum = Agora.Models.Enums.FileOperationEnum;
using GlobalContentType = Agora.Models.Enums.GlobalContentType;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;

namespace Agora.Operations.Models.Comments.Commands.UpdateComment
{
    [PreAuthorize("@Security.IsHearingResponder(#request.HearingId) && @Security.IsCommentOwnerByCommentId(#request.Id)")]
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class UpdateCommentCommand : IRequest<Comment>
    {
        public int Id { get; set; }
        public int HearingId { get; set; }
        public string Text { get; set; }
        public string OnBehalfOf { get; set; }
        public string CommentDeclineReason { get; set; }
        public CommentStatusEnum CommentStatus { get; set; }
        public IEnumerable<FileOperation> FileOperations { get; set; }


        public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Comment>
        {
            private readonly IHearingDao _hearingDao;
            private readonly ICommentDao _commentDao;
            private readonly ICommentDeclineInfoDao _commentDeclineInfoDao;
            private readonly IContentDao _contentDao;
            private readonly IContentTypeDao _contentTypeDao;
            private readonly IFileService _fileService;
            private readonly ISecurityExpressions _securityExpressions;
            private readonly IGlobalContentDao _globalContentDao;
            private readonly IConsentDao _consentDao;
            private readonly IPluginService _pluginService;
            private readonly ICurrentUserService _currentUserService;
            private readonly IUserDao _userDao;

            public UpdateCommentCommandHandler(
                IHearingDao hearingDao, 
                ICommentDao commentDao, 
                ICommentDeclineInfoDao commentDeclineInfoDao, 
                IContentDao contentDao, 
                IContentTypeDao contentTypeDao,
                IFileService fileService, 
                ISecurityExpressions securityExpressions, 
                IGlobalContentDao globalContentDao, 
                IConsentDao consentDao,
                IPluginService pluginService, 
                ICurrentUserService currentUserService, 
                IUserDao userDao)
            {
                _hearingDao = hearingDao;
                _commentDao = commentDao;
                _commentDeclineInfoDao = commentDeclineInfoDao;
                _contentDao = contentDao;
                _contentTypeDao = contentTypeDao;
                _fileService = fileService;
                _securityExpressions = securityExpressions;
                _globalContentDao = globalContentDao;
                _consentDao = consentDao;
                _pluginService = pluginService;
                _currentUserService = currentUserService;
                _userDao = userDao;
            }

            public async Task<Comment> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
            {
                var commentIncludes = IncludeProperties.Create<Comment>(null, new List<string>
                {
                    nameof(Comment.Contents), 
                    $"{nameof(Comment.Contents)}.{nameof(Content.ContentType)}",
                    nameof(Comment.CommentType),
                    $"{nameof(Comment.CommentType)}.{nameof(Comment.CommentType.CommentStatuses)}",
                    nameof(Comment.Consent),
                    nameof(Comment.CommentDeclineInfo)
                });
                var currentComment = await _commentDao.GetAsync(request.Id, commentIncludes);

                if (currentComment == null)
                {
                    throw new NotFoundException(nameof(Comment), request.Id);
                }

                if (currentComment.IsDeleted)
                {
                    throw new InvalidOperationException("Comment cannot be updated if deleted");
                }

                var allPossibleCommentStatus = currentComment.CommentType.CommentStatuses;

                var statusBeforeUpdate = currentComment.CommentStatus.Status;
                var newCommentStatus = allPossibleCommentStatus.SingleOrDefault(x => x.Status == request.CommentStatus);
                if (newCommentStatus == null)
                {
                    throw new InvalidOperationException($"Comment with type: {currentComment.CommentType.Type} with status: {request.CommentStatus} is not valid.");
                }

                var commentDeclinedStatus = allPossibleCommentStatus.Single(x => x.Status == CommentStatusEnum.NOT_APPROVED);
                var commentApprovedStatus =
                    allPossibleCommentStatus.Single(x => x.Status == CommentStatusEnum.APPROVED);
                var isDecliningComment = commentDeclinedStatus == newCommentStatus;
                var isApprovingComment = commentApprovedStatus == newCommentStatus;
                if (isDecliningComment && String.IsNullOrEmpty(request.CommentDeclineReason))
                {
                    throw new InvalidOperationException($"A reason must be provided when declining a comment");
                }

                var textContent = currentComment.Contents.Single(x => x.ContentType.Type == ContentType.TEXT);
                var fileContent = currentComment.Contents.Where(x => x.ContentType.Type == ContentType.FILE).ToList();

                var currentTextContent = await _contentDao.GetAsync(textContent.Id);

                if (currentTextContent == null)
                {
                    throw new NotFoundException(nameof(Content), textContent.Id);
                }

                var hearingIncludes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    nameof(Hearing.UserHearingRoles),
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.User)}"
                });

                var currentHearing = await _hearingDao.GetAsync(request.HearingId, hearingIncludes);

                if (currentHearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.HearingId);
                }

                var allContentTypes = await _contentTypeDao.GetAllAsync();
                var fileContentType = allContentTypes.Single(contentType => contentType.Type == ContentType.FILE);

                var isCommentStatusChangeValid =
                    ValidateNewCommentStatus(newCommentStatus.Status, currentComment.CommentType.Type, currentHearing.Id);

                if (!isCommentStatusChangeValid)
                {
                    throw new InvalidOperationException("Invalid comment status update.");
                }

                request.FileOperations = request.FileOperations != null ? await _pluginService.BeforeFileOperation(request.FileOperations) : null;
                if (request.FileOperations?.Any(fileOperation => fileOperation.MarkedByScanner) ?? false)
                {
                    var errors = request.FileOperations.Where(fileOperation => fileOperation.MarkedByScanner)
                        .Select(fileOperation => fileOperation.File.Name);
                    throw new FileUploadException(errors);
                }

                currentComment.CommentStatusId = newCommentStatus.Id;
                currentComment.CommentStatus = newCommentStatus;
                currentComment.OnBehalfOf = request.OnBehalfOf;

                currentComment.PropertiesUpdated = new List<string> 
                    { 
                        nameof(Comment.CommentStatusId), 
                        nameof(Comment.CommentStatus), 
                        nameof(Comment.OnBehalfOf)
                    };

                if (isDecliningComment && currentComment.CommentDeclineInfo == null)
                {
                    var newCommentDeclineInfo = await _commentDeclineInfoDao.CreateAsync(new CommentDeclineInfo
                    {
                        DeclineReason = request.CommentDeclineReason,
                        DeclinerInitials = _currentUserService.EmployeeName,
                    });
                    currentComment.CommentDeclineInfoId = newCommentDeclineInfo.Id;

                    currentComment.PropertiesUpdated.Add(nameof(Comment.CommentDeclineInfo));
                }

                await _commentDao.UpdateAsync(currentComment);

                if (isApprovingComment && currentComment.CommentDeclineInfo != null)
                {
                    await _commentDeclineInfoDao.DeleteAsync(currentComment.CommentDeclineInfo.Id);
                }

                currentTextContent.TextContent = request.Text;
                currentTextContent.PropertiesUpdated = new List<string> { nameof(Content.TextContent) };

                await _contentDao.UpdateAsync(currentTextContent);

                await _pluginService.AfterCommentTextContentUpdate(currentComment.Id, currentTextContent.Id);

                if (request.FileOperations != null)
                {
                    foreach (var fileOperation in request.FileOperations.Where(x =>
                        x.Operation == FileOperationEnum.DELETE))
                    {
                        var contentToDelete = fileContent.SingleOrDefault(x => x.Id == fileOperation.ContentId);

                        if (contentToDelete != null)
                        {
                            _fileService.DeleteFileFromDisk(contentToDelete.FilePath);
                            await _contentDao.DeleteAsync(contentToDelete.Id);
                        }
                    }

                    foreach (var fileOperation in request.FileOperations.Where(
                        x => x.Operation == FileOperationEnum.ADD))
                    {
                        var filePath = await _fileService.SaveCommentFileToDisk(fileOperation.File.Content, currentHearing.Id,
                            currentComment.Id);

                        var newFileContent = await _contentDao.CreateAsync(new Content
                        {
                            ContentTypeId = fileContentType.Id,
                            CommentId = currentComment.Id,
                            HearingId = currentHearing.Id,
                            FileName = $"{CommentCommandUtils.FormatFileName(fileOperation.File.Name)}",
                            FileContentType = fileOperation.File.ContentType,
                            FilePath = filePath
                        });

                        await _pluginService.AfterCommentFileContentCreate(currentComment.Id, newFileContent.Id);
                    }
                }

                var globalContent = await _globalContentDao.GetLatestVersionOfTypeAsync(GlobalContentType.TERMS_AND_CONDITIONS);
                if (globalContent == null)
                {
                    throw new NotFoundException($"Latest version of entity: {nameof(GlobalContent)} with type: {GlobalContentType.TERMS_AND_CONDITIONS} was not found");
                }

                var isHearingResponse = currentComment.CommentType.Type == CommentType.HEARING_RESPONSE;
                var isHearingResponseReply = currentComment.CommentType.Type == CommentType.HEARING_RESPONSE_REPLY;

                if (isDecliningComment)
                {
                    var hearingCommentResponder = currentComment.User?.Id;
                    if (hearingCommentResponder != null)
                    {
                        await _pluginService.NotifyAfterHearingResponseDecline(currentHearing.Id, (int)hearingCommentResponder, currentComment.Id);
                    }
                }
                else if (isHearingResponse)
                {
                    var consent = currentComment.Consent;
                    consent.GlobalContentId = globalContent.Id;
                    consent.GlobalContent = globalContent;
                    await _consentDao.UpdateAsync(consent);

                    var loggedInUserId = _currentUserService.UserId;
                    var loggedInUser = await _userDao.FindUserByIdentifier(loggedInUserId);
                    if (loggedInUser != null)
                    {
                        await _pluginService.NotifyAfterHearingResponse(currentHearing.Id, loggedInUser.Id);
                    }
                }
                else if (isHearingResponseReply)
                {
                    // do nothing for now...
                }
                else
                {
                    await _pluginService.NotifyAfterHearingReview(currentHearing.Id);
                }

                var result = await EvaluateStatusAfterScan(currentComment.Id, allPossibleCommentStatus.ToList(), isHearingResponse,
                    statusBeforeUpdate, currentHearing.AutoApproveComments, isDecliningComment);

                return result;
            }

            private async Task<Comment> EvaluateStatusAfterScan(int commentId, List<CommentStatus> allCommentStatus, bool isHearingResponse,
                CommentStatusEnum prevStatus, bool autoAccept, bool isDecliningComment)
            {
                // Round-trip to database to get the relationships
                var defaultIncludes = IncludeProperties.Create<Comment>();
                var result = await _commentDao.GetAsync(commentId, defaultIncludes);

                if (isHearingResponse && !isDecliningComment && autoAccept && !result.ContainsSensitiveInformation && prevStatus == CommentStatusEnum.APPROVED)
                {
                    var approvedStatus = allCommentStatus.Single(status =>
                        status.Status == CommentStatusEnum.APPROVED);
                    result.CommentStatusId = approvedStatus.Id;
                    result.CommentStatus = approvedStatus;
                    result.PropertiesUpdated = new List<string> { nameof(Comment.CommentStatusId), nameof(Comment.CommentStatus) };
                    result = await _commentDao.UpdateAsync(result);
                } 

                return result;
            }

            private bool ValidateNewCommentStatus(CommentStatusEnum newCommentStatus, CommentType commentType, int hearingId)
            {
                var isHearingOwner = _securityExpressions.IsHearingOwnerByHearingId(hearingId);
                if (commentType == CommentType.HEARING_RESPONSE)
                {
                    if (isHearingOwner)
                    {
                        return true;
                    }
                    if (newCommentStatus == CommentStatusEnum.AWAITING_APPROVAL)
                    {
                        return true;
                    }
                }
                if (commentType == CommentType.HEARING_REVIEW)
                {
                    if (newCommentStatus == CommentStatusEnum.NONE)
                    {
                        return true;
                    }
                }

                if (commentType == CommentType.HEARING_RESPONSE_REPLY)
                {
                    if (newCommentStatus == CommentStatusEnum.NONE)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}