using AutoMapper;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Models;
using NUnit.Framework;
using System;
using System.Runtime.Serialization;
using BallerupKommune.DAOs.Mappings;

namespace BallerupKomune.Entities.UnitTest.Mappings
{
    public class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void AssertIsConfigurationValid()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Test]
        [TestCase(typeof(CommentEntity), typeof(Comment))]
        [TestCase(typeof(CommentStatusEntity), typeof(CommentStatus))]
        [TestCase(typeof(CommentTypeEntity), typeof(CommentType))]
        [TestCase(typeof(ConsentEntity), typeof(Consent))]
        [TestCase(typeof(ContentEntity), typeof(Content))]
        [TestCase(typeof(ContentTypeEntity), typeof(ContentType))]
        [TestCase(typeof(FieldEntity), typeof(Field))]
        [TestCase(typeof(FieldTemplateEntity), typeof(FieldTemplate))]
        [TestCase(typeof(FieldTypeEntity), typeof(FieldType))]
        [TestCase(typeof(FieldTypeSpecificationEntity), typeof(FieldTypeSpecification))]
        [TestCase(typeof(GlobalContentEntity), typeof(GlobalContent))]
        [TestCase(typeof(GlobalContentTypeEntity), typeof(GlobalContentType))]
        [TestCase(typeof(HearingEntity), typeof(Hearing))]
        [TestCase(typeof(HearingTemplateEntity), typeof(HearingTemplate))]
        [TestCase(typeof(HearingTypeEntity), typeof(HearingType))]
        [TestCase(typeof(KleHierarchyEntity), typeof(KleHierarchy))]
        [TestCase(typeof(KleMappingEntity), typeof(KleMapping))]
        [TestCase(typeof(NotificationEntity), typeof(Notification))]
        [TestCase(typeof(NotificationQueueEntity), typeof(NotificationQueue))]
        [TestCase(typeof(NotificationTemplateEntity), typeof(NotificationTemplate))]
        [TestCase(typeof(NotificationTypeEntity), typeof(NotificationType))]
        [TestCase(typeof(SubjectAreaEntity), typeof(SubjectArea))]
        [TestCase(typeof(UserCapacityEntity), typeof(UserCapacity))]
        [TestCase(typeof(UserEntity), typeof(User))]
        [TestCase(typeof(UserHearingRoleEntity), typeof(UserHearingRole))]
        [TestCase(typeof(ValidationRuleEntity), typeof(ValidationRule))]
        [TestCase(typeof(JournalizedStatusEntity), typeof(JournalizedStatus))]
        [TestCase(typeof(CompanyEntity), typeof(Company))]
        [TestCase(typeof(CompanyHearingRoleEntity), typeof(CompanyHearingRole))]
        public void TestMappingFromSourceToDestination(Type source, Type destination)
        {
            var sourceInstance = GetInstanceOf(source);
            var destinationSource = GetInstanceOf(destination);

            // Map from Entity to Model
            _mapper.Map(sourceInstance, destinationSource);

            // Map from Model To Entity
            _mapper.Map(destinationSource, sourceInstance);
        }

        private object GetInstanceOf(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null ? Activator.CreateInstance(type) : FormatterServices.GetUninitializedObject(type);
        }
    }
}
