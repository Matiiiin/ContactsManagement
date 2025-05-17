using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.ServiceContracts.Persons;
using Serilog;

namespace ContactsManagement.Core.Services.Persons
{
    public class PersonsDeleterService(IPersonsRepository _personsRepository , IDiagnosticContext _diagnosticContext) : IPersonsDeleterService
    {
        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            var person = await _personsRepository.GetPersonByPersonID(personID);
            if (person == null) return false;
            await _personsRepository.DeletePerson(personID);
            return true;
        }   
    }
}
