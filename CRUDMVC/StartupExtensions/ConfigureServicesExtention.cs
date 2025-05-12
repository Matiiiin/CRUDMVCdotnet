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
using Services;

namespace CRUDMVC.StartupExtensions;

public static class ConfigureServicesExtention
{
   public static IServiceCollection ConfigureServices(this IServiceCollection services , IConfiguration configuration , IWebHostEnvironment webHostEnvironment)
   {
      // services.AddExceptionHandler(options =>
      // {
      //    options.ExceptionHandlingPath = "/Error";
      // });


      services.AddControllersWithViews(options =>
      {
         options.Filters.Add<AddCustomHeaderResponseGlobalActionFilter>();
         // options.Filters.Add<AuthTokenCheckAuthorizationFilter>();
      });
      services.AddScoped<ICountriesService, CountriesService>();
      services.AddScoped<IPersonsService, PersonsService>();
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