using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthService
{
    [TableName("mz_menu")]
    public class MZ_Menu : BaseEntity
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        [ID(false)]
        [JsonPropertyName("menuId")]
        public virtual long? menu_id { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [JsonPropertyName("menuName")]
        [Required(ErrorMessage = "菜单名称不能为空")]
        [StringLength(maximumLength: 50, ErrorMessage = "菜单名称长度不能超过50个字符")]
        public virtual string menu_name { get; set; }

        /// <summary>
        /// 路由名称
        /// </summary>
        public virtual string name { get; set; }

        /// <summary>
        /// 父菜单名称
        /// </summary>
        [DataIgnore]
        public virtual string parent_name { get; set; }

        /// <summary>
        /// 父菜单ID
        /// </summary>
        [JsonPropertyName("parentId")]
        public virtual long? parent_id { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        [JsonPropertyName("orderNum")]
        public virtual int? order_num { get; set; }

        /// <summary>
        /// 路由地址
        /// </summary>
        [StringLength(200, ErrorMessage = "路由地址不能超过200个字符")]
        public virtual string path { get; set; }

        /// <summary>
        /// 组件路径
        /// </summary>
        [StringLength(200, ErrorMessage = "组件路径不能超过255个字符")]
        public virtual string component { get; set; }

        /// <summary>
        /// 路由参数
        /// </summary>
        public virtual string query { get; set; }

        /// <summary>
        /// 是否为外链（0否 1是）
        /// </summary>
        [JsonPropertyName("isFrame")]
        public virtual byte? is_frame { get; set; }

        /// <summary>
        /// 是否缓存（0缓存 1不缓存）
        /// </summary>
        [JsonPropertyName("isCache")]
        public virtual byte? is_cache { get; set; }

        /// <summary>
        /// 类型（M目录 C菜单 F按钮）
        /// </summary>
        [Required(ErrorMessage = "菜单类型不能为空")]
        [JsonPropertyName("menuType")]
        public virtual string menu_type { get; set; }

        /// <summary>
        /// 显示状态（0显示 1隐藏）
        /// </summary>
        public virtual string visible { get; set; }

        /// <summary>
        /// 菜单状态（0正常 1停用）
        /// </summary>
        public virtual string status { get; set; }

        /// <summary>
        /// 权限字符串
        /// </summary>
        [StringLength(maximumLength: 100, ErrorMessage = "权限标识长度不能超过100个字符")]
        public virtual string perms { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public virtual string icon { get; set; }
        /// <summary>
        /// 是否过滤数据
        /// </summary>
        public virtual byte? scope { get; set; }
        [DataIgnore]
        public virtual List<MZ_Menu> children { get; set; }

    }
}
