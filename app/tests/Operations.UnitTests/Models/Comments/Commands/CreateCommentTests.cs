using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.Comments.Commands.CreateComment;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Agora.Operations.UnitTests.Models.Comments.Commands
{
    public class CreateCommentCommandTests : ModelsTestBase<CreateCommentCommand, Comment>
    {
        public CreateCommentCommandTests()
        {
            RequestHandlerDelegateMock
                .Setup(x=>x())
                .Returns(Task.FromResult(new Comment()));
        }
        [SetUp]
        public void SetUp()
        {

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(false);

            SecurityExpressionsMock.Setup(x => x.IsHearingOwner(It.IsAny<int>()))
                .Returns(false);

            SecurityExpressionsMock.Setup(x => x.IsHearingReviewer(It.IsAny<int>()))
                .Returns(false);

            SecurityExpressionRoot.Setup(x => x.HasAnyRole(It.IsAny<List<string>>()))
                .Returns(false);

        }

        [Test]
        [TestCase(true,true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public async Task CreateComment_HasRole_Employee_Citizen(bool isEmployee, bool isCitizen)
        {

            SecurityExpressionRoot.Setup(x => x.HasAnyRole(It.IsAny<List<string>>()))
                .Returns((isEmployee));
            
            if (!isEmployee)
            {
                SecurityExpressionRoot.Setup(x => x.HasAnyRole(It.IsAny<List<string>>()))
                    .Returns(isCitizen);
            }

            var request = new CreateCommentCommand();
            if (!isEmployee && !isCitizen)
            {
                FluentActions
                    .Invoking(() =>
                        SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                    .Should().Throw<ForbiddenAccessException>();
            }
            else
            {
                var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
                Assert.IsNotNull(result);
            }
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public async Task CreateComment_HearingOwner_HearingReviewer(bool isOwner, bool isReviewer)
        {
            SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
                .Returns(isOwner);
            SecurityExpressionsMock.Setup(x => x.IsHearingReviewerByHearingId(It.IsAny<int>()))
                .Returns(isReviewer);
            var request = new CreateCommentCommand();

            if (!isOwner && !isReviewer)
            {
                FluentActions
                    .Invoking(() =>
                        SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                    .Should().Throw<ForbiddenAccessException>();
            }
            else
            {
                var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
                Assert.IsNotNull(result);
            }
        }
    }
}
