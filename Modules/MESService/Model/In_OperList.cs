using AuthService.Fields;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    public class In_OperList : BaseQueryParam
    {
        /// <summary>
        /// 关键词搜索
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 过滤扩展字段
        /// </summary>
        public FieldFilterItem[] Items { get; set; }
    }
}
