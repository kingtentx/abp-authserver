using BaseService.Systems.RoleManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.RoleManagement
{
    public interface IRoleAppService : IApplicationService
    {

        Task<ResultDto<Guid>> Create(BaseIdentityRoleCreateDto input);

        Task<ResultDto<bool>> Update(BaseIdentityRoleUpdateDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);
        Task<ResultDto<BaseIdentityRoleDto>> Get(Guid id);

        Task<ResultDto<PagedResultDto<BaseIdentityRoleDto>>> GetList(GetBaseIdentityRoleInput input);

        Task<ResultDto<ListResultDto<BaseIdentityRoleDto>>> LoadAllRoles(string filter);

        Task<ResultDto<ListResultDto<BaseIdentityRoleDto>>> GetRoles(Guid userId);
    }
}
