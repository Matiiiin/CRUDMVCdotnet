using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContracts.Enums;

namespace ServiceContracts.Persons
{
    public interface IPersonsSorterService
    {
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons , string sortBy, SortOrderOptions sortOrder);
    }

}
