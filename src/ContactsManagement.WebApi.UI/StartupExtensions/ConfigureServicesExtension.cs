using System.Text.Json;
using ContactsManagement.Core.Domain.IdentityEntities;
using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.ServiceContracts.Persons;
using ContactsManagement.Core.Services.Persons;
using ContactsManagement.Infrastructure.Database;
using ContactsManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using Serilog;

namespace ContactsManagement.WebApi.UI.StartupExtensions;

public static class ConfigureServicesExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddControllers(config =>
        {
        }).AddNewtonsoftJson();
        
        
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

        services.AddIdentityApiEndpoints<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 1;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
            .AddRoles<ApplicationRole>()
            .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = BearerTokenDefaults.AuthenticationScheme;
        });
        services.AddAuthorization(options =>
        {
        });
        
        return services;
    }
}