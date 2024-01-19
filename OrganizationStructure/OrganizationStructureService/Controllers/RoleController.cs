using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganizationStructureService.Services.RoleService;
using OrganizationStructureShared.Models.DTOs;

namespace OrganizationStructureService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
                _roleService = roleService;
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

            return Ok(response);
        }

        [HttpPost("Update-Role")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateRole(RoleDTO roleDTO)
        {
            var response = await _roleService.UpdateRole(roleDTO);

            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("Delete-Role")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteRole(int roleId)
        {
            var response = await _roleService.DeleteRoles(roleId);

            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }
    }
}
