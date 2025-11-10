using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    [TableName("mz_factory_mes")]
    public class MZ_FactoryMes
    {
        /// <summary>
        /// Id编号
        /// </summary>
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 生产计划审核模板
        /// </summary>
        public long? PlanTemplateId { get; set; }
        /// <summary>
        /// 生产计划审核模板名称
        /// </summary>
        [DataIgnore]
        public string PlanTemplateName { get; set; }
        /// <summary>
        /// 报工审核模板
        /// </summary>
        public long? ReportTemplateId { get; set; }
        /// <summary>
        /// 报工审核模板名称
        /// </summary>
        [DataIgnore]
        public string ReportTemplateName { get; set; }
        /// <summary>
        /// 生产计划流程初始化json
        /// </summary>
        public string PlanFlowInitJson { get; set; }
        /// <summary>
        /// 报工流程初始化json
        /// </summary>
        public string ReportFlowInitJson { get; set; }
    }
}
