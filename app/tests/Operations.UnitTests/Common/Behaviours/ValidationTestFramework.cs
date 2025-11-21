using Agora.Operations.Common.Behaviours;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Common.Behaviours
{
    public static class ValidationTestFramework
    {
        public static ValidationTestBuilder<TRequest, TResponse> For<TRequest, TResponse>() where TRequest : IRequest<TResponse>
        {
            return new ValidationTestBuilder<TRequest, TResponse>();
        }
    }

    public class ValidationTestBuilder<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private IValidator<TRequest>[] _validators = Array.Empty<IValidator<TRequest>>();

        public ValidationTestBuilder<TRequest, TResponse> WithValidators(params IValidator<TRequest>[] validators)
        {
            _validators = validators ?? Array.Empty<IValidator<TRequest>>();
            return this;
        }

        public ValidationTestBuilder<TRequest, TResponse> WithoutValidators()
        {
            _validators = Array.Empty<IValidator<TRequest>>();
            return this;
        }

        public async Task ShouldPassValidation(TRequest request)
        {
            var validationBehaviour = new ValidationBehaviour<TRequest, TResponse>(_validators);
            var handlerMock = new Mock<RequestHandlerDelegate<TResponse>>();

            await validationBehaviour.Handle(request, CancellationToken.None, handlerMock.Object);

            handlerMock.Verify(x => x(), Times.Once);
        }

        public async Task ShouldFailValidation(TRequest request, params string[] expectedErrorKeys)
        {
            var validationBehaviour = new ValidationBehaviour<TRequest, TResponse>(_validators);
            var handlerMock = new Mock<RequestHandlerDelegate<TResponse>>();

            var exception = await FluentActions.Invoking(async () =>
                await validationBehaviour.Handle(request, CancellationToken.None, handlerMock.Object))
                .Should().ThrowAsync<Agora.Operations.Common.Exceptions.ValidationException>();

            if (expectedErrorKeys?.Length > 0)
            {
                exception.Which.Errors.Should().ContainKeys(expectedErrorKeys);
            }

            handlerMock.Verify(x => x(), Times.Never);
        }

        public async Task ShouldFailValidationWithError(TRequest request, string expectedErrorKey)
        {
            await ShouldFailValidation(request, expectedErrorKey);
        }

        public async Task ShouldThrowNullReferenceException(TRequest request)
        {
            var validationBehaviour = new ValidationBehaviour<TRequest, TResponse>(_validators);
            var handlerMock = new Mock<RequestHandlerDelegate<TResponse>>();

            await FluentActions.Invoking(async () =>
                await validationBehaviour.Handle(request, CancellationToken.None, handlerMock.Object))
                .Should().ThrowAsync<NullReferenceException>();

            handlerMock.Verify(x => x(), Times.Never);
        }
    }
}