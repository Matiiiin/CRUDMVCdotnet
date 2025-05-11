using CRUDMVC.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDMVC.Filters.ExceptionFilters.Persons;

public class PersonsIndexExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger<PersonsIndexExceptionFilter> _logger;

    public PersonsIndexExceptionFilter(ILogger<PersonsIndexExceptionFilter> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        _logger.LogError("in {className} , exception raised: {exceptionMessage}" , nameof(PersonsIndexExceptionFilter) , context.Exception.Message);
        base.OnException(context);
    }
}