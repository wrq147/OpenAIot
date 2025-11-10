using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MyAccess.Core
{
    public class Crypter
    {
        /// <summary>
        /// SHA1 加密，返回大写字符串
        /// </summary>
        /// <param name="content">需要加密字符串</param>
        /// <param name="encode">指定加密编码</param>
        /// <returns>返回40位大写字符串</returns>
        public static string SHA1(string content, Encoding encode)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_in = encode.GetBytes(content);
            byte[] bytes_out = sha1.ComputeHash(bytes_in);
            sha1.Dispose();
            string result = BitConverter.ToString(bytes_out);
            result = result.Replace("-", "");
            return result;
        }
        /// <summary>
        /// SHA256 加密，返回大写字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string SHA256(string content, Encoding encode)
        {
            SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(encode.GetBytes(content));
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }
        public static string PasswordMD5(string md5)
        {
            //这里的md5值为MyAccess的
            return MD5(md5 + "a8d51334053d0eed3c7c7a4d3d0f4f59");
        }
        public static string MD5Bit16(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(input)), 4, 8).ToLower();
            t2 = t2.Replace("-", "");
            return t2;
        }
        public static string MD5(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public enum CrypterOut
        {
            Base64,
            Hex
        }
        public enum KeyBit
        {
            Bit128,
            Bit256
        }
        #region 旧的des加解密码
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
        public static string Decrypt(string encrypted, string key, CrypterOut o = CrypterOut.Hex)
        {
            byte[] buffer = null;
            if (o == CrypterOut.Base64)
            {
                buffer = Convert.FromBase64String(encrypted);
            }
            else
            {
                buffer = Utility.strToToHexByte(encrypted);
            }

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
        public static string Encrypt(string original, string key, CrypterOut o = CrypterOut.Hex)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(original);
            byte[] buffer2 = ASCIIEncoding.ASCII.GetBytes(key);
            if (o == CrypterOut.Base64)
            {
                return Convert.ToBase64String(Encrypt(bytes, buffer2));
            }
            else
            {
                return Utility.byteToHexStr(Encrypt(bytes, buffer2));
            }
        }
        #endregion

        public static byte[] DesEncrypt(byte[] original, byte[] key, CipherMode mode, PaddingMode padd)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Mode = mode;
            provider.Padding = padd;
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
        public static string DesEncrypt(string original, string key, CrypterOut o = CrypterOut.Base64)
        {
            return DesEncrypt(original, key, Encoding.UTF8, CipherMode.ECB, PaddingMode.Zeros, o);
        }
        public static string DesEncrypt(string original, string key, Encoding encoding, CipherMode mode, PaddingMode padd, CrypterOut o = CrypterOut.Base64)
        {
            byte[] bytes = encoding.GetBytes(original);
            byte[] buffer2 = encoding.GetBytes(key);
            if (o == CrypterOut.Base64)
            {
                return Convert.ToBase64String(DesEncrypt(bytes, buffer2, mode, padd));
            }
            else
            {
                return Utility.byteToHexStr(DesEncrypt(bytes, buffer2, mode, padd));
            }
        }

        public static byte[] DesDecrypt(byte[] encrypted, byte[] key, CipherMode mode, PaddingMode padd)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Mode = mode;
            provider.Padding = padd;
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
        public static string DesDecrypt(string encrypted, string key, CrypterOut o = CrypterOut.Base64)
        {
            return DesDecrypt(encrypted, key, Encoding.UTF8, CipherMode.ECB, PaddingMode.Zeros, o);
        }
        public static string DesDecrypt(string encrypted, string key, Encoding encoding, CipherMode mode, PaddingMode padd, CrypterOut o = CrypterOut.Base64)
        {
            byte[] buffer = null;
            if (o == CrypterOut.Base64)
            {
                buffer = Convert.FromBase64String(encrypted);
            }
            else
            {
                buffer = Utility.strToToHexByte(encrypted);
            }

            byte[] bytes = encoding.GetBytes(key);
            return encoding.GetString(DesDecrypt(buffer, bytes, mode, padd));
        }


        /// <summary>AES加密</summary>  
        /// <param name="text">明文</param>  
        /// <param name="key">密钥,长度为16的字符串</param>  
        /// <param name="iv">偏移量,长度为16的字符串</param>  
        /// <param name="keySize">密钥长度</param> 
        /// <returns>密文</returns>  
        public static string EncodeAES(string text, string key, string iv, CrypterOut o = CrypterOut.Hex, KeyBit keySize = KeyBit.Bit128)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.Zeros;
            rijndaelCipher.KeySize = keySize == KeyBit.Bit128 ? 128 : 256;
            rijndaelCipher.BlockSize = 128;
            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] keyBytes;
            if (keySize == KeyBit.Bit128)
            {
                keyBytes = new byte[16];
            }
            else
            {
                keyBytes = new byte[32];
            }
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(text);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
            if (o == CrypterOut.Base64)
            {
                return Convert.ToBase64String(cipherBytes);
            }
            else
            {
                return Utility.byteToHexStr(cipherBytes);
            }
        }

        /// <summary>AES解密</summary>
        /// <param name="text">密文</param>
        /// <param name="key">密钥,长度为16的字符串</param>
        /// <param name="iv">偏移量,长度为16的字符串</param>
        /// <param name="keySize">密钥长度</param>
        /// <returns>明文</returns>  
        public static string DecodeAES(string text, string key, string iv, CrypterOut o = CrypterOut.Hex, KeyBit keySize = KeyBit.Bit128)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.Zeros;
            rijndaelCipher.KeySize = keySize == KeyBit.Bit128 ? 128 : 256;
            rijndaelCipher.BlockSize = 128;
            byte[] encryptedData = null;
            if (o == CrypterOut.Base64)
            {
                encryptedData = Convert.FromBase64String(text);
            }
            else
            {
                encryptedData = Utility.strToToHexByte(text);
            }
            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] keyBytes;
            if (keySize == KeyBit.Bit128)
            {
                keyBytes = new byte[16];
            }
            else
            {
                keyBytes = new byte[32];
            }
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;

            rijndaelCipher.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }

        /// <summary>
        /// 汉字转换为Unicode编码
        /// </summary>
        /// <param name="str">要编码的汉字字符串</param>
        /// <returns>Unicode编码的的字符串</returns>
        public static string ToUnicode(string str)
        {
            byte[] bts = Encoding.Unicode.GetBytes(str);
            string r = "";
            for (int i = 0; i < bts.Length; i += 2) r += "\\u" + bts[i + 1].ToString("x").PadLeft(2, '0') + bts[i].ToString("x").PadLeft(2, '0');
            return r;
        }
        /// <summary>
        /// 将Unicode编码转换为汉字字符串
        /// </summary>
        /// <param name="str">Unicode编码字符串</param>
        /// <returns>汉字字符串</returns>
        public static string ToGB2312(string str)
        {
            string r = "";
            MatchCollection mc = Regex.Matches(str, @"\\u([\w]{2})([\w]{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            byte[] bts = new byte[2];
            foreach (Match m in mc)
            {
                bts[0] = (byte)int.Parse(m.Groups[2].Value, System.Globalization.NumberStyles.HexNumber);
                bts[1] = (byte)int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
                r += Encoding.Unicode.GetString(bts);
            }
            return r;
        }

        ///base64编码
        public static string EncodeBase64(string code, Encoding encoding)
        {
            string encode = "";
            byte[] bytes = encoding.GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        ///base64解码
        public static string DecodeBase64(string code, Encoding encoding)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = encoding.GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
    }
}
