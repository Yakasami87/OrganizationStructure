namespace OrganizationStructureService.Services.RoleService
{
    public interface IRoleService
    {
        Task<ServiceResponse<List<RoleDTO>>> GetRoles();
        Task<ServiceResponse<bool>> CreateRole(RoleDTO roleDTO);
        Task<ServiceResponse<bool>> UpdateRole(RoleDTO roleDTO);
        Task<ServiceResponse<bool>> DeleteRoles(int roleId);
    }
}
