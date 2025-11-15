using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    public class In_Production
    {
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long OrgId { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 统计日
        /// </summary>
        public string DDate { get; set; }

        /// <summary>
        /// 产量
        /// </summary>
        public double OutPut { get; set; }
    }

    public class In_ProductionUpdate
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 产量
        /// </summary>
        public double OutPut { get; set; }
    }
}
