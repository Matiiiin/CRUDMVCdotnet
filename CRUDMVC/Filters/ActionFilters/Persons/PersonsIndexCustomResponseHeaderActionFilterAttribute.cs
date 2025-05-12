using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDMVC.Filters.ActionFilters.Persons;

public class PersonsIndexCustomResponseHeaderActionFilterAttribute : Attribute , IFilterFactory
{
    private readonly string _key;
    private readonly string _value;
    private readonly int _order;
    public PersonsIndexCustomResponseHeaderActionFilterAttribute(string value, string key, int order = 0)
    {
        _order = order;
        _value = value;
        _key = key;
    }
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var filter = serviceProvider.GetRequiredService<PersonsIndexCustomResponseHeaderActionFilter>();
        filter.Order = _order;
        filter.Value = _value;
        filter.Key = _key;
        return filter;
    }

    public bool IsReusable { get; } = false;
}

public class PersonsIndexCustomResponseHeaderActionFilter : ActionFilterAttribute
{
    private readonly ILogger<PersonsIndexCustomResponseHeaderActionFilter> _logger;
    public string? Key { get; set; }
    public string? Value { get; set; }

    public PersonsIndexCustomResponseHeaderActionFilter(ILogger<PersonsIndexCustomResponseHeaderActionFilter> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("Executing {methodName} in {className} class" , nameof(OnActionExecuting) , nameof(PersonsIndexCustomResponseHeaderActionFilter));
        context.HttpContext.Response.Headers[Key] = Value;
    }
}