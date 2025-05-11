using CRUDMVC.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CRUDMVC.Filters.ActionFilters.Persons;

public class PersonsIndexActionFilter : ActionFilterAttribute
{
    private readonly ILogger<PersonsIndexActionFilter> _logger;

    public PersonsIndexActionFilter(ILogger<PersonsIndexActionFilter> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("in {className} , performing {methodName}" ,nameof(PersonsIndexActionFilter) , nameof(OnActionExecuting));
        //Validate if the searchBy parameter is already one of person attributes
        if (context.ActionArguments.ContainsKey("searchBy"))
        {
            var searchBy = context.ActionArguments["searchBy"] as string;
            var propertyExists = typeof(PersonResponse).GetProperties().Any(p=> p.Name.Equals(searchBy));
            if (!propertyExists)
            {
                _logger.LogWarning(
                    "Invalid searchBy parameter: {SearchBy}. No matching property found in PersonResponse.", searchBy);
                context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                _logger.LogInformation("Replaced searchBy Parameter with: {SearchBy}" , nameof(PersonResponse.PersonName));
            }
        }
        context.HttpContext.Items["arguments"] = context.ActionArguments;

    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("in {className} , performing {methodName}" ,nameof(PersonsIndexActionFilter) , nameof(OnActionExecuted));

        var personsController = context.Controller as PersonsController;
        personsController!.ViewBag.SearchFields = new Dictionary<string, string>()
        {
            {nameof(PersonResponse.PersonName) , "Person Name"},
            {nameof(PersonResponse.Email) , "Email"},
            {nameof(PersonResponse.DateOfBirth) , "Date Of Birth"},
            {nameof(PersonResponse.Gender) , "Gender"},
            {nameof(PersonResponse.Country) , "Country"},
            {nameof(PersonResponse.Address) , "Address"},
        };
        var actionArguments = context.HttpContext.Items["arguments"] as IDictionary<string, object>;
        personsController.ViewBag.CurrentSearchString = actionArguments != null && actionArguments.TryGetValue("searchString", out var searchString) ? searchString : null;
        personsController.ViewBag.CurrentSearchBy = actionArguments != null && actionArguments.TryGetValue("searchBy", out var searchBy) ? searchBy : null;

        personsController.ViewBag.CurrentSortBy = actionArguments != null && actionArguments.TryGetValue("sortBy", out var sortBy) ? sortBy : null;
        personsController.ViewBag.CurrentSortOrder = actionArguments != null && actionArguments.TryGetValue("sortOrder", out var sortOrder) ? sortOrder : null;
    }
}