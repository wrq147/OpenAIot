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
    [TableName("t_eng_factoryear")]
    public class T_ENG_FactorYear : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
    }
}
