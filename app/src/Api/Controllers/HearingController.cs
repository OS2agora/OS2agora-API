using Agora.Api.Mappings;
using Agora.Api.Models.DTOs;
using Agora.Api.Models.JsonApi;
using Agora.DTOs.Common.CustomResponseDto;
using Agora.DTOs.Mappings;
using Agora.DTOs.Models;
using Agora.DTOs.Models.Multipart;
using Agora.Models.Common.CustomResponse;
using Agora.Models.Common.CustomResponse.Pagination;
using Agora.Models.Common.CustomResponse.SortAndFilter;
using Agora.Models.Models;
using Agora.Models.Models.Files;
using Agora.Models.Models.Invitations;
using Agora.Models.Models.Multiparts;
using Agora.Operations.Common.Interfaces.Security;
using Agora.Operations.Models.Comments.Commands.CreateComment;
using Agora.Operations.Models.Comments.Commands.SoftDeleteComment;
using Agora.Operations.Models.Comments.Commands.UpdateComment;
using Agora.Operations.Models.Comments.Queries.GetComments;
using Agora.Operations.Models.Fields.Commands.UpdateFields;
using Agora.Operations.Models.Hearings.Command.CreateHearing;
using Agora.Operations.Models.Hearings.Command.DeleteHearing;
using Agora.Operations.Models.Hearings.Command.DeleteInvitationSource;
using Agora.Operations.Models.Hearings.Command.DeleteInvitationSourceMappings;
using Agora.Operations.Models.Hearings.Command.UpdateHearing;
using Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromCsvFile;
using Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromExcelFile;
using Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromInvitationGroup;
using Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromPersonalInviteeIdentifiers;
using Agora.Operations.Models.Hearings.Command.UploadReviewers;
using Agora.Operations.Models.Hearings.Queries.ExportHearing;
using Agora.Operations.Models.Hearings.Queries.GetHearing;
using Agora.Operations.Models.Hearings.Queries.GetHearings;
using Agora.Operations.Models.NotificationContents.Commands;
using Agora.Operations.Models.NotificationContentSpecifications.Queries;
using Agora.Operations.Models.Notifications.Commands.SendTestNotification;
using Agora.Operations.Models.Notifications.Queries.ExportNotification;
using Agora.Operations.Models.Notifications.Queries.GetInvitationNotifications;
using Agora.Operations.Models.UserHearingRoles.Commands.CreateUserHearingRole;
using Agora.Operations.Models.UserHearingRoles.Commands.DeleteUserHearingRole;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommentDto = Agora.Api.Models.DTOs.CommentDto;
using CommentStatus = Agora.Models.Enums.CommentStatus;
using CommentType = Agora.Models.Enums.CommentType;
using ContentDto = Agora.DTOs.Models.ContentDto;
using ExportFormat = Agora.Api.Models.Enums.ExportFormat;
using HearingDto = Agora.Api.Models.DTOs.HearingDto;
using NotificationContentDto = Agora.Api.Models.DTOs.NotificationContentDto;
using NotificationContentSpecificationDto = Agora.Api.Models.DTOs.NotificationContentSpecificationDto;
using NotificationType = Agora.Api.Models.Enums.NotificationType;
using PaginationParametersDto = Agora.DTOs.Common.CustomResponseDto.PaginationParametersDto;
using UserHearingRoleDto = Agora.Api.Models.DTOs.UserHearingRoleDto;

namespace Agora.Api.Controllers
{
    public class HearingController : ApiController
    {
        private readonly ISecurityExpressions _securityExpressions;

        public HearingController(ISecurityExpressions securityExpressions)
        {
            _securityExpressions = securityExpressions;
        }

