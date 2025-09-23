using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Resolvers;
using MediatR;
using Moq;
using NUnit.Framework;

namespace BallerupKommune.Operations.UnitTests.Resolvers
{
    internal class HearingAccessResolverTest
    {

        private HearingAccessResolver _resolver;
        private Mock<ISender> _mediatorMock;

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<ISender>();
            _resolver = new HearingAccessResolver(_mediatorMock.Object);
        }

        [Test]
        [TestCase(1,true)]
        [TestCase(2,false)]
        public async Task CanSeeHearingById(int hearingId, bool correctanswer)
        {
            var hearings = new List<Hearing>
            {
                new Hearing
                {
                    Id = 1,
                    UserHearingRoles = new List<UserHearingRole>
                    {
                        new UserHearingRole
                        {
                            Id = 1,
                            UserId = 1,
                            HearingId = 1,
                            HearingRoleId = 1
                        },
                        new UserHearingRole
                        {
                            Id = 2,
                            UserId = 1,
                            HearingId = 1,
                            HearingRoleId = 2
                        }
                    }

                }
                
            };
            _mediatorMock.Setup(mediator => mediator
                .Send(It.IsAny<IRequest<List<Hearing>>>(),It.IsAny<CancellationToken>()))
                .ReturnsAsync(hearings);
            bool resolverResult = await _resolver.CanSeeHearingById(hearingId);
            Assert.That(resolverResult.Equals(correctanswer));
        }

        [Test]
        [TestCase(1, true)]
        [TestCase(2, false)]
        public async Task CanSeeSubjectAreaByHearingId(int subjectareaId, bool correctanswer)
        {
            var hearings = new List<Hearing>
            {
                new Hearing
                {
                    Id = 1,
                    SubjectAreaId = 1
                },
                new Hearing
                {
                    Id = 2, 
                    SubjectAreaId = 1
                }

            };
            _mediatorMock.Setup(mediator => mediator
                    .Send(It.IsAny<IRequest<List<Hearing>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(hearings);
            bool resolverResult = await _resolver.CanSeeHearingBySubjectAreaId(subjectareaId);
            Assert.That(resolverResult.Equals(correctanswer));
        }
    }
}
