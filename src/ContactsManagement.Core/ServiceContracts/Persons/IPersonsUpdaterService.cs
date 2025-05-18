using ContactsManagement.Core.DTO;
using ContactsManagement.Core.DTO.Persons;

namespace ContactsManagement.Core.ServiceContracts.Persons
{
    public interface IPersonsUpdaterService
    {
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    }

}
