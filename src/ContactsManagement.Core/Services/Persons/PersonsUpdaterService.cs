using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.DTO;
using ContactsManagement.Core.Helpers;
using ContactsManagement.Core.ServiceContracts.Persons;
using Serilog;

namespace ContactsManagement.Core.Services.Persons
{
    public class PersonsUpdaterService(IPersonsRepository _personsRepository , IDiagnosticContext _diagnosticContext) :IPersonsUpdaterService
    {
        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }
            ModelValidation.Validate(personUpdateRequest);
            
            // var person = _db.Persons.sp_GetPersonByID(personUpdateRequest.PersonID);
            if (await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID) == null)
            {
                throw new ArgumentException("Given person does not exist");
            }

            // _db.sp_UpdatePerson(personUpdateRequest.ToPerson());
            var x = await _personsRepository.UpdatePerson(personUpdateRequest.ToPerson());
            return (x).ToPersonResponse();
        }
    }
}
