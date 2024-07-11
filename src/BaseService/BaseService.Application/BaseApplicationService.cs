using BaseService.Consts;
using BaseService.Systems;
using BaseService.Systems.AuthorityManagerment.Dto;
using Cimc.Model.Base;
using Volo.Abp.Application.Services;

namespace BaseService
{
    public class BaseApplicationService : ApplicationService
    {
        private readonly IDefaultAppService DefaultAppService;

        public BaseApplicationService(
              IDefaultAppService defaultAppService
            )
        {
            DefaultAppService = defaultAppService;
        }

        /// <summary>
        /// 当前用户登录的权限对象
        /// </summary>
        public AuthorityConfigDto CurrentAuthority
        {
            get
            {
                try
                {
                    return DefaultAppService.GetCurrentAuthorityConfig(CurrentUser.Id.Value, SystemConsts.ServiceName).Result.Data;
                }
                catch
                {
                    return new AuthorityConfigDto();
                }
            }
        }      
    }


}
