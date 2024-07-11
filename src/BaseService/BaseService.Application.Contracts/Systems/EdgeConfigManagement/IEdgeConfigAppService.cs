using BaseService.Systems.EdgeConfigManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.EdgeConfigManagement
{
    public interface IEdgeConfigAppService : IApplicationService
    {
        Task<ResultDto<Guid>> Create(CreateOrUpdateEdgeConfigDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);

        Task<ResultDto<bool>> Update(CreateOrUpdateEdgeConfigDto input);

        Task<ResultDto<PagedResultDto<EdgeConfigDto>>> GetList(GetEdgeConfigInputDto input);

        Task<ResultDto<EdgeConfigDto>> Get(Guid id);
    }
}
