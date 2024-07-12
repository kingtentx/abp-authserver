using BaseService.ExtModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace BaseService.BaseData
{
    public class UserFeature : AuditedEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// 特征名称
        /// </summary>
        [Comment("名称"), MaxLength(ModelUnits.Len_100)]
        public string Name { get; set; }

        /// <summary>
        /// 健值
        /// </summary>
        [MaxLength(ModelUnits.Len_100)]
        public string DataKey { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [MaxLength(ModelUnits.Len_1000)]
        public string DataValue { get; set; }

        /// <summary>
        /// 特征类型  如：0-个性化表格列 1-皮肤 ...
        /// </summary>
        [Comment("特征类型")]
        public int FeatureType { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        public UserFeature(Guid id) : base(id)
        {
            
        }

    }
}
