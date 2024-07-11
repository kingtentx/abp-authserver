using BaseService.Systems.AuthorityManagerment.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.AuthorityManagerment
{
    public interface IAuthorityAppService : IApplicationService
    {
        Task<ResultDto<Guid>> Create(CreateOrUpdateAuthorityDto input);

        Task<ResultDto<bool>> Update(CreateOrUpdateAuthorityDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);

        Task<ResultDto<AuthorityDto>> Get(Guid id);

        Task<ResultDto<PagedResultDto<AuthorityDto>>> GetList(GetAuthorityInputDto input);

        Task<ResultDto<ListResultDto<AuthorityDto>>> LoadAll(Guid? id);

        Task<ResultDto<ListResultDto<AuthorityDto>>> LoadAllNodes();

        //Task<ResultDto<ListResultDto<AuthorityDto>>> LoadRoleAll(Guid roleId);

        Task<ResultDto<ListResultDto<AuthorityTreeDto>>> GetAuthorityTree(Guid roleId);

    }
}
