using System.Text.Json;
using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.ServiceContracts.Persons;
using ContactsManagement.Core.Services.Persons;
using ContactsManagement.Infrastructure.Database;
using ContactsManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ContactsManagement.WebApi.UI.StartupExtensions;

public static class ConfigureServicesExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddControllers().AddNewtonsoftJson();
        services.AddOpenApi();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Default"));
            options.EnableSensitiveDataLogging();
        });

        services.AddScoped<IPersonsRepository, PersonsRepository>();
        services.AddScoped<ICountriesRepository, CountriesRepository>();
        services.AddScoped<IPersonsSorterService,PersonsSorterService>();
        services.AddScoped<IPersonsGetterService, PersonsGetterService>();
        services.AddScoped<IPersonsAdderService, PersonsAdderService>();
        services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
        services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();

        services.AddLogging();
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.Response;
        });
        return services;
    }
}