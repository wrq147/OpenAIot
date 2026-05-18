using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayService.Model
{
    /// <summary>
    /// 支付渠道表
    /// </summary>
    [TableName("mz_pay_channels")]
    public class MZ_PayChannel
    {
        /// <summary>
        /// 支付渠道ID
        /// </summary>
        [ID]
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id,为0表示系统渠道
        /// </summary>
        public long OrgId { get; set; }

        /// <summary>
        /// 支付渠道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 渠道标识符：支付宝、微信
        /// </summary>
        public string ChannelLabel { get; set; }

        /// <summary>
        /// 商户的appid
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 支付公钥
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// 商户私钥
        /// </summary>
        public string MerchantPrivateKey { get; set; }

        /// <summary>
        /// 其他配置
        /// </summary>
        public string OtherConfig { get; set; }

        /// <summary>
        /// 存储加密的密钥
        /// </summary>
        public string EncryptKey { get; set; }

        /// <summary>
        /// 手续费率，按比例收取
        /// </summary>
        public decimal FeeRate { get; set; }
    }
}