        [HttpGet]
        public async Task<ActionResult<JsonApiTopLevelDto<IList<HearingDto>>>> GetHearings(
            [FromQuery] string include,
            [FromQuery] PaginationParametersDto paginationParametersDto,
            [FromQuery] FilterParametersDto filterParametersDto,
            [FromQuery] SortingParametersDto sortingParametersDto)
        {
            var paginationParameters =
                Mapper.Map<PaginationParametersDto, PaginationParameters>(paginationParametersDto);
            var filterParameters =
                Mapper.Map<FilterParametersDto, FilterParameters>(filterParametersDto);
            var sortingParameters = Mapper.Map<SortingParametersDto, SortingParameters>(sortingParametersDto);


            var operationResponse = await Mediator.Send(new GetHearingsQuery
            {
                RequestIncludes = include.MapToIncludeList(),
                PaginationParameters = paginationParameters,
                FilterParameters = filterParameters,
                SortingParameters = sortingParameters
            });


            if (operationResponse is ResponseList<Hearing> responseList)
            {
                var hearingsDtoList = Mapper.Map<ResponseList<Hearing>, ResponseListDto<DTOs.Models.HearingDto>>(responseList);
                var responseWithMeta = ResponseListDtoMapper<DTOs.Models.HearingDto>.MapToDocumentRoot(hearingsDtoList);
                return Ok(responseWithMeta);
            }
            else
            {
                var hearingsDtoList = operationResponse
                    .Select(hearing => Mapper.Map<Hearing, DTOs.Models.HearingDto>(hearing)).ToList();
                return Ok(hearingsDtoList);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingDto>>> GetHearing(int id, [FromQuery] string include)
        {
            var operationResponse = await Mediator.Send(new GetHearingQuery
            {
                Id = id,
                RequestIncludes = include.MapToIncludeList()
            });

            // If the current user is the owner of the hearing, include personal information in the response
            var hearingDto = _securityExpressions.IsHearingOwnerByHearingId(id) ?
                Mapper.Map<Hearing, DTOs.Models.HearingDto>(operationResponse, options => options.Items.Add(PersonalInformationValueResolver.GetIncludePersonalInfoOption(true))) :
                Mapper.Map<Hearing, DTOs.Models.HearingDto>(operationResponse);

            return Ok(hearingDto);
        }

        [HttpPost]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingDto>>> CreateHearing([FromQuery] string include)
        {
            var createdHearing = await Mediator.Send(new CreateHearingCommand
            {
                RequestIncludes = include.MapToIncludeList()
            });
            var hearingDto = Mapper.Map<Hearing, DTOs.Models.HearingDto>(createdHearing);
            return Ok(hearingDto);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingDto>>> UpdateHearing(int id,
            JsonApiTopLevelDto<HearingDto> dto, [FromQuery] string include)
        {
            var resourceDto =
                dto.MapFromJsonApiDtoToDto<DTOs.Models.HearingDto, JsonApiTopLevelDto<HearingDto>, HearingDto,
                    HearingDto.HearingAttributeDto>();

            if (resourceDto.Id != id)
            {
                return BadRequest("Id from URL must match Id from DTO");
            }

            var resource = Mapper.Map<DTOs.Models.HearingDto, Hearing>(resourceDto);

            var operationResponse = await Mediator.Send(new UpdateHearingCommand
            {
                Hearing = resource,
                RequestIncludes = include.MapToIncludeList()
            });

            var hearingDto = Mapper.Map<Hearing, DTOs.Models.HearingDto>(operationResponse);
            return Ok(hearingDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingDto>>> DeleteHearing(int id)
        {
            await Mediator.Send(new DeleteHearingCommand
            {
                Id = id
            });

            var hearingDto = new DTOs.Models.HearingDto
            {
                Id = id
            };
            return Ok(hearingDto);
        }

        // Used by administrators to delete an arbitrary hearing, independent of its status
        [HttpDelete("{id}/force")]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingDto>>> ForceDeleteHearing(int id)
        {
            await Mediator.Send(new ForceDeleteHearingCommand
            {
                Id = id
            });

            var hearingDto = new DTOs.Models.HearingDto
            {
                Id = id
            };
            return Ok(hearingDto);
        }

        [HttpGet("{hearingId}/invitationNotifications")]
        public async Task<ActionResult<JsonApiTopLevelDto<List<NotificationDto>>>> GetInvitationNotifications(
            int hearingId)
        {
            var operationResponse = await Mediator.Send(new GetInvitationNotificationsQuery
            {
                HearingId = hearingId
            });

            var invitationNotificationsDtoList = operationResponse
                .Select(notification
                    => Mapper.Map<Notification, NotificationDto>(notification, options
                        => options.Items.Add(PersonalInformationValueResolver.GetIncludePersonalInfoOption(true))))
                .ToList();

            return Ok(invitationNotificationsDtoList);
        }

        [HttpDelete("{hearingId}/invitationSource/{invitationSourceName}")]
        public async Task<ActionResult> DeleteInvitationSource(int hearingId, string invitationSourceName)
        {
            var operationResponse = await Mediator.Send(new DeleteInvitationSourceCommand
            {
                HearingId = hearingId,
                InvitationSourceName = invitationSourceName
            });

            var metaDataResponseDto = Mapper.Map<MetaDataResponse<Hearing, InvitationMetaData>,
                MetaDataResponseDto<DTOs.Models.HearingDto, InvitationMetaDataDto>>(operationResponse, options => options.Items.Add(PersonalInformationValueResolver.GetIncludePersonalInfoOption(true)));

            return Ok(MetaDataResponseDtoMapper<DTOs.Models.HearingDto, InvitationMetaDataDto>.MapToDocumentRoot(metaDataResponseDto));
        }

        [HttpPost("{hearingId}/invitationSourceMappings/delete")]
        public async Task<ActionResult> DeleteInvitationSourceMappings(int hearingId,
            [FromBody] List<int> invitationSourceMappingIds,
            [FromQuery] bool deleteFromAllInvitationSources)
        {
            var operationResponse = await Mediator.Send(new DeleteInvitationSourceMappingsCommand
            {
                HearingId = hearingId,
                InvitationSourceMappingIds = invitationSourceMappingIds,
                DeleteFromAllInvitationSources = deleteFromAllInvitationSources
            });
            var metaDataResponseDto = Mapper.Map<MetaDataResponse<Hearing, InvitationMetaData>,
                MetaDataResponseDto<DTOs.Models.HearingDto, InvitationMetaDataDto>>(operationResponse, options => options.Items.Add(PersonalInformationValueResolver.GetIncludePersonalInfoOption(true)));
            return Ok(MetaDataResponseDtoMapper<DTOs.Models.HearingDto, InvitationMetaDataDto>.MapToDocumentRoot(metaDataResponseDto));
        }

        [HttpPost("{hearingId}/invite/{invitationSourceId}/csv")]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingDto>>> UpdateInviteesFromCsvFile(int hearingId, int invitationSourceId, IFormFile formFile)
        {
            var file = Mapper.Map<IFormFile, File>(formFile);

            var operationResponse = await Mediator.Send(new UpdateInviteesFromCsvFileCommand
            {
                HearingId = hearingId,
                File = file,
                InvitationSourceId = invitationSourceId
            });

            var metaDataResponseDto = Mapper.Map<MetaDataResponse<Hearing, InvitationMetaData>,
                MetaDataResponseDto<DTOs.Models.HearingDto, InvitationMetaDataDto>>(operationResponse, options => options.Items.Add(PersonalInformationValueResolver.GetIncludePersonalInfoOption(true)));

            return Ok(MetaDataResponseDtoMapper<DTOs.Models.HearingDto, InvitationMetaDataDto>.MapToDocumentRoot(metaDataResponseDto));
        }

        [HttpPost("{hearingId}/invite/{invitationSourceId}/excel")]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingDto>>> UpdateInviteesFromExcelFile(int hearingId, int invitationSourceId, IFormFile formFile)
        {
            var file = Mapper.Map<IFormFile, File>(formFile);

            var operationResponse = await Mediator.Send(new UpdateInviteesFromExcelFileCommand
            {
                HearingId = hearingId,
                File = file,
                InvitationSourceId = invitationSourceId
            });

            var metaDataResponseDto = Mapper.Map<MetaDataResponse<Hearing, InvitationMetaData>,
                MetaDataResponseDto<DTOs.Models.HearingDto, InvitationMetaDataDto>>(operationResponse, options => options.Items.Add(PersonalInformationValueResolver.GetIncludePersonalInfoOption(true)));

            return Ok(MetaDataResponseDtoMapper<DTOs.Models.HearingDto, InvitationMetaDataDto>.MapToDocumentRoot(metaDataResponseDto));
        }

        [HttpPost("{hearingId}/invite/{invitationSourceId}/personal")]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingDto>>> UpdateInviteesFromPersonalInviteeIdentifiers(int hearingId, int invitationSourceId, List<JsonApiTopLevelDto<InviteeIdentifiersDto>> dtos)
        {
            var inviteeIdentifiersDtos = dtos.Select(dto => dto.MapFromJsonApiDtoToDto<DTOs.Models.Invitations.InviteeIdentifiersDto, JsonApiTopLevelDto<InviteeIdentifiersDto>, InviteeIdentifiersDto, InviteeIdentifiersDto.InviteeIdentifiersAttributeDto>());
            var inviteeIdentifiers = inviteeIdentifiersDtos.Select(resourceDto => Mapper.Map<DTOs.Models.Invitations.InviteeIdentifiersDto, InviteeIdentifiers>(resourceDto)).ToList();

            var operationResponse = await Mediator.Send(new UpdateInviteesFromPersonalInviteeIdentifiersCommand
            {
                HearingId = hearingId,
                InvitationSourceId = invitationSourceId,
                InviteeIdentifiers = inviteeIdentifiers
            });

            var metaDataResponseDto = Mapper.Map<MetaDataResponse<Hearing, InvitationMetaData>,
                MetaDataResponseDto<DTOs.Models.HearingDto, InvitationMetaDataDto>>(operationResponse, options => options.Items.Add(PersonalInformationValueResolver.GetIncludePersonalInfoOption(true)));

            return Ok(MetaDataResponseDtoMapper<DTOs.Models.HearingDto, InvitationMetaDataDto>.MapToDocumentRoot(metaDataResponseDto));
        }

        [HttpPost("{hearingId}/invite/{invitationSourceId}/invitationGroup/{invitationGroupId}")]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingDto>>> UpdateInviteesFromInvitationGroup(int hearingId, int invitationSourceId, int invitationGroupId)
        {
            var operationResponse = await Mediator.Send(new UpdateInviteesFromInvitationGroupCommand
            {
                HearingId = hearingId,
                InvitationSourceId = invitationSourceId,
                InvitationGroupId = invitationGroupId
            });

            var metaDataResponseDto = Mapper.Map<MetaDataResponse<Hearing, InvitationMetaData>,
                MetaDataResponseDto<DTOs.Models.HearingDto, InvitationMetaDataDto>>(operationResponse, options => options.Items.Add(PersonalInformationValueResolver.GetIncludePersonalInfoOption(true)));

            return Ok(MetaDataResponseDtoMapper<DTOs.Models.HearingDto, InvitationMetaDataDto>.MapToDocumentRoot(metaDataResponseDto));
        }


        [HttpPost("{id}/reviewers")]
        public async Task<ActionResult<List<UserHearingRoleDto>>> UpdateReviewerList(int id, List<JsonApiTopLevelDto<UserHearingRoleDto>> dtos)
        {
            var resourceDtos = dtos.Select(dto =>
                dto.MapFromJsonApiDtoToDto<DTOs.Models.UserHearingRoleDto, JsonApiTopLevelDto<UserHearingRoleDto>,
                    UserHearingRoleDto, UserHearingRoleDto.UserHearingRoleAttributeDto>());

            var resources = resourceDtos.Select(resourceDto =>
                Mapper.Map<DTOs.Models.UserHearingRoleDto, UserHearingRole>(resourceDto)).ToList();

            var operationResponse = await Mediator.Send(new UploadReviewersCommand
            {
                Id = id,
                Reviewers = resources
            });

            var userHearingRolesDtoList = operationResponse.Select(userHearingRole =>
                Mapper.Map<UserHearingRole, DTOs.Models.UserHearingRoleDto>(userHearingRole)).ToList();

            return Ok(userHearingRolesDtoList);
        }

        [HttpGet("{id}/export/{format}")]
        public async Task<FileResult> ExportHearing(int id, ExportFormat format)
        {
            var command = new ExportHearingQuery
            {
                Id = id,
                Format = (Agora.Models.Enums.ExportFormat)format
            };

            var result = await Mediator.Send(command);

            return File(result.Content, result.ContentType, result.FileName);
        }

        [HttpGet("{hearingId}/notification/{type}/export")]
        public async Task<FileResult> ExportNotification(int hearingId, NotificationType type)
        {
            var command = new ExportNotificationQuery
            {
                HearingId = hearingId,
                NotificationType = (Agora.Models.Enums.NotificationType)type
            };
            var result = await Mediator.Send(command);

            return File(result.Content, result.ContentType, result.FileName);
        }

        [HttpPost("{hearingId}/notification/{type}/send")]
        public async Task<ActionResult> SendNotification(int hearingId, NotificationType type)
        {
            var command = new SendTestNotificationCommand
            {
                HearingId = hearingId,
                NotificationType = (Agora.Models.Enums.NotificationType)type
            };
            await Mediator.Send(command);

            return Ok();
        }

        [HttpGet("{hearingId}/comment")]
        public async Task<ActionResult<JsonApiTopLevelDto<List<CommentDto>>>> GetComments(int hearingId, [FromQuery] string include, [FromQuery] PaginationParametersDto paginationParametersDto, [FromQuery] bool myCommentsOnly = false)
        {

            var paginationParameters =
                Mapper.Map<PaginationParametersDto, PaginationParameters>(paginationParametersDto);

            var operationResponse = await Mediator.Send(new GetCommentsQuery
            {
                HearingIds = new List<int> { hearingId },
                RequestIncludes = include.MapToIncludeList(),
                PaginationParameters = paginationParameters,
                MyCommentsOnly = myCommentsOnly
            });

            if (operationResponse is ResponseList<Comment> responseList)
            {
                var commentsDtoList = Mapper.Map<ResponseList<Comment>, ResponseListDto<DTOs.Models.CommentDto>>(responseList);
                var responseWithMeta = ResponseListDtoMapper<DTOs.Models.CommentDto>.MapToDocumentRoot(commentsDtoList);
                return Ok(responseWithMeta);
            }
            else
            {
                var commentsDtoList = operationResponse.Select(comment => Mapper.Map<Comment, DTOs.Models.CommentDto>(comment)).ToList();
                return Ok(commentsDtoList);
            }
        }

        // Setting request size limit to 150 MB for this request
        [RequestSizeLimit(150000000)]
        [HttpPost("{hearingId}/comment")]
        public async Task<ActionResult<JsonApiTopLevelDto<CommentDto>>> CreateComment(int hearingId,
            [FromForm] MultiPartCreateCommentDto dto)
        {
            var fileOperations = dto?.FileOperations?.Select(operation => Mapper.Map<FileOperation>(operation)) ?? Enumerable.Empty<FileOperation>();

            var command = new CreateCommentCommand
            {
                HearingId = hearingId,
                Text = dto?.Content,
                OnBehalfOf = dto?.OnBehalfOf,
                CommentType = dto != null ? (CommentType)dto.CommentType : CommentType.NONE,
                CommentParrentId = dto?.CommentParrentId,
                FileOperations = fileOperations
            };

            var result = await Mediator.Send(command);
            var dtoResult = Mapper.Map<DTOs.Models.CommentDto>(result);

            return Ok(dtoResult);
        }

        [HttpPatch("{hearingId}/comment/{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<CommentDto>>> UpdateComment(int hearingId, int id)
        {
            var command = new SoftDeleteCommentCommand
            {
                Id = id,
                HearingId = hearingId
            };
            var result = await Mediator.Send(command);
            var dtoResult = Mapper.Map<DTOs.Models.CommentDto>(result);

            return Ok(dtoResult);
        }

        // Setting request size limit to 150 MB for this request
        [RequestSizeLimit(150000000)]
        [HttpPatch("{hearingId}/comment/{id}/content")]
        public async Task<ActionResult<JsonApiTopLevelDto<CommentDto>>> UpdateContent(int hearingId, int id,
            [FromForm] MultiPartUpdateCommentDto dto)
        {
            var fileOperations = dto?.FileOperations?.Select(operation => Mapper.Map<FileOperation>(operation)) ?? Enumerable.Empty<FileOperation>();

            var command = new UpdateCommentCommand
            {
                HearingId = hearingId,
                Id = id,
                Text = dto?.Content,
                OnBehalfOf = dto?.OnBehalfOf,
                CommentStatus = dto != null ? (CommentStatus)dto.CommentStatus : CommentStatus.NONE,
                CommentDeclineReason = dto?.CommentDeclineReason,
                FileOperations = fileOperations
            };

            var result = await Mediator.Send(command);
            var dtoResult = Mapper.Map<DTOs.Models.CommentDto>(result);

            return Ok(dtoResult);
        }

        // Setting request size limit to 150 MB for this request
        [RequestSizeLimit(150000000)]
        [HttpPatch("{hearingId}/fieldContent")]
        public async Task<ActionResult<JsonApiTopLevelDto<ContentDto>>> UpdateFieldContent(int hearingId, [FromQuery] int hearingStatusId, [FromQuery] bool notifyAboutChanges,
            [FromForm] MultiPartFieldsDto dto)
        {
            var multiPartFields = dto?.Fields?.Select(field => Mapper.Map<MultiPartField>(field)) ?? Enumerable.Empty<MultiPartField>();

            var command = new UpdateFieldsCommand
            {
                HearingId = hearingId,
                Fields = multiPartFields,
                HearingStatusId = hearingStatusId,
                NotifyAboutChanges = notifyAboutChanges
            };

            var result = await Mediator.Send(command);
            var dtoResult = result.Select(x => Mapper.Map<ContentDto>(x));

            return Ok(dtoResult);
        }

        [HttpDelete("{hearingId}/userHearingRole/{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<UserHearingRoleDto>>> DeleteUserHearingRole(int hearingId, int id)
        {
            await Mediator.Send(new DeleteUserHearingRoleCommand
            {
                Id = id,
                HearingId = hearingId
            });

            var userHearingRoleDto = new DTOs.Models.UserHearingRoleDto
            {
                Id = id
            };
            return Ok(userHearingRoleDto);
        }

        [HttpPost("{hearingId}/userHearingRole")]
        public async Task<ActionResult<JsonApiTopLevelDto<UserHearingRoleDto>>> CreateUserHearingRole(int hearingId,
            JsonApiTopLevelDto<UserHearingRoleDto> inputDto)
        {
            var resourceDto = inputDto.MapFromJsonApiDtoToDto<DTOs.Models.UserHearingRoleDto, JsonApiTopLevelDto<UserHearingRoleDto>, UserHearingRoleDto, UserHearingRoleDto.UserHearingRoleAttributeDto>();
            var resource = Mapper.Map<DTOs.Models.UserHearingRoleDto, UserHearingRole>(resourceDto);

            var operationResponse = await Mediator.Send(new CreateUserHearingRoleCommand
            {
                UserHearingRole = resource,
                HearingId = hearingId
            });

            var userHearingRoleDto = Mapper.Map<UserHearingRole, DTOs.Models.UserHearingRoleDto>(operationResponse);
            return Ok(userHearingRoleDto);
        }

        [HttpGet("{hearingId}/notificationContentSpecification")]
        public async Task<ActionResult<JsonApiTopLevelDto<List<NotificationContentSpecificationDto>>>> GetNotificationContentSpecifications(int hearingId)
        {
            var operationResponse = await Mediator.Send(new GetNotificationContentSpecificationsQuery
            {
                HearingId = hearingId
            });

            var notificationContentSpecificationDtoList = operationResponse.Select(ncs => Mapper.Map<NotificationContentSpecification, DTOs.Models.NotificationContentSpecificationDto>(ncs)).ToList();
            return Ok(notificationContentSpecificationDtoList);
        }

        [HttpPatch("{hearingId}/notificationContentSpecification/{notificationContentSpecificationId}/notificationContent")]
        public async Task<ActionResult<JsonApiTopLevelDto<NotificationContentDto>>> UpdateNotificationContent(int hearingId, int notificationContentSpecificationId, List<JsonApiTopLevelDto<NotificationContentDto>> dtos)
        {
            var resourceDtos = dtos.Select(dto => dto.MapFromJsonApiDtoToDto<DTOs.Models.NotificationContentDto, JsonApiTopLevelDto<NotificationContentDto>, NotificationContentDto, NotificationContentDto.NotificationContentAttributeDto>());

            var resources = resourceDtos.Select(resourceDto => Mapper.Map<DTOs.Models.NotificationContentDto, NotificationContent>(resourceDto)).ToList();

            var operationResponse = await Mediator.Send(new UpdateNotificationContentsCommand
            {
                HearingId = hearingId,
                NotificationContentSpecificationId = notificationContentSpecificationId,
                NotificationContents = resources
            });

            var notificationContentDtoList = operationResponse.Select(notificationContent => Mapper.Map<NotificationContent, DTOs.Models.NotificationContentDto>(notificationContent)).ToList();
            return Ok(notificationContentDtoList);
        }
    }
}