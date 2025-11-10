
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    public static class StringHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(StringHelper));

        /// <summary>
        /// 验证是否是有效邮箱地址
        /// </summary>
        /// <param name="ipt">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsEmail(this string ipt)
        {
            if (ipt != null)
            {
                return Regex.IsMatch(ipt, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            }
            return false;
        }
        /// <summary>
        /// 是否是手机号码
        /// </summary>
        /// <param name="ipt"></param>
        /// <returns></returns>
        public static bool IsMobile(this string ipt)
        {
            if (ipt != null)
            {
                return Regex.IsMatch(ipt, "^\\+?[1-9][0-9]{7,14}$");
            }
            return false;
        }

        /// <summary>
        /// 是否是网址
        /// </summary>
        /// <param name="ipt"></param>
        /// <returns></returns>
        public static bool IsHttp(this string ipt)
        {
            if (ipt != null)
            {
                ipt = ipt.Trim().ToLower();
                if (ipt.StartsWith("http://") || ipt.StartsWith("https://"))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 返回最后的指定字符串之前的字符串
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SubstringBeforeLast(this string sourse, string str)
        {
            try
            {
                int tidx = sourse.LastIndexOf(str);
                return sourse.Substring(0, tidx);
            }
            catch (Exception ex)
            {
                log.Error("SubstringBefore 错误:" + ex.Message);
            }
            return string.Empty;
        }
        /// <summary>
        /// 返回最后的指定字符串之后的字符串
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SubstringAfterLast(this string sourse, string str)
        {
            try
            {
                int tidx = sourse.LastIndexOf(str) + 1;
                if (tidx > sourse.Length) return string.Empty;
                return sourse.Substring(tidx, sourse.Length - tidx);
            }
            catch (Exception ex)
            {
                log.Error("SubstringBefore 错误:" + ex.Message);
            }
            return string.Empty;
        }
        /// <summary>
        /// 返回指定字符串之前的字符串
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SubstringBefore(this string sourse, string str)
        {
            try
            {
                int tidx = sourse.IndexOf(str);
                return sourse.Substring(0, tidx);
            }
            catch (Exception ex)
            {
                log.Error("SubstringBefore 错误:" + ex.Message);
            }
            return string.Empty;
        }
        public static string SubstringBetween(this string sourse, string startstr, string endstr)
        {
            string result = string.Empty;
            int startindex, endindex;
            try
            {
                startindex = sourse.IndexOf(startstr);
                if (startindex == -1)
                    return result;
                string tmpstr = sourse.Substring(startindex + startstr.Length);
                endindex = tmpstr.IndexOf(endstr);
                if (endindex == -1)
                    return result;
                result = tmpstr.Remove(endindex);
            }
            catch (Exception ex)
            {
                log.Error("SubstringBetween 错误:" + ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 限制字符串长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Limit(this string str, int length)
        {
            return str.Substring(0, Math.Min(str.Length, length));
        }
        /// <summary>
        /// 如果字符串以startStr开头，则删除开头字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startStr"></param>
        /// <returns></returns>
        public static string RemoveStart(this string str, string startStr)
        {
            if (str.StartsWith(startStr))
            {
                str = str.Substring(startStr.Length);
            }
            return str;
        }
        /// <summary>
        /// SQL关键词过滤
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SqlLikeFilter(this string str)
        {
            if (str == null) { return string.Empty; }
            StringBuilder strbuild = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char chr = str[i];
                if (chr == '\"' || chr == '\'' || chr == '%' || chr == '[' || chr == ']' || chr == '_')
                {
                    continue;
                }
                strbuild.Append(chr);
            }

            return strbuild.ToString();
        }
    }
}
