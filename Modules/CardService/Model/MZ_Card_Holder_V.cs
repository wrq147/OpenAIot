using System;

namespace CardService.Model
{
    /// <summary>
    /// 列表通讯录视图
    /// </summary>
    public class MZ_Card_Holder_V : MZ_Card
    {
        /// <summary>
        /// 加入时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
    }
}
