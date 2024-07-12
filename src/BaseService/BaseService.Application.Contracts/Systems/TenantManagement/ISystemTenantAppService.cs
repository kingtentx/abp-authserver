using BaseService.Systems.TenantManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.TenantManagement;

namespace BaseService.Systems.TenantManagement
{
    public interface ISystemTenantAppService : IApplicationService
    {
        Task<ResultDto<Guid>> Create(TenantCreateDto input);

        Task<ResultDto<bool>> Update(SystemTenantUpdateDto input);

        Task<ResultDto<bool>> Delete(Guid id);

        Task<ResultDto<TenantDto>> Get(Guid id);

        Task<ResultDto<PagedResultDto<TenantDto>>> GetList(GetSystemTenantInput input);

        Task<ResultDto<ListResultDto<string>>> GetAllNames();
    }
}
