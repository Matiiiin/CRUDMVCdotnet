using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
        
    }

    public virtual DbSet<Person> Persons { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        //Settings table names
        modelBuilder.Entity<Person>().ToTable("Persons");    
        modelBuilder.Entity<Country>().ToTable("Countries");
        
        //Seeding
        var countries = new List<Country>
        {
            new() { CountryID = Guid.Parse("4d6681c6-d6d4-4520-8b4b-9ad183ee271c"), CountryName = "Germany" },
            new() { CountryID = Guid.Parse("ff642272-7ae8-4a19-98fc-c51b6954ec58"), CountryName = "USA" },
            new() { CountryID = Guid.Parse("34ccdd2d-da1d-4b71-9d4d-3963a33fadaf"), CountryName = "Italy" },
            new() { CountryID = Guid.Parse("6b93e03b-24a5-4975-81b0-39cc5832a80c"), CountryName = "Spain" },
            new() { CountryID = Guid.Parse("54b1e29d-acc5-4a74-914e-51143301af44"), CountryName = "France" },
            new() { CountryID = Guid.Parse("3c061c30-c967-4a1a-a2ed-8da4ac4ab918"), CountryName = "Canada" },
            new() { CountryID = Guid.Parse("aebe6d4e-aa50-4cba-8879-91cecf7b6110"), CountryName = "Colombia" },
        };
        var persons = new List<Person>()
        {
            new()
            {
                PersonID = Guid.Parse("0c97d5dd-5984-436a-a1f2-2fe1f3857a59"),
                PersonName = "Michael Johnson",
                Email = "michael.johnson@example.com",
                DateOfBirth = new DateTime(1985, 3, 12),
                Gender = "Male",
                CountryID = countries[0].CountryID,
                Address = "123 Maple Street, New York, NY",
                RecievesNewsLetters = true
            },
            new()
            {
                PersonID = Guid.Parse("be245ea5-9e28-4cb4-97c0-290bc619b082"),
                PersonName = "Emily Davis",
                Email = "emily.davis@example.com",
                DateOfBirth = new DateTime(1992, 7, 25),
                Gender = "Female",
                CountryID = countries[4].CountryID,
                Address = "456 Oak Avenue, Los Angeles, CA",
                RecievesNewsLetters = false
            },
            new()
            {
                PersonID = Guid.Parse("e4ae92cb-76ef-4180-af85-e3117a7bf45a"),
                PersonName = "James Smith",
                Email = "james.smith@example.com",
                DateOfBirth = new DateTime(1978, 11, 5),
                Gender = "Male",
                CountryID = countries[5].CountryID,
                Address = "789 Pine Road, Chicago, IL",
                RecievesNewsLetters = true
            },
            new()
            {
                PersonID = Guid.Parse("c7972b4b-c1cb-465e-948b-8c50969d56e8"),
                PersonName = "Sophia Brown",
                Email = "sophia.brown@example.com",
                DateOfBirth = new DateTime(2000, 4, 18),
                Gender = "Female",
                CountryID = countries[1].CountryID,
                Address = "321 Cedar Lane, Houston, TX",
                RecievesNewsLetters = false
            },
            new()
            {
                PersonID = Guid.Parse("32cc403b-38a6-41ce-87c4-415aacab9b9d"),
                PersonName = "William Garcia",
                Email = "william.garcia@example.com",
                DateOfBirth = new DateTime(1995, 9, 30),
                Gender = "Male",
                CountryID = countries[2].CountryID,
                Address = "654 Birch Street, Phoenix, AZ",
                RecievesNewsLetters = true
            },
            new()
            {
                PersonID = Guid.Parse("2c503e0b-5ae8-4248-a020-30bed949e283"),
                PersonName = "Olivia Martinez",
                Email = "olivia.martinez@example.com",
                DateOfBirth = new DateTime(1988, 6, 22),
                Gender = "Female",
                CountryID = countries[1].CountryID,
                Address = "987 Spruce Drive, Philadelphia, PA",
                RecievesNewsLetters = true
            },
            new()
            {
                PersonID = Guid.Parse("5eda0c41-f885-4ec2-8a1c-68bf060cb9a2"),
                PersonName = "Benjamin Wilson",
                Email = "benjamin.wilson@example.com",
                DateOfBirth = new DateTime(1990, 1, 15),
                Gender = "Male",
                CountryID = countries[2].CountryID,
                Address = "159 Elm Court, San Antonio, TX",
                RecievesNewsLetters = false
            },
            new()
            {
                PersonID = Guid.Parse("878e4edf-f877-4db5-86fa-ef37dfbe1a2f"),
                PersonName = "Isabella Anderson",
                Email = "isabella.anderson@example.com",
                DateOfBirth = new DateTime(1998, 12, 10),
                Gender = "Female",
                CountryID = countries[3].CountryID,
                Address = "753 Willow Way, San Diego, CA",
                RecievesNewsLetters = true
            },
            new()
            {
                PersonID = Guid.Parse("d2f86a9c-8681-4f76-89ab-aa18ea43bbc3"),
                PersonName = "Alexander Thomas",
                Email = "alexander.thomas@example.com",
                DateOfBirth = new DateTime(1983, 5, 8),
                Gender = "Male",
                CountryID = countries[4].CountryID,
                Address = "852 Aspen Circle, Dallas, TX",
                RecievesNewsLetters = false
            },
            new()
            {
                PersonID = Guid.Parse("574ae25f-2d09-4d57-8c76-56913731e0a1"),
                PersonName = "Mia Taylor",
                Email = "mia.taylor@example.com",
                DateOfBirth = new DateTime(1993, 10, 20),
                Gender = "Female",
                CountryID = countries[6].CountryID,
                Address = "951 Redwood Boulevard, San Jose, CA",
                RecievesNewsLetters = true
            }
        };
        
        
        modelBuilder.Entity<Person>().HasData(persons);
        modelBuilder.Entity<Country>().HasData(countries);
        
        //Fluent API
        modelBuilder.Entity<Person>().Property(p => p.DateOfBirth).HasComment("Comment of dateofbirth");
        
        //Table Relations
        // modelBuilder.Entity<Person>(p =>
        // {
        //     p.HasOne<Country>(p => p.Country).WithMany(c=>c.Persons).HasForeignKey(p => p.CountryID);
        // });
    }
}

