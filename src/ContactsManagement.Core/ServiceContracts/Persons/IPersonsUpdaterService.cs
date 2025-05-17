using ContactsManagement.Core.DTO;

namespace ContactsManagement.Core.ServiceContracts.Persons
{
    public interface IPersonsUpdaterService
    {
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    }

}
