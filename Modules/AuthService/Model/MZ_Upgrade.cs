using Common.Attr;
using Microsoft.Extensions.DependencyInjection;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    /// <summary>
    /// App升级中心
    /// </summary>
    [TableName("mz_upgrade")]
    public class MZ_Upgrade
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(true)]
        public int? Id { get; set; }
        /// <summary>
        /// 所属企业Id，0为平台
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属主题Id,空为默认
        /// </summary>
        public string StyleId { get; set; }
        /// <summary>
        /// 所属企业名称
        /// </summary>
        [DataIgnore]
        public string OrgName { get; set; }
        /// <summary>
        /// 包类型：0为原生App安装包，1为wgt资源包
        /// </summary>
        public int? PackageType { get; set; }
        /// <summary>
        /// 更新标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 更新内容
        /// </summary>
        public string UpContent { get; set; }
        /// <summary>
        /// 平台：android、ios（多个,号分隔）
        /// </summary>
        public string Platform { get; set; }
        /// <summary>
        /// 当前包版本号，必须大于当前线上发行版本号
        /// </summary>
        public string UpVersion { get; set; }
        /// <summary>
        /// Wgt资源包时，原生App最低版本
        /// </summary>
        public string MinAppVersion { get; set; }
        /// <summary>
        /// 下载链接
        /// </summary>
        [JsonConverter(typeof(FileUrl))]
        public string UpUrl { get; set; }
        /// <summary>
        /// WGT是否静默更新
        /// </summary>
        public bool? IsSilently { get; set; }
        /// <summary>
        /// App安装包是否强制更新
        /// </summary>
        public bool? IsMandatory { get; set; }
        /// <summary>
        /// 是否上线发行
        /// </summary>
        public bool? IsPublish { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOn { get; set; }
    }
}
