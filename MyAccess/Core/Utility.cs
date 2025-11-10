using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Dynamic;

namespace MyAccess.Core
{
    public static class Utility
    {
        /// <summary>
        /// 多线程随机数
        /// </summary>
        static int seed = (int)(DateTime.Now.Ticks & int.MaxValue);
        public static readonly ThreadLocal<Random> ThreadLocalRandom = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));
        public static D Mapper<D>(object s)
        {
            D d = Activator.CreateInstance<D>();
            Type Types = s.GetType();//获得类型  
            Type Typed = typeof(D);
            PropertyInfo[] splist = Types.GetProperties();
            foreach (PropertyInfo sp in splist)//获得类型的属性字段  
            {
                PropertyInfo dp = Typed.GetProperty(sp.Name);
                if (dp == null || !sp.GetType().Equals(dp.GetType()))
                {
                    continue;
                }
                dp.SetValue(d, sp.GetValue(s, null), null);//获得s对象属性的值复制给d对象的属性  
            }
            return d;
        }

        /// <summary>
        /// 获取服务器操作系统
        /// </summary>
        public static string ServerOS
        {
            get
            {
                return Environment.OSVersion.VersionString;
            }
        }

        public static string GetEnvironmentVersion()
        {
            return Environment.Version.Major + "." + Environment.Version.Minor + "." + Environment.Version.Build + "." + Environment.Version.Revision;
        }
        //AspNet 内存占用 
        public static string GetAspNetN()
        {
            string temp;
            try
            {
                temp = ((Double)System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("N2") + "MB";
            }
            catch
            {
                temp = "未知";
            }
            return temp;
        }
        //CPU时间
        public static string GetNetCpu()
        {
            string temp;
            try
            {
                temp = ((TimeSpan)System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime).TotalSeconds.ToString("N0");
            }
            catch
            {
                temp = "未知";
            }
            return temp;
        }
        //应用程序占用内存 
        public static string GetServerAppN()
        {
            string temp;
            try
            {
                temp = ((Double)GC.GetTotalMemory(false) / 1048576).ToString("N2") + "MB";
            }
            catch
            {
                temp = "未知";
            }
            return temp;
        }

        public static long GetDirectorySize(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    long Size = 0;
                    DirectoryInfo info = new DirectoryInfo(directoryPath);
                    FileInfo[] fis = info.GetFiles();
                    foreach (FileInfo fi in fis)
                    {
                        Size += fi.Length;
                    }
                    DirectoryInfo[] dis = info.GetDirectories();
                    foreach (DirectoryInfo di in dis)
                    {
                        Size += GetDirectorySize(di.FullName);
                    }
                    return Size;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
        public static long address(IPAddress address)
        {
            byte[] addressBytes = address.GetAddressBytes();
            if (addressBytes.Length < 8)
            {
                return (long)BitConverter.ToUInt32(addressBytes, 0);
            }
            return (long)BitConverter.ToUInt64(addressBytes, 0);
        }


        public static int GetRandomInt32()
        {
            return ThreadLocalRandom.Value.Next(0x7fffffff);
        }

        public static long GetRandomInt64()
        {
            long num = ThreadLocalRandom.Value.Next(0x7fffffff);
            long num2 = ThreadLocalRandom.Value.Next(0x7fffffff);
            return ((num << 0x20) | num2);
        }

        /// <summary> 
        /// 字符串转16进制字节数组 
        /// </summary> 
        /// <param name="hexString"></param> 
        /// <returns></returns> 
        public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary> 
        /// 字节数组转16进制字符串 
        /// </summary> 
        /// <param name="bytes"></param> 
        /// <returns></returns> 
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        /// <summary>
        /// 判断dynamic是否包含指定属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public static bool IsPropertyExist(dynamic data, string propertyname)
        {
            if (data is ExpandoObject)
                return ((IDictionary<string, object>)data).ContainsKey(propertyname);
            return data.GetType().GetProperty(propertyname) != null;
        }
    }
}
