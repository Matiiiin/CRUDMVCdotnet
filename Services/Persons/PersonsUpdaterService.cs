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
    public class PersonsUpdaterService(IPersonsRepository _personsRepository , IDiagnosticContext _diagnosticContext) :IPersonsUpdaterService
    {
        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }
            ModelValidation.Validate(personUpdateRequest);
            
            // var person = _db.Persons.sp_GetPersonByID(personUpdateRequest.PersonID);
            if (await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID) == null)
            {
                throw new ArgumentException("Given person does not exist");
            }

            // _db.sp_UpdatePerson(personUpdateRequest.ToPerson());
            var x = await _personsRepository.UpdatePerson(personUpdateRequest.ToPerson());
            return (x).ToPersonResponse();
        }
    }
}
