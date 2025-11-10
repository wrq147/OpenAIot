using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class In_StockRecordPage : BaseQueryParam
    {
        /// <summary>
        /// 仓库
        /// </summary>
        public string HouseId { get; set; }
        /// <summary>
        /// 存储类型：0半成品、1成品
        /// </summary>
        public int TargetType { get; set; }
        /// <summary>
        /// 目标物品Id
        /// </summary>
        public string TargetId { get; set; }
    }
}
