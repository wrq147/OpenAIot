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
    [TableName("mz_defect_type")]
    public class MZ_DefectType
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
        /// 不良品项名称
        /// </summary>
        public string DefectName { get; set; }
        /// <summary>
        /// 不良类别(外观/功能/性能/其它等)
        /// </summary>
        public string DefectCategory { get; set; }
    }
}
