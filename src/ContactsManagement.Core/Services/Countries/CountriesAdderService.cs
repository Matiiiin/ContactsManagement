using ContactsManagement.Core.Domain.RepositoryContracts;
using ContactsManagement.Core.DTO;
using ContactsManagement.Core.ServiceContracts.Countries;

namespace ContactsManagement.Core.Services.Countries
{
    public class CountriesAdderService(ICountriesRepository _countriesRepository) :ICountriesAdderService
    {
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }
            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
            {
                throw new ArgumentException("There is already a Country with this name");
            }

            return (await _countriesRepository.AddCountry(countryAddRequest.ToCountry())).ToCountryResponse();
        }
    }
}
