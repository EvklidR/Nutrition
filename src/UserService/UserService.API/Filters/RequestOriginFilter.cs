using Microsoft.AspNetCore.Mvc.Filters;

namespace UserService.API.Filters;

public class RequestOriginFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;

        var forwardedProto = request.Headers["X-Forwarded-Proto"].FirstOrDefault();
        var forwardedHost = request.Headers["X-Forwarded-Host"].FirstOrDefault();

        var scheme = forwardedProto ?? request.Scheme;
        var host = forwardedHost ?? request.Host.ToString();

        var requestOrigin = $"{scheme}://{host}";

        context.HttpContext.Items["RequestOrigin"] = requestOrigin;
    }
}
