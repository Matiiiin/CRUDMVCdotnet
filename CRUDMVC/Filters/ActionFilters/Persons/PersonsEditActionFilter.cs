using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts;

namespace CRUDMVC.Filters.ActionFilters.Persons;

public class PersonsEditActionFilter : ActionFilterAttribute
{
    private readonly ILogger<PersonsEditActionFilter> _logger;
    private readonly IPersonsService _personsService;

    public PersonsEditActionFilter(ILogger<PersonsEditActionFilter> logger, IPersonsService personsService)
    {
        _logger = logger;
        _personsService = personsService;
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

        var personResponse = await _personsService.GetPersonByPersonID(personID);
        if (personResponse == null)
        {
            _logger.LogInformation("Person with personID : {personID} not found" , personID);
            context.Result = new NotFoundResult();
            return;
        }

    }
}