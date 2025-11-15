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
    [TableName("t_com_process")]
    public class T_Com_Process
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 模型编码
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// 环节编码
        /// </summary>
        public string LinkId { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 工序序号
        /// </summary>
        public string ProcessNo { get; set; }
    }
}
