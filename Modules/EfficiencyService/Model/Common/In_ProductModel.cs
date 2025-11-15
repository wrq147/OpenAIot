using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    public class In_ProductModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 模型编码
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// 统计开始时间
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 统计结束时间
        /// </summary>
        public string EndDate { get; set; }
    }
}
