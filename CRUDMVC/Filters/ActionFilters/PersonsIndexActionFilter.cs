using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CRUDMVC.Filters.ActionFilters;

public class PersonsIndexActionFilter : ActionFilterAttribute
{
    private readonly ILogger<PersonsIndexActionFilter> _logger;

    public PersonsIndexActionFilter(ILogger<PersonsIndexActionFilter> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
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

    }
}