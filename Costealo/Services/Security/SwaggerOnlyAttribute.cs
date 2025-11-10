using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Costealo.Services.Security;

public class SwaggerOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext ctx)
    {
        var referer = ctx.HttpContext.Request.Headers["Referer"].ToString();
        var isSwagger = !string.IsNullOrEmpty(referer) && referer.Contains("/swagger");
        var hasHeaderKey = ctx.HttpContext.Request.Headers.TryGetValue("X-Swagger-Key", out var k)
                           && k == ctx.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Swagger:Key"];
        if (!(isSwagger || hasHeaderKey)) ctx.Result = new ForbidResult();
    }
}