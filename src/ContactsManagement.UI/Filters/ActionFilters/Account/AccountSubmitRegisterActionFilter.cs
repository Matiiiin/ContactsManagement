﻿using ContactsManagement.Core.DTO.Identities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;

namespace ContactsManagement.UI.Filters.ActionFilters.Account;

public class AccountSubmitRegisterActionFilter : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controller = context.Controller as Controller;
        if (context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new RedirectToActionResult("Index", "Persons", null);
        }

        else if (!controller!.ModelState.IsValid)
        {
            var errors = controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            controller.ViewBag.Errors = errors;
            context.Result = new ViewResult()
            {
                ViewName = "~/Views/Account/Register.cshtml",
            };
        }
        else
        {
            await next();
        }

    }
}