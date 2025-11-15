using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Share;

namespace EfficiencyService.Model
{
    public class In_Provider : BaseQueryParam
    {
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// 供应商代码
        /// </summary>
        public string ProviderCode { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// 所属区域
        /// </summary>
        public string Area { get; set; }
    }

    public class In_Material : BaseQueryParam
    {
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 物料代码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        public string MaterialType { get; set; }

        /// <summary>
        /// 物料单位
        /// </summary>
        public string MaterialUnit { get; set; }
    }
}
