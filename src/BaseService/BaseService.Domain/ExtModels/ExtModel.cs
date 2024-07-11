using System;

namespace BaseService.ExtModels
{
    public interface IAuthority
    {
        /// <summary>
        /// 权限对象
        /// </summary>
        Guid? AuthorityId { get; set; }
    }

    public interface IActive
    {
        /// <summary>
        /// 是否激活
        /// </summary>
        bool IsActive { get; set; }
    }
}
