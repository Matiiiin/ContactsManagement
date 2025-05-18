using ContactsManagement.Core.DTO;
using ContactsManagement.Core.DTO.Persons;
using ContactsManagement.Core.Enums;

namespace ContactsManagement.Core.ServiceContracts.Persons
{
    public interface IPersonsSorterService
    {
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons , string sortBy, SortOrderOptions sortOrder);
    }

}
