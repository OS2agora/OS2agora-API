using AutoMapper;
using BallerupKommune.Entities.Common;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using Models.Extension;

namespace BallerupKommune.DAOs.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BaseEntity, BaseModel>().ForMember(basemodel => basemodel.PropertiesUpdated, 
                    opt => opt.Ignore())
                .Include<HearingEntity, Hearing>()
                .Include<SubjectAreaEntity,SubjectArea>()
                .Include<HearingStatusEntity,HearingStatus>()
                .Include<HearingTypeEntity,HearingType>()
                .Include<KleMappingEntity,KleMapping>()
                .Include<KleHierarchyEntity,BallerupKommune.Models.Models.KleHierarchy>()
                .Include<HearingTemplateEntity,HearingTemplate>()
                .Include<FieldTemplateEntity,FieldTemplate>()
                .Include<FieldEntity,Field>()
                .Include<ValidationRuleEntity,ValidationRule>()
                .Include<FieldTypeEntity,FieldType>()
                .Include<FieldTypeSpecificationEntity,FieldTypeSpecification>()
                .Include<ContentEntity,Content>()
                .Include<ContentTypeEntity,ContentType>()
                .Include<CommentEntity,Comment>()
                .Include<CommentDeclineInfoEntity,CommentDeclineInfo>()
                .Include<CommentTypeEntity,CommentType>()
                .Include<CommentStatusEntity,CommentStatus>()
                .Include<ConsentEntity,Consent>()
                .Include<GlobalContentEntity,GlobalContent>()
                .Include<GlobalContentTypeEntity,GlobalContentType>()
                .Include<UserEntity,User>()
                .Include<UserCapacityEntity,UserCapacity>()
                .Include<UserHearingRoleEntity,UserHearingRole>()
                .Include<HearingRoleEntity,HearingRole>()
                .Include<NotificationEntity,Notification>()
                .Include<NotificationTypeEntity,NotificationType>()
                .Include<NotificationTemplateEntity,NotificationTemplate>()
                .Include<NotificationQueueEntity,NotificationQueue>()
                .Include<JournalizedStatusEntity, JournalizedStatus>()
                .Include<CompanyEntity, Company>()
                .Include<CompanyHearingRoleEntity, CompanyHearingRole>()
                .ForAllMembers(opt => opt.ExplicitExpansion())
                ;

            CreateEntityToModelMap<HearingEntity, Hearing>().ForMember(model => model.CommentAmount, options => options.Ignore());
            CreateModelToEntityMap<Hearing,HearingEntity>();
            
            CreateEntityToModelMap<SubjectAreaEntity,SubjectArea>();
            CreateModelToEntityMap<SubjectArea,SubjectAreaEntity>();

            CreateEntityToModelMap<HearingStatusEntity,HearingStatus>();
            CreateModelToEntityMap<HearingStatus,HearingStatusEntity>();

            CreateEntityToModelMap<HearingTypeEntity,HearingType>();
            CreateModelToEntityMap<HearingType,HearingTypeEntity>();

            CreateEntityToModelMap<KleMappingEntity,KleMapping>();
            CreateModelToEntityMap<KleMapping,KleMappingEntity>();

            CreateEntityToModelMap<KleHierarchyEntity,BallerupKommune.Models.Models.KleHierarchy>();
            CreateModelToEntityMap<BallerupKommune.Models.Models.KleHierarchy,KleHierarchyEntity>();

            CreateEntityToModelMap<HearingTemplateEntity,HearingTemplate>();
            CreateModelToEntityMap<HearingTemplate,HearingTemplateEntity>();

            CreateEntityToModelMap<FieldTemplateEntity,FieldTemplate>();
            CreateModelToEntityMap<FieldTemplate,FieldTemplateEntity>();

            CreateEntityToModelMap<FieldEntity,Field>();
            CreateModelToEntityMap<Field,FieldEntity>();

            CreateEntityToModelMap<ValidationRuleEntity,ValidationRule>();
            CreateModelToEntityMap<ValidationRule,ValidationRuleEntity>();

            CreateEntityToModelMap<FieldTypeEntity,FieldType>();
            CreateModelToEntityMap<FieldType,FieldTypeEntity>();

            CreateEntityToModelMap<FieldTypeSpecificationEntity,FieldTypeSpecification>();
            CreateModelToEntityMap<FieldTypeSpecification,FieldTypeSpecificationEntity>();

            CreateEntityToModelMap<ContentEntity,Content>();
            CreateModelToEntityMap<Content,ContentEntity>();

            CreateEntityToModelMap<ContentTypeEntity,ContentType>();
            CreateModelToEntityMap<ContentType,ContentTypeEntity>();

            CreateEntityToModelMap<CommentEntity,Comment>();
            CreateModelToEntityMap<Comment,CommentEntity>();

            CreateEntityToModelMap<CommentDeclineInfoEntity,CommentDeclineInfo>();
            CreateModelToEntityMap<CommentDeclineInfo, CommentDeclineInfoEntity>();

            CreateEntityToModelMap<CommentTypeEntity,CommentType>();
            CreateModelToEntityMap<CommentType,CommentTypeEntity>();

            CreateEntityToModelMap<CommentStatusEntity,CommentStatus>();
            CreateModelToEntityMap<CommentStatus,CommentStatusEntity>();

            CreateEntityToModelMap<ConsentEntity,Consent>().ForMember(model => model.CommentId, options => options.Ignore()); ;
            CreateModelToEntityMap<Consent,ConsentEntity>();

            CreateEntityToModelMap<GlobalContentEntity,GlobalContent>();
            CreateModelToEntityMap<GlobalContent,GlobalContentEntity>();

            CreateEntityToModelMap<GlobalContentTypeEntity, GlobalContentType>();
            CreateModelToEntityMap<GlobalContentType, GlobalContentTypeEntity>();

            CreateEntityToModelMap<UserEntity,User>();
            CreateModelToEntityMap<User,UserEntity>();

            CreateEntityToModelMap<UserCapacityEntity,UserCapacity>();
            CreateModelToEntityMap<UserCapacity,UserCapacityEntity>();

            CreateEntityToModelMap<UserHearingRoleEntity,UserHearingRole>();
            CreateModelToEntityMap<UserHearingRole,UserHearingRoleEntity>();

            CreateEntityToModelMap<HearingRoleEntity,HearingRole>();
            CreateModelToEntityMap<HearingRole,HearingRoleEntity>();

            CreateEntityToModelMap<NotificationEntity,Notification>();
            CreateModelToEntityMap<Notification,NotificationEntity>();

            CreateEntityToModelMap<NotificationTypeEntity,NotificationType>();
            CreateModelToEntityMap<NotificationType,NotificationTypeEntity>();

            CreateEntityToModelMap<NotificationTemplateEntity,NotificationTemplate>();
            CreateModelToEntityMap<NotificationTemplate,NotificationTemplateEntity>();

            CreateEntityToModelMap<NotificationQueueEntity,NotificationQueue>();
            CreateModelToEntityMap<NotificationQueue,NotificationQueueEntity>();

            CreateEntityToModelMap<JournalizedStatusEntity, JournalizedStatus>();
            CreateModelToEntityMap<JournalizedStatus, JournalizedStatusEntity>();

            CreateEntityToModelMap<CompanyEntity, Company>();
            CreateModelToEntityMap<Company, CompanyEntity>();

            CreateEntityToModelMap<CompanyHearingRoleEntity, CompanyHearingRole>();
            CreateModelToEntityMap<CompanyHearingRole, CompanyHearingRoleEntity>();
        }
        /*
        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var iMapFromTypes = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityMapper<,>)))
                .ToList();

            foreach (var type in iMapFromTypes)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping") ?? type.GetInterface("IEntityMapper`2").GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
        */
        private IMappingExpression<TEntity, TModel> CreateEntityToModelMap<TEntity, TModel>() where TEntity : BaseEntity where TModel : BaseModel
        {
            // Entity to model
            var map = CreateMap<TEntity, TModel>()
                .ForMember(model => model.PropertiesUpdated, options => options.Ignore());
            var baseModelReferences = ModelExtension.GetPropertyInfoForBaseModelFields<TModel>();
            foreach (var propertyInfo in baseModelReferences)
            {
                //var configuration = new MapperConfiguration(cfg => )
                map.ForMember(propertyInfo.Name, opts => opts.ExplicitExpansion());
                
            }
            
            return map;
        }

        private IMappingExpression<TModel, TEntity> CreateModelToEntityMap<TModel, TEntity>() where TEntity : BaseEntity where TModel : BaseModel
        {
            return CreateMap<TModel, TEntity>()
                ;
        }
    }
}