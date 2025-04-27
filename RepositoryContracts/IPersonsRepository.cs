using System.Linq.Expressions;
using Entities;

namespace RepositoryContracts;

public interface IPersonsRepository
{
    Task<Person> AddPerson(Person person);
    Task<Person?> GetPersonByPersonID(Guid? personID);
    Task<List<Person>> GetAllPersons();
    Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);
    Task<Person> UpdatePerson(Person person);
    Task<bool> DeletePerson(Guid? personID);
}