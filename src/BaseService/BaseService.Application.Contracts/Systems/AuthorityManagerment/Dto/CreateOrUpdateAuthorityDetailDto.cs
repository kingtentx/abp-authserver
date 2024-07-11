using System;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.AuthorityManagerment.Dto
{
    /// <summary>
    /// 权限对象子集
    /// </summary>
    public class CreateOrUpdateAuthorityDetailDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid AuthorityId { get; set; }
        /// <summary>
        /// 数据项名称 
        /// </summary>
        public string DataName { get; set; }
        /// <summary>
        /// 数据项ID 
        /// </summary>       
        public Guid DataItemId { get; set; }
        /// <summary>
        /// 数据类型 0-设备 1-生产线
        /// </summary>
        public int DataType { get; set; }

    }
}
