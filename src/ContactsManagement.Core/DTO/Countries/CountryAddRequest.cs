using ContactsManagement.Core.Domain.Entities;

namespace ContactsManagement.Core.DTO.Countries
{
    public class CountryAddRequest
    {
        public string? CountryName{ get; set; }

        public Country ToCountry()
        {
            return new Country()
            {
                CountryID = Guid.NewGuid(),
                CountryName = CountryName
            };
        }

    }
}
