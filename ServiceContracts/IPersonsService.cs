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
        Task<List<PersonResponse>> GetAllPersons();
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);
        Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString);
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons , string sortBy, SortOrderOptions sortOrder);
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
        Task<bool> DeletePerson(Guid? personID);
    }

}
