using Common.Attr;
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
    /// 通用分组表
    /// </summary>
    [TableName("mz_group_view")]
    public class MZ_GroupView
    {
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        [JsonConverter(typeof(OnlySeriaize))]
        public long? OrgId { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分组的表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 二级分组字段
        /// </summary>
        public string LevelCode { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 排序值：越小越前面
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 过滤条件的json
        /// </summary>
        public string ConditionJson { get; set; }
        /// <summary>
        /// 列表字段的json
        /// </summary>
        public string ListFieldsJson { get; set; }
    }
}
