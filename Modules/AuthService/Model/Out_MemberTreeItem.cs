using Common.Attr;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AuthService.Model
{
    public class Out_MemberTreeItem
    {
        /// <summary>
        /// user为用户，dept为部门，role为角色，device为设备
        /// </summary>
        public string type { get; set; }
        public long id { get; set; }

        public string name { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        [JsonConverter(typeof(AvatarUrl))]
        public string avatar { get; set; }
        public bool selected { get; set; }
        public string remark { get; set; }

        public static Out_MemberTreeItem CreateFrom(IDictionary<string, object> data)
        {
            Out_MemberTreeItem item = new Out_MemberTreeItem();
            foreach (var kvp in data)
            {
                switch (kvp.Key)
                {
                    case "type":
                        item.type = (kvp.Value ?? "").ToString();
                        break;
                    case "id":
                        item.id = Convert.ToInt64(kvp.Value ?? 0);
                        break;
                    case "name":
                        item.name = (kvp.Value ?? "").ToString();
                        break;
                    case "avatar":
                        item.avatar = (kvp.Value ?? "").ToString();
                        break;
                    case "selected":
                        item.selected = Convert.ToBoolean(kvp.Value ?? false);
                        break;
                }

            }
            return item;
        }
    }
}
