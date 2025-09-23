using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Models.HearingTypes.Queries.GetHearingTypes;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.UnitTests.Models.HearingTypes.Queries
{
    public class GetHearingTypesTest : ModelsTestBase<GetHearingTypesQuery, List<HearingType>>
    {
        public GetHearingTypesTest()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasAnyRole(It.IsAny<List<string>>()))
                .Returns((List<string> param) => param.Any(x => SecurityExpressionRoot.Object.HasRole(x)));
        }

        [SetUp]
        public void SetUp()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResult()));
        }

        [Test]
        public async Task GetHearingTypes_HearingOwner_Administrator_Returns_All()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => param == "Administrator" || param == "HearingOwner");

            var request = new GetHearingTypesQuery();

            var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.IsTrue(result.Count == 3);
            Assert.IsTrue(result.All(x => x.KleMappings.Any() && x.HearingTemplate != null && x.Hearings.Any() && x.FieldTemplates.Any() && x.Id != 0 && x.IsActive));
        }

        [Test]
        public async Task GetHearingTypes_Anonymous_Citizen_Returns_ExternalHearings_WithRedaction()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => param == "Anonymous" || param == "Citizen");

            var request = new GetHearingTypesQuery();

            var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.All(x => !x.IsInternalHearing && x.KleMappings == null && x.HearingTemplate == null && x.Hearings.Any() && x.FieldTemplates.Any() && x.Id != 0 && x.IsActive));
        }

        [Test]
        public async Task GetHearingTypes_Employee_InternalHearings_WithRedaction()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => param == "Employee");

            var request = new GetHearingTypesQuery();

            var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.All(x => x.IsInternalHearing && x.KleMappings == null && x.HearingTemplate == null && x.Hearings.Any() && x.FieldTemplates.Any() && x.Id != 0 && x.IsActive));
        }

        [Test]
        public async Task GetHearingTypes_Employee_Administrator_HearingOwner_Returns_All()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => param == "Administrator" || param == "HearingOwner" || param == "Employee");

            var request = new GetHearingTypesQuery();

            var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.IsTrue(result.Count == 3);
            Assert.IsTrue(result.All(x => x.KleMappings.Any() && x.HearingTemplate != null && x.Hearings.Any() && x.FieldTemplates.Any() && x.Id != 0 && x.IsActive));
        }

        private List<HearingType> GetHandlerResult()
        {
            return new List<HearingType>
            {
                new HearingType
                {
                    Id = 3,
                    IsInternalHearing = true,
                    IsActive = true,
                    FieldTemplates = new List<FieldTemplate>
                    {
                        new FieldTemplate()
                    },
                    KleMappings = new List<KleMapping>
                    {
                        new KleMapping()
                    },
                    HearingTemplate = new HearingTemplate(),
                    Hearings = new List<Hearing>
                    {
                        new Hearing()
                    }

                },
                new HearingType
                {
                    Id = 2,
                    IsInternalHearing = true,
                    IsActive = true,
                    FieldTemplates = new List<FieldTemplate>
                    {
                        new FieldTemplate()
                    },
                    KleMappings = new List<KleMapping>
                    {
                        new KleMapping()
                    },
                    HearingTemplate = new HearingTemplate(),
                    Hearings = new List<Hearing>
                    {
                        new Hearing()
                    }
                },
                new HearingType
                {
                    Id = 1,
                    IsInternalHearing = false,
                    IsActive = true,
                    FieldTemplates = new List<FieldTemplate>
                    {
                        new FieldTemplate()
                    },
                    KleMappings = new List<KleMapping>
                    {
                        new KleMapping()
                    },
                    HearingTemplate = new HearingTemplate(),
                    Hearings = new List<Hearing>
                    {
                        new Hearing()
                    }
                }
            };
        }
    }
}