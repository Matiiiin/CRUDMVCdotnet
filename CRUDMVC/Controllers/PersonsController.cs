using Microsoft.AspNetCore.Mvc;

namespace CRUDMVC.Controllers;

public class PersonsController : Controller
{
    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }
}