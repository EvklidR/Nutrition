using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UserService.API.Filters
{
    public class RequestOriginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var scheme = context.HttpContext.Request.Scheme;
            var host = context.HttpContext.Request.Host;

            var requestOrigin = $"{scheme}://{host}";

            context.HttpContext.Items["RequestOrigin"] = requestOrigin;
        }
    }
}
