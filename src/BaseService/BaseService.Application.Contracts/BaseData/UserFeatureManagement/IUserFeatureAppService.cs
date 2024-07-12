using BaseService.BaseData.UserFeatureManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.BaseData.UserFeatureManagement
{

    public interface IUserFeatureAppService : IApplicationService
    {
        Task<ResultDto<Guid?>> Create(CreateOrUpdateUserFeatureDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);

        Task<ResultDto<bool>> Update(CreateOrUpdateUserFeatureDto input);

        Task<ResultDto<PagedResultDto<UserFeatureDto>>> GetList(GetUserFeatureInputDto input);

        Task<ResultDto<UserFeatureDto>> Get(Guid id);

        Task<ResultDto<UserFeatureDto>> Get(string dataKey);
    }
}
