using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDMVC.Controllers;

public class PersonsController(IPersonsService personsService)  : Controller
{
    private readonly IPersonsService _personsService = personsService;

    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index()
    {
        return View(_personsService.GetAllPersons());
    }
}