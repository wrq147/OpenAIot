using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class In_LeaveApplyList : BaseQueryParam
    {
        /// <summary>
        /// 申请单号、物品名称
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 提交状态：0、待提交；1、待审批；2、申请成功；3、申请失败；4、已取消;
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 0、待出库;1、出库中;2、已出库;3、取消出库
        /// </summary>
        public int? OutStatus { get; set; }
        /// <summary>
        /// 过滤出库仓库
        /// </summary>
        public string HouseId { get; set; }
        /// <summary>
        /// 只显示我的
        /// </summary>
        public bool? OnlyMy { get; set; }
        /// <summary>
        /// 是否显示详情列表
        /// </summary>
        public bool? ShowDetail { get; set; }
    }
}
