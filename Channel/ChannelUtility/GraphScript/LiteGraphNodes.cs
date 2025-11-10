using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;

namespace ChannelUtility.GraphScript
{
    public class ConstNumberNode : LGraphNode
    {
        public static string type = "常用/数值";
        public double value { get; set; } = 0;

        public ConstNumberNode()
        {
            this.addOutput("数值", DataType.NUMBER);
        }

        override public void onExecute(int step)
        {
            this.setOutputData(0, value);
        }
    }

    public class ConstBoolNode : LGraphNode
    {
        public static string type = "常用/布尔值";
        public bool value { get; set; } = true;

        public ConstBoolNode()
        {
            this.addOutput("布尔值", DataType.BOOL);
        }

        override public void onExecute(int step)
        {
            this.setOutputData(0, value);
        }
    }

    public class ConstStringNode : LGraphNode
    {
        public static string type = "常用/字符串";
        public string value { get; set; } = string.Empty;

        public ConstStringNode()
        {
            this.addOutput("字符串", DataType.STRING);
        }

        override public void onExecute(int step)
        {
            this.setOutputData(0, value);
        }
    }

    public class ConstObjectNode : LGraphNode
    {
        public static string type = "常用/对象";

        public ConstObjectNode()
        {
            this.addOutput("对象", DataType.OBJECT);
        }

        override public void onExecute(int step)
        {
            this.setOutputData(0, new Dictionary<string, object>());
        }
    }

    public class JSONParseNode : LGraphNode
    {
        public static string type = "常用/JSON解释器";

        public JSONParseNode()
        {
            this.addInput("json字符串", DataType.STRING);
            this.addOutput("输出对象", DataType.OBJECT);
        }

