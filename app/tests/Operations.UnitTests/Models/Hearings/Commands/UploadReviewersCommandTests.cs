using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Models.Hearings.Command.UploadReviewers;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BallerupKommune.Operations.UnitTests.Models.Hearings.Commands
{
    public class UploadReviewersCommandTests : ModelsTestBase<UploadReviewersCommand, List<UserHearingRole>>
    {
        public UploadReviewersCommandTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(new List<UserHearingRole>()));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task UploadReviewersList_IsHearingOwner(bool isHearingOwner)
        {
            SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
                .Returns(isHearingOwner);

            var request = new UploadReviewersCommand();

            if (isHearingOwner)
            {
                var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
                Assert.IsNotNull(result);
            }
            else
            {
                FluentActions
                    .Invoking(() =>
                        SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                    .Should().Throw<ForbiddenAccessException>();
            }
        }
    }
}
