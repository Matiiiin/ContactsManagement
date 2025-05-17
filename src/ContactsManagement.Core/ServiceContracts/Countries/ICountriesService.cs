using ContactsManagement.Core.DTO;

namespace ContactsManagement.Core.ServiceContracts.Countries;

public interface ICountriesGetterService
{
    Task<List<CountryResponse>> GetAllCountries();
    Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);
}