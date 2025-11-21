using Agora.DAOs.Identity;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace Agora.DAOs.Persistence
{
    public interface IApplicationDbContext
    {
        DbSet<CityAreaEntity> CityAreas { get; set; }
        DbSet<CommentDeclineInfoEntity> CommentDeclineInfos { get; set; }
        DbSet<CommentEntity> Comments { get; set; }
        DbSet<CommentStatusEntity> CommentStatuses { get; set; }
        DbSet<CommentTypeEntity> CommentTypes { get; set; }
        DbSet<ConsentEntity> Consents { get; set; }
        DbSet<ContentEntity> Contents { get; set; }
        DbSet<ContentTypeEntity> ContentTypes { get; set; }
        DbSet<EventEntity> Events { get; set; }
        DbSet<EventMappingEntity> EventMappings { get; set; }
        DbSet<FieldEntity> Fields { get; set; }
        DbSet<FieldTemplateEntity> FieldTemplates { get; set; }
        DbSet<FieldTypeEntity> FieldTypes { get; set; }
        DbSet<FieldTypeSpecificationEntity> FieldTypeSpecifications { get; set; }
        DbSet<GlobalContentEntity> GlobalContents { get; set; }
        DbSet<GlobalContentTypeEntity> GlobalContentTypes { get; set; }
        DbSet<HearingEntity> Hearings { get; set; }
        DbSet<HearingRoleEntity> HearingRoles { get; set; }
        DbSet<HearingStatusEntity> HearingStatus { get; set; }
        DbSet<HearingTemplateEntity> HearingTemplates { get; set; }
        DbSet<HearingTypeEntity> HearingTypes { get; set; }
        DbSet<InvitationGroupEntity> InvitationGroups { get; set; }
        DbSet<InvitationGroupMappingEntity> InvitationGroupMappings { get; set; }
        DbSet<InvitationKeyEntity> InvitationKeys { get; set; }
        DbSet<InvitationSourceEntity> InvitationSources { get; set; }
        DbSet<InvitationSourceMappingEntity> InvitationSourceMappings { get; set; }
        DbSet<JournalizedStatusEntity> JournalizedStatuses { get; set; }
        DbSet<KleHierarchyEntity> KleHierarchies { get; set; }
        DbSet<KleMappingEntity> KleMappings { get; set; }
        DbSet<NotificationContentSpecificationEntity> NotificationContentSpecifications { get; set; }
        DbSet<NotificationContentTypeEntity> NotificationContentTypes { get; set; }
        DbSet<NotificationEntity> Notifications { get; set; }
        DbSet<NotificationContentEntity> NotificationContents { get; set; }
        DbSet<NotificationQueueEntity> NotificationQueues { get; set; }
        DbSet<NotificationTemplateEntity> NotificationTemplates { get; set; }
        DbSet<NotificationTypeEntity> NotificationTypes { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<SubjectAreaEntity> SubjectAreas { get; set; }
        DbSet<UserCapacityEntity> UserCapacities { get; set; }
        DbSet<UserEntity> UsersDb { get; set; }
        DbSet<UserHearingRoleEntity> UserHearingRoles { get; set; }
        DbSet<ValidationRuleEntity> ValidationRules { get; set; }
        DbSet<CompanyEntity> Companies { get; set; }
        DbSet<CompanyHearingRoleEntity> CompanyHearingRoles { get; set; }
    }
}
