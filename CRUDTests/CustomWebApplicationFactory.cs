using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceContracts;
using Services;

namespace CRUDTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
        {
            // Remove all existing DbContextOptions registrations
            var descriptors = services.Where(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)).ToList();
            if (descriptors != null)
            {
                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }
            }

            // Add the InMemory database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("DatabaseForTesting");
            });
            // services.AddScoped<IPersonsService , PersonsService>();


        });

        builder.UseEnvironment("Testing");
    }
}