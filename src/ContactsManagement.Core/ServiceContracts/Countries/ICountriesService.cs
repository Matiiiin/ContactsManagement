using ContactsManagement.Core.DTO;
using ContactsManagement.Core.DTO.Countries;

namespace ContactsManagement.Core.ServiceContracts.Countries;

public interface ICountriesGetterService
{
    Task<List<CountryResponse>> GetAllCountries();
    Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);
}