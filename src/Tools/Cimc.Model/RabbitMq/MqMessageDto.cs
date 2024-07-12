
namespace Cimc.Model.RabbitMq
{
    public class MqMessageDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string MsgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 泛型数据
        /// </summary>
        public object Data { get; set; }
    }
}
