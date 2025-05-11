using CRUDMVC.Filters.GlobalFilters;
using Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
// Log.Logger = new LoggerConfiguration()
//     .ReadFrom.Configuration(builder.Configuration) // Read settings from appsettings.json
//     .WriteTo.Console() // Log to console
//     .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // Log to file
//     .CreateLogger();

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
        .ReadFrom.Services(services); //read out current app's services and make them available to serilog
} );
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AddCustomHeaderResponseGlobalFilter>();
});
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.Request;
});

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        options.EnableSensitiveDataLogging();
    });
}

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 99999999999999;
});

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseHttpLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { }