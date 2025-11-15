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
    [TableName("t_prod_providermaterial")]
    public class T_Prod_ProviderMaterial
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// 供应商编码
        /// </summary>
        public string ProviderId { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        public string ProductModel { get; set; }
    }
}
