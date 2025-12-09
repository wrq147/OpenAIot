using System;
using System.Text;

namespace ShortLinkService
{
    public class ShortCodeGenerator
    {
        private const string Base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// 数字ID转Base62短链接码
        /// </summary>
        /// <param name="id">自增ID/分布式ID（正数）</param>
        /// <returns>Base62短码（如 1000000 → 4C9）</returns>
        /// <exception cref="ArgumentOutOfRangeException">ID为负数</exception>
        public static string Encode(long id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id), "ID必须为非负数");

            if (id == 0)
                return Base36Chars[0].ToString();

            var sb = new StringBuilder();
            while (id > 0)
            {
                sb.Append(Base36Chars[(int)(id % 36)]);
                id /= 36;
            }

            // 反转字符串（因为编码时是从低位到高位）
            var charArray = sb.ToString().ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
     

    }
}
