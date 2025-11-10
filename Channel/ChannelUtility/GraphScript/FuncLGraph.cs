using ChannelUtility.Js;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace ChannelUtility.GraphScript
{
    /// <summary>
    /// 功能图形
    /// </summary>
    public class FuncLGraph : LGraph
    {
        private FuncMessageContext _context;
        public FuncMessageContext Context { get { return _context; } }
        public FuncLGraph(FuncMessageContext context)
        {
            _context = context;
        }
    }

    /// <summary>
    /// 功能回复
    /// </summary>
    public class FuncOutNode : LGraphNode
    {
        public static string type = "功能/功能回复";
        public bool IsSuccess = true;
        public string ErrorMessage = string.Empty;

        public FuncOutNode()
        {
            this.addInput("对象", DataType.OBJECT);
        }

        public override void onConfigure(JsonNode json_node)
        {
            this.IsSuccess = json_node["properties"]["是否成功"].GetValue<bool>();
            this.ErrorMessage = json_node["properties"]["错误消息"].GetValue<string>();
        }

        public override void onExecute(int step)
        {
            var funcLGraph = this.graph as FuncLGraph;
            if (funcLGraph != null)
            {
                foreach (var ipt in this.inputs)
                {
                    if (ipt.name == "是否成功")
                    {
                        this.IsSuccess = this.getInputData(ipt.num, true);
                    }
                    else if (ipt.name == "错误消息")
                    {
                        this.ErrorMessage = this.getInputData(ipt.num, string.Empty);
                    }
                }

                var iptdata = this.getInputData(0, new Dictionary<string, object>());
                if (this.IsSuccess)
                {
                    funcLGraph.Context.ConfirmSuccess(iptdata);
                }
                else
                {
                    funcLGraph.Context.ConfirmError(this.ErrorMessage);
                }
            }
        }
    }

    /// <summary>
    /// 刷新功能页面
    /// </summary>
    public class RefreshNode : LGraphNode
    {
        public static string type = "功能/刷新页面";

        public RefreshNode()
        {
        }

        public override void onExecute(int step)
        {
            var funcLGraph = this.graph as FuncLGraph;
            if (funcLGraph != null)
            {
                funcLGraph.Context.Refresh();
            }
        }
    }

    /// <summary>
    /// 展示文字
    /// </summary>
    public class MsgOutNode : LGraphNode
    {
        public static string type = "功能/展示文字";
        public string ShowMessage { get; set; } = string.Empty;

        public MsgOutNode()
        {
        }

        public override void onConfigure(JsonNode json_node)
        {
            var tmpnode = json_node["properties"]["展示文字"];
            this.ShowMessage = tmpnode.GetValue<string>();
        }

        public override void onExecute(int step)
        {
            var funcLGraph = this.graph as FuncLGraph;
            if (funcLGraph != null)
            {
                foreach (var ipt in this.inputs)
                {
                    if (ipt.name == "展示文字")
                    {
                        this.ShowMessage = this.getInputData(ipt.num, string.Empty);
                    }
                }

                funcLGraph.Context.ShowMsg(this.ShowMessage);
            }
        }
    }

    /// <summary>
    /// 重置指定事件的沉默周期
    /// </summary>
    public class SilenceTimeNode : LGraphNode
    {
        public static string type = "功能/重置沉默周期";
        public string EventCode { get; set; } = string.Empty;

        public SilenceTimeNode()
        {
        }

        public override void onConfigure(JsonNode json_node)
        {
            var tmpnode = json_node["properties"]["事件标识"];
            this.EventCode = tmpnode.GetValue<string>();
        }

        public override void onExecute(int step)
        {
            var funcLGraph = this.graph as FuncLGraph;
            if (funcLGraph != null)
            {
                foreach (var ipt in this.inputs)
                {
                    if (ipt.name == "事件标识")
                    {
                        this.EventCode = this.getInputData(ipt.num, string.Empty);
                    }
                }

                funcLGraph.Context.ResetSilenceTime(this.EventCode);
            }
        }
    }

    /// <summary>
    /// 跳转到指定网址
    /// </summary>
    public class GoUrlNode : LGraphNode
    {
        public static string type = "功能/跳转网址";
        public string Url { get; set; } = string.Empty;

        public GoUrlNode()
        {
        }

        public override void onConfigure(JsonNode json_node)
        {
            var tmpnode = json_node["properties"]["网址"];
            this.Url = tmpnode.GetValue<string>();
        }

        public override void onExecute(int step)
        {
            var funcLGraph = this.graph as FuncLGraph;
            if (funcLGraph != null)
            {
                foreach (var ipt in this.inputs)
                {
                    if (ipt.name == "网址")
                    {
                        this.Url = this.getInputData(ipt.num, string.Empty);
                    }
                }

                funcLGraph.Context.GoUrl(this.Url);
            }
        }
    }

    /// <summary>
    /// 功能的输入参数
    /// </summary>
    public class FuncParamsNode : LGraphNode
    {
        public static string type = "功能/输入参数";

        public FuncParamsNode()
        {
            this.addOutput("对象", DataType.OBJECT);
        }

        override public void onExecute(int step)
        {
            var funcLGraph = this.graph as FuncLGraph;
            if (funcLGraph != null)
            {
                this.setOutputData(0, funcLGraph.Context.FuncMessage().Inputs);
            }
        }
    }

    /// <summary>
    /// 执行指定设备的功能
    /// </summary>
    public class ExecuteFuncNode : LGraphNode
    {
        public static string type = "功能/执行功能";
        public string DtuId { get; set; } = string.Empty;
        public string FunId { get; set; } = string.Empty;

        public ExecuteFuncNode()
        {
            this.addInput("参数对象", DataType.OBJECT);
            this.addOutput("对象", DataType.OBJECT);
        }

        public override void onConfigure(JsonNode json_node)
        {
            var tmpnode1 = json_node["properties"]["通讯编码"];
            this.DtuId = tmpnode1.GetValue<string>();
            var tmpnode2 = json_node["properties"]["功能编码"];
            this.FunId = tmpnode2.GetValue<string>();
        }

        public override void onExecute(int step)
        {
            var funcLGraph = this.graph as FuncLGraph;
            if (funcLGraph != null)
            {
                var iptdata = this.getInputData(0, new Dictionary<string, object>());
                if (string.IsNullOrEmpty(this.DtuId))
                {
                    funcLGraph.Context.Execute(this.FunId, iptdata);
                }
                else
                {
                    funcLGraph.Context.ExecuteOther(this.DtuId, this.FunId, iptdata);
                }
            }
        }
    }

    /// <summary>
    /// 下发消息
    /// </summary>
    public class PublicWaitNode : LGraphNode
    {
        public static string type = "功能/下发消息";

        public PublicWaitNode()
        {
            this.addInput("消息标识", DataType.STRING);
            this.addInput("下发数据", DataType.STRING);
            this.addInput("是否为HEX", DataType.BOOL);
            this.addOutput("字符串", DataType.STRING);
        }

        override public void onExecute(int step)
        {
            var funcLGraph = this.graph as FuncLGraph;
            if (funcLGraph != null)
            {
                string msgId = this.getInputData(0, (string)null);
                string data = this.getInputData(1, string.Empty);
                bool ishex = this.getInputData(2, false);

                if (this.outputs.Count > 0 && this.outputs[0].links.Count > 0)
                {
                    string rs = funcLGraph.Context.PublicStrWait(msgId, data, ishex);
                    this.setOutputData(0, rs);
                }
                else
                {
                    funcLGraph.Context.PublicStr(data, ishex);
                }
            }
        }
    }
}
