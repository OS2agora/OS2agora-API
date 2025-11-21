using System.Collections.Generic;
using Agora.Models.Common;
using Agora.Models.Extensions;
using Agora.Models.Models;
using NUnit.Framework;

namespace Agora.Models.UnitTests.Extensions
{
    public class TypeExtensionsTests
    {
        [Test]
        public void GetRelationships_GetsRelationships()
        {
            List<RelationshipInfo<GlobalContent>> relationships = TypeExtensions.GetRelationships<GlobalContent>();

            Assert.Multiple(() =>
            {
                Assert.That(relationships, Has.Count.EqualTo(1));
                Assert.That(relationships, Has.Exactly(1).Matches<RelationshipInfo<GlobalContent>>(relationship =>
                    relationship.IdPropertyInfo.Name == nameof(GlobalContent.GlobalContentTypeId) &&
                    relationship.ObjectPropertyInfo.Name == nameof(GlobalContent.GlobalContentType)));
            });
        }
    }
}