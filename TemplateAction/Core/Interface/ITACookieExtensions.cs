using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    public static class ITACookieExtensions
    {
        /// <summary>
        /// 重置过期时间
        /// </summary>
        /// <param name="time"></param>
        public static void ResetExpires(this ITACookie cookie, double time)
        {
            if (cookie.Expires > DateTime.Now)
            {
                TimeSpan span = cookie.Expires - DateTime.Now;
                if (span.TotalMinutes < time / 2)
                {
                    cookie.SetExpires(time);
                }
            }
            else
            {
                cookie.SetExpires(time);
            }
        }
        /// <summary>
        /// 重置过期长时间
        /// </summary>
        /// <param name="day"></param>
        public static void ResetLongExpires(this ITACookie cookie, double day)
        {
            if (cookie.Expires > DateTime.Now)
            {
                TimeSpan span = cookie.Expires - DateTime.Now;
                if (span.TotalHours < (day / 2 * 24))
                {
                    cookie.SetLongExpires(day);
                }
            }
            else
            {
                cookie.SetLongExpires(day);
            }
        }
        /// <summary>
        /// 以分钟为单位
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="time"></param>
        public static void SetExpires(this ITACookie cookie, double time)
        {
            cookie.Expires = DateTime.Now.AddMinutes(time);
        }
        public static void SetLongExpires(this ITACookie cookie, double day)
        {
            cookie.Expires = DateTime.Now.AddDays(day);
        }
    }
}
