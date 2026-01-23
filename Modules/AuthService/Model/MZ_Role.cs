using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Text.Json.Serialization;

namespace AuthService
{
    [TableName("mz_role")]
    public class MZ_Role: BaseEntity
    {
        [JsonPropertyName("roleId")]
        [ID(true)]
        public virtual long? RoleID { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [JsonPropertyName("roleName")]
        public virtual string RoleName { get; set; }

        [JsonPropertyName("roleSort")]
        public virtual int? RoleSort { get; set; }
        [JsonPropertyName("remark")]
        public virtual string RoleDesc { get; set; }
        [JsonPropertyName("status")]
        public virtual string Status { get; set; }
        /// <summary>
        /// 是否为系统角色，不可删除，不可修改
        /// </summary>
        public virtual string IsSystem { get; set; }
        /// <summary>
        /// 是否禁止分配:1为禁止，0为可分配
        /// </summary>
        public virtual string NoAlloca { get; set; }
        /// <summary>
        /// 关联组织ID
        /// </summary>
        public virtual long? OrgId { get; set; }
        [DataIgnore]
        public virtual long[] menuIds { get; set; }
        [DataIgnore]
        public virtual long[] deptIds { get; set; }
    }
}
