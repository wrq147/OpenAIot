using AuthService.Fields;
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
    /// 生产批次
    /// </summary>
    [TableName("mz_work_batch")]
    public class MZ_WorkBatch : IFieldEntity
    {
        /// <summary>
        /// 批次编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [ID(false)]
        [JsonConverter(typeof(OnlySeriaize))]
        public long? OrgId { get; set; }
        /// <summary>
        /// 通讯编号
        /// </summary>
        public string LNumber { get; set; }

        /// <summary>
        /// 关联的工单Id
        /// </summary>
        public string WorkOrderId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
        /// <summary>
        /// 生产是否完成
        /// </summary>
        public bool? IsFinish { get; set; }
        /// <summary>
        /// 扩展的关联对象
        /// </summary>
        [DataIgnore]
        [JsonConverter(typeof(OnlySeriaize))]
        public Dictionary<string, object> ExtObjects { get; set; }
        [DataIgnore]
        public Dictionary<string, object> ExtVals { get; set; }

        public string GetFormId()
        {
            return this.Id;
        }

        public string GetFormName()
        {
            return "报工";
        }
    }
}
