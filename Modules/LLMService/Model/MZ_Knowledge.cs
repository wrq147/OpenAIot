using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Text.Json.Serialization;

namespace LLMService.Model
{
    [TableName("llm_knowledge")]
    public class MZ_Knowledge : BaseEntity
    {
        [ID]
        public string Id { get; set; }
        /// <summary>
        /// 封面URL
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string Cover { get; set; }
        /// <summary>
        /// 文库名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文库描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 是否公开
        /// </summary>
        public bool? IsPublic { get; set; }

        /// <summary>
        /// 状态：3-审核失败，2-审核中，1-已发布，0-草稿
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 文档数量
        /// </summary>
        public int? DocCount { get; set; }
    }

}