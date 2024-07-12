using BaseService.Systems.EdgeConfigManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.EdgeConfigManagement
{
    public interface IGatewayConfigAppService : IApplicationService
    {
        Task<ResultDto<Guid>> Create(CreateOrUpdateGatewayConfigDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);

        Task<ResultDto<bool>> Update(CreateOrUpdateGatewayConfigDto input);

        Task<ResultDto<PagedResultDto<GatewayConfigDto>>> GetList(GetGatewayConfigInputDto input);

        Task<ResultDto<GatewayConfigDto>> Get(Guid id);
    }
}
