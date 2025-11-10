using System;
using TemplateAction.Common;
using TemplateAction.Core;

namespace TemplateAction.Label
{
    /// <summary>
    /// 内置函数标签类
    /// </summary>
    public class TempFuns
    {
        /// <summary>
        /// 判断路径是否不包含命名空间
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static bool NotNS(TAAction handle)
        {
            if (!handle.Context.Request.Path.ToLower().StartsWith("/" + handle.NameSpace))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(string input)
        {
            return string.IsNullOrEmpty(input);
        }

        /// <summary>
        /// java长整形时间转指定日期字符串
        /// </summary>
        /// <param name="time">longtime</param>
        /// <param name="format">格式化输出字符串</param>
        /// <returns></returns>
        public static string Long2Date(long time, string format)
        {
            DateTime dt = TAUtility.JavaLongTime2CSharp(time);
            return dt.ToString(format);
        }
        public static DateTime Long2DateTime(long time)
        {
            return TAUtility.JavaLongTime2CSharp(time);
        }
        /// <summary>
        /// 格式化输出字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Printf(string format, params object[] args)
        {
            return string.Format(format, args);
        }
        /// <summary>
        /// 通用格式化输出，可格式化字符串大小写
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Format(object obj, string f = "")
        {
            if (obj == null) return string.Empty;
            string strtarget = obj as string;
            if (strtarget != null)
            {
                if (">".Equals(f))
                {
                    return strtarget.ToUpper();
                }
                else if ("<".Equals(f))
                {
                    return strtarget.ToLower();
                }
                return strtarget;
            }
            else
            {
                if (obj.GetType().IsEnum)
                {
                    if ("short".Equals(f))
                    {
                        return ((short)obj).ToString();
                    }
                    else
                    {
                        return ((int)obj).ToString();
                    }
                }
                else
                {
                    IFormattable target = obj as IFormattable;
                    if (target == null) return string.Empty;
                    return target.ToString(f, null);
                }
            }
        }
        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        public static DateTime Now()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// 字符串比较
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int Strcompare(string strA, string strB, bool ign = false)
        {
            return string.Compare(strA, strB, ign);
        }


        /// <summary>
        /// 日期转换成模糊时间
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string Date2Str(DateTime date)
        {
            DateTime tnow = DateTime.Now;
            TimeSpan span;
            string afx = "后";
            if (tnow > date)
            {
                span = tnow - date;
                afx = "前";
            }
            else
            {
                afx = "后";
                span = date - tnow;
            }
            int year = span.Days / 365;
            int month = span.Days / 30;
            if (year > 0)
            {
                return year + "年" + afx;
            }
            else if (month > 0 && year == 0)
            {
                return month + "个月" + afx;
            }
            else if (span.Days > 0)
            {
                return span.Days + "天" + afx;
            }
            else if (span.Hours > 0)
            {
                return string.Format("{0}小时{1}", span.Hours, afx);
            }
            else if (span.Minutes > 0)
            {
                return string.Format("{0}分钟{1}", span.Minutes, afx);
            }
            else if (span.Seconds > 0)
            {
                return string.Format("{0}秒{1}", span.Seconds, afx);
            }
            else
            {
                return "1秒内";
            }
        }


        /// <summary>
        /// 字符串裁剪函数
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static string Cut(string str, int start, int length, bool symbol = true)
        {
            string retStr = string.Empty;
            try
            {
                str = str.Substring(start);

                if (System.Text.Encoding.Unicode.GetByteCount(str) <= (length + 6))
                {
                    if (symbol)
                    {
                        retStr = str;
                    }
                    else
                    {
                        retStr = TAUtility.BSubStr(str, length);
                    }
                }
                else
                {
                    retStr = TAUtility.BSubStr(str, length);
                }
                if (str.Length > retStr.Length)
                {
                    if (symbol)
                    {
                        retStr += "...";
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
            return retStr;
        }

        /// <summary>
        /// 查找指定字符串的位置
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static int IndexOf(string input, string source)
        {
            return input.IndexOf(source);
        }
        public static string[] Split(string input, string sep)
        {
            return input.Split(sep.ToCharArray());
        }
        /// <summary>
        /// 跟据分子和分母计算得出百分比数值
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Per(float sub, float total, string format = "")
        {
            float ret = 0;
            if (total != 0)
            {
                ret = (sub / total * 100.00f);
            }
            if (string.IsNullOrEmpty(format))
            {
                return ret.ToString("0.00");
            }
            else
            {
                return ret.ToString(format);
            }
        }
        public static string Html(string input)
        {
            return input == null ? string.Empty : System.Net.WebUtility.HtmlEncode(input);
        }
    }
}



