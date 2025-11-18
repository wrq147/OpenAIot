using Common.Share;
using System;


namespace IoTService.Models
{
    public class In_ProductListPage : BaseQueryParam
    {
        /// <summary>
        /// 过滤组织（前端不用传）
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 名称过滤
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分类过滤
        /// </summary>
        public string ClassId { get; set; }
        /// <summary>
        /// 分类路径过滤
        /// </summary>
        public string ClassPath { get; set; }
        /// <summary>
        /// 状态过滤（0未发布、1已发布）
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 过滤联网
        /// </summary>
        public bool? IsNet { get; set; }
        /// <summary>
        /// 过滤协议Id数组
        /// </summary>
        public string[] Ids { get; set; }
    }
}
