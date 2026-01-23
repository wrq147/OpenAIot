using FlowService.FlowNode.Builder.Step;
using FlowService.FlowNode.FormFields;
using FlowService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlowService.FlowNode
{
    public class UserActionForm
    {
        public string FormName { get; set; }
        public long FlowId { get; set; }
        public NodeStatus NodeStatus { get; set; }
        public FormField[] NodeField { get; set; }
        public WorkflowStep Step { get; set; }
        public long TemplateId { get; set; }
        public Dictionary<string, List<Out_UserItem>> Assign { get; set; }
        [JsonIgnore]
        public MZ_Flow Flow { get; set; }
        public Dictionary<string, object> Model { get; set; }
        /// <summary>
        /// 经过的流程结点列表
        /// </summary>
        public List<Out_Flow_Node> NodeList { get; set; }
    }
}
