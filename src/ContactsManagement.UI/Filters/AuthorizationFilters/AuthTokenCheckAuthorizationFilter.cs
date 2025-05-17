using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManagement.UI.Filters.AuthorizationFilters;

public class AuthTokenCheckAuthorizationFilter : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.ContainsKey("AuthToken"))
        {
            context.Result = new UnauthorizedResult();
            return;
        } 
        if (context.HttpContext.Request.Headers["AuthToken"] != "ABCD")
        {
            context.Result = new UnauthorizedResult();
        }
    }
}