using Entities;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDMVC.Filters.ResultFilters.Persons;

public class PersonsIndexResultFilter : ResultFilterAttribute
{
    private readonly ILogger<PersonsIndexResultFilter> _logger;

    public PersonsIndexResultFilter(ILogger<PersonsIndexResultFilter> logger)
    {
        _logger = logger;
    }

    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        _logger.LogInformation("in {className} , performing {methodName}" ,nameof(PersonsIndexResultFilter) , nameof(OnResultExecutionAsync));
        await next();
    }
}