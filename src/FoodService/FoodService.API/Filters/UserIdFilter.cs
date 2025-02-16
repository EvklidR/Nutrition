using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using FoodService.Application.Exceptions;

namespace FoodService.API.Filters
{
    public class UserIdFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            var userIdClaim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value.ToString();

            if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
            {
                context.HttpContext.Items["UserId"] = userId;
            }
            else
            {
                throw new Unauthorized("Token doesn't have correct id");
            }

        }
    }
}
