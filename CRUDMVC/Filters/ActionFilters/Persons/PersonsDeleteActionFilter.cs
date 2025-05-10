using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts;

namespace CRUDMVC.Filters.ActionFilters;

public class PersonsDeleteActionFilter : ActionFilterAttribute
{
    private readonly ILogger<PersonsDeleteActionFilter> _logger;
    private readonly IPersonsService _personsService;

    public PersonsDeleteActionFilter(ILogger<PersonsDeleteActionFilter> logger, IPersonsService personsService)
    {
        _logger = logger;
        _personsService = personsService;
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

        var personResponse = await _personsService.GetPersonByPersonID(personID);
        if (personResponse == null)
        {
            _logger.LogWarning("Person with personID: {personID} not found.", personID);
            context.Result = new NotFoundObjectResult("Person not found.");
            return;
        }

        await next();
    }
}