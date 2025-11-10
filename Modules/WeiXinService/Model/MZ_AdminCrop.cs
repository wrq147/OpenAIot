using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinService.Model
{
    /// <summary>
    /// 账号与企业微信应用关联
    /// </summary>
    [TableName("mz_admin_crop")]
    public class MZ_AdminCrop
    {
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 企业微信的AppId
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 绑定的企业微信UserId
        /// </summary>
        public string CropUserId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
    }
}
