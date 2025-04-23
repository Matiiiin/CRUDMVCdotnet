using Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDMVC.Controllers;

[Route("[controller]")]
public class PersonsController(IPersonsService personsService , ICountriesService countriesService , ApplicationDbContext db)  : Controller
{
    private readonly IPersonsService _personsService = personsService;
    private readonly ICountriesService _countriesService = countriesService;
    private readonly ApplicationDbContext _db = db;

    #region FileUpload test Action

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

    #endregion

    [Route("[action]")]
    public IActionResult test()
    {
       var query =  _db.Persons.FromSqlRaw("exec getallpersonss");
        return Json(query.ToList());
    }  
    
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

    [Route("[action]/{personID:guid}")]
    [HttpGet]
    public IActionResult Edit([FromRoute] Guid personID)
    {
        var personResponse = _personsService.GetPersonByPersonID(personID);
        if (personResponse == null)
        {
            return NotFound();
        }
        ViewBag.Countries = _countriesService.GetAllCountries()
            .Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryID.ToString() });
        return View(personResponse.ToPersonUpdateRequest());
    }

    [Route("[action]/{personID:guid}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update([FromForm] PersonUpdateRequest? personUpdateRequest , Guid personID)
    {
        if (personUpdateRequest == null) return BadRequest("Please provide a valid person data");
        if (!ModelState.IsValid)
        {
            ViewBag.Countries = _countriesService.GetAllCountries()
                .Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryID.ToString() });
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return View("Edit" , _personsService.GetPersonByPersonID(personUpdateRequest.PersonID)!.ToPersonUpdateRequest()); 
        }
        // personUpdateRequest.PersonID = personID;
        _personsService.UpdatePerson(personUpdateRequest);
        return RedirectToAction("Index" , "Persons");
    }

    [Route("[action]")]
    [HttpGet]
    public IActionResult Delete([FromQuery] Guid personID)
    {
        if (personID == Guid.Empty) return BadRequest("Please provide a valid person data");
        var personResponse = _personsService.GetPersonByPersonID(personID);
        return View(personResponse);
    }
    
    [Route("[action]")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitDelete([FromForm] Guid personID)
    {
        if (personID == Guid.Empty) return BadRequest("Please provide a valid person data");
        if (_personsService.GetPersonByPersonID(personID) == null) return NotFound("Person not found");
        
        _personsService.DeletePerson(personID);
        return RedirectToAction("Index");
    }
}
