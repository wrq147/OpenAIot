using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MyAccess.Core
{
    public class TypeConvert
    {
        public static List<Dictionary<string, object>> DataTableToDict(DataTable dataTable)
        {
            var list = dataTable.AsEnumerable().Select(row => row.Table.Columns.Cast<DataColumn>()
                .ToDictionary(column => column.ColumnName, column => row[column])).ToList();
            return list;
        }
        public static string DictToUrl(Dictionary<string, object> dict)
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in dict)
            {
                if (pair.Value != null)
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }
        public static Dictionary<string, object> UrlToDict(string url)
        {
            string[] xdicts = url.Split('&');
            Dictionary<string, object> dics = new Dictionary<string, object>();
            foreach (string xs in xdicts)
            {
                int kvidx = xs.IndexOf("=");
                if (kvidx > 0)
                {
                    string tkey = xs.Substring(0, kvidx);
                    string tval = xs.Substring(kvidx + 1);
                    dics.Add(tkey, tval);
                }
            }
            return dics;
        }

        /// <summary>
        /// unix时间转C#
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime Unix2Time(long unixTime)
        {
            var dto = DateTimeOffset.FromUnixTimeMilliseconds(unixTime);
            return dto.LocalDateTime;
        }
        /// <summary>
        /// C#时间转unix时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long Time2Unix(DateTime time)
        {
            DateTimeOffset dto = new DateTimeOffset(time);
            return dto.ToUnixTimeMilliseconds();
        }
        /// <summary>
        /// 长整型转36进制激活码
        /// </summary>
        /// <returns></returns>
        public static string ToCode(long input)
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string a = "";
            while (input >= 1)
            {
                int index = Convert.ToInt16(input - (input / 36) * 36);
                a = chars[index] + a;
                input = input / 36;
            }
            return a;
        }
        public static bool isNull(object value)
        {
            return (value == null || Convert.IsDBNull(value)) ? true : false;
        }



        #region 转换对像为布尔值
        /// <summary>
        /// 转换对象为布尔值
        /// </summary>
        /// <param name="Value">对象</param>
        /// <returns>转换后的布尔值</returns>
        public static bool StrToBool(object Value)
        {
            if (!isNull(Value))
            {
                string[] array = new string[] { "true", "yes", "1" };
                return (Array.IndexOf<string>(array, Value.ToString().ToLower()) >= 0);
            }
            return false;
        }

        /// <summary>
        /// 转换对象为布尔值
        /// </summary>
        /// <param name="Value">对象</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns>转换后的布尔值</returns>
        public static bool StrToBool(object Value, bool DefaultValue)
        {
            if (!isNull(Value))
            {
                string[] array = new string[] { "true", "yes", "1" };
                return ((Array.IndexOf<string>(array, Value.ToString().ToLower()) >= 0));
            }
            return DefaultValue;
        }

        #endregion

        #region 转换对象为字符串
        /// <summary>
        /// 转换对象为字符串
        /// </summary>
        /// <param name="Value">对象</param>
        /// <returns>转换后的字符串</returns>
        public static string ToString(object Value)
        {
            if (isNull(Value))
            {
                return string.Empty;
            }
            return Value.ToString();
        }

        /// <summary>
        /// 转换对象为字符串
        /// </summary>
        /// <param name="Value">对象</param>
        /// <param name="DefaultValue">默认值</param>
        /// <param name="Trim">是否去除空格</param>
        /// <returns>转换后的字符串</returns>
        public static string ToString(object Value, string DefaultValue, bool Trim)
        {
            if (isNull(Value))
            {
                return ToString(DefaultValue);
            }
            string tRtStr = Value.ToString();
            if (tRtStr == string.Empty)
            {
                return ToString(DefaultValue);
            }
            if (Trim)
            {
                return tRtStr.Trim();
            }
            return tRtStr;
        }
        public static string ToString(object Value, string DefaultValue)
        {
            return ToString(Value, DefaultValue, true);
        }

        #endregion


        #region int型转换为string型
        /// <summary>
        /// int型转换为string型
        /// </summary>
        /// <returns>转换后的string类型结果</returns>
        public static string IntToStr(int intValue)
        {
            return Convert.ToString(intValue);
        }
        #endregion


        public static int[] StrToIntArr(string input)
        {
            string[] strArr = input.Split(new char[] { ',' });
            int[] intArr = new int[strArr.Length];
            for (int i = 0; i < strArr.Length; i++)
            {
                string str = strArr[i];
                intArr[i] = Convert.ToInt32(str);
            }
            return intArr;
        }
        public static List<string> StrToList(string input)
        {
            string[] strArr = input.Split(new char[] { ',' });
            List<string> _list = new List<string>();
            foreach (string str in strArr)
            {
                _list.Add(str);
            }
            return _list;
        }
        public static string ListToStr(List<string> arr)
        {
            string _output = "";
            for (int i = 0; i < arr.Count; i++)
            {
                if (i == 0)
                {
                    _output += arr[i];
                }
                else
                {
                    _output += "," + arr[i];
                }
            }
            return _output;
        }
        /// <summary>
        /// 四舍五入取整
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int Round(double input)
        {
            return (int)ToRound(input, 0);
        }
        /// <summary>
        /// 真正四舍五入
        /// </summary>
        /// <param name="input"></param>
        /// <param name="accuracy">要进的位</param>
        /// <returns></returns>
        public static float ToRound(double input, int accuracy)
        {
            float k = 1;
            for (int i = 0; i < accuracy; i++)
                k *= 10;
            int a = (int)(input * k + 0.5);
            return a / k;
        }
    }
}
