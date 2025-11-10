using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class In_AllItem
    {
        /// <summary>
        /// 搜索设备、耗材名称或唯一编号
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 过滤所在仓库
        /// </summary>
        public string HouseId { get; set; }
        /// <summary>
        /// 过滤盘点单
        /// </summary>
        public string InventId { get; set; }
    }
}
