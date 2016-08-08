using UnityEngine;
using System.Collections;

namespace MoreFun.Utils
{
    public class TimeUtil
    {
        public static ulong GetLocalTime()
        {
            System.DateTime time = System.DateTime.Now;
            return (ulong)time.DateToUnix();
        }

        public static string GetFormattime(uint seconds)
        {
            string tt = (seconds / 3600).ToString("00") + ":";
            seconds %= 3600;
            tt += (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
            return tt;
        }

        /// <summary>
        /// 传入unix时间获取C#DateTime格式的时间
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static System.DateTime GetDateTime(uint seconds)
        {
            System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return startTime.AddSeconds(seconds);
        }

        /// <summary>
        /// 传入unix时间获取C#DateTime格式的时间
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static System.DateTime GetDateTime(double seconds)
        {
            System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return startTime.AddSeconds(seconds);
        }

        public static bool IsSameDay(System.DateTime date1, System.DateTime date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day;
        }

        public static bool IsSameHour(System.DateTime date1, System.DateTime date2)
        {
            return IsSameDay(date1, date2) && date1.Hour == date2.Hour;
        }

        /// <summary>
        /// 传入两个unix时间，判断是否为同一天
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static bool IsSameDay(double time1, double time2)
        {
            System.DateTime date1 = GetDateTime(time1);
            System.DateTime date2 = GetDateTime(time2);
            return date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day;
        }

        public static bool IsSameHour(double time1, double time2)
        {
            System.DateTime date1 = GetDateTime(time1);
            System.DateTime date2 = GetDateTime(time2);
            return IsSameHour(date1, date2);
        }

        public static string GetFormattimeEx(double seconds)
        {
            //string tt = "";
            //System.DateTime now = GetDateTime(seconds);
            //tt = now.Hour.ToString("00") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00");
            //return tt;
            int deltaTime = (int)(seconds - GetLocalTime());
            if(deltaTime < 0)
            {
                return "";
            }
            string tt = (deltaTime / 3600).ToString("00") + ":";
            deltaTime %= 3600;
            tt += (deltaTime / 60).ToString("00") + ":" + (deltaTime % 60).ToString("00");
            return tt;
        }
    }

}
