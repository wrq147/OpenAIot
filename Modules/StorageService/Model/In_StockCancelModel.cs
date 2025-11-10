using FlowService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class In_StockCancelModel
    {
        /// <summary>
        /// 要撤销的入库单Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 出库单编号
        /// </summary>
        public string StockNumber { get; set; }
        /// <summary>
        /// 物流单号
        /// </summary>
        public string ExpressNumber { get; set; }
        /// <summary>
        /// 物流公司
        /// </summary>
        public string ExpressCompany { get; set; }
        /// <summary>
        /// 顺风用联系电话
        /// </summary>
        public string ExpressPhone { get; set; }
        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime? OutDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 初始化表单
        /// </summary>
        public Dictionary<string, object> model { get; set; }
        /// <summary>
        /// 自选人
        /// </summary>
        public Dictionary<string, List<Out_UserItem>> assign { get; set; }
        /// <summary>
        /// 要编辑的工作流
        /// </summary>
        public long flowId { get; set; }
    }
}
