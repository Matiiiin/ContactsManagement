using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManagement.UI.Filters.GlobalFilters;

public class AddCustomHeaderResponseGlobalActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        context.HttpContext.Response.Headers["X-Custom-Header"] = "Custom Header Value";
    }
}