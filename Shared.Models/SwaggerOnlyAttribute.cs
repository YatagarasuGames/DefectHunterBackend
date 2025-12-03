using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shared.Models
{
    public class SwaggerOnlyAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;

            var referer = request.Headers["Referer"].ToString();
            var userAgent = request.Headers["User-Agent"].ToString();


            var isFromSwagger = referer.Contains("/swagger/") ||
                               userAgent.Contains("Swagger") ||
                               request.Headers.ContainsKey("X-Swagger-Request");

            if (!isFromSwagger)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
