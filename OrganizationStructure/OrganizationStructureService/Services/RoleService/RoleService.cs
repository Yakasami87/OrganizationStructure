
using Microsoft.EntityFrameworkCore;
using OrganizationStructureService.Data.DataContexts;
using OrganizationStructureShared.Models.DTOs;

namespace OrganizationStructureService.Services.RoleService
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly OrgStrDataContext _orgStrDataContext;

        public RoleService(IMapper mapper, OrgStrDataContext orgStrDataContext)
        {
            _mapper = mapper;
            _orgStrDataContext = orgStrDataContext;
        }

        public async Task<ServiceResponse<List<RoleDTO>>> GetRoles()
        {
            try
            {
                var rolesDomain = await _orgStrDataContext.Roles.ToArrayAsync();

                if(rolesDomain == null) return new ServiceResponse<List<RoleDTO>>
                {
                    Success = false,
                    Message = "Error while loading Roles, please check the DB."
                };

                var rolesDTO = _mapper.Map<List<RoleDTO>>(rolesDomain);

                return new ServiceResponse<List<RoleDTO>>
                {
                    Data = rolesDTO,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<RoleDTO>>
                {
                    Success = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ServiceResponse<bool>> CreateRole(RoleDTO roleDTO)
        {
            try
            {
                if(await _orgStrDataContext.Roles.AnyAsync(x => x.Name == roleDTO.Name)) return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Role {roleDTO.Name} already exists."
                };

                var roleDomain = _mapper.Map<Role>(roleDTO);

                _orgStrDataContext.Attach(roleDomain);

                await _orgStrDataContext.Roles.AddAsync(roleDomain);
                await _orgStrDataContext.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = $"Role {roleDTO.Name} has been Created."
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

        public async Task<ServiceResponse<bool>> UpdateRole(RoleDTO roleDTO)
        {
            try
            {
                var roleDomain = await _orgStrDataContext.Roles.FindAsync(roleDTO.Id);

                if (roleDomain == null) return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Role {roleDTO.Name} not found."
                };

                roleDomain.Name = roleDTO.Name;

                await _orgStrDataContext.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = $"Role {roleDTO.Name} has been Updated."
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

        public async Task<ServiceResponse<bool>> DeleteRoles(int roleId)
        {
            try
            {
                var roleDomain = await _orgStrDataContext.Roles.FindAsync(roleId);

                if (roleDomain == null) return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Role with ID = {roleId} not found."
                };

                _orgStrDataContext.Attach(roleDomain);
                _orgStrDataContext.Remove(roleDomain);

                await _orgStrDataContext.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = $"Role {roleDomain.Name} has been Deleted."
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
