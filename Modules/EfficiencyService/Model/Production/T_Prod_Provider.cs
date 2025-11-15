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
    [TableName("t_prod_provider")]
    public class T_Prod_Provider: BaseEntity
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
        /// 联系人
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 所属区域
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
    }

    public class T_Prod_Provider2 : T_Prod_Provider
    {
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string District { get; set; }
    }

    public class In_Prod_Provider
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
        /// 联系人
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 所属区域
        /// </summary>
        public string Area { get; set; }
    }

    public class In_Prod_Provider1
    {
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
        /// 联系人
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 所属区域
        /// </summary>
        public string Area { get; set; }
    }

    public class Out_Prod_Provider
    {
        /// <summary>
        /// 供应商编码
        /// </summary>
        public string ProviderId { get; set; }
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
    }
}
