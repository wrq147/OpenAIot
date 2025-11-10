using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class In_InventoryList : BaseQueryParam
    {
        /// <summary>
        /// 盘点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 盘点状态：0、待提交；1、待开始；2、初盘中；3、复盘中；4、已结束；5、已修正；6、已取消；
        /// </summary>
        public int? Status { get; set; }
    }
}
