using Common;
using Common.Attr;
using Common.Share;
using MyAccess.DB.Attr;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;

namespace AuthService
{
    [TableName("mz_admin")]
    public class MZ_AdminInfo : BaseEntity
    {
        [ID(false)]
        public virtual long? Id { get; set; }
        /// <summary>
        /// 当前所在部门
        /// </summary>
        [DataIgnore]
        public virtual long? dept_id { get; set; }
        /// <summary>
        /// 当前职位
        /// </summary>
        [DataIgnore]
        public virtual string post_name { get; set; }
        /// <summary>
        /// 是否为部门负责人
        /// </summary>
        [DataIgnore]
        public virtual bool? IsLeader { get; set; }
        /// <summary>
        /// 是否为主要部门
        /// </summary>
        [DataIgnore]
        public virtual bool? IsPrimary { get; set; }
        /// <summary>
        /// 当前使用的企业编号（不用传）
        /// </summary>
        public virtual long? OrgId { get; set; }
        /// <summary>
        /// 所属企业名称（多个分号隔开）
        /// </summary>
        [DataIgnore]
        public virtual string OrgNames { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [DataIgnore]
        public virtual string dept_name { get; set; }

        /// <summary>
        /// 后台用户名
        /// </summary>
        public virtual string UserName { get; set; }
        /// <summary>
        /// 真实姓名(初始为用户名）
        /// </summary>
        public virtual string RealName { get; set; }
        /// <summary>
        /// 后台密码
        /// </summary>
        [JsonConverter(typeof(OnlyDeserialize))]
        public virtual string Password { get; set; }
        /// <summary>
        /// 密码盐
        /// </summary>
        [JsonIgnore]
        public virtual string Salt { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public virtual string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public virtual string Email { get; set; }
        /// <summary>
        /// 邮箱是否激活
        /// </summary>
        public virtual bool? EmailActive { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl), true)]
        public virtual string Avatar { get; set; }
        /// <summary>
        /// 用户性别（0男 1女 2未知）
        /// </summary>
        public virtual string Sex { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Introduction { get; set; }
        /// <summary>
        /// 帐号状态（0正常 1停用）
        /// </summary>
        public virtual string status { get; set; }
        /// <summary>
        /// 工作签名
        /// </summary>
        public virtual string Signature { get; set; }
        /// <summary>
        /// 原签名
        /// </summary>
        public virtual string WaitSignature { get; set; }
        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        [JsonIgnore]
        public virtual string del_flag { get; set; }

        /// <summary>
        /// 角色组
        /// </summary>
        [DataIgnore]
        public virtual long[] roleIds { get; set; }

    }

}
