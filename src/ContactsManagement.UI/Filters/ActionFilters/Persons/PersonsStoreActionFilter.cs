﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CRUDMVC.Filters.ActionFilters.Persons;

public class PersonsStoreActionFilter : ActionFilterAttribute
{
    private readonly ILogger<PersonsStoreActionFilter> _logger;

    public PersonsStoreActionFilter(ILogger<PersonsStoreActionFilter> logger)
    {
        _logger = logger;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.ContainsKey(nameof(PersonAddRequest)))
        {
            _logger.LogError("No Person Add Request is provided");
            context.Result = new BadRequestObjectResult("No Person Add data is provided");
            return;
        }
        else
        {
            await next();
        }

    }
}