using BallerupKommune.DAOs.Identity;
using BallerupKommune.DAOs.Persistence.Configurations;
using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Common;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Operations.Common.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        private const string UnidentifiedUser = "Unidentified User";

        private readonly IEncryptionValueConverterFactory _encryptionValueConverterFactory;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTimeService;

        public ApplicationDbContext(
            DbContextOptions options,
            IEncryptionValueConverterFactory encryptionValueConverterFactory,
            ICurrentUserService currentUserService,
            IDateTime dateTimeService)
            : base(options)
        {
            _encryptionValueConverterFactory = encryptionValueConverterFactory;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public DbSet<CommentEntity> Comments { get; set; }
        public DbSet<CommentDeclineInfoEntity> CommentDeclineInfos { get; set; }
        public DbSet<CommentStatusEntity> CommentStatuses { get; set; }
        public DbSet<CommentTypeEntity> CommentTypes { get; set; }
        public DbSet<ConsentEntity> Consents { get; set; }
        public DbSet<ContentEntity> Contents { get; set; }
        public DbSet<ContentTypeEntity> ContentTypes { get; set; }
        public DbSet<FieldEntity> Fields { get; set; }
        public DbSet<FieldTemplateEntity> FieldTemplates { get; set; }
        public DbSet<FieldTypeEntity> FieldTypes { get; set; }
        public DbSet<FieldTypeSpecificationEntity> FieldTypeSpecifications { get; set; }
        public DbSet<GlobalContentEntity> GlobalContents { get; set; }
        public DbSet<GlobalContentTypeEntity> GlobalContentTypes { get; set; }
        public DbSet<HearingEntity> Hearings { get; set; }
        public DbSet<HearingRoleEntity> HearingRoles { get; set; }
        public DbSet<HearingStatusEntity> HearingStatus { get; set; }
        public DbSet<HearingTemplateEntity> HearingTemplates { get; set; }
        public DbSet<HearingTypeEntity> HearingTypes { get; set; }
        public DbSet<JournalizedStatusEntity> JournalizedStatuses { get; set; }
        public DbSet<KleHierarchyEntity> KleHierarchies { get; set; }
        public DbSet<KleMappingEntity> KleMappings { get; set; }
        public DbSet<NotificationEntity> Notifications { get; set; }
        public DbSet<NotificationQueueEntity> NotificationQueues { get; set; }
        public DbSet<NotificationTemplateEntity> NotificationTemplates { get; set; }
        public DbSet<NotificationTypeEntity> NotificationTypes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<SubjectAreaEntity> SubjectAreas { get; set; }
        public DbSet<UserCapacityEntity> UserCapacities { get; set; }
        public DbSet<UserEntity> UsersDb { get; set; }
        public DbSet<UserHearingRoleEntity> UserHearingRoles { get; set; }
        public DbSet<ValidationRuleEntity> ValidationRules { get; set; }
        public DbSet<CompanyEntity> Companies { get; set; }
        public DbSet<CompanyHearingRoleEntity> CompanyHearingRoles { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        // If JWT Bearer token was not sent with the request we can't identify the current User unless explicit set in DAO
                        entry.Entity.CreatedBy ??= _currentUserService.UserId ?? UnidentifiedUser;
                        entry.Entity.Created = _dateTimeService.Now;
                        break;

                    case EntityState.Modified:
                        // If JWT Bearer token was not sent with the request we can't identify the current User unless explicit set in DAO
                        entry.Entity.LastModifiedBy = _currentUserService.UserId ?? UnidentifiedUser;
                        entry.Entity.LastModified = _dateTimeService.Now;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Configuration classes that requires injected services must be applied manually.
            builder.ApplyConfiguration(new NotificationQueueConfiguration(_encryptionValueConverterFactory));
            builder.ApplyConfiguration(new ContentConfiguration(_encryptionValueConverterFactory));
            builder.ApplyConfiguration(new UserConfiguration(_encryptionValueConverterFactory));
            builder.ApplyConfiguration(new CompanyConfiguration(_encryptionValueConverterFactory));
            builder.ApplyConfiguration(new CommentDeclineInfoConfiguration(_encryptionValueConverterFactory));

            base.OnModelCreating(builder);

            // changes to applicationUser must be applied after base-configurations
            builder.ApplyConfiguration(new ApplicationUserConfiguration(_encryptionValueConverterFactory));
        }
    }
}