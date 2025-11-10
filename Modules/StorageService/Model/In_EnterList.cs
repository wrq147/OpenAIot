using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class In_EnterList : BaseQueryParam
    {
        /// <summary>
        /// 过滤所入仓库
        /// </summary>
        public string ToHouseId { get; set; }
        /// <summary>
        /// 提交状态：-1、全部；0、待提交；1、待审批；2、入库成功；3、入库失败；
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 搜索入库单编号、物品名称、来源企业
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 来源企业
        /// </summary>
        public string FromCompany { get; set; }
        /// <summary>
        /// 入库方式：0、出库，1、退货，2、调拨，3、手动，21、所有可退货单
        /// </summary>
        public int? EnterMethod { get; set; }
        /// <summary>
        /// 是否显示入库单的物品列表
        /// </summary>
        public bool? ShowItems { get; set; }
    }
}
