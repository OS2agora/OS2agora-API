using Agora.Operations.Common.Exceptions;
using JsonApiSerializer.JsonApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Agora.Models.Models;
using Agora.Operations.Common.Constants;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;

namespace Agora.Api.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
        private readonly ILogger<ApiExceptionFilterAttribute> _logger;

        public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
        {
            _logger = logger;
            // Register known exception types and handlers.
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ValidationException), HandleValidationException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
                { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
                { typeof(InvalidOperationException), HandleInvalidOperationException },
                { typeof(FileUploadException), HandleFileUploadException },
                { typeof(KleMappingException), HandleKleMappingException },
                { typeof(EmptyConclusionException), HandleEmptyConclusionException },
                { typeof(SbsipAuthorizationException), HandleSbsipAuthorizationException },
                { typeof(PaginationException), HandlePaginationException },
                { typeof(SortAndFilterException), HandleSortAndFilterException },
                { typeof(InvalidFileContentException), HandleInvalidFileContentException}
            };
        }

        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            _logger.LogError(exception, $"OnException caught the following error: {exception.Message}");

            HandleException(context);

            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            var type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            var innerExceptionType = context.Exception.InnerException?.GetType();

            if (innerExceptionType != null && _exceptionHandlers.ContainsKey(innerExceptionType))
            {
                _exceptionHandlers[innerExceptionType].Invoke(context);
                return;
            }

            if (!context.ModelState.IsValid)
            {
                HandleInvalidModelStateException(context);
                return;
            }

            HandleUnknownException(context);
        }

        private void HandleNotFoundException(ExceptionContext context)
        {
            var exception = context.Exception as NotFoundException;
            var innerException = context.Exception.InnerException as NotFoundException;

            var message = exception != null ? exception.Message :
                innerException != null ? innerException.Message : string.Empty;

            var details = new Error
            {
                Title = "The specified resource was not found.",
                Status = StatusCodes.Status404NotFound.ToString(),
                Detail = message,
                Code = ExceptionCodes.NotFound
            };

            context.Result = new NotFoundObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleForbiddenAccessException(ExceptionContext context)
        {
            var details = new Error
            {
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden.ToString(),
                Code = ExceptionCodes.ForbiddenAccess
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

            context.ExceptionHandled = true;
        }

        private void HandleInvalidOperationException(ExceptionContext context)
        {
            var exception = (InvalidOperationException)context.Exception;

            var details = new Error
            {
                Title = "Invalid Operation",
                Status = StatusCodes.Status400BadRequest.ToString(),
                Detail = exception.Message,
                Code = ExceptionCodes.InvalidOperation
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

            context.ExceptionHandled = true;
        }

        private void HandleUnauthorizedAccessException(ExceptionContext context)
        {
            var details = new Error
            {
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized.ToString(),
                Code = ExceptionCodes.UnauthorizedAccess
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };

            context.ExceptionHandled = true;
        }

        private void HandlePaginationException(ExceptionContext context)
        {
            var exception = context.Exception;

            var paginationFailure = new Error
            {
                Title = "PaginationError",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest.ToString(),
                Code = ExceptionCodes.Validation
            };

            context.Result = new BadRequestObjectResult(paginationFailure);

            context.ExceptionHandled = true;
        }

        private void HandleValidationException(ExceptionContext context)
        {
            var exception = context.Exception as ValidationException;

            var validationFailures = exception.Errors.Select(failure => new Error
            {
                Title = exception.Message,
                Status = StatusCodes.Status422UnprocessableEntity.ToString(),
                Code = ExceptionCodes.Validation,
                Source = new ErrorSource
                {
                    Pointer = failure.Key
                },
                Meta = failure.Value.Aggregate(new Meta(), (meta, value) =>
                {
                    meta.Add($"message-{meta.Count() + 1}", JToken.FromObject(value ?? string.Empty));
                    return meta;
                })
            });

            context.Result = new BadRequestObjectResult(validationFailures);

            context.ExceptionHandled = true;
        }

        private void HandleKleMappingException(ExceptionContext context)
        {
            var exception = context.Exception as KleMappingException;
            var kleHierarchies = exception?.KleHierarchies ?? new List<KleHierarchy>();


            var kleMappingFailure = new Error
            {
                Title = exception.Message,
                Status = StatusCodes.Status422UnprocessableEntity.ToString(),
                Code = ExceptionCodes.KleMapping,
                Meta = kleHierarchies.Aggregate(new Meta(), (meta, value) =>
                {
                    var kleNumber = $"{value.Number} {value.Name}";
                    meta.Add($"{value.Id}", JToken.FromObject(kleNumber));
                    return meta;
                })
            };

            context.Result = new BadRequestObjectResult(kleMappingFailure);

            context.ExceptionHandled = true;
        }

        private void HandleEmptyConclusionException(ExceptionContext context)
        {
            var exception = context.Exception as EmptyConclusionException;

            var emptyConclusionFailure = new Error
            {
                Title = exception.Message,
                Status = StatusCodes.Status400BadRequest.ToString(),
                Code = ExceptionCodes.EmptyConclusion
            };

            context.Result = new BadRequestObjectResult(emptyConclusionFailure);

            context.ExceptionHandled = true;
        }

        private void HandleSbsipAuthorizationException(ExceptionContext context)
        {
            var exception = context.Exception as SbsipAuthorizationException;

            var sbsipAuthorizationFailure = new Error
            {
                Title = exception.Message,
                Status = StatusCodes.Status400BadRequest.ToString(),
                Code = ExceptionCodes.SbsipAuthorization
            };

            context.Result = new ObjectResult(sbsipAuthorizationFailure)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };

            context.ExceptionHandled = true;
        }

        private void HandleInvalidModelStateException(ExceptionContext context)
        {
            var details = new Error
            {
                Title = "A validation failure found in the model state.",
                Status = StatusCodes.Status422UnprocessableEntity.ToString(),
                Code = ExceptionCodes.InvalidModelState
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            var details = new Error
            {
                Title = "An error occurred while processing your request.",
                Status = StatusCodes.Status422UnprocessableEntity.ToString(),
                Code = ExceptionCodes.Unknown
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }

        private void HandleFileUploadException(ExceptionContext context)
        {
            var exception = context.Exception as FileUploadException;
            var fileNames = exception?.FileNames ?? Array.Empty<string>();

            var details = new Error
            {
                Title = "A file error occurred",
                Status = StatusCodes.Status422UnprocessableEntity.ToString(),
                Code = ExceptionCodes.FileUpload,
                Meta = fileNames.Aggregate(new Meta(), (meta, value) =>
                {
                    meta.Add($"file-{meta.Count() + 1}", JToken.FromObject(value ?? string.Empty));
                    return meta;
                })
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }

        private void HandleSortAndFilterException(ExceptionContext context)
        {
            var exception = context.Exception as SortAndFilterException;

            var sortAndFilterFailures = exception.Errors.Select(failure => new Error
            {
                Title = exception.Message,
                Status = failure.Key == SortAndFilterExceptionTypes.ProcessingError ? StatusCodes.Status400BadRequest.ToString() : StatusCodes.Status422UnprocessableEntity.ToString(),
                Code = ExceptionCodes.SortAndFilter,
                Source = new ErrorSource
                {
                    Pointer = failure.Key
                },
                Meta = failure.Value.Aggregate(new Meta(), (meta, value) =>
                {
                    meta.Add($"message-{meta.Count() + 1}", JToken.FromObject(value ?? string.Empty));
                    return meta;
                })
            });

            context.Result = new BadRequestObjectResult(sortAndFilterFailures);

            context.ExceptionHandled = true;
        }

        private void HandleInvalidFileContentException(ExceptionContext context)
        {
            var exception = context.Exception as InvalidFileContentException;

            var details = new Error
            {
                Title = "Invalid file content",
                Status = StatusCodes.Status422UnprocessableEntity.ToString(),
                Code = ExceptionCodes.InvalidFileContent,
                Detail = exception?.Message,
            };

            context.Result = new UnprocessableEntityObjectResult(details);

            context.ExceptionHandled = true;
        }
    }
}