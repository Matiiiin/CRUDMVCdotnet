using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using RepositoryContracts;
using Serilog;
using ServiceContracts.Enums;
using ServiceContracts.Persons;

namespace Services.Persons
{
    public class PersonsSorterService(IPersonsRepository _personsRepository , IDiagnosticContext _diagnosticContext) : IPersonsSorterService
    {
        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return persons;
            }

            List<PersonResponse> sortedPersons = (sortBy, sortOrder)
                switch
                {
                    (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                    (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                    
                    
                    (nameof(PersonResponse.Email), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                    (nameof(PersonResponse.Email), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                    
                    
                    (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.DateOfBirth).ToList(),
                    (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.DateOfBirth).ToList(),
                    
                    
                    (nameof(PersonResponse.Age), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.Age).ToList(),
                    (nameof(PersonResponse.Age), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.Age).ToList(),
                    
                    
                    (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.Gender , StringComparer.OrdinalIgnoreCase).ToList(),
                    (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.Gender , StringComparer.OrdinalIgnoreCase).ToList(),

                    
                    (nameof(PersonResponse.Country), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.Country , StringComparer.OrdinalIgnoreCase).ToList(),
                    (nameof(PersonResponse.Country), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.Country , StringComparer.OrdinalIgnoreCase).ToList(),

                    
                    (nameof(PersonResponse.Address), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.Address , StringComparer.OrdinalIgnoreCase).ToList(),
                    (nameof(PersonResponse.Address), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.Address , StringComparer.OrdinalIgnoreCase).ToList(),
                    
                    
                    (nameof(PersonResponse.RecievesNewsLetters), SortOrderOptions.ASC) => persons
                        .OrderBy(p => p.RecievesNewsLetters).ToList(),
                    (nameof(PersonResponse.RecievesNewsLetters), SortOrderOptions.DESC) => persons
                        .OrderByDescending(p => p.RecievesNewsLetters).ToList(),
                    
                    _ => persons
                };
            return sortedPersons;
        }
    }
}
