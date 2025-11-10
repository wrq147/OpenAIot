using ChannelUtility.Js;
using System;

namespace ChannelUtility.GraphScript
{
    public class LiteGraphParser
    {
        private static object _lock = new object();
        private static LiteGraphParser _instance;
        public static LiteGraphParser Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LiteGraphParser();
                        }
                    }
                }
                return _instance;
            }
        }
        private LiteGraphParser()
        {
            //注册常用节点
            Globals.registerType(ConstNumberNode.type, () => new ConstNumberNode());
            Globals.registerType(ConstBoolNode.type, () => new ConstBoolNode());
            Globals.registerType(ConstStringNode.type, () => new ConstStringNode());
            Globals.registerType(ConstObjectNode.type, () => new ConstObjectNode());
            Globals.registerType(JSONParseNode.type, () => new JSONParseNode());
            Globals.registerType(ConstArrayNode.type, () => new ConstArrayNode());
            Globals.registerType(SetArrayNode.type, () => new SetArrayNode());
            Globals.registerType(ElementNode.type, () => new ElementNode());
            Globals.registerType(ObjectPropertyNode.type, () => new ObjectPropertyNode());
            Globals.registerType(ObjectKeyNode.type, () => new ObjectKeyNode());
            Globals.registerType(SetObjectNode.type, () => new SetObjectNode());
            Globals.registerType(MergeObjectsNode.type, () => new MergeObjectsNode());
            Globals.registerType(ArrayLength.type, () => new ArrayLength());

            //注册逻辑节点
            Globals.registerType(GenericCompareNode.type, () => new GenericCompareNode());
            Globals.registerType(LogicAndNode.type, () => new LogicAndNode());
            Globals.registerType(LogicOrNode.type, () => new LogicOrNode());
            Globals.registerType(LogicNotNode.type, () => new LogicNotNode());
            Globals.registerType(LogicCompareNode.type, () => new LogicCompareNode());
            Globals.registerType(LogicBranchNode.type, () => new LogicBranchNode());


            //注册数学节点
            Globals.registerType(ToNumberNode.type, () => new ToNumberNode());
            Globals.registerType(MathRandNode.type, () => new MathRandNode());
            Globals.registerType(MathClampNode.type, () => new MathClampNode());
            Globals.registerType(MathAbsNode.type, () => new MathAbsNode());
            Globals.registerType(MathFloorNode.type, () => new MathFloorNode());
            Globals.registerType(MathFracNode.type, () => new MathFracNode());
            Globals.registerType(MathOperationNode.type, () => new MathOperationNode());
            Globals.registerType(MathTrigonometryNode.type, () => new MathTrigonometryNode());

            //注册程序节点
            Globals.registerType(DebugOutNode.type, () => new DebugOutNode());
            Globals.registerType(RuningStepNode.type, () => new RuningStepNode());
            Globals.registerType(SubgraphNode.type, () => new SubgraphNode());
            Globals.registerType(GraphInputNode.type, () => new GraphInputNode());
            Globals.registerType(GraphOutputNode.type, () => new GraphOutputNode());

            //注册字符串节点
            Globals.registerType(ToStringNode.type, () => new ToStringNode());
            Globals.registerType(StringCompareNode.type, () => new StringCompareNode());
            Globals.registerType(StringConcatNode.type, () => new StringConcatNode());
            Globals.registerType(StringContainsNode.type, () => new StringContainsNode());
            Globals.registerType(ToUpperCaseNode.type, () => new ToUpperCaseNode());

            //注册功能节点
            Globals.registerType(MsgOutNode.type, () => new MsgOutNode());
            Globals.registerType(RefreshNode.type, () => new RefreshNode());
            Globals.registerType(GoUrlNode.type, () => new GoUrlNode());
            Globals.registerType(SilenceTimeNode.type, () => new SilenceTimeNode());
            Globals.registerType(FuncOutNode.type, () => new FuncOutNode());
            Globals.registerType(ExecuteFuncNode.type, () => new ExecuteFuncNode());
            Globals.registerType(FuncParamsNode.type, () => new FuncParamsNode());
            Globals.registerType(PublicWaitNode.type, () => new PublicWaitNode());

            //注册事件节点
            Globals.registerType(TimeoutNode.type, () => new TimeoutNode());
            Globals.registerType(SequenceNode.type, () => new SequenceNode());
        }

        public void Run(FuncMessageContext context, string json)
        {
            FuncLGraph graph = new FuncLGraph(context);
            graph.fromJSONText(json);
            graph.runStep();
        }
    }
}
