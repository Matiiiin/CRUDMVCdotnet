using CRUDMVC.Filters.ActionFilters.Persons;
using CRUDMVC.Filters.ExceptionFilters.Persons;
using Entities;
using Microsoft.AspNetCore.Authorization;
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
public class PersonsController(IPersonsService personsService , ICountriesService countriesService)  : Controller
{
    private readonly IPersonsService _personsService = personsService;
    private readonly ICountriesService _countriesService = countriesService;

    #region FileUpload test Action

    [Route("[action]")]
    public async Task TestLog([FromServices]ILogger<PersonsController> logger)
    {
        
        logger.LogInformation("Test Log");
        logger.LogError("Test Error");
        logger.LogWarning("Test Warning");
        logger.LogCritical("Test Critical");
        logger.LogTrace("test Trace");
        logger.LogDebug("test Debug");
        logger.Log(LogLevel.None,"test ");
        
    }
    #endregion
    
    [Route("[action]")]
    [Route("/")]
    [TypeFilter<PersonsIndexActionFilter>]
    [TypeFilter<PersonsIndexExceptionFilter>]
    public async Task<IActionResult> Index(string? searchString , string? searchBy , string sortBy = nameof(PersonResponse.PersonName) , SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        //Search
        var filteredPersons =await _personsService.GetFilteredPersons(searchBy,searchString);
        
        //Sorting
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
    [TypeFilter<PersonsStoreActionFilter>]
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
    [TypeFilter<PersonsEditActionFilter>]
    public async Task<IActionResult> Edit([FromRoute] Guid personID)
    {
        var allCountries = await _countriesService.GetAllCountries();
        ViewBag.Countries = allCountries
            .Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryID.ToString() });
        return View((await _personsService.GetPersonByPersonID(personID))?.ToPersonUpdateRequest());
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
            return View("Edit" , personResponse?.ToPersonUpdateRequest()); 
        }
        // personUpdateRequest.PersonID = personID;
        await _personsService.UpdatePerson(personUpdateRequest);
        return RedirectToAction("Index" , "Persons");
    }

    [Route("[action]")]
    [HttpGet]
    [TypeFilter<PersonsDeleteActionFilter>]
    public async Task<IActionResult> Delete([FromQuery] Guid personID)
    {
        var personResponse = await _personsService.GetPersonByPersonID(personID);
        return View(personResponse);
    }
    
    [Route("[action]")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [TypeFilter<PersonsSubmitDeleteActionFilter>]
    public async Task<IActionResult> SubmitDelete([FromForm] Guid personID)
    {
        await _personsService.DeletePerson(personID);
        return RedirectToAction("Index" , "Persons");
    }
}
