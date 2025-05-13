using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.Enums;

namespace ServiceContracts.Persons
{
    public interface IPersonsGetterService
    {
        Task<List<PersonResponse>> GetAllPersons();
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);
        Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString);
    }

}
