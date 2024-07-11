
namespace Cimc.Model.RabbitMq
{
    public class EmqxClientinfoDto
    {
        /// <summary>
        /// 服务器节点
        /// </summary>
        public string Node { get; set; }
        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 连接主机
        /// </summary>
        public string Peerhost { get; set; }
        /// <summary>
        /// 联机状态 true:在线 false:离线
        /// </summary>
        public bool Online { get; set; }
    }
}
