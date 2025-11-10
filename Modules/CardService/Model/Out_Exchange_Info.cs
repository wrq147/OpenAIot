using System;
namespace CardService.Model
{
    /// <summary>
    /// 指定名片的交换信息
    /// </summary>
    public class Out_Exchange_Info
    {
        /// <summary>
        /// 是否已递名片（已互换名片、互申请中为true）
        /// </summary>
        public bool Exchanging { get; set; }
        /// <summary>
        /// 是否已在通讯录中
        /// </summary>
        public bool InHolder { get; set; }
    }
}
