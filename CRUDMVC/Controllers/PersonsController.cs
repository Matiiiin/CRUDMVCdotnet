using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDMVC.Controllers;

public class PersonsController : Controller
{
    private readonly ICountriesService _countriesService;
    private readonly IPersonsService _personsService;

    public PersonsController(ICountriesService countriesService , IPersonsService personsService)
    {
        _countriesService = countriesService;
        _personsService = personsService;
    }
    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index()
    {
        return Json(_personsService.GetAllPersons());
    }
}