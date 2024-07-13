using BaseService.Systems.AuthorityManagerment.Dto;
using BaseService.Systems.MenuManagement.Dto;
using BaseService.Systems.ProductManagement.Dto;
using BaseService.Systems.RoleMenusManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.Systems
{
    public interface IDefaultAppService : IApplicationService
    {
        Task<ResultDto<ListResultDto<AuthorityFactoryDto>>> GetCurrentUserAuthority();

        Task<ResultDto<AuthorityConfigDto>> GetCurrentAuthorityConfig(Guid userId, string serviceName);

        Task<ResultDto<bool>> SetCurrentAuthority(Guid id);

        Task<ResultDto<ListResultDto<MenuNodesDto>>> GetRoleMenus(int clientType);

        Task<ResultDto<AuthorityConfigDto>> GetAuthorityEdgeConfig(string appId);

        ResultDto<ListResultDto<ProductServiceDto>> GetProductService();

        Task<ResultDto<ListResultDto<AuthorityTreeDto>>> GetAuthorityNodes();

        //Task<List<Authority>> GetCurrentUserAuthorityList();
    }
}
