using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinService.Model
{
    /// <summary>
    /// 企业微信同步任务表
    /// </summary>
    [TableName("mz_corp_task")]
    public class MZ_CorpTask
    {
        [ID(false)]
        public string TaskId { get; set; }
        /// <summary>
        /// 企业微信的AppId
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 是否完成更新部门
        /// </summary>
        public bool? IsUpdateDept { get; set; }
        /// <summary>
        /// 是否完成新增部门
        /// </summary>
        public bool? IsAddDept { get; set; }
        /// <summary>
        /// 是否完成移动部门
        /// </summary>
        public bool? IsMoveDept { get; set; }
        /// <summary>
        /// 是否完成删除部门
        /// </summary>
        public bool? IsDelDept { get; set; }
        /// <summary>
        /// 是否完成更新人员
        /// </summary>
        public bool? IsUpdateMem { get; set; }
        /// <summary>
        /// 是否完成新增人员
        /// </summary>
        public bool? IsAddMem { get; set; }
        /// <summary>
        /// 是否完成删除人员
        /// </summary>
        public bool? IsDelMem { get; set; }
        /// <summary>
        /// 更新部门失败信息
        /// </summary>
        public string UpdateDeptErr { get; set; }
        /// <summary>
        /// 新增部门失败信息
        /// </summary>
        public string AddDeptErr { get; set; }
        /// <summary>
        /// 移动部门失败信息
        /// </summary>
        public string MoveDeptErr { get; set; }
        /// <summary>
        /// 删除部门失败信息
        /// </summary>
        public string DelDeptErr { get; set; }
        /// <summary>
        /// 更新人员失败信息
        /// </summary>
        public string UpdateMemErr { get; set; }
        /// <summary>
        /// 新增人员失败信息
        /// </summary>
        public string AddMemErr { get; set; }
        /// <summary>
        /// 删除人员失败信息
        /// </summary>
        public string DelMemErr { get; set; }
        /// <summary>
        /// 0为同步中，1为已完成
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
    }
}
