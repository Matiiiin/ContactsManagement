using ContactsManagement.Core.Domain.Entities;

namespace ContactsManagement.Core.DTO.Countries
{
    public class CountryResponse
    {
        public Guid CountryID{ get; set; }
        public string CountryName{ get; set; }
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != typeof(CountryResponse))
            {
                return false;
            }
            var otherObject = obj as CountryResponse;
            return CountryID == otherObject.CountryID && CountryName == otherObject.CountryName;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryID = country.CountryID,
                CountryName = country.CountryName,
            };
        }
    }
}
