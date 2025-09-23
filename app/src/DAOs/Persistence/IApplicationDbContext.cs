using BallerupKommune.DAOs.Identity;
using BallerupKommune.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace BallerupKommune.DAOs.Persistence
{
    public interface IApplicationDbContext
    {
        DbSet<CommentEntity> Comments { get; set; }
        DbSet <CommentDeclineInfoEntity> CommentDeclineInfos { get; set; }
        DbSet<CommentStatusEntity> CommentStatuses { get; set; }
        DbSet<CommentTypeEntity> CommentTypes { get; set; }
        DbSet<ContentEntity> Contents { get; set; }
        DbSet<ContentTypeEntity> ContentTypes { get; set; }
        DbSet<FieldEntity> Fields { get; set; }
        DbSet<FieldTemplateEntity> FieldTemplates { get; set; }
        DbSet<FieldTypeEntity> FieldTypes { get; set; }
        DbSet<FieldTypeSpecificationEntity> FieldTypeSpecifications { get; set; }
        DbSet<HearingEntity> Hearings { get; set; }
        DbSet<HearingRoleEntity> HearingRoles { get; set; }
        DbSet<HearingStatusEntity> HearingStatus { get; set; }
        DbSet<HearingTemplateEntity> HearingTemplates { get; set; }
        DbSet<HearingTypeEntity> HearingTypes { get; set; }
        DbSet<JournalizedStatusEntity> JournalizedStatuses { get; set; }
        DbSet<KleHierarchyEntity> KleHierarchies { get; set; }
        DbSet<KleMappingEntity> KleMappings { get; set; }
        DbSet<NotificationEntity> Notifications { get; set; }
        DbSet<NotificationQueueEntity> NotificationQueues { get; set; }
        DbSet<NotificationTemplateEntity> NotificationTemplates { get; set; }
        DbSet<NotificationTypeEntity> NotificationTypes { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<SubjectAreaEntity> SubjectAreas { get; set; }
        DbSet<UserEntity> UsersDb { get; set; }
        DbSet<UserHearingRoleEntity> UserHearingRoles { get; set; }
        DbSet<ValidationRuleEntity> ValidationRules { get; set; }
        DbSet<CompanyEntity> Companies { get; set; }
        DbSet<CompanyHearingRoleEntity> CompanyHearingRoles { get; set; }
    }
}
