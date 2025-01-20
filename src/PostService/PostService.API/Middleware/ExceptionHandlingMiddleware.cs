using PostService.BusinessLogic.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace PostService.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            HttpStatusCode statusCode;
            object result;

            switch (exception)
            {
                case AlreadyExists alreadyExistsEx:
                    statusCode = HttpStatusCode.Conflict;
                    result = alreadyExistsEx.Message;
                    break;
                case BadRequest badRequestEx:
                    statusCode = HttpStatusCode.BadRequest;
                    result = badRequestEx.Errors;
                    break;
                case Unauthorized unauthorEx:
                    statusCode = HttpStatusCode.Unauthorized;
                    result = unauthorEx.Message;
                    break;
                case NotFound notFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    result = notFoundEx.Message;
                    break;
                case Forbidden forbiddenEx:
                    statusCode = HttpStatusCode.Forbidden;
                    result = forbiddenEx.Message;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    result = exception.Message;
                    break;
            }

            logger.LogError($"Error occurred during execution. Code: {statusCode}, message: {result}");

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}
