using System;
using MediatR;
using NovaSec;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Telemetry;

namespace Agora.Operations.Common.Behaviours
{
    public class SecurityBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly SecurityContext _securityContext;

        public SecurityBehaviour(SecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            MethodInfo handlerMethodInfo = GetHandlerMethodInfo();
            var handlerArguments = new object[] {request, cancellationToken};
            
            bool preAuthorized = PreAuthorizeRequest(request, handlerMethodInfo, handlerArguments);
            if (!preAuthorized)
            {
                throw new ForbiddenAccessException();
            }

            TResponse response = await next();

            return PostFilterResponse(response, request, handlerMethodInfo, handlerArguments);
        }

        private bool PreAuthorizeRequest(TRequest request, MethodInfo handlerMethodInfo, object[] handlerArguments)
        {
            using Activity activity = Instrumentation.Source.StartActivity($"NovaSec pre-authorize: {typeof(TRequest).Name}");
            
            List<PreAuthorizeAttribute> preAuthorizeAttributes = GetPreAuthorizeAttributes(request);
            var observer = new SecurityContextObserver();

            bool preAuthorizeResult = _securityContext.PreAuthorize(handlerMethodInfo, handlerArguments, preAuthorizeAttributes, observer);
            AddPreAuthorizeTelemetryTags(activity, observer);

            return preAuthorizeResult;
        }

        private TResponse PostFilterResponse(TResponse response, TRequest request, MethodInfo handlerMethodInfo, object[] handlerArguments)
        {
            bool responseIsEnumerable = typeof(TResponse).GetInterfaces().Any(interfaceType =>
                interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            Type responseModelType = responseIsEnumerable ? typeof(TResponse).GenericTypeArguments[0] : typeof(TResponse);
            
            using Activity activity = Instrumentation.Source.StartActivity($"NovaSec post-filters: {responseModelType.Name}");

            List<PostFilterAttribute> postFilterAttributes = GetPostFilterAttributes(request, responseModelType);
            
            var observer = new SecurityContextObserver();
                
            if (responseIsEnumerable)
            {
                // Since we are now sure about TResponse being a IEnumerable<X> we can dynamic give it to PostFilter
                dynamic changedResult = response;

                var postFilterResult = _securityContext.PostFilter(changedResult, postFilterAttributes, handlerMethodInfo, handlerArguments, observer);
                AddPostFilterTelemetryTags(activity, observer);
                return postFilterResult;
            }
            else
            {
                // Not an IEnumerable<X> so we put it inside a list, and take it out after
                var responseInsideList = new List<TResponse> { response };
                var postFilterResult = _securityContext.PostFilter(responseInsideList, postFilterAttributes, handlerMethodInfo, handlerArguments, observer);
                AddPostFilterTelemetryTags(activity, observer);
                return postFilterResult.FirstOrDefault();
            }
        }

        private static MethodInfo GetHandlerMethodInfo()
        {
            Type correctHandler = typeof(DependencyInjection).Assembly.GetTypes().Single(type =>
                type.IsClass && type.GetInterfaces().Any(interfaceType =>
                    interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) &&
                    interfaceType.GenericTypeArguments[0] == typeof(TRequest)));

            return correctHandler.GetMethod(nameof(IRequestHandler<TRequest,TResponse>.Handle));
        }

        private static List<PreAuthorizeAttribute> GetPreAuthorizeAttributes(TRequest request)
        {
            return GetRequestAttributes<PreAuthorizeAttribute>(request);
        }
        
        private static List<PostFilterAttribute> GetPostFilterAttributes(TRequest request, Type responseModelType)
        {
            IEnumerable<PostFilterAttribute> modelPostFilterAttributes =
                responseModelType.GetCustomAttributes<PostFilterAttribute>();
            return GetRequestAttributes<PostFilterAttribute>(request).Concat(modelPostFilterAttributes).ToList();
        }

        private static List<TAttribute> GetRequestAttributes<TAttribute>(TRequest request) where TAttribute : BaseAttribute
        {
            return request.GetType().GetCustomAttributes<TAttribute>().ToList();
        }
        
        private static void AddPreAuthorizeTelemetryTags(Activity activity, SecurityContextObserver observer)
        {
            if (activity is null || observer is null)
            {
                return;
            }

            AddCommonTelemetryTags(activity, observer);
            activity.AddTag("novasec.accumulated.preauthorize.duration", $"{observer.AccumulatedPreAuthorizeDuration} ms");
        }

        private static void AddPostFilterTelemetryTags(Activity activity, SecurityContextObserver observer)
        {
            if (activity is null || observer is null)
            {
                return;
            }
            
            AddCommonTelemetryTags(activity, observer);
            activity.AddTag("novasec.accumulated.postfilter.duration", $"{observer.AccumulatedPostFilterDuration} ms");
            activity.AddTag("novasec.postfilter.total", observer.PostFilterTotal);
            activity.AddTag("novasec.postfilter.passed", observer.PostFilterPassed);
            activity.AddTag("novasec.postfilter.failed", observer.PostFilterFailed);
        }

        private static void AddCommonTelemetryTags(Activity activity, SecurityContextObserver observer)
        {
            activity.AddTag("novasec.accumulated.compilation.duration", $"{observer.AccumulatedCompilationDuration} ms");
            activity.AddTag("novasec.accumulated.evaluation.duration", $"{observer.AccumulatedEvaluationDuration} ms");
        }
    }
}