using ContactsManagement.Core.DTO;

namespace ContactsManagement.Core.ServiceContracts.Persons
{
    public interface IPersonsGetterService
    {
        Task<List<PersonResponse>> GetAllPersons();
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);
        Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString);
    }

}
