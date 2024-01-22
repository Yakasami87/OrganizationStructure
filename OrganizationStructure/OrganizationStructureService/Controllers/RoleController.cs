using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OrganizationStructureService.Services.RoleService;
using OrganizationStructureService.SignalIR;
using OrganizationStructureShared.Models.DTOs;

namespace OrganizationStructureService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IHubContext<MessageHub> _messageHub;

        public RoleController(IRoleService roleService, IHubContext<MessageHub> messageHub)
        {
            _roleService = roleService;
            _messageHub = messageHub;
        }

        [HttpGet("Get-Roles")]
        public async Task<ActionResult<ServiceResponse<List<RoleDTO>>>> GetRoles()
        {
            var response = await _roleService.GetRoles();

            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Create-Role")]
        public async Task<ActionResult<ServiceResponse<bool>>> CreateRole(RoleDTO roleDTO)
        {
            var response = await _roleService.CreateRole(roleDTO);

            if (!response.Success) return BadRequest(response);

            await _messageHub.Clients.All.SendAsync("Refresh", "Roles");

            return Ok(response);
        }

        [HttpPost("Update-Role")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateRole(RoleDTO roleDTO)
        {
            var response = await _roleService.UpdateRole(roleDTO);

            if (!response.Success) return BadRequest(response);

            await _messageHub.Clients.All.SendAsync("Refresh", "Roles");

            return Ok(response);
        }

        [HttpPost("Delete-Role")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteRole(RoleDTO roleDTO)
        {
            var response = await _roleService.DeleteRoles(roleDTO.Id);

            if (!response.Success) return BadRequest(response);

            await _messageHub.Clients.All.SendAsync("Refresh", "Roles");

            return Ok(response);
        }
    }
}
