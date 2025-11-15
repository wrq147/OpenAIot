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
    [TableName("t_com_class")]
    public class T_Com_Class : BaseEntity
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
        /// 排放类别序号
        /// </summary>
        public string ClassNo { get; set; }

        /// <summary>
        /// 排放类别名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 排放范围（1代表范围1 2代表范围2 3代表范围3）
        /// </summary>
        public string RangeId { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
    }

    public class In_SubClass
    {
        /// <summary>
        /// 编码，新增为空
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 子排放类别名称
        /// </summary>
        public string SubClassName { get; set; }
    }
    public class Out_Class : T_Com_Class
    {
        /// <summary>
        /// 自排放类型名称集合
        /// </summary>
        public string SubClassName { get; set; }


        /// <summary>
        /// 子排放类型
        /// </summary>
        public List<T_Com_SubClass> SubClass { get; set; }
    }

    public class In_Class
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
        /// 排放类别序号
        /// </summary>
        public string ClassNo { get; set; }

        /// <summary>
        /// 排放类别名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 排放范围（1代表范围1 2代表范围2 3代表范围3）
        /// </summary>
        public string RangeId { get; set; }

        /// <summary>
        /// 子排放类型
        /// </summary>
        public List<In_SubClass> SubClass { get; set; }
    }
}
