using ContactsManagement.Core.DTO;
using ContactsManagement.Core.DTO.Countries;

namespace ContactsManagement.Core.ServiceContracts.Countries
{
    public interface ICountriesAdderService
    {
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
    }
}
