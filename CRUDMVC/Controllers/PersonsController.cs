using Entities;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDMVC.Controllers;

public class PersonsController(IPersonsService personsService)  : Controller
{
    private readonly IPersonsService _personsService = personsService;

    [Route("persons/index")]
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
}