using Jint;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChannelUtility.Tsl
{
    public class BaseValueOption
    {
        /// <summary>
        /// 数据类型（必填项）
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 预处理表达式（格式：data>0?1:0）
        /// </summary>
        public string express { get; set; }
        private static float ObjToFloat(object input)
        {
            try
            {
                var linput = Convert.ToInt32(input);
                byte[] bytes = BitConverter.GetBytes(linput);
                return BitConverter.ToSingle(bytes);
            }
            catch
            {
                return 0;
            }
        }
        private static uint ObjToUnsigned(object input)
        {
            try
            {
                if(input is short sval)
                {
                    return (ushort)sval;
                }
                else
                {
                    return (uint)input;
                }
            }
            catch
            {
                return 0;
            }
        }
        delegate string ByteTrueToDelegate(object input, params string[] strs);
        private static string ByteTrueTo(object input, params string[] strs)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                long linput = Convert.ToInt64(input);
                if (strs.Length == 0)
                {
                    List<long> result = new List<long>(31);
                    for (int i = 0; i < 31; i++)
                    {
                        if ((linput & (1L << i)) != 0) //检查第i位是否为1
                        {
                            result.Add(1 << i);
                        }
                    }

                    return string.Join(',', result);
                }
                else
                {
                    int jdge = 1;
                    foreach (var s in strs)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            if ((linput & jdge) != 0)
                            {
                                sb.Append(s).Append(",");
                            }
                        }
                        jdge = jdge << 1;
                    }
                    return sb.ToString().Trim(',');
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        private static string ByteAllTo(object input, params string[] strs)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                long linput = Convert.ToInt64(input);
                int jdge = 1;
                for (int i = 0; i < strs.Length; i += 2)
                {
                    if (!string.IsNullOrEmpty(strs[i]))
                    {
                        if ((linput & jdge) != 0)
                        {
                            sb.Append(strs[i]).Append(",");
                        }
                        else
                        {
                            if ((i + 1) >= strs.Length)
                            {
                                continue;
                            }
                            sb.Append(strs[i + 1]).Append(",");
                        }
                    }

                    jdge = jdge << 1;
                }
                return sb.ToString().Trim(',');
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 原数据转换成显示数据
        /// </summary>
        /// <param name="input"></param>
        /// <param name="getProp"></param>
        /// <returns></returns>
        public object RawTo(object input, Func<string, object> getProp)
        {
            if (string.IsNullOrEmpty(express))
            {
                return InnerRawTo(input);
            }
            else
            {
                try
                {
                    var eng = new Engine()
                    .SetValue("toFloat", new Func<object, float>(ObjToFloat))
                    .SetValue("toUnsigned", new Func<object, uint>(ObjToUnsigned))
                    .SetValue("data", input)
                    .SetValue("byteTo", new ByteTrueToDelegate(ByteTrueTo))
                    .SetValue("byteAllTo", new ByteTrueToDelegate(ByteAllTo))
                    .SetValue("prop", getProp);
                    var res = eng.Evaluate(express);
                    return InnerRawTo(res.ToObject());
                }
                catch
                {
                    switch (this.type)
                    {
                        case "geo":
                        case "enum":
                        case "string":
                            return string.Empty;
                        default:
                            return 0;
                    }
                }

            }
        }
        protected virtual object InnerRawTo(object input)
        {
            return input;
        }
  
    }
}
