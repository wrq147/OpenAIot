using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    /// <summary>
    /// 生产批次数据采集的历史数据表
    /// </summary>
    public class MZ_BatchDevHis
    {
        /// <summary>
        /// Id编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [JsonConverter(typeof(OnlySeriaize))]
        public long? OrgId { get; set; }
        /// <summary>
        /// 批次编号
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 关联的工序Id
        /// </summary>
        public string OperId { get; set; }
        /// <summary>
        /// 关联的工序名称
        /// </summary>
        [DataIgnore]
        public string OperName { get; set; }
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
        /// 采集的数值型属性值
        /// </summary>
        public double? PropVal { get; set; }
        /// <summary>
        /// 采集的字符串属性值
        /// </summary>
        public string PropStrVal { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
    }
}
