using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.Model
{
    public class In_MyProcess : BaseQueryParam
    {
        /// <summary>
        /// 流程名称过滤
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 过滤流程状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 模板Id
        /// </summary>
        public long? templateId { get; set; }
    }
}
