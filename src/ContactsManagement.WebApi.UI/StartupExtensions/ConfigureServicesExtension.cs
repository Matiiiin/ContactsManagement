using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Asp.Versioning;
using ContactsManagement.Core.Domain.IdentityEntities;
using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.ServiceContracts.Authentication;
using ContactsManagement.Core.ServiceContracts.Persons;
using ContactsManagement.Core.Services.Authentication;
using ContactsManagement.Core.Services.Persons;
using ContactsManagement.Infrastructure.Database;
using ContactsManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;

namespace ContactsManagement.WebApi.UI.StartupExtensions;

public static class ConfigureServicesExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddControllers(options =>
            {
                options.Filters.Add(new ProducesAttribute("application/json"));
                options.Filters.Add(new ConsumesAttribute("application/json"));
            })
            .AddNewtonsoftJson()
            .AddXmlSerializerFormatters();
        
        
        services.AddProblemDetails();
        
        //Swagger
       services
            .AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });;
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();


        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<SwaggerDefaultValues>();
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ContactsManagement.WebApi.UI.xml"));
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });
        
        
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
        services.AddScoped<IJwtService, JwtService>();

        services.AddLogging();
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.Response;
        });

        services.AddIdentityApiEndpoints<ApplicationUser>(options =>
            {
                // options.RoutePrefix = "auth"; // Set your desired prefix
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
            options.DefaultScheme =
            options.DefaultAuthenticateScheme =
            options.DefaultSignOutScheme =
            options.DefaultChallengeScheme =
            options.DefaultSignInScheme =
            options.DefaultForbidScheme =
            JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateActor = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"]!))
            };
        });

        
        services.AddAuthorization();
        
        services.AddCors(options =>
        {
        });
        return services;
    }
}





