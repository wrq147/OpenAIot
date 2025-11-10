using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node
{
    public class HttpNode : RuleBaseNode
    {
        public HttpProps props { get; set; }
    }
    public class HttpProps
    {
        /// <summary>
        /// 请求方法 支持GET/POST
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// URL地址，可以直接带参数
        /// </summary>
        public string url { get; set; }
        public HttpDataItem[] headers { get; set; }
        /// <summary>
        /// 请求参数类型
        /// </summary>
        public string contentType { get; set; }
        public HttpDataItem[] xparams { get; set; }
        public string rawString { get; set; }
        public string okTxt { get; set; }
    }

    public class HttpDataItem
    {
        public string name { get; set; }
        public bool isField { get; set; }
        /// <summary>
        /// 支持表达式 ${xxx} xxx为表单字段名称
        /// </summary>
        public string value { get; set; }
        public string valueTitle { get; set; }
    }
}
