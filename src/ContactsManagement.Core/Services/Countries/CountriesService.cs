using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.DTO;
using ContactsManagement.Core.ServiceContracts.Countries;

namespace ContactsManagement.Core.Services.Countries
{
    public class CountriesGetterService(ICountriesRepository _countriesRepository) : ICountriesGetterService
    {
        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries()).Select(c=>c.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
            {
                return null;
            }

            return (await _countriesRepository.GetCountryByCountryID(countryID))?.ToCountryResponse();
        }


    }
}
