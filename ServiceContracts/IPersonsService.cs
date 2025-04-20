using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonsService
    {
        List<PersonResponse> GetAllPersons();
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);
        PersonResponse? GetPersonByPersonID(Guid? personID);
        List<PersonResponse> GetFilteredPersons(string? searchBy, string? searchString);
        List<PersonResponse> GetSortedPersons(List<PersonResponse> persons , string sortBy, SortOrderOptions sortOrder);
        PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);
        bool DeletePerson(Guid? personID);
    }

}
