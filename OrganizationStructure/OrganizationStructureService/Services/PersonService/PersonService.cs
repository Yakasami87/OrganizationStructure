
using Microsoft.EntityFrameworkCore;
using OrganizationStructureService.Data.DataContexts;
using OrganizationStructureShared.Models.DTOs;

namespace OrganizationStructureService.Services.PersonService
{
    public class PersonService : IPersonService
    {
        private readonly IMapper _mapper;
        private readonly OrgStrDataContext _orgStrDataContext;

        public PersonService(IMapper mapper, OrgStrDataContext orgStrDataContext)
        {
            _mapper = mapper;
            _orgStrDataContext = orgStrDataContext;
        }

        public async Task<ServiceResponse<List<PersonDTO>>> GetPersons()
        {
            try
            {
                var personsDomain = await _orgStrDataContext.Persons.Include("Role").ToListAsync();

                if (personsDomain == null) return new ServiceResponse<List<PersonDTO>>
                {
                    Success = false,
                    Message = "Error while loading Persons, please check the DB."
                };

                var personsDTO = _mapper.Map<List<PersonDTO>>(personsDomain);

                return new ServiceResponse<List<PersonDTO>>
                {
                    Data = personsDTO
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<PersonDTO>>
                {
                    Success = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ServiceResponse<bool>> CreatePerson(PersonDTO personDTO)
        {
            try
            {
                var personName = $"{personDTO.FirstName} {personDTO.LastName}";

                if (await _orgStrDataContext.Persons.AnyAsync(x => x.FirstName == personDTO.FirstName && x.LastName == personDTO.LastName)) 
                    return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Person {personName} already exists."
                };

                var personDomain = _mapper.Map<Person>(personDTO);

                _orgStrDataContext.Attach(personDomain);

                await _orgStrDataContext.Persons.AddAsync(personDomain);
                await _orgStrDataContext.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = $"Person {personName} has been Created."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ServiceResponse<bool>> UpdatePerson(PersonDTO personDTO)
        {
            try
            {
                var personName = $"{personDTO.FirstName} {personDTO.LastName}";

                var personDomain = await _orgStrDataContext.Persons.FindAsync(personDTO.Id);

                if (personDomain == null) return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Person {personName} not found"
                };

                personDomain.FirstName = personDTO.FirstName;
                personDomain.LastName = personDTO.LastName;
                personDomain.Manager = _mapper.Map<Person>(personDTO.Manager);
                personDomain.Role = _mapper.Map<Role>(personDTO.Role); 

                await _orgStrDataContext.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = $"Person {personName} has been Updated."
                };

            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeletePersons(int personId)
        {
            try
            {
                var personDomain = await _orgStrDataContext.Persons.FindAsync(personId);

                if (personDomain == null) return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Person with ID = {personId} not found"
                };

                var personName = $"{personDomain.FirstName} {personDomain.LastName}";

                _orgStrDataContext.Attach(personDomain);
                _orgStrDataContext.Remove(personDomain);

                await _orgStrDataContext.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = $"Person {personName} has been Deleted."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
