using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManagement.UI.Filters.ActionFilters.Account;

public class AccountSubmitRegisterActionFilter : ActionFilterAttribute
{
    public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controller = context.Controller as Controller;
        if (context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }

        if (!controller!.ModelState.IsValid)
        {
            var errors = controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            context.Result = new BadRequestObjectResult(errors);
        }
        return base.OnActionExecutionAsync(context, next);
    }
}