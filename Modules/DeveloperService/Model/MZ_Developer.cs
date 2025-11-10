using AuthService;
using DeveloperService.Controller;
using MyAccess.DB.Attr;
using System;
using TemplateAction.Core;

namespace DeveloperService.Model
{
    /// <summary>
    /// 开发者实体
    /// </summary>
    [TableName("mz_developer")]
    public class MZ_Developer
    {
        /// <summary>
        /// 开发者Id
        /// </summary>
        [ID(false)]
        public string DevId { get; set; }
        /// <summary>
        /// 开发者SecKey
        /// </summary>
        public string SecKey { get; set; }
        /// <summary>
        /// 0为简单验证，1为OAuth验证
        /// </summary>
        public byte? KeyType { get; set; }
        /// <summary>
        /// 0为个人开发者，1为企业开发者
        /// </summary>
        public byte? UserType { get;set; }
        /// <summary>
        /// 开发者关联用户
        /// </summary>
        public long? UserId { get; set; }
        /// <summary>
        /// 开发者关联组织
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateOn { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
        /// <summary>
        /// 开发者名称
        /// </summary>
        [DataIgnore]
        public string DeveloperName { get; set; }
        /// <summary>
        /// 开发者组织名称
        /// </summary>
        [DataIgnore]
        public string DeveloperOrgName { get; set; }


        public static MZ_Developer From(ITAContext context)
        {
            return context.Items["ApiDeveloper"] as MZ_Developer;
        }
        public DeveloperUserInfo ToUserInfo()
        {
            return new DeveloperUserInfo(this.UserId.Value, this.OrgId.Value);
        }
    }
}
