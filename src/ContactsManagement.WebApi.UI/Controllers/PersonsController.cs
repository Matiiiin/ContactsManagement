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
    public class PersonsController : CustomApiController
    {
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly ILogger<PersonsController> _logger;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsUpdaterService _personsUpdaterService;

        public PersonsController(IPersonsDeleterService personsDeleterService,ILogger<PersonsController> logger,IPersonsGetterService personsGetterService, IPersonsAdderService personsAdderService, IPersonsUpdaterService personsUpdaterService)
        {
            _personsDeleterService = personsDeleterService;
            _logger = logger;
            _personsGetterService = personsGetterService;
            _personsAdderService = personsAdderService;
            _personsUpdaterService = personsUpdaterService;
        }

        // GET: api/Persons
        [HttpGet]
        public async Task<IOrderedEnumerable<PersonResponse>> GetPersons()
        {
            return (await _personsGetterService.GetAllPersons()).OrderBy(o=>o.PersonName);
        }

        // GET: api/Persons/5
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

        // PUT: api/Persons/5
        [HttpPut("{personID}")]
        public async Task<IActionResult> PutPerson([FromBody] PersonUpdateRequest? personUpdateRequest ,Guid personID)
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
                _logger.LogError("Error in Updating person : {error}.\n Exception :{exception}" , e.Message , e);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in adding person : {error}.\n Exception :{exception}", e.Message, e);

                throw;
            }
            return NoContent();
        }
        
        // // POST: api/Persons
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PersonResponse>> PostPerson([FromBody]PersonAddRequest? personAddRequest)
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
        
        // // DELETE: api/Persons/5
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
