using System;
using System.Collections.Generic;

namespace ChannelUtility.Tsl
{
    public class BaseProperty : BaseAll
    {
        /// <summary>
        /// 前缀标识符
        /// </summary>
        public string prefixcode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 显示方式：null表示全部显示，空为都不显示，org为来源组织显示，own拥有者组织显示，use使用者组织显示，person个人使用者显示，多个用逗号分隔
        /// </summary>
        public string showway { get; set; }

        public BaseValueOption option { get; set; }
    }
}
