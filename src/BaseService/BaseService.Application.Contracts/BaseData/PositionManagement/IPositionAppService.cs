using BaseService.BaseData.PositionManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.BaseData.PositionManagement
{
    public interface IPositionAppService : IApplicationService
    {

        Task<ResultDto<Guid>> Create(CreateOrUpdatePositionDto input);

        Task<ResultDto<bool>> Update(CreateOrUpdatePositionDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);
        Task<ResultDto<PositionDto>> Get(Guid id);
        Task<ResultDto<PagedResultDto<PositionDto>>> GetList(GetPositionInputDto input);
        Task<ResultDto<ListResultDto<PositionDto>>> GetAllPositions(string filter);
    }
}
