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
    [TableName("t_com_facility")]
    public class T_Com_Facility : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 设施代码
        /// </summary>
        public string FacilityCode { get; set; }

        /// <summary>
        /// 设施名称
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 设施分类（False代表目录级 Ture代表绑定设备级）
        /// </summary>
        public bool FacilityType { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
    }
}
