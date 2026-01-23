using Common.Attr;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    [TableName("mz_work_defect")]
    public class MZ_WorkDefect
    {
        /// <summary>
        /// Id编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [OnlySeriaize]
        public long? OrgId { get; set; }
        /// <summary>
        /// 关联的工单Id
        /// </summary>
        public string WorkOrderId { get; set; }
        /// <summary>
        /// 关联的任务Id
        /// </summary>
        public string WorkTaskId { get; set; }
        /// <summary>
        /// 关联的工序Id
        /// </summary>
        public string OperId { get; set; }
        /// <summary>
        /// 报工Id
        /// </summary>
        public string ReportId { get; set; }
        /// <summary>
        /// 不良品编号
        /// </summary>
        public string DefectId { get; set; }
        /// <summary>
        /// 不良品项名称
        /// </summary>
        public string DefectName { get; set; }
        /// <summary>
        /// 不良类别(外观/功能/性能/其它等)
        /// </summary>
        public string DefectCategory { get; set; }
        /// <summary>
        /// 不良品数
        /// </summary>
        public decimal? DefectNum { get; set; }
    }
}
