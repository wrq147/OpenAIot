using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyAccess.DB.Attr;

namespace EfficiencyService.Model
{
    public class In_FacilityAdd
    {

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
        /// false：分组 true：设施
        /// </summary>
        public bool FacilityType { get; set; }
    }

    public class In_FacilityEdit
    {

        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

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

    }
}
