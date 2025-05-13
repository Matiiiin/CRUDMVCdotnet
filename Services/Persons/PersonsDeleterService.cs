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
    public class PersonsDeleterService(IPersonsRepository _personsRepository , IDiagnosticContext _diagnosticContext) : IPersonsDeleterService
    {
        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            var person = await _personsRepository.GetPersonByPersonID(personID);
            if (person == null) return false;
            await _personsRepository.DeletePerson(personID);
            return true;
        }   
    }
}
