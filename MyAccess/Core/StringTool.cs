using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
namespace MyAccess.Core
{
    public static class StringTool
    {
        /// <summary>
        /// Sql参数防注入
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SqlLikeFilter(string str)
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
        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Capitalize(string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }
        /// <summary>
        /// 版本号比较
        /// </summary>
        /// <param name="oldVersion"></param>
        /// <param name="newVersion"></param>
        /// <returns>旧版本大于新版本则返回-1，反之返回1，相等返回0</returns>
        public static int VersionCompare(string oldVersion, string newVersion)
        {
            string[] oldarr = oldVersion.Split(new char[] { '.' });
            string[] newarr = newVersion.Split(new char[] { '.' });

            for (int i = 0; i < oldarr.Length; i++)
            {
                if (i < newarr.Length)
                {
                    int oldval;
                    if (!int.TryParse(oldarr[i], out oldval))
                    {
                        oldval = 0;
                    }
                    int newval;
                    if (!int.TryParse(newarr[i], out newval))
                    {
                        newval = 0;
                    }
                    if (oldval > newval)
                    {
                        return -1;
                    }
                    else if (oldval < newval)
                    {
                        return 1;
                    }
                }
                else
                {
                    //如果新版'.'更多
                    return 1;
                }
            }
            return 0;
        }


        public static string GetGUID()
        {
            return Guid.NewGuid().ToString("N");
        }
        /// <summary>
        /// 得到时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDateTimeStr()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        /// <summary>
        /// 随机日期值
        /// </summary>
        /// <returns></returns>
        public static string RandomDateStr()
        {
            return DateTime.Now.ToString("yyMMddHHmmssffff") + Utility.ThreadLocalRandom.Value.Next(99);
        }

        /// <summary>
        /// 字符串反转算法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StrReverse(string str)
        {
            string ret = "";
            for (int i = str.Length - 1; i > -1; i--)
            {
                ret += str.Substring(i, 1);
            }
            return ret;
        }
        public static int CaculateWeekDay(int y, int m, int d)
        {
            return (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7;
        }
        public static string DateToRelative(DateTime startDate, DateTime endDate)
        {
            TimeSpan span = endDate - startDate;
            if (span.TotalDays > 60)
            {
                return endDate.ToString("yyyy-MM-dd");
            }
            else if (span.TotalDays > 32)
            {
                return (endDate.Month - startDate.Month) + "个月";
            }
            else if (span.TotalDays > 1)
            {
                return string.Format("{0}天{1}小时", (int)Math.Floor(span.TotalDays), span.Hours);
            }
            else if (span.TotalHours > 1)
            {
                return string.Format("{0}小时", (int)Math.Floor(span.TotalHours));
            }
            else if (span.TotalMinutes > 1)
            {
                return string.Format("{0}分钟", (int)Math.Floor(span.TotalMinutes));
            }
            else if (span.TotalSeconds >= 1)
            {
                return string.Format("{0}秒", (int)Math.Floor(span.TotalSeconds));
            }
            else
            {
                return "1秒";
            }
        }
        public static int DateDiff(string unit, DateTime startDate, DateTime endDate)
        {
            TimeSpan timeSpan = (TimeSpan)(endDate - startDate);
            int retValue = 0;
            int diffYear = endDate.Year - startDate.Year;
            int diffMonth = endDate.Month - startDate.Month;
            int diffDay = (int)Math.Abs(timeSpan.TotalDays);
            int diffHour = (int)Math.Abs(timeSpan.TotalHours);
            int diffMinute = (int)Math.Abs(timeSpan.TotalMinutes);
            int diffSecond = (int)Math.Abs(timeSpan.TotalSeconds);
            if (unit == "year")
            {
                retValue = diffYear;
            }
            if (unit == "month")
            {
                retValue = diffYear * 12 + diffMonth;
            }
            if (unit == "day")
            {

                retValue = diffDay;
            }
            if (unit == "hour")
            {
                retValue = diffHour;
            }
            if (unit == "minute")
            {
                retValue = diffMinute;
            }
            if (unit == "second")
            {
                retValue = diffSecond;
            }
            return retValue;
        }


        public static string bSubstring(string s, int length)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s);
            int n = 0;
            int i = 0;
            for (; i < bytes.Length && n < length; i++)
            {
                if (i % 2 == 0)
                {
                    n++;
                }
                else
                {
                    if (bytes[i] > 0)
                    {
                        n++;
                    }
                }

            }

            if (i % 2 == 1)
            {
                if (bytes[i] > 0)
                    i = i - 1;
                else
                    i = i + 1;
            }
            return Encoding.Unicode.GetString(bytes, 0, i);
        }
        public static string Cutstr(string str, int startIndex, int length, bool moreSymbol, bool htmlencode)
        {
            string retStr = "";
            try
            {
                if (htmlencode)
                {
                    str = System.Net.WebUtility.HtmlDecode(str);
                }

                str = str.Substring(startIndex);

                if (Encoding.Unicode.GetByteCount(str) <= (length + 6))
                {
                    if (moreSymbol)
                    {
                        retStr = str;
                    }
                    else
                    {
                        retStr = bSubstring(str, length);
                    }
                }
                else
                {
                    retStr = bSubstring(str, length);
                }
                if (htmlencode)
                {
                    retStr = System.Net.WebUtility.HtmlEncode(retStr);
                }
                if (str.Length > retStr.Length)
                {
                    if (moreSymbol)
                    {
                        retStr += "...";
                    }
                }
            }
            catch
            {
                return "";
            }
            return retStr;
        }
        /// <summary>
        /// 字符串截取
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Cutstr(string str, int length)
        {
            return Cutstr(str, 0, length, true, true);
        }
        public static string Cutstr(string str, int length, bool htmlencode)
        {
            return Cutstr(str, 0, length, true, htmlencode);
        }
        /// <summary>
        /// 获取随机数字
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetDigitChar(int length)
        {
            int num;
            string rtStr = "";
            string[] digitList = new string[] {
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                };

            for (num = 0; num < length; num++)
            {
                rtStr = rtStr + digitList[Utility.ThreadLocalRandom.Value.Next(0, digitList.Length)];
            }
            return rtStr;
        }

