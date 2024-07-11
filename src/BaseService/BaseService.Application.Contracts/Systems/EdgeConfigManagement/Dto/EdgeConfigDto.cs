using System;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.EdgeConfigManagement.Dto
{
    /// <summary>
    /// 边缘网关
    /// </summary>
    public class EdgeConfigDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 网关名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 访问地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 应用密钥
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 权限ID 
        /// </summary>
        public Guid? AuthorityId { get; set; }
        /// <summary>
        /// 系统产品
        /// </summary>
        public string ServiceValue { get; set; }
    }
}
