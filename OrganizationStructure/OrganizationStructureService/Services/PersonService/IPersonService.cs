namespace OrganizationStructureService.Services.PersonService
{
    public interface IPersonService
    {
        Task<ServiceResponse<List<PersonDTO>>> GetPersons();
        Task<ServiceResponse<bool>> CreatePerson(PersonDTO personDTO);
        Task<ServiceResponse<bool>> UpdatePerson(PersonDTO personDTO);
        Task<ServiceResponse<bool>> DeletePersons(int personId);
    }
}
