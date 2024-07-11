using BaseService.Systems.AuthorityManagerment.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.AuthorityManagerment
{
    public interface IAuthorityGroupAppService : IApplicationService
    {
        Task<ResultDto<Guid>> Create(CreateOrUpdateAuthorityGroupDto input);

        Task<ResultDto<bool>> Update(CreateOrUpdateAuthorityGroupDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);

        Task<ResultDto<AuthorityGroupDto>> Get(Guid id);

        Task<ResultDto<PagedResultDto<AuthorityGroupDto>>> GetList(GetAuthorityGroupInputDto input);

        Task<ResultDto<ListResultDto<AuthorityGroupDto>>> GetAll();
    }
}
