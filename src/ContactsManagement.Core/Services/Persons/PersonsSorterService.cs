using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.DTO;
using ContactsManagement.Core.DTO.Persons;
using ContactsManagement.Core.Enums;
using ContactsManagement.Core.ServiceContracts.Persons;
using Serilog;

namespace ContactsManagement.Core.Services.Persons;

public class PersonsSorterService(IPersonsRepository _personsRepository , IDiagnosticContext _diagnosticContext) : IPersonsSorterService
{
    public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return persons;
        }

        List<PersonResponse> sortedPersons = (sortBy, sortOrder)
            switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => persons
                    .OrderBy(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => persons
                    .OrderByDescending(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                    
                    
                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => persons
                    .OrderBy(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => persons
                    .OrderByDescending(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                    
                    
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => persons
                    .OrderBy(p => p.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => persons
                    .OrderByDescending(p => p.DateOfBirth).ToList(),
                    
                    
                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => persons
                    .OrderBy(p => p.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => persons
                    .OrderByDescending(p => p.Age).ToList(),
                    
                    
                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => persons
                    .OrderBy(p => p.Gender , StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => persons
                    .OrderByDescending(p => p.Gender , StringComparer.OrdinalIgnoreCase).ToList(),

                    
                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => persons
                    .OrderBy(p => p.Country , StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => persons
                    .OrderByDescending(p => p.Country , StringComparer.OrdinalIgnoreCase).ToList(),

                    
                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => persons
                    .OrderBy(p => p.Address , StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => persons
                    .OrderByDescending(p => p.Address , StringComparer.OrdinalIgnoreCase).ToList(),
                    
                    
                (nameof(PersonResponse.RecievesNewsLetters), SortOrderOptions.ASC) => persons
                    .OrderBy(p => p.RecievesNewsLetters).ToList(),
                (nameof(PersonResponse.RecievesNewsLetters), SortOrderOptions.DESC) => persons
                    .OrderByDescending(p => p.RecievesNewsLetters).ToList(),
                    
                _ => persons
            };
        return sortedPersons;
    }
}