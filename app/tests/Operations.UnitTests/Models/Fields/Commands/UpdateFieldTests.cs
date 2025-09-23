using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Models.Fields.Commands.UpdateFields;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.UnitTests.Models.Fields.Commands
{
    public class UpdateFieldTests : ModelsTestBase<UpdateFieldsCommand, List<Content>>
    {
        public UpdateFieldTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResults()));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task UpdateFieldCommand_HasRole(bool shouldSucceed)
        {
            var request = new UpdateFieldsCommand();

            SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
                .Returns(shouldSucceed);

            if (!shouldSucceed)
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
                Assert.IsTrue(result.All(x => !string.IsNullOrEmpty(x.FileContentType) && x.Hearing != null
                    && x.Field != null && !string.IsNullOrEmpty(x.FileName) && !string.IsNullOrEmpty(x.FilePath) &&
                    !string.IsNullOrEmpty(x.TextContent) && x.Comment != null && x.ContentType != null));
            }
        }

        private List<Content> GetHandlerResults()
        {
            return new List<Content>
            {
                new Content
                {
                    Id = 3,
                    Field = new Field(),
                    Hearing = new Hearing(),
                    Comment = new Comment(),
                    ContentType = new ContentType(),
                    FileContentType = "Test",
                    FileName = "Test",
                    FilePath = "Test",
                    TextContent = "Test"
                },
                new Content
                {
                    Id = 2,
                    Field = new Field(),
                    Hearing = new Hearing(),
                    Comment = new Comment(),
                    ContentType = new ContentType(),
                    FileContentType = "Test2",
                    FileName = "Test2",
                    FilePath = "Test2",
                    TextContent = "Test2"
                }
            };
        }
    }
}