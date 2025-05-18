using ContactsManagement.Core.DTO;
using ContactsManagement.Core.DTO.Persons;

namespace ContactsManagement.Core.ServiceContracts.Persons;

public interface IPersonsAdderService
{
    Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
}