using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    [TableName("mz_iot_warn_config")]
    public class MZ_IotWarnConfig
    {
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属物联产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 告警工单执行流程
        /// </summary>
        public long? WarnFlowId { get; set; }
        /// <summary>
        /// 表单初始化映射
        /// </summary>
        public string WarnFlowInitJson { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        [DataIgnore]
        public string WarnFlowName { get; set; }
    }
}
