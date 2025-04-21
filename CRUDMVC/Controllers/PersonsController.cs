using Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDMVC.Controllers;

[Route("[controller]")]
public class PersonsController(IPersonsService personsService , ICountriesService countriesService)  : Controller
{
    private readonly IPersonsService _personsService = personsService;
    private readonly ICountriesService _countriesService = countriesService;

    // [Route("[action]")]
    // [HttpPost]
    // [RequestSizeLimit(2_000_000_000)]
    // // [DisableRequestSizeLimit] // Alternatively use [RequestSizeLimit(100_000_000)]
    // [RequestFormLimits(MultipartBodyLengthLimit = 2_000_000_000)]
    // public async Task<IActionResult> Download(IFormFile file)
    // {
    //     // using (var stream = new FileStream($"./{file.FileName}" , FileMode.Create))
    //     // {
    //     //     file.CopyTo(stream);
    //     // }
    //     // using (MemoryStream stream = new MemoryStream())
    //     // {
    //     //     await Request.Body.CopyToAsync(stream);
    //     //     //the “stream” streams the file that is uploaded from the client
    //     //     return Json(stream);
    //     //
    //     // }
    // }

    [Route("[action]")]
    [Route("/")]
    public IActionResult Index(string? searchString , string? searchBy , string sortBy = nameof(PersonResponse.PersonName) , SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        //Search
        ViewBag.SearchFields = new Dictionary<string, string>()
        {
            {nameof(PersonResponse.PersonName) , "Person Name"},
            {nameof(PersonResponse.Email) , "Email"},
            {nameof(PersonResponse.DateOfBirth) , "Date Of Birth"},
            {nameof(PersonResponse.Gender) , "Gender"},
            {nameof(PersonResponse.Country) , "Country"},
            {nameof(PersonResponse.Address) , "Address"},
        };
        var filteredPersons = _personsService.GetFilteredPersons(searchBy,searchString);
        ViewBag.CurrentSearchString = searchString;
        ViewBag.CurrentSearchBy = searchBy;
        
        
        //Sorting
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder;
        var sortedPersons = _personsService.GetSortedPersons(filteredPersons , sortBy ,sortOrder);
        
        return View(sortedPersons);
    }
    
    [Route("[action]")]
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Countries = _countriesService.GetAllCountries()
            .Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryID.ToString() });
        return View();
    }    
    
    
    [Route("[action]")]
    [HttpPost]
    public IActionResult Store([FromForm] PersonAddRequest personAddRequest)
    {

        if (!ModelState.IsValid)
        {
            ViewBag.Countries = _countriesService.GetAllCountries()
                .Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryID.ToString() });
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return View("Create");
        }

        _personsService.AddPerson(personAddRequest);
        return RedirectToAction("Index" , "Persons");
    }
}
