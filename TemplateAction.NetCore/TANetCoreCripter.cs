using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TemplateAction.NetCore
{
    internal class TANetCoreCripter
    {
        public static byte[] Decrypt(byte[] encrypted, byte[] key)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = key;
            provider.IV = key;
            MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(encrypted, 0, encrypted.Length);
                cs.FlushFinalBlock();
                cs.Close();
            }
            return ms.ToArray();
        }

        /// <summary>
        /// 对称解密
        /// </summary>
        /// <param name="encrypted"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string encrypted, string key)
        {
            byte[] buffer = Convert.FromBase64String(encrypted);
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(key);
            return Encoding.UTF8.GetString(Decrypt(buffer, bytes));
        }

        public static byte[] Encrypt(byte[] original, byte[] key)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = key;
            provider.IV = key;
            MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(original, 0, original.Length);
                cs.FlushFinalBlock();
                cs.Close();
            }
            return ms.ToArray();
        }
        /// <summary>
        /// 对称加密
        /// </summary>
        /// <param name="original"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string original, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(original);
            byte[] buffer2 = ASCIIEncoding.ASCII.GetBytes(key);
            return Convert.ToBase64String(Encrypt(bytes, buffer2));
        }
    }
}
