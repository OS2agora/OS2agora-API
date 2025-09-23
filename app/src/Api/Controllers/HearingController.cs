using BallerupKommune.Api.Mappings;
using BallerupKommune.Api.Models.JsonApi;
using BallerupKommune.DTOs.Models;
using BallerupKommune.DTOs.Models.Multipart;
using BallerupKommune.Models.Models;
using BallerupKommune.Models.Models.Files;
using BallerupKommune.Models.Models.Multiparts;
using BallerupKommune.Operations.Models.Comments.Commands.CreateComment;
using BallerupKommune.Operations.Models.Comments.Commands.SoftDeleteComment;
using BallerupKommune.Operations.Models.Comments.Commands.UpdateComment;
using BallerupKommune.Operations.Models.Comments.Queries.GetComments;
using BallerupKommune.Operations.Models.Fields.Commands.UpdateFields;
using BallerupKommune.Operations.Models.Hearings.Command.CreateHearing;
using BallerupKommune.Operations.Models.Hearings.Command.DeleteHearing;
using BallerupKommune.Operations.Models.Hearings.Command.UpdateHearing;
using BallerupKommune.Operations.Models.Hearings.Command.UploadInvitee;
using BallerupKommune.Operations.Models.Hearings.Command.UploadReviewers;
using BallerupKommune.Operations.Models.Hearings.Queries.ExportHearing;
using BallerupKommune.Operations.Models.Hearings.Queries.GetHearing;
using BallerupKommune.Operations.Models.Hearings.Queries.GetHearings;
using BallerupKommune.Operations.Models.UserHearingRoles.Commands.CreateUserHearingRole;
using BallerupKommune.Operations.Models.UserHearingRoles.Commands.DeleteUserHearingRole;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommentDto = BallerupKommune.Api.Models.DTOs.CommentDto;
using CommentStatus = BallerupKommune.Models.Enums.CommentStatus;
using CommentType = BallerupKommune.Models.Enums.CommentType;
using ExportFormat = BallerupKommune.Api.Models.Enums.ExportFormat;
using HearingDto = BallerupKommune.Api.Models.DTOs.HearingDto;
using UserHearingRoleDto = BallerupKommune.Api.Models.DTOs.UserHearingRoleDto;

namespace BallerupKommune.Api.Controllers
{
    public class HearingController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<JsonApiTopLevelDto<List<HearingDto>>>> GetHearings([FromQuery] string include)
        {
            var operationResponse = await Mediator.Send(new GetHearingsQuery
            {
                RequestIncludes = include.MapToIncludeList()
            });

            var hearingsDtoList = operationResponse
                .Select(hearing => Mapper.Map<Hearing, DTOs.Models.HearingDto>(hearing)).ToList();
            return Ok(hearingsDtoList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingDto>>> GetHearing(int id, [FromQuery] string include)
        {
            var operationResponse = await Mediator.Send(new GetHearingQuery
            {
                Id = id,
                RequestIncludes = include.MapToIncludeList()
            });

            var hearingDto = Mapper.Map<Hearing, DTOs.Models.HearingDto>(operationResponse);
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

        [AllowAnonymous]
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

        [HttpPost("{id}/invite")]
        public async Task<ActionResult<JsonApiTopLevelDto<HearingDto>>> UploadInviteList(int id, IFormFile formFile, 
            [FromQuery] string include)
        {
            var file = Mapper.Map<IFormFile, File>(formFile);

            var operationResponse = await Mediator.Send(new UploadInviteeCommand
            {
                Id = id,
                File = file,
                RequestIncludes = include.MapToIncludeList()
            });

            var hearingDto = Mapper.Map<Hearing, DTOs.Models.HearingDto>(operationResponse);
            return Ok(hearingDto);
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
                Format = (BallerupKommune.Models.Enums.ExportFormat)format
            };

            var result = await Mediator.Send(command);

            return File(result.Content, result.ContentType, result.FileName);
        }

        [HttpGet("{hearingId}/comment")]
        public async Task<ActionResult<JsonApiTopLevelDto<List<CommentDto>>>> GetComments(int hearingId, [FromQuery] string include)
        {
            var operationResponse = await Mediator.Send(new GetCommentsQuery
            {
                HearingIds = new List<int> { hearingId },
                RequestIncludes = include.MapToIncludeList()
            });

            var commentsDtoList = operationResponse.Select(comment => Mapper.Map<Comment, DTOs.Models.CommentDto>(comment)).ToList();
            return Ok(commentsDtoList);
        }

        // Setting unlimited file size for this request
        [DisableRequestSizeLimit]
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

        // Setting unlimited file size for this request
        [DisableRequestSizeLimit]
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

        // Setting unlimited file size for this request
        [DisableRequestSizeLimit]
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
                CommentDeclineReason = dto.CommentDeclineReason,
                FileOperations = fileOperations
            };

            var result = await Mediator.Send(command);
            var dtoResult = Mapper.Map<DTOs.Models.CommentDto>(result);

            return Ok(dtoResult);
        }

        // Setting unlimited file size for this request
        [DisableRequestSizeLimit]
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
    }
}