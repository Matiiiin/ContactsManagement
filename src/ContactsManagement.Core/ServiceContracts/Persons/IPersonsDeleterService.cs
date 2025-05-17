namespace ContactsManagement.Core.ServiceContracts.Persons
{
    public interface IPersonsDeleterService
    {

        Task<bool> DeletePerson(Guid? personID);

    }

}
