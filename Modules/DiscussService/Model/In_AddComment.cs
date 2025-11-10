using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscussService.Model
{
    [TableName("mz_comment")]
    public class In_AddComment : MZ_Comment
    {
        /// <summary>
        /// 关联对象Id
        /// </summary>
        [DataIgnore]
        public string TargetId { get; set; }
        /// <summary>
        /// 主题类型
        /// </summary>
        [DataIgnore]
        public string TargetType { get; set; }
    }
}
