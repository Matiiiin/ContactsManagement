using ContactsManagement.Core.Domain.Entities;

namespace ContactsManagement.Core.Domain.RepositoryContracts;

public interface ICountriesRepository
{
    Task<Country> AddCountry(Country country);
    Task<Country?> GetCountryByCountryID(Guid? countryID);
    Task<Country?> GetCountryByCountryName(string countryName);
    Task<List<Country>> GetAllCountries();
}