        /// <summary>
        /// 获取随机英文字符
        /// </summary>
        /// <param name="length"></param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static string GetEnglishChar(int length, bool ignoreCase = false)
        {
            int num;
            string rtStr = "";
            string[] enCharList = new string[] {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
                "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F",
                "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V",
                "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                };
            if (ignoreCase)
            {
                enCharList = new string[] {
                "A", "B", "C", "D", "E", "F",
                "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V",
                "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                };
            }
  
            for (num = 0; num < length; num++)
            {
                rtStr = rtStr + enCharList[Utility.ThreadLocalRandom.Value.Next(0, enCharList.Length)];
            }
            return rtStr;
        }
        /// <summary>
        /// 获取随机中文
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetChinaChar(int length)
        {
            string[] strArray = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
            Random random = Utility.ThreadLocalRandom.Value;
            object[] objArray = new object[length];
            for (int i = 0; i < length; i++)
            {
                int num3;
                int num5;
                int index = random.Next(11, 14);
                string str = strArray[index].Trim();
                if (index == 13)
                {
                    num3 = random.Next(0, 7);
                }
                else
                {
                    num3 = random.Next(0, 0x10);
                }
                string str2 = strArray[num3].Trim();
                int num4 = random.Next(10, 0x10);
                string str3 = strArray[num4].Trim();
                if (num4 == 10)
                {
                    num5 = random.Next(1, 0x10);
                }
                else if (num4 == 15)
                {
                    num5 = random.Next(0, 15);
                }
                else
                {
                    num5 = random.Next(0, 0x10);
                }
                string str4 = strArray[num5].Trim();
                byte num6 = Convert.ToByte(str + str2, 0x10);
                byte num7 = Convert.ToByte(str3 + str4, 0x10);
                byte[] buffer = new byte[] { num6, num7 };
                objArray.SetValue(buffer, i);
            }
            int num;
            string rtStr = "";
            Encoding encoding = Encoding.GetEncoding("gb2312");
            for (num = 0; num < objArray.Length; num++)
            {
                rtStr = rtStr + encoding.GetString((byte[])Convert.ChangeType(objArray[num], typeof(byte[])));
            }
            return rtStr;
        }


        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                {
                    return new string[] { strContent };
                }
                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            return new string[0];
        }
        public static bool InArray(string str, string[] stringarray)
        {
            return InArray(str, stringarray, false);
        }

        public static bool InArray(string str, string stringarray)
        {
            return InArray(str, SplitString(stringarray, ","), false);
        }
        public static bool InArray(string str, string stringarray, bool caseInsensetive)
        {
            return InArray(str, SplitString(stringarray, ","), caseInsensetive);
        }
        public static bool InArray(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return (GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0);
        }

        public static bool InArray(string str, string stringarray, string strsplit)
        {
            return InArray(str, SplitString(stringarray, strsplit), false);
        }

        public static bool InArray(string str, string stringarray, string strsplit, bool caseInsensetive)
        {
            return InArray(str, SplitString(stringarray, strsplit), caseInsensetive);
        }
        public static int GetInArrayID(string strSearch, string[] stringArray)
        {
            return GetInArrayID(strSearch, stringArray, true);
        }

        public static int GetInArrayID(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower())
                    {
                        return i;
                    }
                }
                else if (strSearch == stringArray[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public static string[] GetWords(string input)
        {
            List<string> twords = new List<string>();
            if (input.Length != 2)
            {
                twords.Add(input);
            }
            int tc = input.Length - 2;
            for (int ci = 0; ci <= tc; ci++)
            {
                int _in = ci + 1;
                string tStr = "";
                tStr += input[ci];
                tStr += input[_in];
                twords.Add(tStr);
            }
            return twords.ToArray();
        }
        /// <summary>
        /// 从身份证判断男女：0为女，1为男
        /// </summary>
        /// <param name="idcard"></param>
        /// <returns></returns>
        public static int GetSexByIDCard(string idcard)
        {
            string sex = "";
            if (idcard.Length == 18)
            {
                sex = idcard.Substring(14, 3);
            }
            else
            {
                sex = idcard.Substring(12, 3);

            }
            if (int.Parse(sex) % 2 == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        /// <summary>
        /// 从身份证判断出生日期
        /// </summary>
        /// <param name="idcard"></param>
        /// <returns></returns>
        public static string GetBirthByIDCard(string idcard)
        {
            string birthday = "";
            if (idcard.Length == 18)
            {
                birthday = idcard.Substring(6, 4) + "-" + idcard.Substring(10, 2) + "-" + idcard.Substring(12, 2);
            }
            else
            {
                birthday = "19" + idcard.Substring(6, 2) + "-" + idcard.Substring(8, 2) + "-" + idcard.Substring(10, 2);

            }
            return birthday;
        }

    }
}
