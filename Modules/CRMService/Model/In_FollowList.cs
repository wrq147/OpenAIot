using Common.Attr;
using Common.Share;
using FluentMigrator.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    public class In_FollowList : BaseQueryParam
    {
        /// <summary>
        /// 过滤跟进目标类型：0为客户、1为线索（新增必填）
        /// </summary>
        public int? TargetType { get; set; }
        /// <summary>
        /// 过滤跟进目标Id（新增必填）
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 过滤搜索关键字
        /// </summary>
        public string Key { get; set; }
    }
}
