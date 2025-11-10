using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Model
{
    public class In_ListPage : BaseQueryParam
    {
        /// <summary>
        /// 触发方式：0为订阅，1为Http，2为定时
        /// </summary>
        public int? way { get; set; }
        /// <summary>
        /// 状态:（0正常 1暂停）
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 过滤创建源
        /// </summary>
        public string from { get; set; }

        /// <summary>
        /// 分组过滤
        /// </summary>
        public string GroupId { get; set; }
    }
}
