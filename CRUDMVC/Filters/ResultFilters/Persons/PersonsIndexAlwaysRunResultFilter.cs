using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDMVC.Filters.ResultFilters.Persons;

public class PersonsIndexAlwaysRunResultFilter : IAsyncAlwaysRunResultFilter
{
    private readonly ILogger<PersonsIndexAlwaysRunResultFilter> _logger;

    public PersonsIndexAlwaysRunResultFilter(ILogger<PersonsIndexAlwaysRunResultFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        _logger.LogInformation("Executing {methodName} method in {className} class" , nameof(OnResultExecutionAsync) , nameof(PersonsIndexAlwaysRunResultFilter));
        context.HttpContext.Response.Headers.Append("My-Test-Header", "Test header set in always run result filter");
        await next();
    }
    
}