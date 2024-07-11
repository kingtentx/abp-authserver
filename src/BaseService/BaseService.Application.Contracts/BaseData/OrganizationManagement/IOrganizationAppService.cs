using BaseService.BaseData.OrganizationManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.BaseData.OrganizationManagement
{
    public interface IOrganizationAppService : IApplicationService
    {
        Task<ResultDto<Guid>> Create(CreateOrUpdateOrganizationDto input);

        Task<ResultDto<bool>> Update(CreateOrUpdateOrganizationDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);
        Task<ResultDto<OrganizationDto>> Get(Guid id);
        Task<ResultDto<PagedResultDto<OrganizationDto>>> GetList(GetOrganizationInputDto input);

        Task<ResultDto<ListResultDto<OrganizationDto>>> LoadAll(Guid? id, string filter);

        Task<ResultDto<ListResultDto<OrganizationDto>>> LoadAllNodes(string filter);
    }
}
