using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.DTO;
using ContactsManagement.Core.ServiceContracts.Persons;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace ContactsManagement.Core.Services.Persons
{
    public class PersonsGetterService(IPersonsRepository _personsRepository , IDiagnosticContext _diagnosticContext) : IPersonsGetterService
    {
        public async Task<List<PersonResponse>> GetAllPersons()
        {
            // return _db.Persons.sp_GetAllPersons().Select(p => ConvertPersonToPersonResponse(p)).ToList();
            return (await _personsRepository.GetAllPersons()).Select(p=>p.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            // var person = _db.Persons.sp_GetPersonByID(personID);
            var person = await _personsRepository.GetPersonByPersonID(personID);
            return person?.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString)
        {
            List<PersonResponse> matchingPersons = new(){};
            if (searchString.IsNullOrEmpty())
            {
                return (await _personsRepository.GetAllPersons()).Select(p => p.ToPersonResponse()).ToList();
            }
            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = (await _personsRepository.GetFilteredPersons(p=> p.PersonName.Contains(searchString))).Select(p=>p.ToPersonResponse()).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = (await _personsRepository.GetFilteredPersons(temp => temp.DateOfBirth.Value.ToString().Contains(searchString))).Select(p => p.ToPersonResponse()).ToList();
                    break;

                
                case nameof(PersonResponse.Gender):
                    matchingPersons = (await _personsRepository.GetFilteredPersons(p => p.Gender != null && p.Gender.Equals(searchString))).Select(p=>p.ToPersonResponse()).ToList();
                    break;
                
                case nameof(PersonResponse.Country):
                    matchingPersons = (await _personsRepository.GetFilteredPersons(p=> p.Country != null && p.Country.CountryName.Contains(searchString))).Select(p=>p.ToPersonResponse()).ToList();
                    break;
                
                case nameof(PersonResponse.Email):
                    matchingPersons = (await _personsRepository.GetFilteredPersons(p => p.Email != null && p.Email.Contains(searchString))).Select(p=>p.ToPersonResponse()).ToList();
                    break;
                
                case nameof(PersonResponse.Address):
                    matchingPersons = (await _personsRepository.GetFilteredPersons(p => p.Address != null && p.Address.Contains(searchString))).Select(p=>p.ToPersonResponse()).ToList();
                    break;
                default:
                    matchingPersons = (await _personsRepository.GetAllPersons()).Select(p=>p.ToPersonResponse()).ToList();
                    break;
            }
            _diagnosticContext.Set("Persons" , matchingPersons);
            return matchingPersons;

        }
    }
}
