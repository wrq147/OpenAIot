using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AuthService
{
    [TableName("mz_dept")]
    public class MZ_Dept : BaseEntity
    {

        /// <summary>
        /// 部门ID
        /// </summary>
        [JsonPropertyName("deptId")]
        [ID(false)]
        public long? dept_id { get; set; }
        /// <summary>
        /// 父部门ID
        /// </summary>
        [JsonPropertyName("parentId")]
        public long? parent_id { get; set; }
        /// <summary>
        /// 祖级列表
        /// </summary>
        public string ancestors { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [JsonPropertyName("deptName")]
        public string dept_name { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        [JsonPropertyName("orderNum")]
        public int? order_num { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 部门状态:0正常,1停用
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 部门所属组织
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        [JsonIgnore]
        public string del_flag { get; set; }
        /// <summary>
        /// 父部门名称
        /// </summary>
        [DataIgnore]
        public string parentName { get; set; }
        /// <summary>
        /// 子部门
        /// </summary>
        [DataIgnore]
        public List<MZ_Dept> children { get; set; }

        /// <summary>
        /// 部门树转选择树
        /// </summary>
        /// <param name="deptTree"></param>
        /// <returns></returns>
        public static List<TreeSelect<long>> DeptList2Tree(List<MZ_Dept> deptTree)
        {
            List<TreeSelect<long>> treeList = new List<TreeSelect<long>>();
            foreach (MZ_Dept dept in deptTree)
            {
                TreeSelect<long> ts = new TreeSelect<long>();
                ts.parentId = dept.parent_id.Value;
                ts.id = dept.dept_id.Value;
                ts.label = dept.dept_name;
                if (dept.children != null)
                {
                    ts.children = DeptList2Tree(dept.children);
                }
                treeList.Add(ts);
            }
            return treeList;
        }
    }
}
