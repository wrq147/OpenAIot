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
    [TableName("t_prod_material")]
    public class T_Prod_Material : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long OrgId { get; set; }

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
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }

    }

    public class In_Prod_Material
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long OrgId { get; set; }

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
        /// 供应商列表
        /// </summary>
        public List<T_Prod_ProviderMaterial> ProviderMaterials { get; set; }
    }

    public class Out_Material : T_Prod_Material
    {
        /// <summary>
        /// 供应商数量
        /// </summary>
       public int ProviderCount { get; set; }

        /// <summary>
        /// 供应商列表
        /// </summary>
        public List<Out_Prod_Provider> ProviderMaterials { get; set; }

    }

    public class Inpotr_Material
    {
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
        /// 供应商代码
        /// </summary>
        public string ProviderCode { get; set; }

        /// <summary>
        /// 供应商规格型号
        /// </summary>
        public string ProductModel { get; set; }
    }
}
