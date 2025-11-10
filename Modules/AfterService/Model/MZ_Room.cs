using AuthService;
using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;

namespace AfterService.Model
{
    [TableName("mz_room")]
    public class MZ_Room : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 房间名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 房间所属企业Id，为0则无所属
        /// </summary>
        public long? TargetOrgId { get; set; }
        /// <summary>
        /// 房间所属企业名称
        /// </summary>
        [DataIgnore]
        public string TargetName { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public long? DeptId { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public long? LeaderId { get; set; }
        /// <summary>
        /// 协作者（多个,号分隔）
        /// </summary>
        public string Helper { get; set; }
        /// <summary>
        /// 所属分类
        /// </summary>
        public string CategoryId { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        [DataIgnore]
        public string CategoryName { get; set; }
        /// <summary>
        /// 排序值：越小越前面
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否自动从关联客户或负责人添加设备到房间
        /// </summary>
        public bool? AutoAdd { get; set; }
        /// <summary>
        /// 房间的监控报表
        /// </summary>
        public string ReportToken { get; set; }
        /// <summary>
        /// 负责人信息
        /// </summary>
        [DataIgnore]
        public MZ_AdminInfo LeaderInfo { get; set; }
        /// <summary>
        /// 所属企业信息
        /// </summary>
        [DataIgnore]
        public MZ_Org OrgInfo { get; set; }
        /// <summary>
        /// 目标企业信息
        /// </summary>
        [DataIgnore]
        public MZ_Org TargetOrgInfo { get; set; }
        /// <summary>
        /// 协作人信息
        /// </summary>
        [DataIgnore]
        public List<MZ_AdminInfo> HelperUsers { get; set; }
    }
}
