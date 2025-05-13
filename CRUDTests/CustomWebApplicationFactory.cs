using CRUDMVC.Controllers;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceContracts;
using ServiceContracts.Persons;
using Services;
using Services.Persons;

namespace CRUDTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)).ToList();
            if (descriptors != null)
            {
                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("DatabaseForTesting");
            });
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
                // Add seed data
                var countries = new List<Country>()
                {
                    new() { CountryID = Guid.Parse("4d6681c6-d6d4-4520-8b4b-9ad183ee271c"), CountryName = "Germany" },
                    new() { CountryID = Guid.Parse("ff642272-7ae8-4a19-98fc-c51b6954ec58"), CountryName = "USA" }
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
                        CountryID = countries[1].CountryID,
                        Address = "456 Oak Avenue, Los Angeles, CA",
                        RecievesNewsLetters = false
                    }
                };
                
                context.Countries.AddRange(countries);
                context.Persons.AddRange(persons);
                context.SaveChanges();
            }

            services.AddSingleton<IPersonsGetterService, PersonsGetterService>();
            services.AddSingleton<IPersonsUpdaterService,PersonsUpdaterService>();
        });
        
        builder.UseEnvironment("Testing");
    }
}