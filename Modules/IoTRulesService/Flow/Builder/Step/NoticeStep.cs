using AuthService;
using Common.EventBus;
using IoTRulesService.Flow.Node;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.Flow.Builder.Step
{
    public class NoticeStep : RuleflowStep
    {
        public NoticeProps props { get; set; }
        public override async Task Run(RuleExecutionContext context)
        {
            var dataItem = context.Data.LastOrDefault();
            IDictionary<string, object> inputs = null;
            if (dataItem != null)
            {
                inputs = dataItem.Data;
            }
            else
            {
                inputs = new Dictionary<string, object>();
            }
            if (string.IsNullOrEmpty(props.TargetValue))
            {
                await context.Print("通知对象不能为空");
                await context.ExcuteNext(RuleResult.Next());
                return;
            }
            string tcont = props.Title;
            foreach (var tkvp in inputs)
            {
                tcont = tcont.Replace($"${tkvp.Key}", (tkvp.Value == null ? "" : tkvp.Value.ToString()));
            }
            List<TargetUser> targets = new List<TargetUser>();
            var template = await context.GetRuleTemplate();
            if (template == null)
            {
                await context.Print("规则已不存在");
                await context.ExcuteNext(RuleResult.Next());
                return;
            }
            List<string> waylist = new List<string>();
            switch (props.NoticeWay)
            {
                case "APP":
                    {
                        waylist.Add("APP");
                        var recvUser = await context.Provider.GetService<UserDAL>().GetAdminById(Convert.ToInt64(props.TargetValue));
                        targets.Add(new TargetUser()
                        {
                            uid = recvUser.Id.Value,
                            email = recvUser.Email,
                            phone = recvUser.Mobile
                        });
                    }
                    break;
                case "EMAIL":
                    {
                        waylist.Add("EMAIL");
                        targets.Add(new TargetUser()
                        {
                            uid = 2,
                            email = props.TargetValue,
                            phone = string.Empty
                        });
                    }
                    break;
                case "SMS":
                    {
                        waylist.Add("SMS");
                        targets.Add(new TargetUser()
                        {
                            uid = 2,
                            email = string.Empty,
                            phone = props.TargetValue
                        });
                    }
                    break;
                default:
                    await context.Print("未知的通知方式");
                    await context.ExcuteNext(RuleResult.Next());
                    return;
            }

            var nt = new NoticeEvent(2, targets.ToArray(), waylist.ToArray());
            nt.OrgId = template.OrgId.Value;
            nt.TargetType = "规则通知";
            nt.TargetUrl = string.Empty;
            nt.Content = tcont;
            nt.Label = $"规则 {template.Name} 的通知";
            await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
            await context.ExcuteNext(RuleResult.Next());
        }
    }
}
