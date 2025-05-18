using ContactsManagement.Core.DTO;
using ContactsManagement.Core.DTO.Persons;

namespace ContactsManagement.Core.ServiceContracts.Persons
{
    public interface IPersonsGetterService
    {
        Task<List<PersonResponse>> GetAllPersons();
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);
        Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString);
    }

}
