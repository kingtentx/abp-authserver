using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cimc.Helper.Extensions
{
	public static partial class Extensions
	{
		/// <summary>
		/// 获取格式化字符串，无年月日，格式："HH:mm:ss"
		/// </summary>
		/// <param name="dateTime">日期</param>
		public static string ToTimeString(this DateTime dateTime)
		{
			return dateTime.ToString("HH:mm:ss");
		}

		/// <summary>
		/// 获取格式化字符串，无时分秒，格式："yyyy-MM-dd"
		/// </summary>
		/// <param name="dateTime">日期</param>
		public static string ToDateString(this DateTime dateTime)
		{
			return dateTime.ToString("yyyy-MM-dd");
		}

		/// <summary>
		/// 获取格式化字符串，带时分秒，格式："yyyy-MM-dd HH:mm:ss"
		/// </summary>
		/// <param name="dateTime">日期</param>
		/// <param name="isRemoveTime">是否移除时间</param>
		public static string ToDateTimeString(this DateTime dateTime)
		{
			return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
		}

		/// <summary>
		/// 获取格式化字符串，带毫秒，格式："yyyy-MM-dd HH:mm:ss.fff"
		/// </summary>
		/// <param name="dateTime">日期</param>
		public static string ToMillisecondString(this DateTime dateTime)
		{
			return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		/// <summary>
		/// 获取格式化字符串，不带时分秒，格式："yyyy年MM月dd日"
		/// </summary>
		/// <param name="dateTime">日期</param>
		public static string ToChineseDateString(this DateTime dateTime)
		{
			return dateTime.ToString("yyyy年MM月dd日");
		}

		/// <summary>
		/// 获取格式化字符串，不带时分秒，格式："yyyy年MM月dd日 HH:mm:ss"
		/// </summary>
		/// <param name="dateTime">日期</param>
		public static string ToChineseDateTimeString(this DateTime dateTime)
		{
			return dateTime.ToString("yyyy年MM月dd日 HH:mm:ss");
		}

	}
}
