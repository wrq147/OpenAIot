using AuthService;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 盘点人表
    /// </summary>
    [TableName("mz_inventory_user")]
    public class MZ_InventoryUser
    {
        /// <summary>
        /// 盘点Id
        /// </summary>
        public string InventoryId { get; set; }
        /// <summary>
        /// 0表示初盘人员、1表示复盘人员
        /// </summary>
        public int? TimeIn { get; set; }
        /// <summary>
        /// 盘点人
        /// </summary>
        public long? UserId { get; set; }
        /// <summary>
        /// 盘点人信息
        /// </summary>
        public MZ_AdminInfo UserInfo { get; set; }
    }
}
