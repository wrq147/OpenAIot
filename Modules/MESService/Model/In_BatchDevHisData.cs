using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    public class In_BatchDevHisData
    {
        /// <summary>
        /// 批次编号
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 关联的工序Id
        /// </summary>
        public string OperId { get; set; }
        /// <summary>
        /// 数据采集的历史数据列表
        /// </summary>
        public List<BatchHisItem> Items { get; set; }
    }

    public class BatchHisItem
    {
        /// <summary>
        /// 采集的属性名称
        /// </summary>
        public string PropName { get; set; }
        /// <summary>
        /// 采集的属性标识符
        /// </summary>
        public string PropCode { get; set; }
        /// <summary>
        /// 采集的属性类型
        /// </summary>
        public string PropType { get; set; }
        /// <summary>
        /// 采集的属性值
        /// </summary>
        public object PropVal { get; set; }
    }
}
