using AuthService;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    [TableName("mz_leave_apply")]
    public class MZ_LeaveApply
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 申请单唯一编号
        /// </summary>
        public string ApplyNumber { get; set; }
        /// <summary>
        /// 出库仓库
        /// </summary>
        public string HouseId { get; set; }
        /// <summary>
        /// 出库仓库实体
        /// </summary>
        [DataIgnore]
        public MZ_StoreHouse House { get; set; }
        /// <summary>
        /// 申请类型,参考字典apply_type
        /// </summary>
        public string ApplyType { get; set; }
        /// <summary>
        /// 关联的申请类型工单
        /// </summary>
        public string ApplyWorkId { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public long? ApplyUserId { get; set; }
        /// <summary>
        /// 申请人信息
        /// </summary>
        [DataIgnore]
        public MZ_AdminInfo ApplyUserInfo { get; set; }
        /// <summary>
        /// 申请部门
        /// </summary>
        public long? ApplyDeptId { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime? ApplyOn { get; set; }
        /// <summary>
        /// 申请原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 关联的流程Id
        /// </summary>
        public long? FlowId { get; set; }
        /// <summary>
        /// 提交状态：0、待提交；1、待审批；2、申请成功；3、申请失败；4、已取消;5、待出库;6、已出库
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 0、待出库;1、出库中;2、已出库;3、出库失败
        /// </summary>
        public int? OutStatus { get; set; }

        /// <summary>
        /// 出库申请单详情列表
        /// </summary>
        [DataIgnore]
        public List<MZ_LeaveApplyDetail> List { get; set; }
    }
}
