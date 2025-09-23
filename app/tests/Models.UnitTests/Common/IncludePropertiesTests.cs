using System.Collections.Generic;
using System.Linq;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.TestUtilities;
using NUnit.Framework;

namespace BallerupKommune.Models.UnitTests.Common
{
    public class IncludePropertiesTests
    {
        [Test]
        [GenericTestCase(typeof(SubjectArea))]
        [GenericTestCase(typeof(SubjectArea), "Hearings")]
        [GenericTestCase(typeof(SubjectArea), "Hearings", "Hearings.HearingType", "Hearings.UserHearingRoles")]
        [GenericTestCase(typeof(SubjectArea), "Hearings.HearingType.HearingTemplate.Fields.FieldTemplates")]
        [GenericTestCase(typeof(User), "UserHearingRoles", "UserHearingRoles.HearingRole", "UserCapacity")]
        public void GetIncludes_ValidIncludes<T>(params string[] includes) where T : BaseModel
        {
            Assert.DoesNotThrow(() => IncludeProperties.Create<T>());
        }

        [Test]
        [GenericTestCase(typeof(Comment))]
        [GenericTestCase(typeof(CommentStatus))]
        [GenericTestCase(typeof(CommentType))]
        [GenericTestCase(typeof(Content))]
        [GenericTestCase(typeof(ContentType))]
        [GenericTestCase(typeof(Field))]
        [GenericTestCase(typeof(FieldTemplate))]
        [GenericTestCase(typeof(FieldType))]
        [GenericTestCase(typeof(FieldTypeSpecification))]
        [GenericTestCase(typeof(Hearing))]
        [GenericTestCase(typeof(HearingRole))]
        [GenericTestCase(typeof(HearingStatus))]
        [GenericTestCase(typeof(HearingTemplate))]
        [GenericTestCase(typeof(HearingType))]
        [GenericTestCase(typeof(KleHierarchy))]
        [GenericTestCase(typeof(KleMapping))]
        [GenericTestCase(typeof(Notification))]
        [GenericTestCase(typeof(NotificationQueue))]
        [GenericTestCase(typeof(NotificationTemplate))]
        [GenericTestCase(typeof(NotificationType))]
        [GenericTestCase(typeof(SubjectArea))]
        [GenericTestCase(typeof(User))]
        [GenericTestCase(typeof(UserCapacity))]
        [GenericTestCase(typeof(UserHearingRole))]
        [GenericTestCase(typeof(ValidationRule))]
        [GenericTestCase(typeof(Company))]
        [GenericTestCase(typeof(CompanyHearingRole))]
        public void GetIncludes_IfDefaultIncludesExistsItShouldNotBeEmpty<T>() where T : BaseModel
        {
            var doesDefaultIncludeExist = typeof(T).GetProperty("DefaultIncludes");

            if (doesDefaultIncludeExist != null)
            {
                var defaultIncludes = doesDefaultIncludeExist.GetValue(null) as List<string> ?? new List<string>();
                Assert.IsTrue(defaultIncludes.Any());
            }
        }

        [Test]
        [GenericTestCase(typeof(Comment))]
        [GenericTestCase(typeof(CommentStatus))]
        [GenericTestCase(typeof(CommentType))]
        [GenericTestCase(typeof(Content))]
        [GenericTestCase(typeof(ContentType))]
        [GenericTestCase(typeof(Field))]
        [GenericTestCase(typeof(FieldTemplate))]
        [GenericTestCase(typeof(FieldType))]
        [GenericTestCase(typeof(FieldTypeSpecification))]
        [GenericTestCase(typeof(Hearing))]
        [GenericTestCase(typeof(HearingRole))]
        [GenericTestCase(typeof(HearingStatus))]
        [GenericTestCase(typeof(HearingTemplate))]
        [GenericTestCase(typeof(HearingType))]
        [GenericTestCase(typeof(KleHierarchy))]
        [GenericTestCase(typeof(KleMapping))]
        [GenericTestCase(typeof(Notification))]
        [GenericTestCase(typeof(NotificationQueue))]
        [GenericTestCase(typeof(NotificationTemplate))]
        [GenericTestCase(typeof(NotificationType))]
        [GenericTestCase(typeof(SubjectArea))]
        [GenericTestCase(typeof(User))]
        [GenericTestCase(typeof(UserCapacity))]
        [GenericTestCase(typeof(UserHearingRole))]
        [GenericTestCase(typeof(ValidationRule))]
        [GenericTestCase(typeof(Company))]
        [GenericTestCase(typeof(CompanyHearingRole))]
        public void GetIncludes_ShouldNotBeAField<T>() where T : BaseModel
        {
            var defaultIncludeField = typeof(T).GetField("DefaultIncludes");
            Assert.IsNull(defaultIncludeField);
        }
    }
}