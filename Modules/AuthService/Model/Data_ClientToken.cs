using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService
{
    /// <summary>
    /// 客户端令牌
    /// </summary>
    public class Data_ClientToken
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public long Time { get; set; }
        /// <summary>
        /// 令牌身份认证模式
        /// </summary>
        public TokenMode Mode { get; set; }
        /// <summary>
        /// 扩展数据
        /// </summary>
        public string Ext { get; set; }
        /// <summary>
        /// 签名数据
        /// </summary>
        public string Sign { get; set; }


        /// <summary>
        /// 生成客户端令牌
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="time"></param>
        /// <param name="tk"></param>
        /// <param name="mode"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string MakeClientToken(long uid, long time, string tk, TokenMode mode, string ext = "")
        {
            Data_ClientToken info = new Data_ClientToken();
            info.UserId = uid;
            info.Time = time;
            info.Mode = mode;
            info.Ext = ext;
            StringBuilder sb = new StringBuilder(200);
            sb.Append(info.UserId);
            sb.Append(info.Time);
            sb.Append(mode);
            sb.Append(ext);
            sb.Append(tk);
            info.Sign = MyAccess.Core.Crypter.SHA1(sb.ToString(), System.Text.Encoding.UTF8);
            return MyAccess.Core.Crypter.EncodeBase64(Newtonsoft.Json.JsonConvert.SerializeObject(info), System.Text.Encoding.UTF8);
        }
    }
    public enum TokenMode
    {
        Simple,//简单模式（无刷新令牌）
        Authorization,//授权码模式
        Share//分享模式
    }
}
