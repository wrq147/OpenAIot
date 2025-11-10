using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class In_LeaveList : BaseQueryParam
    {
        /// <summary>
        /// 过滤所出仓库
        /// </summary>
        public string FromHouseId { get; set; }
        /// <summary>
        /// 提交状态：-1、全部；0、待提交；1、待审批；2、出库成功；3、出库失败；
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 搜索出库单编号、物品名称
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 目标企业名
        /// </summary>
        public string ToCompany { get; set; }
        /// <summary>
        /// 出库方式：0、出货，1、退货，2、调拨
        /// </summary>
        public int? LeaveMethod { get; set; }
        /// <summary>
        /// 是否显示出库单的物品列表
        /// </summary>
        public bool? ShowItems { get; set; }
    }
}
