using ContactsManagement.Core.Domain.IdentityEntities;
using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.ServiceContracts.Countries;
using ContactsManagement.Core.ServiceContracts.Persons;
using ContactsManagement.Core.Services.Countries;
using ContactsManagement.Core.Services.Persons;
using ContactsManagement.Infrastructure.Database;
using ContactsManagement.Infrastructure.Repositories;
using ContactsManagement.UI.Filters.ActionFilters.Persons;
using ContactsManagement.UI.Filters.GlobalFilters;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContactsManagement.UI.StartupExtensions;

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
      
      // 1. .AddIdentity<ApplicationUser, ApplicationRole>()
      // Registers ASP.NET Core Identity with your custom user (ApplicationUser) and role (ApplicationRole) classes, both using Guid as the primary key.
      // Sets up the default services for user and role management, password hashing, validation, etc.
      
      // 2. .AddEntityFrameworkStores<ApplicationDbContext>()
      // Configures Identity to use Entity Framework Core for persisting users and roles.
      // ApplicationDbContext inherits from IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, so it contains all the necessary tables for Identity (Users, Roles, UserRoles, etc.).
      
      // 3. .AddDefaultTokenProviders()
      // Adds built-in token providers for features like password reset, email confirmation, and two-factor authentication.
      // These providers generate and validate tokens for secure operations.
      
      // 4. .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
      // Registers a custom user store, which is responsible for all user-related data operations (CRUD).
      // UserStore is a generic class that works with your custom user, role, and context types.
      
      // 5. .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>()
      // Registers a custom role store, which handles all role-related data operations.
      // RoleStore is a generic class for your custom role and context types.
      // <hr></hr> How it works together:  
      // When you inject UserManager<ApplicationUser> or SignInManager<ApplicationUser> into your controllers (like in AccountController), these services use the registered stores and context to manage users and roles in your SQL Server database.
      // All Identity operations (register, login, role assignment, etc.) are handled using your custom types and database context, ensuring full integration with your app’s data model.

      services
         .AddIdentity<ApplicationUser , ApplicationRole>(options =>
         {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 1;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredUniqueChars = 0;
            
            // options.User.RequireUniqueEmail = true;
         })
         .AddEntityFrameworkStores<ApplicationDbContext>()
         .AddDefaultTokenProviders()
         .AddUserStore<UserStore<ApplicationUser , ApplicationRole ,ApplicationDbContext , Guid>>()
         .AddRoleStore<RoleStore<ApplicationRole ,ApplicationDbContext , Guid>>();

      services
         .AddAuthentication(BearerTokenDefaults.AuthenticationScheme);
      
      
      services.AddAuthorization(options => {
         options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
      });
      
      
      return services;
   } 
}