using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class Out_StockRecord : MZ_StockRecord
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string StoreName { get; set; }
    }
}
