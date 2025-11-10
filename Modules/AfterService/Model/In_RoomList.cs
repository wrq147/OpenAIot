using System;

namespace AfterService.Model
{
    public class In_RoomList
    {
        /// <summary>
        /// 过滤所属企业Id
        /// </summary>
        public long? TargetOrgId { get; set; }
        /// <summary>
        /// 过滤分类Id
        /// </summary>
        public string CategoryId { get; set; }
    }
}
