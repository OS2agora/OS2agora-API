using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common.CustomResponse.Pagination;
using Agora.Models.Common.CustomResponse;
using Agora.Operations.Common.Behaviours;
using Agora.Operations.Common.Constants;
using MediatR;
using Moq;
using NUnit.Framework;
using Agora.Operations.Common.CustomRequests;
using Agora.Operations.Common.CustomRequests.Validators;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces;
using FluentAssertions;

namespace Agora.Operations.UnitTests.Common.Behaviours
{
    [TestFixture]
    public class PaginationBehaviourTests
    {
        private Mock<RequestHandlerDelegate<IList<int>>> RequestHandlerDelegateMock;
        private PaginationBehaviour<TestPaginationRequest, IList<int>> PaginationBehaviour;
        private IPaginationValidator PaginationValidator;


        [SetUp]
        public void SetUp()
        {
            RequestHandlerDelegateMock = new Mock<RequestHandlerDelegate<IList<int>>>();
            PaginationValidator = new PaginationValidator();
            PaginationBehaviour = new PaginationBehaviour<TestPaginationRequest, IList<int>>(PaginationValidator);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Handle_WithoutPaginationParameters_ReturnsResponseUnchanged(bool paginationParametersAreNull)
        {
            TestPaginationRequest request = null; 
            if (paginationParametersAreNull)
            {
                request = new TestPaginationRequest(null);
            }
            else
            {
                new TestPaginationRequest(new PaginationParameters { PageIndex = null, PageSize = null });
            }

            var response = GetHandlerResult();
            RequestHandlerDelegateMock.Setup(x => x()).ReturnsAsync(response);

            var result = await PaginationBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.AreEqual(response, result);
        }

        [Test]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, -1)]
        [TestCase(1, null)]
        [TestCase(null, 1)]
        public async Task Handle_WithInvalidPaginationParameters_ThrowsException(int? pageIndex, int? pageSize)
        {

            var request = new TestPaginationRequest(new PaginationParameters { PageIndex = pageIndex, PageSize = pageSize });
            var response = GetHandlerResult();
            RequestHandlerDelegateMock.Setup(x => x()).ReturnsAsync(response);

            FluentActions
                .Invoking(() =>
                    PaginationBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                .Should().Throw<PaginationException>();
        }

        [Test]
        [TestCase(1, false)]
        [TestCase(2, false)]
        [TestCase(1, true)]
        public async Task Handle_WithValidPaginationParameters_ReturnsPaginatedResponse(int pageIndex, bool responseWithMetadata)
        {
            var pageSize = 5;
            var request = new TestPaginationRequest(new PaginationParameters { PageIndex = pageIndex, PageSize = pageSize });

            var response = responseWithMetadata ? GetHandlerResultWithMetaData() : GetHandlerResult();
            RequestHandlerDelegateMock.Setup(x => x()).ReturnsAsync(response);


            var result = await PaginationBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object) as ResponseList<int>;

            var expectedResult = response.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            CollectionAssert.AreEqual(expectedResult, result);
            Assert.IsTrue(result.Meta.ContainsKey(ResponseListMetaDataKeys.Pagination));

            if (responseWithMetadata)
            {
                Assert.IsTrue(result.Meta.Count == 2);
                Assert.IsTrue(result.Meta.ContainsKey("TestMetaData"));
            }
            else
            {
                Assert.IsTrue(result.Meta.Count == 1);
            }


        }

        [Test]
        public async Task Handle_WithPageIndexGreaterThanTotalPages_ReturnsEmptyResponse()
        {

            var request = new TestPaginationRequest(new PaginationParameters { PageIndex = 3, PageSize = 2 });
            var response = new List<int> { 1, 2 };
            RequestHandlerDelegateMock.Setup(x => x()).ReturnsAsync(response);


            var result = await PaginationBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object) as ResponseList<int>;
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
            Assert.IsTrue(result.Meta.ContainsKey(ResponseListMetaDataKeys.Pagination));
        }

        public class TestPaginationRequest : PaginationRequest<IList<int>>
        {
            public TestPaginationRequest(PaginationParameters paginationParameters)
            {
                PaginationParameters = paginationParameters;
            }
        }

        class TestMetaData
        {
            public string Property1 = "Value1";

        }

        private List<int> GetHandlerResult()
        {
            return Enumerable.Range(1, 20).ToList();
        }

        private ResponseList<int> GetHandlerResultWithMetaData()
        {
            return new ResponseList<int>(GetHandlerResult(), "TestMetaData", new TestMetaData());
        }
    }
}
