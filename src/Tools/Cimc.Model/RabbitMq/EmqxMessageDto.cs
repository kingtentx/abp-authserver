namespace Cimc.Model.RabbitMq
{
    /// <summary>
    /// 转发消息对象
    /// </summary>
    public class EmqxMessageDto
    {
        /// <summary>
        /// 服务器节点
        /// </summary>
        public string Node { get; set; }
        /// <summary>
        /// qos
        /// </summary>
        public int Qos { get; set; }
        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public object Payload { get; set; }

    }
}
