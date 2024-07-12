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
        public const string TopicQueue = "emqx.topic.queue";
        /// <summary>
        /// 客户信息
        /// </summary>
        public const string ClientInfoQueue = "emqx.clientinfo.queue";
        /// <summary>
        /// Ota主题
        /// </summary>
        public const string OtaQueue = "emqx.ota.queue";
        /// <summary>
        /// 系统参数配置主题
        /// </summary>
        public const string SysConfigQueue = "emqx.sysconfig.queue";
        /// <summary>
        /// 设备数据
        /// </summary>
        public const string DeviceDataQueue = "emqx.devicedata.queue";
        /// <summary>
        /// 设备告警
        /// </summary>
        public const string DeviceWarningQueue = "emqx.devicewarning.queue";

        /// <summary>
        /// 未知设备数据
        /// </summary>
        public const string DeviceUnknownQueue = "device.unknown.queue";
        /// <summary>
        /// 3期设备数据交换机
        /// </summary>
        public const string ThridDeviceDataExchange = "third.data.exchange";
        /// <summary>
        /// 3期设备数据
        /// </summary>
        public const string ThridDeviceDataQueue = "third.data.queue";
        /// <summary>
        /// 3期设备数据交换机-通讯异常
        /// </summary>
        public const string ThridDeviceDataOfflineExchange = "third.data.offline.exchange";
        /// <summary>
        /// 3期设备数据-通讯异常
        /// </summary>
        public const string ThridDeviceDataOfflineQueue = "third.data.offline.queue";

        /// <summary>
        /// 3期告警数据交换机
        /// </summary>
        public const string ThridWarningDataExchange = "third.warning.exchange";
        /// <summary>
        /// 3期告警数据
        /// </summary>
        public const string ThridWarningDataQueue = "third.warning.queue";
    }
}