public static class ApplicationDbContextExtentions
{
    public static int sp_AddPerson(this ApplicationDbContext context, Person person)
    {
        
        var parameters = new SqlParameter[]
        {
            new ("@PersonID", person.PersonID),
            new ("@PersonName", person.PersonName),
            new ("@Email", person.Email),
            new ("@DateOfBirth", person.DateOfBirth),
            new ("@Gender", person.Gender),
            new ("@Address", person.Address),
            new ("@CountryID", person.CountryID),
            new ("@RecievesNewsLetters", person.RecievesNewsLetters),
        };
        return context.Database.ExecuteSqlRaw("EXEC [dbo].[AddPerson] @PersonID  , @PersonName ,@Email , @DateOfBirth,@Gender,@Address,@CountryID,@RecievesNewsLetters " , parameters);
    }
    public static int sp_UpdatePerson(this ApplicationDbContext context, Person person)
    {
        
        var parameters = new SqlParameter[]
        {
            new ("@PersonID", person.PersonID),
            new ("@PersonName", person.PersonName),
            new ("@Email", person.Email),
            new ("@DateOfBirth", person.DateOfBirth),
            new ("@Gender", person.Gender),
            new ("@Address", person.Address),
            new ("@CountryID", person.CountryID),
            new ("@RecievesNewsLetters", person.RecievesNewsLetters),
        };
        return context.Database.ExecuteSqlRaw("EXEC [dbo].[UpdatePerson] @PersonID  , @PersonName ,@Email , @DateOfBirth,@Gender,@Address,@CountryID,@RecievesNewsLetters " , parameters);
    }
}

public static class PersonsDbSetExtentions
{
    public static List<Person> sp_GetAllPersons(this DbSet<Person> persons)
    {
        return persons.FromSqlRaw("EXEC [dbo].[GetAllPersons]").ToList();
    }

    public static Person sp_GetPersonByID(this DbSet<Person> persons, Guid? personID)
    {
        var parameters = new SqlParameter[]
        {
            new ("@PersonID", personID)
        };
        return persons.FromSqlRaw("EXEC [dbo].[GetPersonByID] @PersonID " , parameters).AsEnumerable().FirstOrDefault();
    } 
}

