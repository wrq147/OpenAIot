using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;

namespace AfterService.Model
{
    /// <summary>
    /// 计划的设备关联
    /// </summary>
    [TableName("mz_plane_target")]
    public class MZ_PlaneTarget
    {
        [ID(false)]
        public string PlaneId { get; set; }
        [ID(false)]
        public string TargetId { get; set; }
        /// <summary>
        /// 0为设备，1为产品，2为房间
        /// </summary>
        public int? TargetType { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        [DataIgnore]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 目标名称
        /// </summary>
        [DataIgnore]
        public string TargetName { get; set; }
    }
}
