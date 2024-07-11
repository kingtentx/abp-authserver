using BaseService.Systems.AuthorityManagerment.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.AuthorityManagerment
{
    public interface IAuthorityDetailAppService : IApplicationService
    {
        Task<ResultDto<Guid>> Create(CreateOrUpdateAuthorityDetailDto input);

        Task<ResultDto<bool>> Update(CreateOrUpdateAuthorityDetailDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);

        Task<ResultDto<AuthorityDetailDto>> Get(Guid id);

        Task<ResultDto<PagedResultDto<AuthorityDetailDto>>> GetList(GetAuthorityDetailInputDto input);
        Task<ResultDto<ListResultDto<AuthorityDetailDto>>> GetAll(Guid authrotyId);

        Task<ResultDto<ListResultDto<AuthorityDetailDto>>> LoadUserAll(Guid userId);
    }
}
