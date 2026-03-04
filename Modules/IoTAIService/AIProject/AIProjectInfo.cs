using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService
{
    public class AIProjectInfo
    {
        /// <summary>
        /// AI项目名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// AI项目代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 执行阶段：Detect、Infer
        /// </summary>
        public string Stage { get; set; }
        /// <summary>
        /// AI项目备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// AI项目的配置参数
        /// </summary>
        public List<AIProjectParam> ParamList { get; set; }
    }
    public class AIProjectParam
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 参数标识符
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 参数类型：boolean、enum、float、string、clip
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 参数说明
        /// </summary>
        public string help { get; set; }
        public object defval { get; set; }
        public float min { get; set; }
        public float max { get; set; }

        /// <summary>
        /// 枚举元素
        /// </summary>
        public Dictionary<string, string> elements { get; set; }
    }
}
