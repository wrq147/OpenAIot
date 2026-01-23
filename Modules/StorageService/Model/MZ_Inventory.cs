using Common.Share;
using MyAccess.DB.Attr;
using Common.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 盘点实体
    /// </summary>
    [TableName("mz_inventory")]
    public class MZ_Inventory : BaseEntity
    {
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 盘点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 盘点仓库
        /// </summary>
        public string HouseId { get; set; }
        /// <summary>
        /// 盘点状态：0、待提交；1、待开始；2、初盘中；3、复盘中；4、已结束；5、已修正；6、已取消
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 初盘时间
        /// </summary>
        public DateTime? StartOn { get; set; }
        /// <summary>
        /// 复盘时间
        /// </summary>
        public DateTime? CheckOn { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndOn { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 盘点仓库数据
        /// </summary>
        [OnlySeriaize]
        public MZ_StoreHouse House { get; set; }
        /// <summary>
        /// 盘点人员
        /// </summary>
        public List<MZ_InventoryUser> UserList { get; set; }
        /// <summary>
        /// 盘点物品
        /// </summary>
        public List<MZ_InventoryItem> Items { get; set; }
    }
}
