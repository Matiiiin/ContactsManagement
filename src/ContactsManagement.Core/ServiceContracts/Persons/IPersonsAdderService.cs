using ContactsManagement.Core.DTO;

namespace ContactsManagement.Core.ServiceContracts.Persons;

public interface IPersonsAdderService
{
    Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
}