using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using NUnit.Framework;

namespace BallerupKommune.Models.UnitTests.Common
{
    public class RelationshipInfoTests
    {
        [Test]
        public void HasTheSameIdsOnProperties_WhenIdsExistsAndAreTheSame_ReturnsTrue()
        {
            const int parentId = 321;
            var parent = new Comment {Id = parentId};
            var comment = new Comment {CommentParrentId = parentId, CommentParrent = parent};
            
            Assert.IsTrue(GetGlobalContentTypeRelationship().HasTheSameIdsOnProperties(comment));
        }
        
        [Test]
        public void HasTheSameIdsOnProperties_WhenIdsExistsAndAreDeffirent_ReturnsFalse()
        {
            const int parentId = 321;
            var parent = new Comment {Id = parentId};
            var comment = new Comment {CommentParrentId = parentId + 1, CommentParrent = parent};
            
            Assert.IsFalse(GetGlobalContentTypeRelationship().HasTheSameIdsOnProperties(comment));
        }
        
        [Test]
        public void HasTheSameIdsOnProperties_WhenIdAndObjectAreNull_ReturnsTrue()
        {
            var comment = new Comment {CommentParrentId = null, CommentParrent = null};
            
            Assert.IsTrue(GetGlobalContentTypeRelationship().HasTheSameIdsOnProperties(comment));
        }
        
        [Test]
        public void HasTheSameIdsOnProperties_WhenIdExistsAndObjectIsNull_ReturnsFalse()
        {
            const int parentId = 321;
            var comment = new Comment {CommentParrentId = parentId, CommentParrent = null};
            
            Assert.IsFalse(GetGlobalContentTypeRelationship().HasTheSameIdsOnProperties(comment));
        }
        
        [Test]
        public void HasTheSameIdsOnProperties_WhenIdIsNullAndObjectIsExists_ReturnsFalse()
        {
            const int parentId = 321;
            var parent = new Comment {Id = parentId};
            var comment = new Comment {CommentParrentId = null, CommentParrent = parent};
            
            Assert.IsFalse(GetGlobalContentTypeRelationship().HasTheSameIdsOnProperties(comment));
        }

        private static RelationshipInfo<Comment> GetGlobalContentTypeRelationship()
        {
            return new RelationshipInfo<Comment>
            {
                IdPropertyInfo = typeof(Comment).GetProperty(nameof(Comment.CommentParrentId)),
                ObjectPropertyInfo = typeof(Comment).GetProperty(nameof(Comment.CommentParrent))
            };
        }
    }
}