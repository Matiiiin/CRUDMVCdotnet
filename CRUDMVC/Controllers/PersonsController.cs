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
    public async Task<IActionResult> test([FromForm] string countryName)
    {
        var country = await _countriesService.AddCountry(new CountryAddRequest() { CountryName = countryName });
        return Json(country);
    }  
    
    [Route("[action]")]
    [Route("/")]
    public async Task<IActionResult> Index(string? searchString , string? searchBy , string sortBy = nameof(PersonResponse.PersonName) , SortOrderOptions sortOrder = SortOrderOptions.ASC)
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
        var filteredPersons =await _personsService.GetFilteredPersons(searchBy,searchString);
        ViewBag.CurrentSearchString = searchString;
        ViewBag.CurrentSearchBy = searchBy;
        
        
        //Sorting
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder;
        var sortedPersons =await _personsService.GetSortedPersons(filteredPersons , sortBy ,sortOrder);
        
        return View(sortedPersons);
    }
    
    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var allCountries = await _countriesService.GetAllCountries();
        ViewBag.Countries =allCountries
            .Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryID.ToString() });
        return View();
    }    
    
    
    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> Store([FromForm] PersonAddRequest personAddRequest)
    {

        if (!ModelState.IsValid)
        {
            var allCountries = await _countriesService.GetAllCountries();
            ViewBag.Countries =allCountries
                .Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryID.ToString() });
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return View("Create");
        }

        await _personsService.AddPerson(personAddRequest);
        return RedirectToAction("Index" , "Persons");
    }

    [Route("[action]/{personID:guid}")]
    [HttpGet]
    public async Task<IActionResult> Edit([FromRoute] Guid personID)
    {
        var personResponse = await _personsService.GetPersonByPersonID(personID);
        if (personResponse == null)
        {
            return NotFound();
        }
        var allCountries = await _countriesService.GetAllCountries();
        ViewBag.Countries = allCountries
            .Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryID.ToString() });
        return View(personResponse.ToPersonUpdateRequest());
    }

    [Route("[action]/{personID:guid}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update([FromForm] PersonUpdateRequest? personUpdateRequest , Guid personID)
    {
        if (personUpdateRequest == null) return BadRequest("Please provide a valid person data");
        if (!ModelState.IsValid)
        {
            var allCountries = await _countriesService.GetAllCountries();
            ViewBag.Countries = allCountries
                .Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryID.ToString() });
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            var personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
            return View("Edit" , personResponse!.ToPersonUpdateRequest()); 
        }
        // personUpdateRequest.PersonID = personID;
        await _personsService.UpdatePerson(personUpdateRequest);
        return RedirectToAction("Index" , "Persons");
    }

    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> Delete([FromQuery] Guid personID)
    {
        if (personID == Guid.Empty) return BadRequest("Please provide a valid person data");
        var personResponse = await _personsService.GetPersonByPersonID(personID);
        return View(personResponse);
    }
    
    [Route("[action]")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitDelete([FromForm] Guid personID)
    {
        if (personID == Guid.Empty) return BadRequest("Please provide a valid person data");
        if (await _personsService.GetPersonByPersonID(personID) == null) return NotFound("Person not found");
        
        await _personsService.DeletePerson(personID);
        return RedirectToAction("Index");
    }
}