        public override void onExecute(int step)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            try
            {
                string json = this.getInputData(0, string.Empty);
                obj = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    json,
                    new JsonSerializerOptions { AllowTrailingCommas = true }
                ) ?? new Dictionary<string, object>();
            }
            catch { }
            this.setOutputData(0, obj);
        }
    }

    public class ConstArrayNode : LGraphNode
    {
        public static string type = "常用/数组";
        public string value { get; set; } = "[]";

        public ConstArrayNode()
        {
            this.addOutput("数组", DataType.ARRAY);
            this.addOutput("长度", DataType.NUMBER);
        }

        public override void onExecute(int step)
        {
            List<object> tarr;
            try
            {
                string json = this.value.StartsWith("[") ? this.value : "[" + this.value + "]";
                tarr = JsonSerializer.Deserialize<List<object>>(
                    json,
                    new JsonSerializerOptions { AllowTrailingCommas = true }
                ) ?? new List<object>();
            }
            catch
            {
                tarr = new List<object>();
            }
            this.setOutputData(0, tarr);
            this.setOutputData(1, tarr.Count);
        }
    }

    public class SetArrayNode : LGraphNode
    {
        public static string type = "常用/修改数组";
        public double index { get; set; } = 0;

        public SetArrayNode()
        {
            this.addInput("数组", DataType.ARRAY);
            this.addInput("任意值", DataType.NONE);
            this.addOutput("数组", DataType.ARRAY);
        }

        public override void onExecute(int step)
        {
            var arr = this.getInputData(0, new List<object>());
            var obj = this.getInputData(1);
            if (this.inputs.Count > 2)
            {
                this.index = this.getInputData(2, 0);
            }

            int validIndex = (int)Math.Floor(this.index);
            if (validIndex >= 0 && validIndex < arr.Count)
            {
                arr[validIndex] = obj;
            }
            this.setOutputData(0, arr);
        }
    }

    public class ElementNode : LGraphNode
    {
        public static string type = "常用/取数组元素";
        public double index { get; set; } = 0;

        public ElementNode()
        {
            this.addInput("数组", DataType.ARRAY | DataType.STRING);
            this.addInput("索引", DataType.NUMBER);
            this.addOutput("任意值", DataType.NONE);
        }

        public override void onExecute(int step)
        {
            var array = this.getInputData(0);
            var tmpidx = this.getInputData(1, -1);
            if (tmpidx < 0)
            {
                tmpidx = this.index;
            }

            int validIndex = (int)Math.Floor(tmpidx);
            try
            {
                if (array is string str && validIndex >= 0 && validIndex < str.Length)
                {
                    this.setOutputData(0, str[validIndex]);
                }
                else if (array is List<object> list && validIndex >= 0 && validIndex < list.Count)
                {
                    this.setOutputData(0, list[validIndex]);
                }
            }
            catch { }
        }
    }

    public class ObjectPropertyNode : LGraphNode
    {
        public static string type = "常用/取对象属性值";
        public string value { get; set; } = string.Empty;

        public ObjectPropertyNode()
        {
            this.addInput("对象", DataType.OBJECT);
            this.addOutput("属性", DataType.NONE);
        }

        public override void onExecute(int step)
        {
            var objdict = this.getInputData(0, new Dictionary<string, object>());
            if (objdict.ContainsKey(this.value))
            {
                this.setOutputData(0, objdict[this.value]);
            }
        }
    }

    public class ObjectKeyNode : LGraphNode
    {
        public static string type = "常用/获取key数组";

        public ObjectKeyNode()
        {
            this.addInput("对象", DataType.OBJECT | DataType.STRING | DataType.ARRAY);
            this.addOutput("数组", DataType.ARRAY);
        }

        public override void onExecute(int step)
        {
            var data = this.getInputData(0);
            List<object> tlist = new List<object>();

            if (data is string s)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    tlist.Add(i);
                }
            }
            else if (data is List<object> list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    tlist.Add(i);
                }
            }
            else if (data is Dictionary<string, object> dict)
            {
                foreach (var tkvp in dict)
                {
                    tlist.Add(tkvp.Key);
                }
            }

            this.setOutputData(0, tlist);
        }
    }

    public class SetObjectNode : LGraphNode
    {
        public static string type = "常用/对象赋值";
        public string property { get; set; } = string.Empty;

        public SetObjectNode()
        {
            this.addInput("对象", DataType.OBJECT);
            this.addInput("任意值", DataType.NONE);
            this.addOutput("对象", DataType.OBJECT);
        }

        public override void onExecute(int step)
        {
            var obj = this.getInputData(0, new Dictionary<string, object>());
            var v = this.getInputData(1);

            if (obj.ContainsKey(this.property))
            {
                obj[this.property] = v;
            }
            else
            {
                obj.Add(this.property, v);
            }

            this.setOutputData(0, obj);
        }
    }

    public class MergeObjectsNode : LGraphNode
    {
        public static string type = "常用/合并对象";

        public MergeObjectsNode()
        {
            this.addInput("A对象", DataType.OBJECT);
            this.addInput("B对象", DataType.OBJECT);
            this.addOutput("对象", DataType.OBJECT);
        }

        public override void onExecute(int step)
        {
            var A = this.getInputData(0, new Dictionary<string, object>());
            var B = this.getInputData(1, new Dictionary<string, object>());
            var C = new Dictionary<string, object>(A);

            foreach (var kvp in B)
            {
                C[kvp.Key] = kvp.Value;
            }

            this.setOutputData(0, C);
        }
    }

    public class ArrayLength : LGraphNode
    {
        public static string type = "常用/获取长度";

        public ArrayLength()
        {
            this.addInput("v", DataType.ARRAY | DataType.STRING);
            this.addOutput("out", DataType.NUMBER);
        }

        public override void onExecute(int step)
        {
            var v = this.getInputData(0);
            if (v == null)
            {
                this.setOutputData(0, 0);
                return;
            }

            if (v is string s)
            {
                this.setOutputData(0, s.Length);
            }
            else if (v is List<object> list)
            {
                this.setOutputData(0, list.Count);
            }
            else
            {
                this.setOutputData(0, 0);
            }
        }
    }

    public class LogicAndNode : LGraphNode
    {
        public static string type = "逻辑/与操作";

        public LogicAndNode()
        {
            this.addInput("a", DataType.BOOL);
            this.addInput("b", DataType.BOOL);
            this.addOutput("布尔值", DataType.BOOL);
        }

        public override void onExecute(int step)
        {
            bool ret = true;
            foreach (var inX in this.inputs)
            {
                if (!this.getInputData(inX.num, false))
                {
                    ret = false;
                    break;
                }
            }
            this.setOutputData(0, ret);
        }
    }

    public class LogicOrNode : LGraphNode
    {
        public static string type = "逻辑/或操作";

        public LogicOrNode()
        {
            this.addInput("a", DataType.BOOL);
            this.addInput("b", DataType.BOOL);
            this.addOutput("布尔值", DataType.BOOL);
        }

        public override void onExecute(int step)
        {
            bool ret = false;
            foreach (var inX in this.inputs)
            {
                if (this.getInputData(inX.num, false))
                {
                    ret = true;
                    break;
                }
            }
            this.setOutputData(0, ret);
        }
    }

    public class LogicNotNode : LGraphNode
    {
        public static string type = "逻辑/非操作";

        public LogicNotNode()
        {
            this.addInput("布尔值", DataType.BOOL);
            this.addOutput("布尔值", DataType.BOOL);
        }

        public override void onExecute(int step)
        {
            var ret = !this.getInputData(0, false);
            this.setOutputData(0, ret);
        }
    }

    public class LogicCompareNode : LGraphNode
    {
        public static string type = "逻辑/相等判断";

        public LogicCompareNode()
        {
            this.addInput("a", DataType.BOOL);
            this.addInput("b", DataType.BOOL);
            this.addOutput("布尔值", DataType.BOOL);
        }

        public override void onExecute(int step)
        {
            var a = this.getInputData(0, false);
            var b = this.getInputData(1, false);
            this.setOutputData(0, a == b);
        }
    }

    public class LogicBranchNode : LGraphNode
    {
        public static string type = "逻辑/条件分支";

        public LogicBranchNode()
        {
            this.addInput("布尔值", DataType.BOOL);
            this.addOutput("为真", DataType.ACTION);
            this.addOutput("为假", DataType.ACTION);
        }

        public override void onExecute(int step)
        {
            var isbb = this.getInputData(0, false);
            if (isbb)
            {
                this.triggerSlot(0, null, null, step);
            }
            else
            {
                this.triggerSlot(1, null, null, step);
            }
        }
    }

    public class GenericCompareNode : LGraphNode
    {
        public static string type = "逻辑/数值比较";

        public enum OPERATION { NONE, GREATER, LOWER, EQUAL, NEQUAL, GEQUAL, LEQUAL };
        public static Dictionary<string, OPERATION> strToOperation = new Dictionary<string, OPERATION>
        {
            { "NONE", OPERATION.NONE },
            { "大于", OPERATION.GREATER },
            { "小于", OPERATION.LOWER },
            { "等于", OPERATION.EQUAL },
            { "不等于", OPERATION.NEQUAL },
            { "小于等于", OPERATION.LEQUAL },
            { "大于等于", OPERATION.GEQUAL }
        };

        public double A = 0;
        public double B = 0;
        public OPERATION OP = OPERATION.EQUAL;

        public GenericCompareNode()
        {
            this.addOutput("A数值", DataType.NUMBER);
            this.addOutput("B数值", DataType.NUMBER);
            this.addOutput("布尔值", DataType.BOOL);
        }

        override public void onExecute(int step)
        {
            double tA = this.getInputData(0, this.A);
            double tB = this.getInputData(1, this.B);

            bool v = false;
            switch (OP)
            {
                case OPERATION.NONE: v = false; break;
                case OPERATION.GREATER: v = tA > tB; break;
                case OPERATION.LOWER: v = tA < tB; break;
                case OPERATION.EQUAL: v = tA == tB; break;
                case OPERATION.NEQUAL: v = tA != tB; break;
                case OPERATION.GEQUAL: v = tA >= tB; break;
                case OPERATION.LEQUAL: v = tA <= tB; break;
            }
            this.setOutputData(2, v);
        }

        // 修改为使用JsonNode作为参数
        override public void onConfigure(JsonNode jsonNode)
        {
            if (jsonNode["properties"] is JsonObject properties)
            {
                if (properties.TryGetPropertyValue("A", out var aNode))
                    A = aNode.GetValue<double>();

                if (properties.TryGetPropertyValue("B", out var bNode))
                    B = bNode.GetValue<double>();

                if (properties.TryGetPropertyValue("OP", out var opNode) &&
                    opNode.AsValue().TryGetValue<string>(out string op))
                {

                    if (strToOperation.ContainsKey(op))
                        OP = strToOperation[op];
                }
            }
        }
    }

    public class ToNumberNode : LGraphNode
    {
        public static string type = "数学/转数值";

        public ToNumberNode()
        {
            this.addInput("任意值", DataType.NONE);
            this.addOutput("数值", DataType.NUMBER);
        }

        public override void onExecute(int step)
        {
            var v = this.getInputData(0);
            try
            {
                this.setOutputData(0, Convert.ToDouble(v));
            }
            catch { }
        }
    }

    public class MathRandNode : LGraphNode
    {
        public static string type = "数学/随机数";
        public double min { get; set; } = 0;
        public double max { get; set; } = 1;
        static int seed = (int)(DateTime.Now.Ticks & int.MaxValue);
        static readonly ThreadLocal<Random> ThreadLocalRandom = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public MathRandNode()
        {
            this.addOutput("数值", DataType.NUMBER);
        }

        public override void onExecute(int step)
        {
            foreach (var ipt in this.inputs)
            {
                if (ipt.name == "min")
                {
                    this.min = this.getInputData(ipt.num, 0);
                }
                else if (ipt.name == "max")
                {
                    this.max = this.getInputData(ipt.num, 1);
                }
            }

            var min = this.min;
            var max = this.max;
            double _last_v = ThreadLocalRandom.Value.NextSingle() * (max - min) + min;
            this.setOutputData(0, _last_v);
        }
    }

    public class MathClampNode : LGraphNode
    {
        public static string type = "数学/限制数值";
        public double min { get; set; } = 0;
        public double max { get; set; } = 1024;

        public MathClampNode()
        {
            this.addInput("数值", DataType.NUMBER);
            this.addOutput("数值", DataType.NUMBER);
        }

        public override void onExecute(int step)
        {
            var val = this.getInputData(0, 0);
            val = Math.Max(min, val);
            val = Math.Min(max, val);  // 修复原代码中的逻辑错误（之前误用了Math.Max）
            this.setOutputData(0, val);
        }
    }

    public class MathAbsNode : LGraphNode
    {
        public static string type = "数学/绝对值";

        public MathAbsNode()
        {
            this.addInput("数值", DataType.NUMBER);
            this.addOutput("数值", DataType.NUMBER);
        }

        public override void onExecute(int step)
        {
            var val = this.getInputData(0, 0);
            this.setOutputData(0, Math.Abs(val));
        }
    }

    public class MathFloorNode : LGraphNode
    {
        public static string type = "数学/向下取整";

        public MathFloorNode()
        {
            this.addInput("数值", DataType.NUMBER);
            this.addOutput("数值", DataType.NUMBER);
        }

        public override void onExecute(int step)
        {
            var val = this.getInputData(0, 0);
            this.setOutputData(0, Math.Floor(val));
        }
    }

    public class MathFracNode : LGraphNode
    {
        public static string type = "数学/取小数";

        public MathFracNode()
        {
            this.addInput("数值", DataType.NUMBER);
            this.addOutput("数值", DataType.NUMBER);
        }

        public override void onExecute(int step)
        {
            var val = this.getInputData(0, 0);
            this.setOutputData(0, val % 1);
        }
    }

    public class MathOperationNode : LGraphNode
    {
        public static string type = "数学/计算器";
        public double A { get; set; } = 0;
        public double B { get; set; } = 0;
        public string OP { get; set; } = "+";

        public MathOperationNode()
        {
            this.addInput("A", DataType.NUMBER);
            this.addInput("B", DataType.NUMBER);
            this.addOutput("=", DataType.NUMBER);
        }

        public override void onExecute(int step)
        {
            var val1 = this.getInputData(0);
            var val2 = this.getInputData(1);
            double tmpva11 = val1 != null ? (double)val1 : this.A;
            double tmpva12 = val2 != null ? (double)val2 : this.B;

            switch (this.OP)
            {
                case "+":
                    this.setOutputData(0, tmpva11 + tmpva12);
                    break;
                case "-":
                    this.setOutputData(0, tmpva11 - tmpva12);
                    break;
                case "×":
                    this.setOutputData(0, tmpva11 * tmpva12);
                    break;
                case "/":
                    this.setOutputData(0, tmpva11 / tmpva12);
                    break;
                case "%":
                    this.setOutputData(0, tmpva11 % tmpva12);
                    break;
                case "^":
                    this.setOutputData(0, Math.Pow(tmpva11, tmpva12));
                    break;
                case "max":
                    this.setOutputData(0, Math.Max(tmpva11, tmpva12));
                    break;
                case "min":
                    this.setOutputData(0, Math.Min(tmpva11, tmpva12));
                    break;
            }
        }
    }

    public class MathTrigonometryNode : LGraphNode
    {
        public static string type = "数学/三角函数";
        public double amplitude { get; set; } = 1;
        public double offset { get; set; } = 0;
        public string OP { get; set; } = "sin";

        public MathTrigonometryNode()
        {
            this.addInput("数值", DataType.NUMBER);
            this.addOutput("数值", DataType.NUMBER);
        }

        // 修改为使用JsonNode作为参数
        public override void onConfigure(JsonNode jsonNode)
        {
            if (jsonNode["properties"] is JsonObject properties)
            {
                if (properties.TryGetPropertyValue("振幅", out var ampNode))
                    amplitude = ampNode.GetValue<double>();

                if (properties.TryGetPropertyValue("偏移", out var offsetNode))
                    offset = offsetNode.GetValue<double>();
            }
        }

        public override void onExecute(int step)
        {
            foreach (var ipt in this.inputs)
            {
                if (ipt.name == "振幅")
                {
                    this.amplitude = this.getInputData(ipt.num, 1);
                }
                else if (ipt.name == "偏移")
                {
                    this.offset = this.getInputData(ipt.num, 0);
                }
            }

            double value = 0;
            double v = this.getInputData(0, 0);
            switch (OP)
            {
                case "sin":
                    value = Math.Sin(v);
                    break;
                case "cos":
                    value = Math.Cos(v);
                    break;
                case "tan":
                    value = Math.Tan(v);
                    break;
                case "asin":
                    value = Math.Asin(v);
                    break;
                case "acos":
                    value = Math.Acos(v);
                    break;
                case "atan":
                    value = Math.Atan(v);
                    break;
            }

            this.setOutputData(0, amplitude * value + offset);
        }
    }

    public class DebugOutNode : LGraphNode
    {
        public static string type = "程序/模拟输出";

        public DebugOutNode()
        {
            this.addOutput("value", DataType.NONE);
        }

        public override void onExecute(int step)
        {
            var obj = this.getInputData(0);
            if (obj == null)
            {
                return;
            }

            var funcLGraph = this.graph as FuncLGraph;
            funcLGraph?.Context.Print(obj);
        }
    }

    public class RuningStepNode : LGraphNode
    {
        public static string type = "程序/运行步数";

        public RuningStepNode()
        {
            this.addOutput("数值", DataType.NUMBER);
        }

        public override void onExecute(int step)
        {
            this.setOutputData(0, step);
        }
    }

    public class SubgraphNode : LGraphNode
    {
        public static string type = "程序/子程序";
        private FuncLGraph _subgraph;
        private int _loop = 1;

        // 修改为使用JsonNode作为参数
        public override void onConfigure(JsonNode jsonNode)
        {
            if (jsonNode["properties"] is JsonObject properties &&
                properties.TryGetPropertyValue("循环", out var loopNode))
            {
                _loop = loopNode.GetValue<int>();
            }

            if (this.graph is FuncLGraph funcLG &&
                jsonNode["subgraph"] is JsonObject subgraphObj)
            {
                _subgraph = new FuncLGraph(funcLG.Context);
                _subgraph.fromJObject(subgraphObj);
            }
        }

        public override void onAction(int step, string name, Dictionary<string, object> objparams, Dictionary<string, object> options, int slot_num)
        {
            if (_subgraph == null)
            {
                return;
            }

            var inputs = _subgraph.nodes.Where(x => x.GetType() == typeof(GraphInputNode));
            foreach (var iptNode in inputs)
            {
                GraphInputNode ipt = (GraphInputNode)iptNode;
                if (ipt.name == name)
                {
                    ipt.onAction(step, name, objparams, options, slot_num);
                    break;
                }
            }
        }

        public override void onExecute(int step)
        {
            if (_subgraph == null)
            {
                return;
            }

            // 输入处理
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (var ipt in this.inputs)
            {
                if (ipt.name != "循环" && ipt.name != "延时")
                {
                    values.Add(ipt.name, ipt.link.data_object);
                }
            }

            var inputs = _subgraph.nodes.Where(x => x.GetType() == typeof(GraphInputNode));
            foreach (var iptNode in inputs)
            {
                GraphInputNode graipt = (GraphInputNode)iptNode;
                if (values.TryGetValue(graipt.name, out object tmpval))
                {
                    iptNode.setOutputData(0, tmpval);
                }
                else
                {
                    iptNode.setOutputData(0, graipt.value);
                }
            }

            _subgraph.runStep((int)this._loop);

            // 输出处理
            Dictionary<string, object> outdict = new Dictionary<string, object>();
            var outputsNodes = _subgraph.nodes.Where(x => x.GetType() == typeof(GraphOutputNode));
            foreach (var outNode in outputsNodes)
            {
                GraphOutputNode graout = (GraphOutputNode)outNode;
                outdict.Add(graout.name, graout.getInputData(0));
            }

            foreach (var outvv in outputs)
            {
                if (outdict.TryGetValue(outvv.name, out object tmpvv))
                {
                    this.setOutputData(outvv.num, tmpvv);
                }
            }
        }
    }

    public class GraphInputNode : LGraphNode
    {
        public static string type = "程序/输入";
        public string name { get; set; }
        public object value { get; set; }

        public GraphInputNode()
        {
            this.addOutput("", DataType.NONE);
        }

        // 修改为使用JsonNode作为参数
        public override void onConfigure(JsonNode jsonNode)
        {
            if (jsonNode["properties"] is JsonObject properties)
            {
                if (properties.TryGetPropertyValue("name", out var nameNode))
                    name = nameNode.GetValue<string>();

                if (properties.TryGetPropertyValue("type", out var typeNode) &&
                    properties.TryGetPropertyValue("value", out var valueNode))
                {
                    string tmptype = typeNode.GetValue<string>();
                    switch (tmptype)
                    {
                        case "number":
                            value = valueNode.GetValue<double>();
                            break;
                        case "boolean":
                            value = valueNode.GetValue<bool>();
                            break;
                        case "string":
                            value = valueNode.GetValue<string>();
                            break;
                    }
                }
            }
        }

        public override void onExecute(int step) { }
    }

    public class GraphOutputNode : LGraphNode
    {
        public static string type = "程序/输出";
        public string name { get; set; } = string.Empty;

        public GraphOutputNode()
        {
            this.addInput("", DataType.NONE);
        }

        public override void onExecute(int step) { }
    }

    public class ToStringNode : LGraphNode
    {
        public static string type = "字符串/转字符串";

        public ToStringNode()
        {
            this.addInput("a", DataType.NONE);
            this.addOutput("输出", DataType.STRING);
        }

        public override void onExecute(int step)
        {
            var iptobj = this.getInputData(0);
            if (iptobj == null)
            {
                return;
            }

            var t = iptobj.GetType();
            if (t == typeof(string))
            {
                this.setOutputData(0, iptobj);
            }
            else if (t.IsValueType)
            {
                this.setOutputData(0, iptobj.ToString());
            }
            else
            {
                this.setOutputData(0, JsonSerializer.Serialize(iptobj, JsonMessageSerializerConfig.SerializeOptions));
            }
        }
    }

    public class StringCompareNode : LGraphNode
    {
        public static string type = "字符串/比较两字符串";

        public StringCompareNode()
        {
            this.addInput("a", DataType.STRING);
            this.addInput("b", DataType.STRING);
            this.addOutput("输出", DataType.BOOL);
        }

        public override void onExecute(int step)
        {
            var a = this.getInputData(0, string.Empty);
            var b = this.getInputData(1, string.Empty);  // 修复原代码中的输入索引错误
            this.setOutputData(0, a == b);
        }
    }

    public class StringConcatNode : LGraphNode
    {
        public static string type = "字符串/拼接两字符串";

        public StringConcatNode()
        {
            this.addInput("a", DataType.STRING);
            this.addInput("b", DataType.STRING);
            this.addOutput("输出", DataType.STRING);  // 修复原代码中的输出类型错误
        }

        public override void onExecute(int step)
        {
            var a = this.getInputData(0, string.Empty);
            var b = this.getInputData(1, string.Empty);  // 修复原代码中的输入索引错误
            this.setOutputData(0, a + b);
        }
    }

    public class StringContainsNode : LGraphNode
    {
        public static string type = "字符串/判断是否包含";

        public StringContainsNode()
        {
            this.addInput("a", DataType.STRING);
            this.addInput("b", DataType.STRING);
            this.addOutput("输出", DataType.BOOL);
        }

        public override void onExecute(int step)
        {
            var a = this.getInputData(0, string.Empty);
            var b = this.getInputData(1, string.Empty);  // 修复原代码中的输入索引错误
            this.setOutputData(0, a.Contains(b));
        }
    }

    public class ToUpperCaseNode : LGraphNode
    {
        public static string type = "字符串/转大写";

        public ToUpperCaseNode()
        {
            this.addInput("a", DataType.STRING);
            this.addOutput("输出", DataType.STRING);
        }

        public override void onExecute(int step)
        {
            var a = this.getInputData(0, string.Empty);
            this.setOutputData(0, a.ToUpper());
        }
    }

    public class StringSplitNode : LGraphNode
    {
        public static string type = "字符串/切割字符串";

        public StringSplitNode()
        {
            this.addInput("str", DataType.STRING);
            this.addInput("separator", DataType.STRING);
            this.addOutput("输出", DataType.ARRAY);
        }

        public override void onExecute(int step)
        {
            var str = this.getInputData(0, string.Empty);
            var separator = this.getInputData(1, ",");  // 修复原代码中的输入索引错误
            List<object> list = new List<object>();
            var arr = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            foreach (var arritem in arr)
            {
                list.Add(arritem);
            }

            this.setOutputData(0, list);
        }
    }

    public class ToFixedNode : LGraphNode
    {
        public static string type = "字符串/数字保留小数点";
        public int precision { get; set; } = 0;

        public ToFixedNode()
        {
            this.addInput("a", DataType.NUMBER);
            this.addOutput("输出", DataType.STRING);
        }

        public override void onExecute(int step)
        {
            var rawnum = this.getInputData(0, 0);
            this.setOutputData(0, rawnum.ToString($"f{this.precision}"));
        }
    }

    public class TimeoutNode : LGraphNode
    {
        public static string type = "事件/定时器";
        private int _delay = 0;

        public TimeoutNode()
        {
            this.addOutput("执行", DataType.ACTION);
        }

        // 修改为使用JsonNode作为参数
        public override void onConfigure(JsonNode jsonNode)
        {
            if (jsonNode["properties"] is JsonObject properties &&
                properties.TryGetPropertyValue("延时(毫秒)", out var delayNode))
            {
                _delay = delayNode.GetValue<int>();
            }
        }

        public override void onAction(int step, string name, Dictionary<string, object> objparams, Dictionary<string, object> options, int slot_num)
        {
            if (this._delay > 60000)
            {
                if (this.graph is FuncLGraph funcLG)
                    funcLG.Context.Print("延时不能大于60000");
                return;
            }

            var timer = new System.Timers.Timer(this._delay);
            timer.Elapsed += delegate (object? sender, System.Timers.ElapsedEventArgs e)
            {
                timer.Enabled = false;
                this.triggerSlot(0, objparams, options, step);
            };
            timer.Enabled = true;
        }
    }

    public class SequenceNode : LGraphNode
    {
        public static string type = "事件/序列触发";

        public SequenceNode()
        {
            this.addInput("", DataType.ACTION);
            this.addInput("", DataType.ACTION);
            this.addInput("", DataType.ACTION);
            this.addOutput("", DataType.ACTION);
            this.addOutput("", DataType.ACTION);
            this.addOutput("", DataType.ACTION);
        }

        public override void onAction(int step, string name, Dictionary<string, object> objparams, Dictionary<string, object> options, int slot_num)
        {
            for (var i = 0; i < this.outputs.Count; ++i)
            {
                this.triggerSlot(i, objparams, options, step);
            }
        }
    }
}
