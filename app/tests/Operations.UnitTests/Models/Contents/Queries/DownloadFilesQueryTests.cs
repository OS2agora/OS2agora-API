using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Models.Contents.Queries.DownloadFiles;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.UnitTests.Models.Contents.Queries
{
    public class DownloadFilesQueryTests : ModelsTestBase<DownloadFilesQuery, FileDownload>
    {
        public DownloadFilesQueryTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(new FileDownload()));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task IsHearingOwner(bool isHearingOwner)
        {
            var request = new DownloadFilesQuery();

            SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
                .Returns(isHearingOwner);

            if (!isHearingOwner)
            {
                FluentActions
                    .Invoking(() =>
                        SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                    .Should().Throw<ForbiddenAccessException>();
            }
            else
            {
                var result = await SecurityBehaviour.Handle(request, CancellationToken.None,
                    RequestHandlerDelegateMock.Object);
                Assert.IsNotNull(result);
            }
        }
    }
}