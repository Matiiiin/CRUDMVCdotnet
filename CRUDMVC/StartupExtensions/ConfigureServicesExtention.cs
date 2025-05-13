using CRUDMVC.Filters.ActionFilters.Persons;
using CRUDMVC.Filters.GlobalFilters;
using CRUDMVC.Middlewares;
using Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.Countries;
using ServiceContracts.Persons;
using Services;
using Services.Countries;
using Services.Persons;

namespace CRUDMVC.StartupExtensions;

public static class ConfigureServicesExtention
{
   public static IServiceCollection ConfigureServices(this IServiceCollection services , IConfiguration configuration , IWebHostEnvironment webHostEnvironment)
   {
      services.AddExceptionHandler(options =>
      {
         options.ExceptionHandlingPath = "/Error";
         options.StatusCodeSelector = (e) => 500;
      });


      services.AddControllersWithViews(options =>
      {
         options.Filters.Add<AddCustomHeaderResponseGlobalActionFilter>();
         // options.Filters.Add<AuthTokenCheckAuthorizationFilter>();
      });
      services.AddScoped<ICountriesAdderService,CountriesAdderService>();
      services.AddScoped<ICountriesGetterService,CountriesGetterService>();
      
      services.AddScoped<IPersonsGetterService,PersonsGetterService>();
      services.AddScoped<IPersonsAdderService,PersonsAdderService>();
      services.AddScoped<IPersonsSorterService,PersonsSorterService>();
      services.AddScoped<IPersonsUpdaterService,PersonsUpdaterService>();
      services.AddScoped<IPersonsDeleterService,PersonsDeleterService>();
      
      services.AddScoped<ICountriesRepository, CountriesRepository>();
      services.AddScoped<IPersonsRepository, PersonsRepository>();
      services.AddTransient<PersonsIndexCustomResponseHeaderActionFilter>();

      services.AddHttpLogging(options =>
      {
         options.LoggingFields = HttpLoggingFields.Request;
      });

      if (!webHostEnvironment.IsEnvironment("Testing"))
      {
         services.AddDbContext<ApplicationDbContext>(options =>
         {
            options.UseSqlServer(configuration.GetConnectionString("Default"));
            options.EnableSensitiveDataLogging();
         });
      }

      services.Configure<FormOptions>(options =>
      {
         options.MultipartBodyLengthLimit = 99999999999999;
      });
      return services;
   } 
}