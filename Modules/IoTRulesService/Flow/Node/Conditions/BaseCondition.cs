using IoTRulesService.Flow.Builder;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Node.Conditions
{
    public abstract class BaseCondition
    {
        /// <summary>
        /// 条件代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 分组名称（不一定有）
        /// </summary>
        public string gname { get; set; }
        public string title { get; set; }
        /// <summary>
        ///  Number 数字、Date 日期、 String 字符串
        /// </summary>
        public string valueType { get; set; }

        public abstract Task<bool> ToExpressionString(RuleExecutionContext context);
    }
}
