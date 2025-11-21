using Agora.Operations.Common.Behaviours;
using Agora.Operations.Common.Interfaces.Security;
using MediatR;
using Moq;
using NovaSec;
using NovaSec.Compiler;
using NovaSec.Compiler.Resolvers;

namespace Agora.Operations.UnitTests.Models
{
    public abstract class ModelsTestBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly Mock<IInjectResolver> InjectResolverMock;
        protected readonly Mock<ISecurityExpressionRoot> SecurityExpressionRoot;
        protected readonly Mock<ISecurityExpressions> SecurityExpressionsMock;
        protected readonly SecurityContext SecurityContext;
        protected readonly Mock<RequestHandlerDelegate<TResponse>> RequestHandlerDelegateMock;
        protected readonly SecurityBehaviour<TRequest, TResponse> SecurityBehaviour;

        protected ModelsTestBase()
        {
            InjectResolverMock = new Mock<IInjectResolver>();
            SecurityExpressionRoot = new Mock<ISecurityExpressionRoot>();
            SecurityExpressionsMock = new Mock<ISecurityExpressions>();


            InjectResolverMock.Setup(x => x.Exists("Security"))
                .Returns(true);
            InjectResolverMock.Setup(x => x.Resolve("Security"))
                .Returns(SecurityExpressionsMock.Object);

            SecurityContext = new SecurityContext(SecurityExpressionRoot.Object, InjectResolverMock.Object);

            RequestHandlerDelegateMock = new Mock<RequestHandlerDelegate<TResponse>>();
            SecurityBehaviour = new SecurityBehaviour<TRequest, TResponse>(SecurityContext);
        }
    }
}