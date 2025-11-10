using AuthService.Fields;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class In_SupplierList : BaseQueryParam
    {
        /// <summary>
        /// 搜索的关键词（搜索供应商编号、供应商名称、供应商全称）
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 过滤联系人
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 过滤联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 状态：0为停用，1为正常
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 过滤扩展字段
        /// </summary>
        public FieldFilterItem[] Items { get; set; }
    }
}
