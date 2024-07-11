using BaseService.BaseData.DataDictionaryManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.BaseData.DataDictionaryManagement
{
    public interface IDictionaryDetailAppService : IApplicationService
    {
        Task<ResultDto<Guid>> Create(CreateOrUpdateDictionaryDetailDto input);

        Task<ResultDto<bool>> Update(CreateOrUpdateDictionaryDetailDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);

        Task<ResultDto<DictionaryDetailDto>> Get(Guid id);
        Task<ResultDto<PagedResultDto<DictionaryDetailDto>>> GetList(GetDictionaryDetailInputDto input);

        Task<ResultDto<ListResultDto<DictionaryDetailDto>>> GetAllByDictionaryName(string name);

    }
}
