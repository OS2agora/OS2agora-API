using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.Comments.Commands.SoftDeleteComment;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Agora.Operations.UnitTests.Models.Comments.Commands
{
    public class SoftDeleteTests : ModelsTestBase<SoftDeleteCommentCommand, Comment>
    {
        public SoftDeleteTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResults()));
        }

        [Test]
        [TestCase(true )]
        [TestCase(false)]
        public async Task UpdateCommentCommand_HasRole( bool ownsComment)
        {
            var request = new SoftDeleteCommentCommand();
            SecurityExpressionsMock.Setup(x => x.IsCommentOwnerByCommentId(It.IsAny<int>()))
                .Returns(ownsComment);


            if (!ownsComment)
            {
                FluentActions
                    .Invoking(() =>
                        SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                    .Should().Throw<ForbiddenAccessException>();
            }
            else
            {
                var result = await
                    SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
                Assert.IsTrue(result.Hearing != null && result.ContainsSensitiveInformation && !result.IsDeleted &&
                              result.CommentParrent != null && result.Contents.Any() && result.User != null &&
                              result.CommentType != null && !string.IsNullOrEmpty(result.OnBehalfOf) &&
                              result.CommentStatus != null);
            }


        }

        private Comment GetHandlerResults()
        {
            return new Comment()
            {
                Id = 1,
                Hearing = new Hearing(),
                CommentParrent = new Comment(),
                Contents = new List<Content>
                {
                    new Content()
                },
                User = new User(),
                CommentType = new CommentType(),
                ContainsSensitiveInformation = true,
                CreatedBy = "Tester",
                IsDeleted = false,
                CommentStatus = new CommentStatus(),
                OnBehalfOf = "Tester2"
            };
        }
    }
}