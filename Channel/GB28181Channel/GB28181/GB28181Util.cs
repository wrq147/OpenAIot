using GB28181Channel.GB28181.Enum;
using Org.BouncyCastle.Tls;
using SIPSorcery.SIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel.GB28181
{
    public static class GB28181Util
    {
        /// <summary>
        /// 计算SIP Digest认证的响应摘要（MD5算法，符合GB28181标准）
        /// </summary>
        public static string CalculateDigestResponse(string username, string realm, string password, string method, string uri, string nonce, string cnonce = "", string qop = "", string nc = "")
        {
            // 步骤1：计算HA1 = MD5(username:realm:password)
            var ha1Input = $"{username}:{realm}:{password}";
            var ha1 = MD5Hash(ha1Input);

            // 步骤2：计算HA2 = MD5(method:uri)
            var ha2Input = $"{method}:{uri}";
            var ha2 = MD5Hash(ha2Input);

            // 步骤3：计算响应摘要 = MD5(HA1:nonce:nc:cnonce:qop:HA2)
            string responseInput;
            if (string.IsNullOrEmpty(qop))
            {
                responseInput = $"{ha1}:{nonce}:{ha2}";
            }
            else
            {
                responseInput = $"{ha1}:{nonce}:{nc}:{cnonce}:{qop}:{ha2}";
            }

            return MD5Hash(responseInput);
        }

        /// <summary>
        /// MD5哈希计算（辅助方法）
        /// </summary>
        public static string MD5Hash(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
