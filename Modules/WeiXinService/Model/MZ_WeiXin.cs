using MyAccess.DB.Attr;
using System;

namespace WeiXinService.Model
{
    [TableName("mz_weixin")]
    public class MZ_WeiXin
    {
        /// <summary>
        /// 微信全局Id
        /// </summary>
        public string UnionId { get; set; }
        /// <summary>
        /// 对应的AppId
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 微信OpenId
        /// </summary>
        public string OpenId { get; set; }

    }
}
