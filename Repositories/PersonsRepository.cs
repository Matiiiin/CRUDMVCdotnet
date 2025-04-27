using System.Linq.Expressions;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories;

public class PersonsRepository(ApplicationDbContext db) : IPersonsRepository
{
    private readonly ApplicationDbContext _db = db;
    
    public async Task<Person> AddPerson(Person person)
    {
        _db.Persons.Add(person);
        await _db.SaveChangesAsync();
        return person;
    }

    public async Task<Person?> GetPersonByPersonID(Guid? personID)
    {
        return await _db.Persons.Include("Country").FirstOrDefaultAsync(p=>p.PersonID == personID);
    }

    public async Task<List<Person>> GetAllPersons()
    {
        return await _db.Persons.Include("Country").ToListAsync();
    }

    public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
    {
        return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
    }

    public async Task<Person> UpdatePerson(Person person)
    {
        var personToUpdate = await _db.Persons.Include("Country").FirstOrDefaultAsync(p => p.PersonID == person.PersonID);
        if ( personToUpdate == null)
        {
            return person;
        }
        
        personToUpdate.PersonName = person.PersonName;
        personToUpdate.DateOfBirth = person.DateOfBirth;
        personToUpdate.Address = person.Address;
        personToUpdate.Email = person.Email;
        personToUpdate.Gender = person.Gender;
        personToUpdate.CountryID = person.CountryID;
        personToUpdate.RecievesNewsLetters = person.RecievesNewsLetters;
        
        await _db.SaveChangesAsync();
        return personToUpdate;
    }

    public async Task<bool> DeletePerson(Guid? personID)
    {
        _db.Persons.RemoveRange(_db.Persons.Where(p=>p.PersonID == personID));
        return await _db.SaveChangesAsync() > 0;
    }
}