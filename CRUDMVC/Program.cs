using CRUDMVC.Filters.ActionFilters.Persons;
using CRUDMVC.Filters.AuthorizationFilters;
using CRUDMVC.Filters.GlobalFilters;
using CRUDMVC.StartupExtensions;
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

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
        .ReadFrom.Services(services); //read out current app's services and make them available to serilog
} );

//Set up the configuration and services using extension method 
builder.Services.ConfigureServices(configuration:builder.Configuration , webHostEnvironment:builder.Environment);


var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseHttpLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// app.UseExceptionHandler();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { }