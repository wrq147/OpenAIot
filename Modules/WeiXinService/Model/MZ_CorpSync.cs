using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinService.Model
{
    [TableName("mz_corp_sync")]
    public class MZ_CorpSync
    {
        /// <summary>
        /// 企业微信的AppId
        /// </summary>
        [ID(false)]
        public string AppId { get; set; }
        /// <summary>
        /// 部门同步关联字典
        /// </summary>
        public string DeptDict { get; set; }
        /// <summary>
        /// 人员同步关联字典
        /// </summary>
        public string MemDict { get; set; }
        /// <summary>
        /// 定时任务Id
        /// </summary>
        public long? JobId { get; set; }
        /// <summary>
        /// 速度：1为快速，2为慢速
        /// </summary>
        public int? Speed { get; set; }
        /// <summary>
        /// 状态（0启用 1停用）
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
        /// <summary>
        /// 账号绑定的企业微信扩展字段
        /// </summary>
        public string UserName { get; set; }
    }
}
