using UserService.Application.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace UserService.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
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
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    result = exception.Message;
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}
