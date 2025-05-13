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
    public class PersonsAdderService(IPersonsRepository _personsRepository , IDiagnosticContext _diagnosticContext) : IPersonsAdderService
    {
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(PersonAddRequest));
            }

            ModelValidation.Validate(personAddRequest);

            // _db.sp_AddPerson(createdPerson);
            return (await _personsRepository.AddPerson(personAddRequest.ToPerson())).ToPersonResponse();
        }
    }
}
