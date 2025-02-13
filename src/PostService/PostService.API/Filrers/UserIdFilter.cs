using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PostService.API.Filters
{
    public class UserIdFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            var userNameClaim = user.FindFirst(ClaimTypes.Name);

            if (userIdClaim == null || userNameClaim == null)
            {
                context.Result = new UnauthorizedResult();

                return;
            }

            context.HttpContext.Items["UserId"] = userIdClaim.Value;
            context.HttpContext.Items["UserName"] = userNameClaim.Value;
        }
    }
}