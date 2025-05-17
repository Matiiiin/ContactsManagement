using ContactsManagement.Core.DTO;

namespace ContactsManagement.Core.ServiceContracts.Countries
{
    public interface ICountriesAdderService
    {
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
    }
}
