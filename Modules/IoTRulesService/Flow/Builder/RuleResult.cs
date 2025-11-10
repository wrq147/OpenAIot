using System;
using System.Threading.Tasks;

namespace IoTRulesService.Flow.Builder
{
    public class RuleResult
    {

        public RuleResultDirective Directive { get; set; }

        public bool ActiveChildren { get; set; }
        public bool Parallel { get; set; }

        public RuleResult(string outcome = null)
        {
            Directive = RuleResultDirective.Next;
            Parallel = false;
        }
        /// <summary>
        /// 执行下一个节点
        /// </summary>
        /// <returns></returns>
        public static RuleResult Next()
        {
            return new RuleResult
            {
                Directive = RuleResultDirective.Next
            };
        }
        /// <summary>
        /// 结束执行
        /// </summary>
        /// <returns></returns>
        public static RuleResult End()
        {
            return new RuleResult
            {
                Directive = RuleResultDirective.EndRuleflow
            };
        }
    }

    public enum RuleResultDirective
    {
        Next = 0,
        EndRuleflow = 1
    }
}
