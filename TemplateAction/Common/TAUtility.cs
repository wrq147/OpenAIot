using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace TemplateAction.Common
{
    public class TAUtility
    {
        /// <summary>
        /// 全局定义
        /// </summary>
        public const string FILE_EXT = ".my";
        public const string FUN_VAR = "var";
        public const string ASSIGN_SRC = "src";
        public const string CONDITION_EX = "ex";
        public const string FOR_FROM = "from";
        public const string FOR_NAME = "name";
        public const string FOR_INDEX = "index";
        public const string NS_KEY = "namespace";
        public const string CONTROLLER_KEY = "controller";
        public const string ACTION_KEY = "action";
        public const string HTML_ENCODE = "html";
        public const string TYPE_NAME_SYMBOL = "$$";
        public const int EXCEPTION_CODE = 604;
        /// <summary>
        /// 模块后缀名
        /// </summary>
        public const string ModExt = ".dll";
        /// <summary>
        /// 类型与名称合并成Key
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string TypeName2ServiceKey(Type serviceType, string name)
        {
            return serviceType.FullName + TYPE_NAME_SYMBOL + name;
        }
  
        /// <summary>
        /// 相对路径转成指定根目录的绝对路径
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RelativeToAbsolutePath(string root, string path)
        {
            if (path == null) return string.Empty;
            int ss = 0;
            if (path.Length > 0)
            {
                if (path[0] == '~')
                {
                    ss++;
                }
            }
            if (ss < path.Length)
            {
                if (path[ss] == '/')
                {
                    ss++;
                }
            }

            if (ss > 0)
            {
                path = path.Substring(ss);
            }
            //windows系统
            if (Path.DirectorySeparatorChar.Equals('\\'))
            {
                path = path.Replace("/", "\\");
            }
            return Path.Combine(root, path);
        }
        public static int ReadFile(out string cont, string path)
        {
            return ReadFile(out cont, path, Encoding.UTF8);
        }
        public static int ReadFile(out string cont, string path, Encoding def)
        {
            try
            {
                string str = string.Empty;
                if (File.Exists(path))
                {

                    StreamReader reader = new StreamReader(path, def);
                    str = reader.ReadToEnd();
                    reader.Close();
                    cont = str;
                    return 0;
                }
                cont = "文件不存在";
                return -1;
            }
            catch
            {
                cont = "读取文件出错";
                return -2;
            }
        }

        /// <summary>
        /// 判断是否为数值类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumerical(Type type)
        {
            return (type.IsPrimitive) && type != typeof(bool) && type != typeof(char);
        }
        /// <summary>
        /// 判断type是否可转换成type,支持泛型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="asType"></param>
        /// <returns></returns>
        public static bool IsAs(Type type, Type asType)
        {
            if (asType.IsAssignableFrom(type)) return true;
            if (type.IsGenericType)
            {
                Type typeReduced = type.GetGenericTypeDefinition();
                Type asTypeReduced = asType;

                if (asType.IsGenericType)
                {
                    asTypeReduced = asType.GetGenericTypeDefinition();
                }
                if (asTypeReduced.IsAssignableFrom(typeReduced))
                {
                    Type[] typeArguments = type.GetGenericArguments();
                    Type[] asTypeArguments = asType.GetGenericArguments();
                    if (typeArguments.Length != asTypeArguments.Length) return false;

                    bool isSuccess = true;
                    for (int i = 0; i < typeArguments.Length; i++)
                    {
                        if (typeArguments[0].IsGenericParameter)
                        {
                            isSuccess = false;
                            break;
                        }
                        if (!asTypeArguments[0].IsGenericParameter)
                        {
                            isSuccess = false;
                            break;
                        }
                    }

                    if (isSuccess)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static string BSubStr(string s, int length)
        {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(s);
            int n = 0;
            int i = 0;
            for (; i < bytes.GetLength(0) && n < length; i++)
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
            return System.Text.Encoding.Unicode.GetString(bytes, 0, i);
        }


        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AllFilter(string str)
        {
            StringBuilder strbuild = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char chr = str[i];
                if (chr == '\'' || chr == '\"' || chr == '<' || chr == '>')
                {
                    continue;
                }
                else if (chr == '\r')
                {
                    strbuild.Append("\\r");
                }
                else if (chr == '\n')
                {
                    strbuild.Append("\\n");
                }
                else
                {
                    strbuild.Append(chr);
                }

            }
            return strbuild.ToString();
        }

        /// <summary>
        /// java时间转C#
        /// </summary>
        /// <param name="time_JAVA_Long"></param>
        /// <returns></returns>
        public static DateTime JavaLongTime2CSharp(long time_JAVA_Long)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = new TimeSpan(time_JAVA_Long * 10000);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        }
        /// <summary>
        /// C#时间转java
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long Time2JavaLong(DateTime time)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = time - dtStart;
            return toNow.Ticks / 10000;
        }




        #region Unicode编码
        private class UnicodeParseMapping
        {
            private Dictionary<char, string> _mapping;
            private HashSet<char> _chineseSymbol;
            public bool TryGetMapping(char c, out string value)
            {
                return _mapping.TryGetValue(c, out value);
            }
            public bool IsChineseSymbol(char c)
            {
                return _chineseSymbol.Contains(c);
            }
            public UnicodeParseMapping()
            {
                _mapping = new Dictionary<char, string>();
                _mapping.Add('"', "\\\"");
                _mapping.Add('\\', "\\\\");
                _mapping.Add('/', "\\/");
                _mapping.Add('\b', "\\b");
                _mapping.Add('\f', "\\f");
                _mapping.Add('\n', "\\n");
                _mapping.Add('\r', "\\r");
                _mapping.Add('\t', "\\t");
                char[] csys = new char[] {'–', '—', '‘', '’', '“', '”',
    '…', '、', '。', '〈', '〉', '《',
    '》', '「', '」', '『', '』', '【',
    '】', '〔', '〕', '！', '（', '）',
    '，', '．', '：', '；', '？'};
                _chineseSymbol = new HashSet<char>();
                foreach (char c in csys)
                {
                    _chineseSymbol.Add(c);
                }
            }
        }
        private static readonly UnicodeParseMapping ParseMapping = new UnicodeParseMapping();
        /// <summary>
        /// Unicode编码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="allUnicode"></param>
        /// <returns></returns>
        public static string Unicode(string input, bool allUnicode = false)
        {
            StringBuilder parsed = new StringBuilder();
            foreach (char c in input)
            {
                string otstr;
                if (ParseMapping.TryGetMapping(c, out otstr))
                {
                    parsed.Append(otstr);
                }
                else
                {
                    if (allUnicode)
                    {
                        if (c < 32 || c >= 127)
                        {
                            parsed.Append("\\u" + ((uint)c).ToString("X4"));
                        }
                        else
                        {
                            parsed.Append(c);
                        }
                    }
                    else
                    {
                        if (c < 32 || c >= 127)
                        {
                            if (ParseMapping.IsChineseSymbol(c) || (c >= 0x4e00 && c <= 0x9fbb))
                            {
                                parsed.Append(c);
                            }
                            else
                            {
                                parsed.Append("\\u" + ((uint)c).ToString("X4"));
                            }
                        }
                        else
                        {
                            parsed.Append(c);
                        }

                    }

                }
            }

            return parsed.ToString();
        }
        #endregion


    }
}
