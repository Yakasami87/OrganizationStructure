using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OrganizationStructureService.Services.PersonService;
using OrganizationStructureService.SignalIR;

namespace OrganizationStructureService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        private readonly IHubContext<MessageHub> _messageHub;

        public PersonController(IPersonService personService, IHubContext<MessageHub> messageHub)
        {
            _personService = personService;
            _messageHub = messageHub;
        }

        [HttpGet("Get-Persons")]
        public async Task<ActionResult<ServiceResponse<List<PersonDTO>>>> GetPersons()
        {
            var response = await _personService.GetPersons();

            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Create-Person")]
        public async Task<ActionResult<ServiceResponse<bool>>> CreatePerson(PersonDTO personDTO)
        {
            var response = await _personService.CreatePerson(personDTO);

            if (!response.Success) return BadRequest(response);

            await _messageHub.Clients.All.SendAsync("Refresh","Persons");

            return Ok(response);
        }

        [HttpPost("Update-Person")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdatePerson(PersonDTO personDTO)
        {
            var response = await _personService.UpdatePerson(personDTO);

            if (!response.Success) return BadRequest(response);

            await _messageHub.Clients.All.SendAsync("Refresh", "Persons");

            return Ok(response);
        }

        [HttpPost("Delete-Person")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeletePerson(PersonDTO personDTO)
        {
            var response = await _personService.DeletePersons(personDTO.Id);

            if (!response.Success) return BadRequest(response);

            await _messageHub.Clients.All.SendAsync("Refresh", "Persons");

            return Ok(response);
        }
    }
}
