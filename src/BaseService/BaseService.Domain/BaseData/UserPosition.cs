using BaseService.ExtModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace BaseService.BaseData
{
    /// <summary>
    /// 用户岗位表
    /// </summary>
     public class UserPosition : Entity, IMultiTenant, IAuthority
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        //用户ID
        public Guid UserId { get; set; }
        /// <summary>
        /// 岗位ID
        /// </summary>
        public Guid PositionId { get; set; }
        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid? AuthorityId { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { UserId, PositionId };
        }

        public UserPosition(Guid? tenantId, Guid userId, Guid positionId, Guid? authorityId)
        {
            TenantId = tenantId;
            UserId = userId;
            PositionId = positionId;
            AuthorityId = authorityId;
        }
    }
}
