using Agora.DTOs.Common;
using Agora.DTOs.Common.CustomResponseDto;
using Agora.DTOs.Common.CustomResponseDto.Converter;
using Agora.DTOs.Models;
using Agora.DTOs.Models.Invitations;
using Agora.DTOs.Models.Multipart;
using Agora.Models.Common;
using Agora.Models.Common.CustomResponse;
using Agora.Models.Common.CustomResponse.Pagination;
using Agora.Models.Common.CustomResponse.SortAndFilter;
using Agora.Models.Extensions;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Models.Models.Multiparts;
using AutoMapper;
using System;
using System.Linq;

namespace Agora.DTOs.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMappings<CityArea, CityAreaDto>();
            CreateMappings<CommentDeclineInfo, CommentDeclineInfoDto>();
            CreateMappings<Comment, CommentDto>(null, dtoToModelExpression => dtoToModelExpression
                .ForMember(dest => dest.Notifications, options => options.Ignore()));
            CreateMappings<CommentStatus, CommentStatusDto>();
            CreateMappings<CommentType, CommentTypeDto>(
                modelToDtoExpression => modelToDtoExpression
                    .ForMember(dest => dest.CommentType, options => options.MapFrom(src => src.Type)),
                dtoToModelExpression => dtoToModelExpression
                    .ForMember(dest => dest.Type, options => options.MapFrom(src => src.CommentType)));
            CreateMappings<Consent, ConsentDto>();
            CreateMappings<Content, ContentDto>();
            CreateMappings<ContentType, ContentTypeDto>(
                modelToDtoExpression => modelToDtoExpression
                    .ForMember(dest => dest.ContentType, options => options.MapFrom(src => src.Type)),
                dtoToModelExpression => dtoToModelExpression
                    .ForMember(dest => dest.Type, options => options.MapFrom(src => src.ContentType)));
            CreateMappings<Field, FieldDto>();
            CreateMappings<FieldTemplate, FieldTemplateDto>();
            CreateMappings<FieldType, FieldTypeDto>(
                modelToDtoExpression => modelToDtoExpression
                    .ForMember(dest => dest.FieldType, options => options.MapFrom(src => src.Type)),
                dtoToModelExpression => dtoToModelExpression
                    .ForMember(dest => dest.Type, options => options.MapFrom(src => src.FieldType)));
            CreateMappings<FieldTypeSpecification, FieldTypeSpecificationDto>();
            CreateMappings<GlobalContent, GlobalContentDto>();
            CreateMappings<GlobalContentType, GlobalContentTypeDto>(
                modelToDtoExpression => modelToDtoExpression
                    .ForMember(dest => dest.GlobalContentType, options => options.MapFrom(src => src.Type)),
                dtoToModelExpression => dtoToModelExpression
                    .ForMember(dest => dest.Type, options => options.MapFrom(src => src.GlobalContentType)));
            CreateMappings<Hearing, HearingDto>(null, dtoToModelExpression => dtoToModelExpression
                .ForMember(dest => dest.Notifications, options => options.Ignore())
                .ForMember(dest => dest.EsdhMetaData, options => options.Ignore())
                .ForMember(dest => dest.ConcludedDate, options => options.Ignore())
                .ForMember(dest => dest.Events, options => options.Ignore())
                .ForMember(dest => dest.NotificationContentSpecifications, options => options.Ignore())
            );
            CreateMappings<JournalizedStatus, JournalizedStatusDto>();
            CreateMappings<HearingRole, HearingRoleDto>();
            CreateMappings<HearingStatus, HearingStatusDto>();
            CreateMappings<HearingTemplate, HearingTemplateDto>();
            CreateMappings<HearingType, HearingTypeDto>();
            CreateMappings<InvitationGroup, InvitationGroupDto>();
            CreateMappings<InvitationGroupMapping, InvitationGroupMappingDto>();
            CreateMappings<InvitationKey, InvitationKeyDto>();
            CreateMappings<KleHierarchy, KleHierarchyDto>();
            CreateMappings<KleMapping, KleMappingDto>();
            CreateMappings<Notification, NotificationDto>(null, dtoToModelExpression => dtoToModelExpression
                .ForMember(dest => dest.EventMappings, options => options.Ignore())
                .ForMember(dest => dest.CommentId, options => options.Ignore()));
            CreateMappings<NotificationContent, NotificationContentDto>();
            CreateMappings<NotificationContentSpecification, NotificationContentSpecificationDto>();
            CreateMappings<NotificationContentType, NotificationContentTypeDto>(modelToDtoExpression => modelToDtoExpression
                .ForMember(dest => dest.NotificationContentType, options => options.MapFrom(src => src.Type)), 
                dtoToModelExpression => dtoToModelExpression
                    .ForMember(dest => dest.Type, options => options.MapFrom(src => src.NotificationContentType))
                    .ForMember(dest => dest.NotificationTemplates, options => options.Ignore())
                    .ForMember(dest => dest.NotificationContents, options => options.Ignore()));
            CreateMappings<NotificationQueue, NotificationQueueDto>(null, modelToDtoExpression => modelToDtoExpression
                .ForMember(dest => dest.Content, options => options.Ignore())
                .ForMember(dest => dest.ErrorTexts, options => options.Ignore())
                .ForMember(dest => dest.Subject, options => options.Ignore())
                .ForMember(dest => dest.RecipientAddress, options => options.Ignore())
                .ForMember(dest => dest.RetryCount, options => options.Ignore())
                .ForMember(dest => dest.MessageId, options => options.Ignore())
                .ForMember(dest => dest.MessageChannel, options => options.Ignore())
                .ForMember(dest => dest.NotificationId, options => options.Ignore()));
            CreateMappings<NotificationType, NotificationTypeDto>(modelToDtoExpression => modelToDtoExpression
                .ForMember(dest => dest.NotificationType, options => options.MapFrom(src => src.Type)),
            dtoToModelExpression => dtoToModelExpression
                .ForMember(dest => dest.Type, options => options.MapFrom(src => src.NotificationType))
                .ForMember(dest => dest.HeaderTemplateId, options => options.Ignore())
                .ForMember(dest => dest.BodyTemplateId, options => options.Ignore())
                .ForMember(dest => dest.FooterTemplateId, options => options.Ignore())
                .ForMember(dest => dest.SubjectTemplateId, options => options.Ignore())
                .ForMember(dest => dest.Notifications, options => options.Ignore())
                .ForMember(dest => dest.NotificationContentSpecifications, options => options.Ignore())
                .ForMember(dest => dest.Events, options => options.Ignore()));
            CreateMappings<SubjectArea, SubjectAreaDto>();
            CreateMappings<UserCapacity, UserCapacityDto>();
            CreateMappings<User, UserDto>(
                modelToDtoConfiguration => modelToDtoConfiguration
                    .ForMember(dest => dest.Cpr, options => options.MapFrom(new PersonalInformationValueResolver(nameof(User.Cpr)))),
                dtoToModelExpression => dtoToModelExpression
                    .ForMember(dest => dest.Notifications, options => options.Ignore())
                    .ForMember(dest => dest.PersonalIdentifier, options => options.Ignore())
                    .ForMember(dest => dest.Cvr, options => options.Ignore())
                    .ForMember(dest => dest.Cpr, options => options.Ignore())
                    .ForMember(dest => dest.Address, options => options.Ignore())
                    .ForMember(dest => dest.Country, options => options.Ignore())
                    .ForMember(dest => dest.Municipality, options => options.Ignore())
                    .ForMember(dest => dest.Events, options => options.Ignore()));
            CreateMappings<UserHearingRole, UserHearingRoleDto>();
            CreateMappings<ValidationRule, ValidationRuleDto>();

            CreateMap<MultiPartFieldDto, MultiPartField>().ReverseMap();
            CreateMap<FileOperationDto, FileOperation>()
                .ForMember(x => x.Operation, options => options.MapFrom(x => x.Operation))
                .ForMember(x => x.MarkedByScanner, options => options.Ignore())
                .ReverseMap();
            CreateMappings<Company, CompanyDto>(
                null,
                dtoToModelExpression => dtoToModelExpression
                .ForMember(dest => dest.Notifications, options => options.Ignore())
                .ForMember(dest => dest.Address, options => options.Ignore())
                .ForMember(dest => dest.Country, options => options.Ignore())
                .ForMember(dest => dest.Municipality, options => options.Ignore()));
            CreateMappings<CompanyHearingRole, CompanyHearingRoleDto>();
            CreateMappings<InvitationSource, InvitationSourceDto>();
            CreateMappings<InvitationSourceMapping, InvitationSourceMappingDto>();
            CreateMap<PaginationParametersDto, PaginationParameters>();
            CreateMap<FilterParametersDto, FilterParameters>().ConvertUsing(typeof(FilterParametersConverter));
            CreateMap<SortingParametersDto, SortingParameters>().ConvertUsing(typeof(SortingParametersConverter));
            CreateMap(typeof(ResponseList<>), typeof(ResponseListDto<>)).ConvertUsing(typeof(ResponseListConverter<,>));

            CreateMap<InviteeIdentifiersDto, InviteeIdentifiers>();
            CreateMap<InvitationMetaData, InvitationMetaDataDto>();
            CreateMetaDataResponseMapping<Hearing, HearingDto, InvitationMetaData, InvitationMetaDataDto>();
        }

        /// <summary>
        /// Creates mappings between a model and the corresponding dto.
        /// <br/>
        /// Special handling has been created for mapping relationships. When mapping from DTO to model, only the ID
        /// property of the relationship is populated. When mapping from model to DTO, the following custom resolver is
        /// used <see cref="RelationshipResolver{TModel,TDto}"/>.
        /// </summary>
        /// <param name="modelToDtoConfiguration">
        /// Optional custom mapping configuration for model to DTO mapping.
        /// </param>
        /// <param name="dtoToModelConfiguration">
        /// Optional custom mapping configuration for DTO to model mapping.
        /// </param>
        /// <typeparam name="TModel">The model type to create mappings for.</typeparam>
        /// <typeparam name="TDto">The DTO type to create mappings for.</typeparam>
        private void CreateMappings<TModel, TDto>(
            Action<IMappingExpression<TModel, TDto>> modelToDtoConfiguration = null,
            Action<IMappingExpression<TDto, TModel>> dtoToModelConfiguration = null)
            where TModel : BaseModel where TDto : BaseDto<TDto>
        {
            var dtoToModelExpression = CreateMap<TDto, TModel>();
            var modelToDtoExpression = CreateMap<TModel, TDto>();

            modelToDtoConfiguration?.Invoke(modelToDtoExpression);
            dtoToModelConfiguration?.Invoke(dtoToModelExpression);

            var relationships = ModelExtension.GetPropertyInfoForSingleBaseModelFields<TModel>();
            var dtoProperties = typeof(TDto).GetProperties().Select(p => p.Name).ToHashSet();

            foreach (var propertyInfo in relationships)
            {
                // Note: This guards against the DTO having less properties than the Model, as there might be relationships that we do not expose
                if (dtoProperties.Contains(propertyInfo.Name))
                {
                    modelToDtoExpression.ForMember(propertyInfo.Name, options => options.MapFrom(new RelationshipResolver<TModel, TDto>(propertyInfo.Name), src => src));
                }
                dtoToModelExpression.ForMember(propertyInfo.Name, options => options.Ignore());
            }
        }

        private void CreateMetaDataResponseMapping<TModel, TDto, TMetaModel, TMetaDto>()
            where TModel : BaseModel
            where TDto : BaseDto<TDto>
            where TMetaModel : class
            where TMetaDto : class
        {
            CreateMap<MetaDataResponse<TModel, TMetaModel>, MetaDataResponseDto<TDto, TMetaDto>>()
                .ForMember(dest => dest.ResponseData, options => options.MapFrom(src => src.ResponseData))
                .ForMember(dest => dest.Meta, options => options.MapFrom(src => src.Meta));
        }
    }
}