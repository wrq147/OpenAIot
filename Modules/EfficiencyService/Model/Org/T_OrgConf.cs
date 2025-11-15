using MyAccess.DB.Attr;
using NPOI.OpenXmlFormats.Dml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model.Org
{
    [TableName("t_effic_org_conf")]
    public class T_OrgConf
    {
        /// <summary>
        /// 企业ID
        /// </summary>
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 电总有功电能标识符
        /// </summary>
        public string ElecCode { get; set; }
        /// <summary>
        /// 电总有功功率标识符
        /// </summary>
        public string ElecPowerCode { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string ContectName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContectTel { get; set; }
        /// <summary>
        /// 电价政策Id
        /// </summary>
        public string PricePolicyId { get; set; }
        /// <summary>
        /// 存放排放源与因子json,格式{"Id":"排放源Id","Name":"排放源名称","FactorId":"排放因子Id"}
        /// </summary>
        public string EmissionSourceJson { get; set; }

    }

    public class EmissionItem
    {
        /// <summary>
        /// 排放源Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 排放源名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排放因子Id
        /// </summary>
        public string FactorId { get; set; }
    }
}
