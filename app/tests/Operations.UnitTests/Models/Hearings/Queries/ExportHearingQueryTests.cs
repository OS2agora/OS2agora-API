using BallerupKommune.Models.Enums;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Models.Hearings.Queries.ExportHearing;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.UnitTests.Models.Hearings.Queries
{
    public class ExportHearingQueryTests : ModelsTestBase<ExportHearingQuery, FileDownload>
    {
        public ExportHearingQueryTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(new FileDownload()));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task IsHearingOwner_Success(bool isHearingOwner)
        {
            SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>())).Returns(isHearingOwner);

            var request = new ExportHearingQuery
            {
                Format = ExportFormat.NONE,
                Id = 1
            };

            if (!isHearingOwner)
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