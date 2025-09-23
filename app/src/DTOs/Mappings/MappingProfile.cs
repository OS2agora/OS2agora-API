using AutoMapper;
using BallerupKommune.DTOs.Models;
using BallerupKommune.DTOs.Models.Multipart;
using BallerupKommune.Models.Models;
using BallerupKommune.Models.Models.Multiparts;
using System;
using System.Collections.Generic;
using System.Reflection;
using BallerupKommune.DTOs.Common;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Extensions;
using Models.Extension;

namespace BallerupKommune.DTOs.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMappings<Comment, CommentDto>();
            CreateMappings<CommentDeclineInfo, CommentDeclineInfoDto>();
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
                .ForMember(dest => dest.EsdhMetaData, options => options.Ignore()));
            CreateMappings<JournalizedStatus, JournalizedStatusDto>();
            CreateMappings<HearingRole, HearingRoleDto>();
            CreateMappings<HearingStatus, HearingStatusDto>();
            CreateMappings<HearingTemplate, HearingTemplateDto>();
            CreateMappings<HearingType, HearingTypeDto>();
            CreateMappings<KleHierarchy, KleHierarchyDto>();
            CreateMappings<KleMapping, KleMappingDto>();
            CreateMappings<SubjectArea, SubjectAreaDto>();
            CreateMappings<UserCapacity, UserCapacityDto>();
            CreateMappings<User, UserDto>(
                modelToDtoExpression => modelToDtoExpression
                    .ForMember(dest => dest.PersonalIdentifier,
                        options => options.Ignore())
                    .ForMember(dest => dest.Cpr, options => options.Ignore()),
                dtoToModelExpression => dtoToModelExpression
                    .ForMember(dest => dest.Notifications, options => options.Ignore()));
            CreateMappings<UserHearingRole, UserHearingRoleDto>();
            CreateMappings<ValidationRule, ValidationRuleDto>();
            CreateMap<MultiPartFieldDto, MultiPartField>().ReverseMap();
            CreateMap<FileOperationDto, FileOperation>()
                .ForMember(x => x.Operation, options => options.MapFrom(x => x.Operation))
                .ForMember(x => x.MarkedByScanner, options => options.Ignore())
                .ReverseMap();
            CreateMappings<Company, CompanyDto>(null, dtoToModelExpression => dtoToModelExpression
                .ForMember(dest => dest.Notifications, options => options.Ignore()));
            CreateMappings<CompanyHearingRole, CompanyHearingRoleDto>();
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
            IMappingExpression<TDto, TModel> dtoToModelExpression = CreateMap<TDto, TModel>();
            IMappingExpression<TModel, TDto> modelToDtoExpression = CreateMap<TModel, TDto>();

            modelToDtoConfiguration?.Invoke(modelToDtoExpression);
            dtoToModelConfiguration?.Invoke(dtoToModelExpression);

            List<PropertyInfo> relationships = ModelExtension.GetPropertyInfoForSingleBaseModelFields<TModel>();

            foreach (PropertyInfo propertyInfo in relationships)
            {
                modelToDtoExpression.ForMember(propertyInfo.Name,
                    options => options.MapFrom(new RelationshipResolver<TModel, TDto>(propertyInfo.Name), src => src));
                dtoToModelExpression.ForMember(propertyInfo.Name, options => options.Ignore());
            }
        }
    }
}