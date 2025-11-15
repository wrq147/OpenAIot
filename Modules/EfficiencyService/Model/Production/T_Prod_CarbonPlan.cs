using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyAccess.DB.Attr;
using Common.Share;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;

namespace EfficiencyService.Model
{
    [TableName("t_prod_carbonplan")]
    public class T_Prod_CarbonPlan
    {
        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long OrgId { get; set; }

        /// <summary>
        /// 碳排数据
        /// </summary>
        public string CarbonType { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 数据粒度
        /// </summary>
        public string Granularity { get; set; }

        /// <summary>
        /// 1月
        /// </summary>
        public double Month1 { get; set; }
        /// <summary>
        /// 2月
        /// </summary>
        public double Month2 { get; set; }
        /// <summary>
        /// 3月
        /// </summary>
        public double Month3 { get; set; }
        /// <summary>
        /// 4月
        /// </summary>
        public double Month4 { get; set; }
        /// <summary>
        /// 5月
        /// </summary>
        public double Month5 { get; set; }
        /// <summary>
        /// 6月
        /// </summary>
        public double Month6 { get; set; }
        /// <summary>
        /// 7月
        /// </summary>
        public double Month7 { get; set; }
        /// <summary>
        /// 8月
        /// </summary>
        public double Month8 { get; set; }
        /// <summary>
        /// 9月
        /// </summary>
        public double Month9 { get; set; }
        /// <summary>
        /// 10月
        /// </summary>
        public double Month10 { get; set; }
        /// <summary>
        /// 11月
        /// </summary>
        public double Month11 { get; set; }
        /// <summary>
        /// 12月
        /// </summary>
        public double Month12 { get; set; }
        /// <summary>
        /// 年度合计
        /// </summary>
        public double YearTotal { get; set; }
    }

    public class In_Prod_CarbonPlan
    {
        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long OrgId { get; set; }
    }
    public class In_CarbonPlan
    {
        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long OrgId { get; set; }

        /// <summary>
        /// 明细
        /// </summary>
        public List<In_CarbonPlanDetail> Details { get; set; }
    }
    public class In_CarbonPlanDetail
    {
        /// <summary>
        /// 碳排数据
        /// </summary>
        public string CarbonType { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 数据粒度
        /// </summary>
        public string Granularity { get; set; }

        /// <summary>
        /// 1月
        /// </summary>
        public double Month1 { get; set; }
        /// <summary>
        /// 2月
        /// </summary>
        public double Month2 { get; set; }
        /// <summary>
        /// 3月
        /// </summary>
        public double Month3 { get; set; }
        /// <summary>
        /// 4月
        /// </summary>
        public double Month4 { get; set; }
        /// <summary>
        /// 5月
        /// </summary>
        public double Month5 { get; set; }
        /// <summary>
        /// 6月
        /// </summary>
        public double Month6 { get; set; }
        /// <summary>
        /// 7月
        /// </summary>
        public double Month7 { get; set; }
        /// <summary>
        /// 8月
        /// </summary>
        public double Month8 { get; set; }
        /// <summary>
        /// 9月
        /// </summary>
        public double Month9 { get; set; }
        /// <summary>
        /// 10月
        /// </summary>
        public double Month10 { get; set; }
        /// <summary>
        /// 11月
        /// </summary>
        public double Month11 { get; set; }
        /// <summary>
        /// 12月
        /// </summary>
        public double Month12 { get; set; }
        /// <summary>
        /// 年度合计
        /// </summary>
        public double YearTotal { get; set; }
    }

    public class In_CarbonAsset
    {
        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long OrgId { get; set; }

        /// <summary>
        /// 明细
        /// </summary>
        public List<In_CarbonPlanDetail> Details { get; set; }

        /// <summary>
        /// 1月碳排放量
        /// </summary>
        public double CarbonEmission1 { get; set; }

        /// <summary>
        /// 1月碳排放量
        /// </summary>
        public double CarbonEmission2 { get; set; }

        /// <summary>
        /// 3月碳排放量
        /// </summary>
        public double CarbonEmission3 { get; set; }

        /// <summary>
        /// 4月碳排放量
        /// </summary>
        public double CarbonEmission4 { get; set; }

        /// <summary>
        /// 5月碳排放量
        /// </summary>
        public double CarbonEmission5 { get; set; }

        /// <summary>
        /// 6月碳排放量
        /// </summary>
        public double CarbonEmission6 { get; set; }

        /// <summary>
        /// 7月碳排放量
        /// </summary>
        public double CarbonEmission7 { get; set; }

        /// <summary>
        /// 8月碳排放量
        /// </summary>
        public double CarbonEmission8 { get; set; }

        /// <summary>
        /// 9月碳排放量
        /// </summary>
        public double CarbonEmission9 { get; set; }

        /// <summary>
        /// 10月碳排放量
        /// </summary>
        public double CarbonEmission10 { get; set; }

        /// <summary>
        /// 11月碳排放量
        /// </summary>
        public double CarbonEmission11 { get; set; }

        /// <summary>
        /// 12月碳排放量
        /// </summary>
        public double CarbonEmission12 { get; set; }

        /// <summary>
        /// 年度碳排放量
        /// </summary>
        public double CarbonEmissionTotal { get; set; }
    }
}
