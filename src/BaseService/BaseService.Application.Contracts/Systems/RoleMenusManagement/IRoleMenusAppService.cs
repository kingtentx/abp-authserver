using BaseService.Systems.AuthorityManagerment.Dto;
using BaseService.Systems.MenuManagement.Dto;
using BaseService.Systems.RoleMenusManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.RoleMenusManagement
{
    public interface IRoleMenusAppService : IApplicationService
    {
        //Task<ResultDto<ListResultDto<MenuNodesDto>>> GetRoleMenus();

        Task<ResultDto<ListResultDto<Guid>>> GetRoleMenuIds(Guid id, int clientType);

        Task<ResultDto<ListResultDto<MenusTreeDto>>> GetUserMenusTree(int? clientType);

        Task<ResultDto<bool>> Update(UpdateRoleMenuDto input);

        Task<ResultDto<ListResultDto<AuthorityDto>>> GetRoleAuthoritys();

        Task<ResultDto<bool>> UpdateTenantMenus(UpdateTenantMenuDto input);

        Task<ResultDto<ListResultDto<Guid>>> GetTenantMenuIds(Guid tenantId, int clientType);

    }
}
