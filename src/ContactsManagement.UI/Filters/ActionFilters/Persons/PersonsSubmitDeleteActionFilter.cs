using ContactsManagement.Core.ServiceContracts.Persons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManagement.UI.Filters.ActionFilters.Persons;

public class PersonsSubmitDeleteActionFilter : ActionFilterAttribute
{
    private readonly ILogger<PersonsSubmitDeleteActionFilter> _logger;
    private readonly IPersonsGetterService _personsGetterService;

    public PersonsSubmitDeleteActionFilter(ILogger<PersonsSubmitDeleteActionFilter> logger, IPersonsGetterService personsGetterService)
    {
        _logger = logger;
        _personsGetterService = personsGetterService;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.ContainsKey("personID") ||
            context.ActionArguments["personID"] is not Guid personID ||
            personID == Guid.Empty)
        {
            _logger.LogError("Invalid or missing personID.");
            context.Result = new BadRequestObjectResult("Invalid or missing personID.");
            return;
        }

        else if (await _personsGetterService.GetPersonByPersonID(personID) == null)
        {
            _logger.LogWarning("Person with personID: {personID} not found.", personID);
            context.Result = new NotFoundObjectResult("Person not found.");
            return;
        }
        else
        {
            await next();
        }

    }
}