using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cimc.Model.Enums
{
    /// <summary>
    /// 数据类型
    /// </summary>
    public enum DataValueType
    {
        /// <summary>
        /// 整数型
        /// </summary>
        [Description("整数型")]
        Int,
        /// <summary>
        /// 字符串
        /// </summary>
        [Description("字符串")]
        String,
        /// <summary>
        /// 布尔型
        /// </summary>
        [Description("布尔型")]
        Bool,
        /// <summary>
        /// 浮点型
        /// </summary>
        [Description("浮点型")]
        Double,
        /// <summary>
        /// 时间型
        /// </summary>
        [Description("时间型")]
        DateTime,
        /// <summary>
        /// 枚举型
        /// </summary>
        [Description("枚举型")]
        Enum,

    }
}
