using BaseService.Systems.UserManagement.Dto;
using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BaseService.Systems.UserManagement
{
    public interface IUserAppService : IApplicationService
    {

        Task<ResultDto<Guid>> Create(BaseIdentityUserCreateDto input);

        Task<ResultDto<bool>> Update(BaseIdentityUserUpdateDto input);

        Task<ResultDto<bool>> Delete(List<Guid> ids);
        Task<ResultDto<BaseIdentityUserDto>> Get(Guid id);
        Task<ResultDto<PagedResultDto<BaseIdentityUserDto>>> GetList(GetBaseIdentityUsersInput input);     
        Task<ResultDto<bool>> ChangeUserPassword(Guid id, ChangePasswordDto input);

        Task<ResultDto<CurrentUserDto>> GetCurrentUser();
        Task<ResultDto<bool>> ResetUserPassword(List<Guid> ids);
        Task<ResultDto<bool>> CheckPassWord(Guid userId, string PassWord);
    }
}
