using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Agora.Operations.Common.Behaviours
{
    public class TelemetryBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            using Activity activity = Telemetry.Instrumentation.Source.StartActivity($"Operation: {typeof(TRequest).Name!}");
            return await next();
        }
    }
}