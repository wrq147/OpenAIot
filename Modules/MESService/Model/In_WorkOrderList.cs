using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    public class In_WorkOrderList : BaseQueryParam
    {
        /// <summary>
        /// 状态：0、待排产；1、待生产；2、生产中；3、已完成；4、已取消
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 过滤父工单
        /// </summary>
        public string ParentId { get; set; }
    }
}
