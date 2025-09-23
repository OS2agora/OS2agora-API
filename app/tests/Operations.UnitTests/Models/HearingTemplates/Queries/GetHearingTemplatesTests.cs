using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Models.HearingTemplates.Queries.GetHearingTemplates;
using Moq;
using NUnit.Framework;

namespace BallerupKommune.Operations.UnitTests.Models.HearingTemplates.Queries
{
    public class GetHearingTemplatesTests : ModelsTestBase<GetHearingTemplatesQuery, List<HearingTemplate>>
    {
        private Mock<IHearingTemplateDao> _hearingTemplateDaoMock;

        [SetUp]
        public void SetUp()
        {
            _hearingTemplateDaoMock = new Mock<IHearingTemplateDao>();
        }

        [TearDown]
        public void TearDown()
        {
            RequestHandlerDelegateMock.Reset();
            SecurityExpressionRoot.Reset();
        }
        
        [Test]
        public async Task GetHearingTemplates_ForAdmin_ShouldNotRedactFieldTemplates()
        {
            SecurityExpressionRoot.Setup(x => x.HasRole("Administrator")).Returns(true);
            var request = new GetHearingTemplatesQuery();
            RequestHandlerDelegateMock.Setup(handler => handler()).ReturnsAsync(CreateQueryResult());
            HearingTemplate result =
                (await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                .Single();
            
            Assert.Multiple(() =>
            {
                Assert.That(result.Fields.Single().FieldTemplates, Is.Not.Empty);
                Assert.That(result.HearingTypes.Single().FieldTemplates, Is.Not.Empty);
            });
        }

        [Test]
        public async Task GetHearingTemplates_ForNonAdmin_ShouldRedactFieldTemplates()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasAnyRole(new List<string> {"Anonymous", "Citizen", "Employee"}))
                .Returns(true);
            var request = new GetHearingTemplatesQuery();
            RequestHandlerDelegateMock.Setup(handler => handler()).ReturnsAsync(CreateQueryResult());
            HearingTemplate result =
                (await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                .Single();
            
            Assert.Multiple(() =>
            {
                Assert.That(result.Fields.Single().FieldTemplates, Is.Null);
                Assert.That(result.HearingTypes.Single().FieldTemplates, Is.Null);
            });
        }

        [Test]
        public async Task GetHearingTemplates_ShouldParseRequestIncludesToDao()
        {
            SecurityExpressionRoot.Setup(x => x.HasRole("Administrator")).Returns(true);
            var requestIncludes = new List<string> {"Fields", "Fields.FieldTypes"};
            
            var request = new GetHearingTemplatesQuery {RequestIncludes = requestIncludes};
            var queryHandler = 
                new GetHearingTemplatesQuery.GetHearingTemplatesQueryHandler(_hearingTemplateDaoMock.Object);

            _hearingTemplateDaoMock
                .Setup(dao => dao.GetAllAsync(It.IsAny<IncludeProperties>()))
                .ReturnsAsync(CreateQueryResult());

            await queryHandler.Handle(request, CancellationToken.None);

            _hearingTemplateDaoMock.Verify(dao =>
                dao.GetAllAsync(It.Is<IncludeProperties>(includes => includes.RequestIncludes == requestIncludes)));
        }

        private static List<HearingTemplate> CreateQueryResult()
        {
            var hearingTemplate = new HearingTemplate {Id = 5};
            
            var hearingType = new HearingType {Id = 10, HearingTemplate = hearingTemplate};
            hearingTemplate.HearingTypes = new List<HearingType> {hearingType};
            
            var field = new Field {Id = 15, HearingTemplate = hearingTemplate};
            hearingTemplate.Fields = new List<Field> {field};
            
            var fieldTemplate = new FieldTemplate {Id = 20, Field = field, HearingType = hearingType};
            field.FieldTemplates = new List<FieldTemplate> {fieldTemplate};
            hearingType.FieldTemplates = new List<FieldTemplate> {fieldTemplate};

            return new List<HearingTemplate> {hearingTemplate};
        }
    }
}