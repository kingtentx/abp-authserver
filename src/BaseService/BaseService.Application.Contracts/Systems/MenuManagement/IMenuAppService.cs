using BaseService.Systems.MenuManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.MenuManagement
{
    public interface IMenuAppService : IApplicationService
    {
        Task<ResultDto<Guid>> Create(CreateOrUpdateMenuDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);

        Task<ResultDto<bool>> Update(CreateOrUpdateMenuDto input);

        Task<ResultDto<ListResultDto<MenuDto>>> GetList(GetMenuInputDto input);

        Task<ResultDto<MenuDto>> Get(Guid id);

        Task<ResultDto<ListResultDto<MenuNodesDto>>> LoadAll(int clientType);

    }
}
