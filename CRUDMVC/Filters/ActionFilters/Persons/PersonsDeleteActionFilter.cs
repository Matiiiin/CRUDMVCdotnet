using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts;
using ServiceContracts.Persons;

namespace CRUDMVC.Filters.ActionFilters.Persons;

public class PersonsDeleteActionFilter : ActionFilterAttribute
{
    private readonly ILogger<PersonsDeleteActionFilter> _logger;
    private readonly IPersonsGetterService _personsGetterService;

    public PersonsDeleteActionFilter(ILogger<PersonsDeleteActionFilter> logger, IPersonsGetterService personsGetterService)
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