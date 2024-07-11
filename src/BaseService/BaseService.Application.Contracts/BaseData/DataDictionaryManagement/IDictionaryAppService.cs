using BaseService.BaseData.DataDictionaryManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.BaseData.DataDictionaryManagement
{
    public interface IDictionaryAppService : IApplicationService
    {
        Task<ResultDto<Guid>> Create(CreateOrUpdateDictionaryDto input);

        Task<ResultDto<bool>> Update(CreateOrUpdateDictionaryDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);

        Task<ResultDto<DictionaryDto>> Get(Guid id);

        Task<ResultDto<PagedResultDto<DictionaryDto>>> GetList(GetDictionaryInputDto input);
    }
}
