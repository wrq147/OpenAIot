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
    [TableName("t_com_processitem")]
    public class T_Com_ProcessItem
    {
        /// <summary>
        /// 模型编码
        /// </summary>
        public string ModelId { get; set; }
        /// <summary>
        /// 工序编码
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 包含子设施编码
        /// </summary>
        public string FacilityIds { get; set; }
    }
}
