using Agora.Entities.Common;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Extensions;
using Agora.Models.Models;
using AutoMapper;

namespace Agora.DAOs.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BaseEntity, BaseModel>().ForMember(basemodel => basemodel.PropertiesUpdated,
                    opt => opt.Ignore())
                .Include<CityAreaEntity, CityArea>()
                .Include<CommentDeclineInfoEntity, CommentDeclineInfo>()
                .Include<CommentEntity, Comment>()
                .Include<CommentStatusEntity, CommentStatus>()
                .Include<CommentTypeEntity, CommentType>()
                .Include<CompanyEntity, Company>()
                .Include<CompanyHearingRoleEntity, CompanyHearingRole>()
                .Include<ConsentEntity, Consent>()
                .Include<ContentEntity, Content>()
                .Include<ContentTypeEntity, ContentType>()
                .Include<EventEntity, Event>()
                .Include<EventMappingEntity, EventMapping>()
                .Include<FieldEntity, Field>()
                .Include<FieldTemplateEntity, FieldTemplate>()
                .Include<FieldTypeEntity, FieldType>()
                .Include<FieldTypeSpecificationEntity, FieldTypeSpecification>()
                .Include<GlobalContentEntity, GlobalContent>()
                .Include<GlobalContentTypeEntity, GlobalContentType>()
                .Include<HearingEntity, Hearing>()
                .Include<HearingRoleEntity, HearingRole>()
                .Include<HearingStatusEntity, HearingStatus>()
                .Include<HearingTemplateEntity, HearingTemplate>()
                .Include<HearingTypeEntity, HearingType>()
                .Include<InvitationGroupEntity, InvitationGroup>()
                .Include<InvitationGroupMappingEntity, InvitationGroupMapping>()
                .Include<InvitationKeyEntity, InvitationKey>()
                .Include<InvitationSourceEntity, InvitationSource>()
                .Include<InvitationSourceMappingEntity, InvitationSourceMapping>()
                .Include<JournalizedStatusEntity, JournalizedStatus>()
                .Include<KleHierarchyEntity, Agora.Models.Models.KleHierarchy>()
                .Include<KleMappingEntity, KleMapping>()
                .Include<NotificationContentEntity, NotificationContent>()
                .Include<NotificationContentSpecificationEntity, NotificationContentSpecification>()
                .Include<NotificationContentTypeEntity, NotificationContentType>()
                .Include<NotificationEntity, Notification>()
                .Include<NotificationQueueEntity, NotificationQueue>()
                .Include<NotificationTemplateEntity, NotificationTemplate>()
                .Include<NotificationTypeEntity, NotificationType>()
                .Include<SubjectAreaEntity, SubjectArea>()
                .Include<UserEntity, User>()
                .Include<UserCapacityEntity, UserCapacity>()
                .Include<UserHearingRoleEntity, UserHearingRole>()
                .Include<ValidationRuleEntity, ValidationRule>()
                .ForAllMembers(opt => opt.ExplicitExpansion());

            CreateEntityToModelMap<HearingEntity, Hearing>().ForMember(model => model.CommentAmount, options => options.Ignore());
            CreateModelToEntityMap<Hearing, HearingEntity>();

            CreateEntityToModelMap<SubjectAreaEntity, SubjectArea>();
            CreateModelToEntityMap<SubjectArea, SubjectAreaEntity>();

            CreateEntityToModelMap<CityAreaEntity, CityArea>();
            CreateModelToEntityMap<CityArea, CityAreaEntity>();

            CreateEntityToModelMap<HearingStatusEntity, HearingStatus>();
            CreateModelToEntityMap<HearingStatus, HearingStatusEntity>();

            CreateEntityToModelMap<HearingTypeEntity, HearingType>();
            CreateModelToEntityMap<HearingType, HearingTypeEntity>();

            CreateEntityToModelMap<KleMappingEntity, KleMapping>();
            CreateModelToEntityMap<KleMapping, KleMappingEntity>();

            CreateEntityToModelMap<KleHierarchyEntity, Agora.Models.Models.KleHierarchy>();
            CreateModelToEntityMap<Agora.Models.Models.KleHierarchy, KleHierarchyEntity>();

            CreateEntityToModelMap<HearingTemplateEntity, HearingTemplate>();
            CreateModelToEntityMap<HearingTemplate, HearingTemplateEntity>();

            CreateEntityToModelMap<FieldTemplateEntity, FieldTemplate>();
            CreateModelToEntityMap<FieldTemplate, FieldTemplateEntity>();

            CreateEntityToModelMap<FieldEntity, Field>();
            CreateModelToEntityMap<Field, FieldEntity>();

            CreateEntityToModelMap<ValidationRuleEntity, ValidationRule>();
            CreateModelToEntityMap<ValidationRule, ValidationRuleEntity>();

            CreateEntityToModelMap<FieldTypeEntity, FieldType>();
            CreateModelToEntityMap<FieldType, FieldTypeEntity>();

            CreateEntityToModelMap<FieldTypeSpecificationEntity, FieldTypeSpecification>();
            CreateModelToEntityMap<FieldTypeSpecification, FieldTypeSpecificationEntity>();

            CreateEntityToModelMap<ContentEntity, Content>();
            CreateModelToEntityMap<Content, ContentEntity>();

            CreateEntityToModelMap<ContentTypeEntity, ContentType>();
            CreateModelToEntityMap<ContentType, ContentTypeEntity>();

            CreateEntityToModelMap<CommentDeclineInfoEntity, CommentDeclineInfo>();
            CreateModelToEntityMap<CommentDeclineInfo, CommentDeclineInfoEntity>();

            CreateEntityToModelMap<CommentEntity, Comment>();
            CreateModelToEntityMap<Comment, CommentEntity>();

            CreateEntityToModelMap<CommentTypeEntity, CommentType>();
            CreateModelToEntityMap<CommentType, CommentTypeEntity>();

            CreateEntityToModelMap<CommentStatusEntity, CommentStatus>();
            CreateModelToEntityMap<CommentStatus, CommentStatusEntity>();

            CreateEntityToModelMap<ConsentEntity, Consent>().ForMember(model => model.CommentId, options => options.Ignore());
            CreateModelToEntityMap<Consent, ConsentEntity>();

            CreateEntityToModelMap<GlobalContentEntity, GlobalContent>();
            CreateModelToEntityMap<GlobalContent, GlobalContentEntity>();

            CreateEntityToModelMap<GlobalContentTypeEntity, GlobalContentType>();
            CreateModelToEntityMap<GlobalContentType, GlobalContentTypeEntity>();

            CreateEntityToModelMap<UserEntity, User>();
            CreateModelToEntityMap<User, UserEntity>();

            CreateEntityToModelMap<UserCapacityEntity, UserCapacity>();
            CreateModelToEntityMap<UserCapacity, UserCapacityEntity>();

            CreateEntityToModelMap<UserHearingRoleEntity, UserHearingRole>();
            CreateModelToEntityMap<UserHearingRole, UserHearingRoleEntity>();

            CreateEntityToModelMap<HearingRoleEntity, HearingRole>();
            CreateModelToEntityMap<HearingRole, HearingRoleEntity>();

            CreateEntityToModelMap<NotificationEntity, Notification>();
            CreateModelToEntityMap<Notification, NotificationEntity>();

            CreateEntityToModelMap<NotificationTypeEntity, NotificationType>();
            CreateModelToEntityMap<NotificationType, NotificationTypeEntity>();

            CreateEntityToModelMap<NotificationTemplateEntity, NotificationTemplate>();
            CreateModelToEntityMap<NotificationTemplate, NotificationTemplateEntity>();

            CreateEntityToModelMap<NotificationQueueEntity, NotificationQueue>();
            CreateModelToEntityMap<NotificationQueue, NotificationQueueEntity>();

            CreateEntityToModelMap<JournalizedStatusEntity, JournalizedStatus>();
            CreateModelToEntityMap<JournalizedStatus, JournalizedStatusEntity>();

            CreateEntityToModelMap<CompanyEntity, Company>();
            CreateModelToEntityMap<Company, CompanyEntity>();

            CreateEntityToModelMap<CompanyHearingRoleEntity, CompanyHearingRole>();
            CreateModelToEntityMap<CompanyHearingRole, CompanyHearingRoleEntity>();

            CreateEntityToModelMap<InvitationGroupEntity, InvitationGroup>();
            CreateModelToEntityMap<InvitationGroup, InvitationGroupEntity>();

            CreateEntityToModelMap<InvitationGroupMappingEntity, InvitationGroupMapping>();
            CreateModelToEntityMap<InvitationGroupMapping, InvitationGroupMappingEntity>();

            CreateEntityToModelMap<InvitationKeyEntity, InvitationKey>();
            CreateModelToEntityMap<InvitationKey, InvitationKeyEntity>();

            CreateEntityToModelMap<InvitationSourceEntity, InvitationSource>();
            CreateModelToEntityMap<InvitationSource, InvitationSourceEntity>();

            CreateEntityToModelMap<InvitationSourceMappingEntity, InvitationSourceMapping>();
            CreateModelToEntityMap<InvitationSourceMapping, InvitationSourceMappingEntity>();

            CreateEntityToModelMap<EventEntity, Event>();
            CreateModelToEntityMap<Event, EventEntity>();
            
            CreateEntityToModelMap<EventMappingEntity, EventMapping>();
            CreateModelToEntityMap<EventMapping, EventMappingEntity>();

            CreateEntityToModelMap<NotificationContentEntity, NotificationContent>();
            CreateModelToEntityMap<NotificationContent, NotificationContentEntity>();

            CreateEntityToModelMap<NotificationContentSpecificationEntity, NotificationContentSpecification>();
            CreateModelToEntityMap<NotificationContentSpecification, NotificationContentSpecificationEntity>();

            CreateEntityToModelMap<NotificationContentTypeEntity, NotificationContentType>();
            CreateModelToEntityMap<NotificationContentType, NotificationContentTypeEntity>();
        }

        private IMappingExpression<TEntity, TModel> CreateEntityToModelMap<TEntity, TModel>() where TEntity : BaseEntity where TModel : BaseModel
        {
            // Entity to model
            var map = CreateMap<TEntity, TModel>()
                .ForMember(model => model.PropertiesUpdated, options => options.Ignore());
            var baseModelReferences = ModelExtension.GetPropertyInfoForBaseModelFields<TModel>();
            foreach (var propertyInfo in baseModelReferences)
            {
                map.ForMember(propertyInfo.Name, opts => opts.ExplicitExpansion());
            }

            return map;
        }

        private IMappingExpression<TModel, TEntity> CreateModelToEntityMap<TModel, TEntity>() where TEntity : BaseEntity where TModel : BaseModel
        {
            return CreateMap<TModel, TEntity>();
        }
    }
}