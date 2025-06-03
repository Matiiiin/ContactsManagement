using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.DTO.Persons;
using ContactsManagement.Core.ServiceContracts.Persons;
using ContactsManagement.Core.Services.Persons;
using ContactsManagement.Infrastructure.Database;
using ContactsManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
        .ReadFrom.Services(services); //read out current app's services and make them available to serilog
} );
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<IPersonsGetterService , PersonsGetterService>();
builder.Services.AddScoped<IPersonsAdderService , PersonsAdderService>();
builder.Services.AddScoped<IPersonsUpdaterService , PersonsUpdaterService>();
builder.Services.AddScoped<ICountriesRepository , CountriesRepository>();
builder.Services.AddScoped<IPersonsDeleterService , PersonsDeleterService>();

builder.Services.AddAuthentication().AddJwtBearer(
    options =>
{
    var validIssuers = builder.Configuration.GetSection("Authentication:Schemes:Bearer:ValidIssuer").Get<string[]>()!.AsEnumerable();
    var validAudiences = builder.Configuration.GetSection("Authentication:Schemes:Bearer:ValidAudiences").Get<string[]>()!.AsEnumerable();
    var signingKey = builder.Configuration["Authentication:Schemes:Bearer:IssuerSigningKey"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuers = validIssuers,
        ValidAudiences = validAudiences,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey!))
    };
}
);
builder.Services.AddAuthorization();
builder.Services.AddCors();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
var app = builder.Build();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


var personsMapGroup = app.MapGroup("persons");

personsMapGroup.MapGet("", async ([FromServices] IPersonsGetterService personsGetterService) =>
{
    return Results.Ok(await personsGetterService.GetAllPersons());
}).RequireAuthorization();

personsMapGroup.MapGet("{personID:guid}", async ( [FromServices]IPersonsGetterService personsGetterService , Guid personId) =>
{
    var person = await personsGetterService.GetPersonByPersonID(personId);

    if (person == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(person);
}).WithName("GetPersonByPersonID");
    
personsMapGroup.MapPost("" , async (ILogger<PersonsAdderService> logger ,[FromServices]IPersonsAdderService personsAdderService, [FromBody]PersonAddRequest personAddRequest) =>
{
    PersonResponse? person;
    try
    {
        person = await personsAdderService.AddPerson(personAddRequest);
    }
    catch (ArgumentNullException e)
    {
        logger.LogError("Error in adding person : {error}.\n Exception :{exception}", e.Message, e);
        return Results.BadRequest(e.Message);
    }
    catch (ArgumentException e)
    {
        logger.LogError("Error in adding person : {error}.\n Exception :{exception}", e.Message, e);
        return Results.BadRequest(e.Message);
    }
    catch (Exception e)
    {
        logger.LogError("Error in adding person : {error}.\n Exception :{exception}", e.Message, e);
        throw;
    }

    return Results.CreatedAtRoute("GetPersonByPersonID", new { personID = person.PersonID }, person);
    
}).AddEndpointFilter( async (context, next) =>
{
    //Before executing endpoint
    var person = context.Arguments.OfType<PersonAddRequest>().FirstOrDefault();
    if (person == null) return Results.BadRequest("No person provided");
    var errors = new List<ValidationResult>();
    var validationContext = new ValidationContext(person!);
    var isValid = Validator.TryValidateObject(person!, validationContext, errors);
    if (!isValid) return Results.BadRequest(errors.Select(e => e.ErrorMessage));
    var result = await next(context);
    //After executing endpoint
    context.HttpContext.Response.Headers.Append("My-custom-response-header", "helloooooooooooo");
    return result;
});

personsMapGroup.MapPut("{personID:guid}", async ([FromServices]IPersonsGetterService personsGetterService,ILogger<PersonsUpdaterService> logger , PersonUpdateRequest personUpdateRequest , [FromServices] IPersonsUpdaterService personsUpdaterService , Guid personID) =>
{
    if (personID != personUpdateRequest!.PersonID)
    {
        return Results.BadRequest("Invalid person ID.");
    }

    var person = await personsGetterService.GetPersonByPersonID(personID);
    if (person == null)
    {
        return Results.NotFound();
    }

    try
    {
        await personsUpdaterService.UpdatePerson(personUpdateRequest);
    }
    catch (ArgumentException e)
    {
        logger.LogError("Error in Updating person : {error}.\n Exception :{exception}", e.Message, e);
        return Results.BadRequest(e.Message);
    }
    catch (Exception e)
    {
        logger.LogError("Error in adding person : {error}.\n Exception :{exception}", e.Message, e);

        throw;
    }
    return Results.NoContent();
});

personsMapGroup.MapDelete("{personID:guid}", async ([FromServices]IPersonsDeleterService personsDeleterService,[FromServices]IPersonsGetterService personsGetterService,ILogger<PersonsUpdaterService> logger,Guid personID) =>
{
    var person = await personsGetterService.GetPersonByPersonID(personID);
    if (person == null)
    {
        return Results.NotFound();
    }

    try
    {
        await personsDeleterService.DeletePerson(personID);
    }
    catch (ArgumentException e)
    {
        logger.LogError("Error in deleting person : {error}.\n Exception :{exception}", e.Message, e);
        return Results.BadRequest(e.Message);
    }
    catch (Exception e)
    {
        logger.LogError("Error in adding person : {error}.\n Exception :{exception}", e.Message, e);
        throw;
    }
    return Results.NoContent();
});
app.Run();

