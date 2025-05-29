using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactsManagement.Core.Domain.Entities;
using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.DTO.Persons;
using ContactsManagement.Core.ServiceContracts.Persons;
using ContactsManagement.Infrastructure.Database;

namespace ContactsManagement.WebApi.UI.Controllers
{
    /// <summary>
    /// Controller for handling API requests related to persons (contacts).
    /// </summary>
public class PersonsController : CustomApiController
{
    private readonly IPersonsDeleterService _personsDeleterService;
    private readonly ILogger<PersonsController> _logger;
    private readonly IPersonsGetterService _personsGetterService;
    private readonly IPersonsAdderService _personsAdderService;
    private readonly IPersonsUpdaterService _personsUpdaterService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonsController"/> class.
    /// </summary>
    /// <param name="personsDeleterService">Service for deleting persons.</param>
    /// <param name="logger">Logger instance for logging errors and information.</param>
    /// <param name="personsGetterService">Service for retrieving persons.</param>
    /// <param name="personsAdderService">Service for adding new persons.</param>
    /// <param name="personsUpdaterService">Service for updating existing persons.</param>
    public PersonsController(
        IPersonsDeleterService personsDeleterService,
        ILogger<PersonsController> logger,
        IPersonsGetterService personsGetterService,
        IPersonsAdderService personsAdderService,
        IPersonsUpdaterService personsUpdaterService)
    {
        _personsDeleterService = personsDeleterService;
        _logger = logger;
        _personsGetterService = personsGetterService;
        _personsAdderService = personsAdderService;
        _personsUpdaterService = personsUpdaterService;
    }

    /// <summary>
    /// Retrieves a list of all persons.
    /// </summary>
    /// <returns>An ordered enumerable list of <see cref="PersonResponse"/> objects.</returns>
    [HttpGet]
    [Produces("application/xml")]
    public async Task<IOrderedEnumerable<PersonResponse>> GetPersons()
    {
        return (await _personsGetterService.GetAllPersons()).OrderBy(o => o.PersonName);
    }

    /// <summary>
    /// Retrieves a person by their unique identifier.
    /// </summary>
    /// <param name="personID">The unique identifier of the person.</param>
    /// <returns>
    /// An <see cref="ActionResult{T}"/> containing the <see cref="PersonResponse"/> if found; otherwise, NotFound.
    /// </returns>
    [HttpGet("{personID}")]
    public async Task<ActionResult<PersonResponse>> GetPerson(Guid personID)
    {
        var person = await _personsGetterService.GetPersonByPersonID(personID);

        if (person == null)
        {
            return NotFound();
        }

        return person;
    }

    /// <summary>
    /// Updates an existing person.
    /// </summary>
    /// <param name="personUpdateRequest">The updated person data.</param>
    /// <param name="personID">The unique identifier of the person to update.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating the result of the operation.
    /// Returns BadRequest if the IDs do not match or if there is a validation error.
    /// Returns NotFound if the person does not exist.
    /// Returns NoContent on success.
    /// </returns>
    [HttpPut("{personID}")]
    public async Task<IActionResult> PutPerson([FromBody] PersonUpdateRequest? personUpdateRequest, Guid personID)
    {
        if (personID != personUpdateRequest!.PersonID)
        {
            return BadRequest();
        }

        var person = await _personsGetterService.GetPersonByPersonID(personID);
        if (person == null)
        {
            return NotFound();
        }

        try
        {
            await _personsUpdaterService.UpdatePerson(personUpdateRequest);
        }
        catch (ArgumentException e)
        {
            _logger.LogError("Error in Updating person : {error}.\n Exception :{exception}", e.Message, e);
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Error in adding person : {error}.\n Exception :{exception}", e.Message, e);

            throw;
        }
        return NoContent();
    }

    /// <summary>
    /// Adds a new person.
    /// </summary>
    /// <param name="personAddRequest">The person data to add.</param>
    /// <returns>
    /// An <see cref="ActionResult{T}"/> containing the created <see cref="PersonResponse"/>.
    /// Returns BadRequest if the request is invalid.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<PersonResponse>> PostPerson([FromBody] PersonAddRequest? personAddRequest)
    {
        PersonResponse? person;
        try
        {
            person = await _personsAdderService.AddPerson(personAddRequest);
        }
        catch (ArgumentNullException e)
        {
            _logger.LogError("Error in adding person : {error}.\n Exception :{exception}", e.Message, e);
            return BadRequest(e.Message);
        }
        catch (ArgumentException e)
        {
            _logger.LogError("Error in adding person : {error}.\n Exception :{exception}", e.Message, e);
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Error in adding person : {error}.\n Exception :{exception}", e.Message, e);
            throw;
        }

        return CreatedAtAction("GetPerson", new { personID = person.PersonID }, person);
    }

    /// <summary>
    /// Deletes a person by their unique identifier.
    /// </summary>
    /// <param name="personID">The unique identifier of the person to delete.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating the result of the operation.
    /// Returns NotFound if the person does not exist.
    /// Returns NoContent on success.
    /// </returns>
    [HttpDelete("{personID}")]
    public async Task<IActionResult> DeletePerson(Guid personID)
    {
        var person = await _personsGetterService.GetPersonByPersonID(personID);
        if (person == null)
        {
            return NotFound();
        }

        try
        {
            await _personsDeleterService.DeletePerson(personID);
        }
        catch (ArgumentException e)
        {
            _logger.LogError("Error in deleting person : {error}.\n Exception :{exception}", e.Message, e);
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Error in adding person : {error}.\n Exception :{exception}", e.Message, e);
            throw;
        }
        return NoContent();
    }
}
}
