using Common.Attr;
using Common.Share;
using FluentMigrator.Infrastructure;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;

namespace StorageService.Model
{
    /// <summary>
    /// 出库单表
    /// </summary>
    [TableName("mz_leave_stock")]
    public class MZ_LeaveStock : BaseEntity
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
        /// 目标企业Id（必传）
        /// </summary>
        public long? ToOrgId { get; set; }
        /// <summary>
        /// 对应的客户Id（不用传）
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// 客户类型：0为代理，1为直销
        /// </summary>
        public int? CustomerType { get; set; }
        /// <summary>
        /// 关联的入库单、领用申请单
        /// </summary>
        public string SourceEnterId { get; set; }
        /// <summary>
        /// 出库单唯一编号
        /// </summary>
        public string StockNumber { get; set; }
        /// <summary>
        /// 所出仓库
        /// </summary>
        public string FromHouseId { get; set; }
        /// <summary>
        /// 所入仓库（不用传）
        /// </summary>
        public string ToHouseId { get; set; }
        /// <summary>
        /// 出库方式：0、出货，1、退货，2、调拨，3、领用
        /// </summary>
        public int? LeaveMethod { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string ExpressNumber { get; set; }
        /// <summary>
        /// 物流公司
        /// </summary>
        public string ExpressCompany { get; set; }
        /// <summary>
        /// 顺风用联系电话
        /// </summary>
        public string ExpressPhone { get; set; }
        /// <summary>
        /// 出库时间
        /// </summary>
        public DateTime? OutDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 提交状态：0、待提交；1、待审批；2、出库成功；3、出库失败；4、已取消
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 关联的流程Id
        /// </summary>
        public long? FlowId { get; set; }
        /// <summary>
        /// 出库单详情列表
        /// </summary>
        [DataIgnore]
        public List<MZ_LeaveDetail> List { get; set; }
        /// <summary>
        /// 出库目标企业名称
        /// </summary>
        [DataIgnore]
        public string ToName { get; set; }
        /// <summary>
        /// 出库目标客户名称
        /// </summary>
        [DataIgnore]
        public string CustomerName { get; set; }
        /// <summary>
        /// 所出仓库名称
        /// </summary>
        [DataIgnore]
        public string FromHouseName { get; set; }

        /// <summary>
        /// 所出仓库审核模板Id
        /// </summary>
        [DataIgnore]
        public long? FromHouseLeaveTemplateId { get; set; }
        /// <summary>
        /// 所入仓库名称
        /// </summary>
        [DataIgnore]
        public string ToHouseName { get; set; }
        /// <summary>
        /// 所入仓库审核模板Id
        /// </summary>
        [DataIgnore]
        public long? ToHouseEnterTemplateId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [DataIgnore]
        public string Creater { get; set; }


    }
}
