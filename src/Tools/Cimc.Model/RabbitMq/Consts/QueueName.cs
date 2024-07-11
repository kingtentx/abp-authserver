using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cimc.Model.RabbitMq
{
    /// <summary>
    /// 队列名称
    /// </summary>
    public class QueueName
    {
        /// <summary>
        /// 主题消息
        /// </summary>
        public const string DeviceDataQueue = "breed.iot.data";
         
        /// <summary>
        /// 设备告警
        /// </summary>
        public const string DeviceWarningQueue = "device.warning.data";
    }
}
