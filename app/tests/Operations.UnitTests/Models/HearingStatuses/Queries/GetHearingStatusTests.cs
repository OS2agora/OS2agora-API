using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Models.HearingStatuses.Queries.GetHearingStatus;
using NUnit.Framework;

namespace BallerupKommune.Operations.UnitTests.Models.HearingStatuses.Queries
{
    public class GetHearingStatusTests : ModelsTestBase<GetHearingStatusesQuery, List<HearingStatus>>
    {
        public GetHearingStatusTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResults()));
        }

        [Test]
        public async Task GetHearingStatus()
        {
            var request = new GetHearingStatusesQuery();

            var result = await 
                SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.All(x => 
                x.Status == BallerupKommune.Models.Enums.HearingStatus.ACTIVE 
                && !string.IsNullOrEmpty(x.Name) 
                && !string.IsNullOrEmpty(x.CreatedBy)
                && x.Hearings == null ));
            
        }

        private List<HearingStatus> GetHandlerResults()
        {
            return new List<HearingStatus>
            {
                new HearingStatus()
                {
                    Id = 1,
                    Name = "Name",
                    Hearings = new List<Hearing>
                    {
                        new Hearing()
                    },
                    Status = BallerupKommune.Models.Enums.HearingStatus.ACTIVE,
                    CreatedBy = "Someone"
                },
                new HearingStatus()
                {
                    Id = 2,
                    Name = "Different Name",
                    Hearings = new List<Hearing>
                    {
                        new Hearing()
                    },
                    Status = BallerupKommune.Models.Enums.HearingStatus.ACTIVE,
                    CreatedBy = "Someone else"
                }
            };
        }
    }
}