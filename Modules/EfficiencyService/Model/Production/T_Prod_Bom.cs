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
    [TableName("t_prod_bom")]
    public class T_Prod_Bom
    {
        /// <summary>
        /// 模型编码
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// 产品碳足迹编码
        /// </summary>
        public string MaterialCarbonId { get; set; }

        /// <summary>
        /// 环节
        /// </summary>
        public string LinkId { get; set; }

        /// <summary>
        /// 用量
        /// </summary>
        public int Dosage { get; set; }

        /// <summary>
        /// 物料清单类型
        /// </summary>
        public string BomType { get; set; }
    }

    public class Out_Bom
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string LinkId { get; set; }

        /// <summary>
        /// 物料清单类型
        /// </summary>
        public string BomType { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialId { get; set; }

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

        /// <summary>
        /// 供应商编码
        /// </summary>
        public string ProviderId { get; set; }

        /// <summary>
        /// 产品生命周期边界
        /// </summary>
        public string ProductBorder { get; set; }

        /// <summary>
        /// 碳排放量
        /// </summary>
        public double CarbonEmission { get; set; }

        /// <summary>
        /// 碳排放单位
        /// </summary>
        public string CarbonUnit { get; set; }

        /// <summary>
        /// 供应商代码
        /// </summary>
        public string ProviderCode { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// 供应商地址
        /// </summary>
        public string ProviderAddress { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 用量
        /// </summary>
        public int Dosage { get; set; }

    }

    public class In_Bom : BaseQueryParam
    {
        /// <summary>
        /// 模型编码
        /// </summary>
        public string ModelId { get; set; }

    }
}
