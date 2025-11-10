using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class In_InventoryItem : BaseQueryParam
    {
        /// <summary>
        /// 盘点Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 只显示库存不正常的
        /// </summary>
        public bool? OnlyRevise { get; set; }
        /// <summary>
        /// 只显示未初盘项
        /// </summary>
        public bool? UnFirst { get; set; }
        /// <summary>
        /// 只显示未复盘项
        /// </summary>
        public bool? UnCheck { get; set; }
    }
}
