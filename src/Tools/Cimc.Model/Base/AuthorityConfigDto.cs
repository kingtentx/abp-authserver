using Volo.Abp.Application.Dtos;

namespace Cimc.Model.Base
{
    /// <summary>
    /// 当前用户权限对象
    /// </summary>
    public class CurrentAuthorityDto : EntityDto<Guid>
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string AuthorityName { get; set; }
        /// <summary>
        /// 权限属性类型: 0-公司 1-部门
        /// </summary>      
        public int AuthType { get; set; }
        /// <summary>
        /// 权限代码
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<AuthorityConfigDto> EdgeConifgs { get; set; }
    }

    public class AuthorityConfigDto
    {
        /// <summary>
        /// 权限的ID
        /// </summary>
        public Guid Id { get; set; } = Guid.Empty; 
        /// <summary>
        /// 显示名称
        /// </summary>
        public string AuthorityName { get; set; }
        /// <summary>
        /// 权限属性类型: 0-公司 1-部门
        /// </summary>      
        public int AuthType { get; set; } = 0;
        /// <summary>
        /// 权限代码
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// 网关名称
        /// </summary>
        public string EdgeName { get; set; } = "null";
        /// <summary>
        /// 访问地址
        /// </summary>
        public string Address { get; set; } = "http://127.0.0.1";
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; } = "null";
        /// <summary>
        /// 应用密钥
        /// </summary>
        public string AppSecret { get; set; } = "null";
        /// <summary>
        /// 系统产品
        /// </summary>
        public string ServiceValue { get; set; }
    }
}
