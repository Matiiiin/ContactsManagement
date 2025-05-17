using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.DTO;
using ContactsManagement.Core.Helpers;
using ContactsManagement.Core.ServiceContracts.Persons;
using Serilog;
// using Repositories;

namespace ContactsManagement.Core.Services.Persons
{
    public class PersonsAdderService(IPersonsRepository _personsRepository , IDiagnosticContext _diagnosticContext) : IPersonsAdderService
    {
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(PersonAddRequest));
            }

            ModelValidation.Validate(personAddRequest);

            // _db.sp_AddPerson(createdPerson);
            return (await _personsRepository.AddPerson(personAddRequest.ToPerson())).ToPersonResponse();
        }
    }
}
