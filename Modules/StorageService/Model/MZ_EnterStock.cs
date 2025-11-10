using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 入库单实体
    /// </summary>
    [TableName("mz_enter_stock")]
    public class MZ_EnterStock : BaseEntity
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
        /// 来源企业Id
        /// </summary>
        public long? FromOrgId { get; set; }
        /// <summary>
        /// 入库单唯一编号
        /// </summary>
        public string StockNumber { get; set; }
        /// <summary>
        /// 所出仓库
        /// </summary>
        public string FromHouseId { get; set; }
        /// <summary>
        /// 所入仓库
        /// </summary>
        public string ToHouseId { get; set; }
        /// <summary>
        /// 入库方式：0、出库，1、退货，2、调拨，3、手动
        /// </summary>
        public int? EnterMethod { get; set; }

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
        /// 入库时间
        /// </summary>
        public DateTime? InDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 提交状态：0、待提交；1、待审批；2、入库成功；3、待退货；4、已退货；
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 关联的流程Id
        /// </summary>
        public long? FlowId { get; set; }
        /// <summary>
        /// 入库单详情列表
        /// </summary>
        [DataIgnore]
        public List<MZ_EnterDetail> List { get; set; }
        /// <summary>
        /// 来源企业名称
        /// </summary>
        [DataIgnore]
        public string FromName { get; set; }
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
