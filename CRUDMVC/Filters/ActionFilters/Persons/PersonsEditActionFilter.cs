using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts;
using ServiceContracts.Persons;

namespace CRUDMVC.Filters.ActionFilters.Persons;

public class PersonsEditActionFilter : ActionFilterAttribute
{
    private readonly ILogger<PersonsEditActionFilter> _logger;
    private readonly IPersonsGetterService _personsGetterService;

    public PersonsEditActionFilter(ILogger<PersonsEditActionFilter> logger, IPersonsGetterService personsGetterService)
    {
        _logger = logger;
        _personsGetterService = personsGetterService;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var personID = Guid.Parse((string)context.RouteData.Values["personID"]);
        if (personID == Guid.Empty)
        {
            _logger.LogError("Person ID: {personID} is invalid", personID);
            context.Result = new BadRequestResult();
            return;
        }

        else if (await _personsGetterService.GetPersonByPersonID(personID) == null)
        {
            _logger.LogInformation("Person with personID : {personID} not found" , personID);
            context.Result = new NotFoundResult();
            return;
        }
        else
        {
            await next();
        }

    }
}