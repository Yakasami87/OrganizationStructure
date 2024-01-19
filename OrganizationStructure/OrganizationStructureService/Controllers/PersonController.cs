using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganizationStructureService.Services.PersonService;

namespace OrganizationStructureService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
                _personService = personService;
        }

        [HttpGet("Get-Persons")]
        public async Task<ActionResult<ServiceResponse<List<PersonDTO>>>> GetPersons()
        {
            var response = await _personService.GetPersons(); 

            if(!response.Success) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Create-Person")]
        public async Task<ActionResult<ServiceResponse<bool>>> CreatePerson(PersonDTO personDTO)
        {
            var response = await _personService.CreatePerson(personDTO);

            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Update-Person")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdatePerson(PersonDTO personDTO)
        {
            var response = await _personService.UpdatePerson(personDTO);

            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Delete-Person")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeletePerson(int personId)
        {
            var response = await _personService.DeletePersons(personId);

            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }
    }
}
