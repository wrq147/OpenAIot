using System;
using Common.Attr;
using MyAccess.DB.Attr;
using Newtonsoft.Json;

namespace Common.Share
{
    public class BaseEntity
    {
        /// <summary>
        /// 创建者Id（不用传）
        /// </summary>
        [JsonConverter(typeof(OnlySeriaize))]
        public virtual long? createId { get; set; }
        /// <summary>
        /// 创建者名称
        /// </summary>
        [DataIgnore]
        public virtual string createName { get; set; }
        /// <summary>
        /// 更新者Id（不用传）
        /// </summary>
        [JsonConverter(typeof(OnlySeriaize))]
        public virtual long? updateId { get; set; }
        /// <summary>
        /// 更新者名称
        /// </summary>
        [DataIgnore]
        public virtual string updateName { get; set; }

        /// <summary>
        /// 创建时间（不用传）
        /// </summary>
        [JsonProperty(PropertyName = "createTime")]
        [JsonConverter(typeof(OnlySeriaize))]
        public virtual DateTime? create_time { get; set; }

        /// <summary>
        /// 更新时间（不用传）
        /// </summary>
        [JsonProperty(PropertyName = "updateTime")]
        [JsonConverter(typeof(OnlySeriaize))]
        public virtual DateTime? update_time { get; set; }

        public void SetCreateBy(IUserInfo info)
        {
            createId = info.UserId;
            create_time = DateTime.Now;
            updateId = info.UserId;
            update_time = create_time;
        }
        public void SetUpdateBy(IUserInfo info)
        {
            updateId = info.UserId;
            update_time = DateTime.Now;
        }
    }
}
