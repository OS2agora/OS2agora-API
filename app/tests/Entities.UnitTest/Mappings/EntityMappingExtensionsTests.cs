using System.Collections.Generic;
using System.Linq;
using BallerupKommune.DAOs.Mappings;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Models.Models;
using NUnit.Framework;

namespace BallerupKomune.Entities.UnitTest.Mappings
{
    public class EntityMappingExtensionsTests
    {
        [Test]
        public void CopyUpdatedPropertiesAndRelationships_ValidComplexProperty()
        {
            var existingEntity = new NotificationQueueEntity();
            var newEntity = new NotificationQueueEntity
            {
                ErrorTexts = new[]
                {
                    "Test1",
                    "Test2"
                }
            };

            Assert.AreNotEqual(newEntity.ErrorTexts, existingEntity.ErrorTexts);

            var updatedProperties = new List<string>
            {
                "ErrorTexts"
            };
            existingEntity.CopyUpdatedPropertiesAndRelationships(newEntity, updatedProperties);

            Assert.AreEqual(newEntity.ErrorTexts, existingEntity.ErrorTexts);
        }

        [Test]
        public void CopyUpdatedPropertiesAndRelationships_InvalidPropertyNames()
        {
            var existingEntity = new SubjectAreaEntity();
            var newEntity = new SubjectAreaEntity
            {
                Name = "TestNavn",
                IsActive = true
            };

            Assert.AreNotEqual(newEntity.Name, existingEntity.Name);
            Assert.AreNotEqual(newEntity.IsActive, existingEntity.IsActive);

            var updatedProperties = new List<string>
            {
                "InvalidName",
                "InvalidIsActive"
            };

            existingEntity.CopyUpdatedPropertiesAndRelationships(newEntity, updatedProperties);

            Assert.AreNotEqual(newEntity.Name, existingEntity.Name);
            Assert.AreNotEqual(newEntity.IsActive, existingEntity.IsActive);
        }

        [Test]
        public void CopyUpdatedPropertiesAndRelationships_ValidPropertyNames()
        {
            var existingEntity = new SubjectAreaEntity();
            var newEntity = new SubjectAreaEntity
            {
                Name = "TestNavn",
                IsActive = true
            };

            Assert.AreNotEqual(newEntity.Name, existingEntity.Name);
            Assert.AreNotEqual(newEntity.IsActive, existingEntity.IsActive);

            var updatedProperties = new List<string>
            {
                "Name",
                "IsActive"
            };

            existingEntity.CopyUpdatedPropertiesAndRelationships(newEntity, updatedProperties);

            Assert.AreEqual(newEntity.Name, existingEntity.Name);
            Assert.AreEqual(newEntity.IsActive, existingEntity.IsActive);
        }

        [Test]
        public void CopyUpdatedPropertiesAndRelationships_ValidAndInvalidPropertyNames()
        {
            var existingEntity = new SubjectAreaEntity();
            var newEntity = new SubjectAreaEntity
            {
                Name = "TestNavn",
                IsActive = true
            };

            Assert.AreNotEqual(newEntity.Name, existingEntity.Name);
            Assert.AreNotEqual(newEntity.IsActive, existingEntity.IsActive);

            var updatedProperties = new List<string>
            {
                "Name",
                "InvalidIsActive"
            };

            existingEntity.CopyUpdatedPropertiesAndRelationships(newEntity, updatedProperties);

            Assert.AreEqual(newEntity.Name, existingEntity.Name);
            Assert.AreNotEqual(newEntity.IsActive, existingEntity.IsActive);
        }

        [Test]
        public void CopyUpdatedPropertiesAndRelationships_EmptyPropertyList()
        {
            var existingEntity = new SubjectAreaEntity();
            var newEntity = new SubjectAreaEntity();

            existingEntity.CopyUpdatedPropertiesAndRelationships(newEntity, new List<string>());

            Assert.AreEqual(newEntity.Name, existingEntity.Name);
            Assert.AreEqual(newEntity.IsActive, existingEntity.IsActive);
        }

        [Test]
        public void CopyUpdatedPropertiesAndRelationships_ListIsNull()
        {
            var existingEntity = new SubjectAreaEntity();
            var newEntity = new SubjectAreaEntity();

            existingEntity.CopyUpdatedPropertiesAndRelationships(newEntity, null);

            Assert.AreEqual(newEntity.Name, existingEntity.Name);
            Assert.AreEqual(newEntity.IsActive, existingEntity.IsActive);
        }

        [Test]
        public void CopyUpdatedPropertiesAndRelationships_Relationships()
        {
            var hearingEntity = new HearingEntity();
            var existingEntity = new SubjectAreaEntity();
            var newEntity = new SubjectAreaEntity
            {
                Hearings = new List<HearingEntity> { hearingEntity }
            };

            var updatedProperties = new List<string> { nameof(SubjectArea.Hearings) };
            existingEntity.CopyUpdatedPropertiesAndRelationships(newEntity, updatedProperties);

            Assert.AreEqual(newEntity.Hearings, existingEntity.Hearings);
        }
        
        [Test]
        public void CopyUpdatedPropertiesAndRelationships_Relationship()
        {
            const int subjectAreaId = 7;
            var existingEntity = new HearingEntity();
            var newEntity = new HearingEntity
            {
                SubjectAreaId = subjectAreaId
            };
            
            existingEntity.CopyUpdatedPropertiesAndRelationships(newEntity, Enumerable.Empty<string>());

            Assert.That(existingEntity.SubjectAreaId, Is.EqualTo(newEntity.SubjectAreaId));
        }
        
        [Test]
        public void CopyUpdatedPropertiesAndRelationships_WithoutRelationship()
        {
            var existingEntity = new HearingEntity {SubjectAreaId = 7};
            var newEntity = new HearingEntity {SubjectAreaId = default};
            
            existingEntity.CopyUpdatedPropertiesAndRelationships(newEntity, Enumerable.Empty<string>());

            Assert.That(existingEntity.SubjectAreaId, Is.Not.EqualTo(newEntity.SubjectAreaId));
        }
    }
}