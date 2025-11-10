using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 企业的进销存配置
    /// </summary>
    public class MZ_StockConf
    {
        /// <summary>
        /// 出库审核模板Id
        /// </summary>
        public long LeaveTemplateId { get; set; }
        /// <summary>
        /// 入库审核模板Id
        /// </summary>
        public long EnterTemplateId { get; set; }
        /// <summary>
        /// 出库审核模板名称
        /// </summary>
        public string LeaveTemplateName { get; set; }
        /// <summary>
        /// 入库审核模板名称
        /// </summary>
        public string EnterTemplateName { get; set; }

    }
}
