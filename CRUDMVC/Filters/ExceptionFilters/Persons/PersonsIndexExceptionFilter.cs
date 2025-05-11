using CRUDMVC.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDMVC.Filters.ExceptionFilters.Persons;

public class PersonsIndexExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger<PersonsIndexExceptionFilter> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PersonsIndexExceptionFilter(ILogger<PersonsIndexExceptionFilter> logger, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    public override void OnException(ExceptionContext context)
    {
        _logger.LogError("in {className} , exception raised: {exceptionMessage}" , nameof(PersonsIndexExceptionFilter) , context.Exception.Message);
        if (_webHostEnvironment.IsDevelopment())
        {
            context.Result = new ContentResult()
            {
                Content = context.Exception.Message,
                StatusCode = 500
            };
        }
    }
}