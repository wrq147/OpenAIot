using FlowService.FlowNode.Builder.Step;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace FlowService.FlowNode.Builder
{
    public class UserTask : WorkflowStep
    {
        public const string ActionUser = "ActionUser";
        /// <summary>
        /// 操作选项列表
        /// </summary>
        public List<OptionItem> Options { get; set; }
        /// <summary>
        /// 表单权限
        /// </summary>
        public FormPermsItem[] FormPerms { get; set; }
        /// <summary>
        /// 选项初始化
        /// </summary>
        public OptionInitItem[] optionInit { get; set; }
        /// <summary>
        /// 更新执行的用户列表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected UserAction UpdateActionUser(StepExecutionContext context)
        {
            var action = ((UserAction)context.ExecutionPointer.EventAction);
            var oldActionUser = context.ExecutionPointer.FindAttribute(ActionUser);
            if (oldActionUser != null)
            {
                var userList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ActionUser>>(oldActionUser.AttributeValue);
                userList.Add(action.User);
                context.ExecutionPointer.UpdateAttribute(ActionUser, Newtonsoft.Json.JsonConvert.SerializeObject(userList));
            }
            else
            {
                var userList = new List<ActionUser>();
                userList.Add(action.User);
                context.ExecutionPointer.UpdateAttribute(ActionUser, Newtonsoft.Json.JsonConvert.SerializeObject(userList));
            }
            return action;
        }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            if (!context.ExecutionPointer.EventPublished)
            {
                var eventKey = context.ExecutionPointer.Id.ToString();
                return await ExecutionResult.WaitForEvent(eventKey);
            }

            if (!(context.ExecutionPointer.EventAction is UserAction))
                throw new ArgumentException();

            var action = UpdateActionUser(context);
            var res = new ExecutionResult(action.OutcomeValue);
            res.ActiveChildren = true;
            return res;
        }
    }
}
