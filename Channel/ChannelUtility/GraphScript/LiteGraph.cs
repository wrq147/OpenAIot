using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ChannelUtility.GraphScript
{
    [Flags]
    public enum DataType
    {
        NONE = 0,
        ENUM = 1,
        NUMBER = 2,
        STRING = 4,
        BOOL = 8,
        ARRAY = 16,
        OBJECT = 32,
        VEC2 = 64,
        VEC3 = 128,
        ACTION = 256
    };

    public struct vec2
    {
        public double x;
        public double y;
    };

    public struct vec3
    {
        public double x;
        public double y;
        public double z;
    };


    //to store connection info
    public class LLink
    {
        public int id;
        public int origin_id;
        public int origin_slot;
        public int target_id;
        public int target_slot;

        public DataType data_type;
        public object data_object;
        public LLink(int id, DataType type, int origin_id, int origin_slot, int target_id, int target_slot)
        {
            this.id = id;
            this.data_type = type;
            this.origin_id = origin_id;
            this.origin_slot = origin_slot;
            this.target_id = target_id;
            this.target_slot = target_slot;
            this.data_object = null;
        }

        public void setData(object data)
        {
            data_object = data;
        }
    }

    //to store slot info
    public class LSlot
    {
        public int num;
        public string name;
        public DataType type;
        public LLink link = null; //for input slots
        public List<LLink> links = new List<LLink>(); //for output slots
        public LSlot(string name, DataType type)
        {
            this.name = name;
            this.type = type;
        }
    }

    //the node base class
    public class LGraphNode
    {
        public int id = -1;

        public LGraph graph = null;
        public int order = -1;
        public int priority = 0;

        public List<LSlot> inputs = new List<LSlot>();
        public List<LSlot> outputs = new List<LSlot>();
        private bool? _existEventInput = null;

        private bool _isexe = false;
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        public void DisabledExecute()
        {
            _lock.EnterWriteLock();
            try
            {
                _isexe = true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        public List<WaitActionInfo> PendingActions = new List<WaitActionInfo>();

        public bool ExistEventInput()
        {
            if (_existEventInput == null)
            {
                _existEventInput = this.inputs.Exists(x => x.type == DataType.ACTION);
            }
            return _existEventInput.Value;
        }
        public LGraphNode()
        {
        }
        public virtual void triggerSlot(int slot_num, Dictionary<string, object> objparams, Dictionary<string, object> options, int step)
        {
            if (inputs.Count <= slot_num)
                return;
            LSlot slot = outputs.Count > slot_num ? outputs[slot_num] : null;
            if (slot == null)
                return;
            for (int i = 0; i < slot.links.Count; ++i)
            {
                LLink link = slot.links[i];
                if (link == null)
                    return;

                if (!graph.nodes_by_id.TryGetValue(link.target_id, out var node))
                    continue;

                if (node.inputs.Count <= link.target_slot)
                    continue;

                var targetSlot = node.inputs[link.target_slot];

                _lock.EnterReadLock();
                try
                {
                    if (_isexe)
                    {
                        this.onAction(step, targetSlot.name, objparams, options, link.target_slot);
                    }
                    else
                    {
                        PendingActions.Add(new WaitActionInfo()
                        {
                            name = targetSlot.name,
                            targetslot = link.target_slot,
                            objparams = objparams,
                            options = options
                        });
                    }
                }
                finally
                {
                    _lock.ExitReadLock();
                }

            }
        }
        public virtual LSlot addInput(string name, DataType type = DataType.NONE)
        {
            LSlot slot = new LSlot(name, type);
            slot.num = inputs.Count;
            inputs.Add(slot);
            return slot;
        }

        public virtual LSlot addOutput(string name, DataType type = DataType.NONE)
        {
            LSlot slot = new LSlot(name, type);
            slot.num = outputs.Count;
            outputs.Add(slot);
            return slot;
        }
        public virtual object getInputData(int slot_num)
        {
            if (inputs.Count <= slot_num) return null;
            LLink link = inputs[slot_num].link;
            if (link != null)
                return link.data_object;
            return null;
        }
        public virtual bool getInputData(int slot_num, bool default_value)
        {
            if (inputs.Count <= slot_num) return default_value;
            LLink link = inputs[slot_num].link;
            try
            {
                if (link != null && link.data_object is bool value)
                    return value;
            }
            catch { }
            return default_value;
        }

        public virtual double getInputData(int slot_num, double default_value)
        {
            if (inputs.Count <= slot_num) return default_value;
            LLink link = inputs[slot_num].link;
            try
            {
                if (link != null && link.data_object is double value)
                    return value;
            }
            catch { }
            return default_value;
        }

        public virtual string getInputData(int slot_num, string default_value)
        {
            if (inputs.Count <= slot_num) return default_value;
            LLink link = inputs[slot_num].link;
            try
            {
                if (link != null && link.data_object is string value)
                    return value;
            }
            catch { }
            return default_value;
        }
        public virtual List<object> getInputData(int slot_num, List<object> default_value)
        {
            if (inputs.Count <= slot_num) return default_value;
            LLink link = inputs[slot_num].link;
            try
            {
                if (link != null && link.data_object is List<object> value)
                    return value;
            }
            catch { }
            return default_value;
        }
        public virtual Dictionary<string, object> getInputData(int slot_num, Dictionary<string, object> default_value)
        {
            if (inputs.Count <= slot_num) return default_value;
            LLink link = inputs[slot_num].link;
            try
            {
                if (link != null && link.data_object is Dictionary<string, object> value)
                    return value;
            }
            catch { }
            return default_value;
        }
        public virtual vec2 getInputData(int slot_num, vec2 default_value)
        {
            if (inputs.Count <= slot_num) return default_value;
            LLink link = inputs[slot_num].link;
            try
            {
                if (link != null && link.data_object is vec2 value)
                    return value;
            }
            catch { }
            return default_value;
        }

        public virtual vec3 getInputData(int slot_num, vec3 default_value)
        {
            if (inputs.Count <= slot_num) return default_value;
            LLink link = inputs[slot_num].link;
            try
            {
                if (link != null && link.data_object is vec3 value)
                    return value;
            }
            catch { }
            return default_value;
        }
        public virtual DataTable getInputData(int slot_num, DataTable default_value)
        {
            if (inputs.Count <= slot_num) return default_value;
            LLink link = inputs[slot_num].link;
            try
            {
                if (link != null && link.data_object is DataTable value)
                    return value;
            }
            catch { }
            return default_value;
        }

        // Helper method to avoid code duplication in setOutputData overloads
        private void SetOutputDataInternal(int slot_num, object value)
        {
            if (outputs.Count <= slot_num)
                return;
            LSlot slot = outputs[slot_num];
            if (slot == null)
                return;
            foreach (var link in slot.links)
            {
                if (link != null)
                    link.setData(value);
            }
        }

        public virtual void setOutputData(int slot_num, bool v)
        {
            SetOutputDataInternal(slot_num, v);
        }
        public virtual void setOutputData(int slot_num, object v)
        {
            SetOutputDataInternal(slot_num, v);
        }
        public virtual void setOutputData(int slot_num, List<object> v)
        {
            SetOutputDataInternal(slot_num, v);
        }
        public virtual void setOutputData(int slot_num, IDictionary<string, object> v)
        {
            SetOutputDataInternal(slot_num, v);
        }

        public virtual void setOutputData(int slot_num, double v)
        {
            SetOutputDataInternal(slot_num, v);
        }

        public virtual void setOutputData(int slot_num, string v)
        {
            SetOutputDataInternal(slot_num, v);
        }

        public virtual void setOutputData(int slot_num, vec2 v)
        {
            SetOutputDataInternal(slot_num, v);
        }

        public virtual void setOutputData(int slot_num, vec3 v)
        {
            SetOutputDataInternal(slot_num, v);
        }
        public virtual void setOutputData(int slot_num, DataTable v)
        {
            SetOutputDataInternal(slot_num, v);
        }
        public virtual void transferData(int input_slot_num, int output_slot_num)
        {
            if (inputs.Count <= input_slot_num) return;

            LLink input_link = inputs[input_slot_num].link;
            if (input_link == null)
                return;

            SetOutputDataInternal(output_slot_num, input_link.data_object);
        }

        public virtual bool connect(int origin_slot, LGraphNode target, int target_slot)
        {
            if (graph == null)
                throw (new Exception("node does not belong to a graph"));
            if (graph != target.graph)
                throw (new Exception("nodes do not belong to same graph"));

            if (outputs.Count <= origin_slot)
                return false;

            if (target.inputs.Count <= target_slot)
                return false;

            LSlot origin_slot_info = this.outputs[origin_slot];
            LSlot target_slot_info = target.inputs[target_slot];
            if (origin_slot_info == null || target_slot_info == null)
                return false;

            if (origin_slot_info.type != target_slot_info.type &&
                origin_slot_info.type != DataType.NONE &&
                target_slot_info.type != DataType.NONE)
                throw (new Exception("connecting incompatible types"));

            int id = graph.last_link_id++;
            LLink link = new LLink(id, origin_slot_info.type, this.id, origin_slot, target.id, target_slot);

            graph.links.Add(link);
            origin_slot_info.links.Add(link);
            target_slot_info.link = link;

            graph.sortByExecutionOrder();
            return true;
        }
        public virtual void onAction(int step, string name, Dictionary<string, object> objparams, Dictionary<string, object> options, int slot_num)
        {
            this.onExecute(step);
        }
        public virtual void onExecute(int step)
        {
        }

        // Helper method to parse DataType from string
        private DataType ParseDataType(string typeStr)
        {
            DataType type = DataType.NONE;
            if (!string.IsNullOrEmpty(typeStr))
            {
                string[] typesarr = typeStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (string ttpe in typesarr)
                {
                    if (Globals.stringToDataType.TryGetValue(ttpe, out var t))
                    {
                        type |= t;
                    }
                }
            }
            return type;
        }

        public virtual void configure(JsonNode json_node)
        {
            if (json_node == null) return;

            // Parse id and order
            this.id = json_node["id"]?.Deserialize<int>() ?? -1;
            this.order = json_node["order"]?.Deserialize<int>() ?? -1;

            // Parse inputs
            var json_inputs = json_node["inputs"];
            if (json_inputs != null && json_inputs is JsonArray inputsArray)
            {
                for (int i = 0; i < inputsArray.Count; i++)
                {
                    var json_slot = inputsArray[i];
                    if (json_slot == null) continue;

                    string str_type = json_slot["type"]?.Deserialize<string>() ?? string.Empty;
                    DataType type = ParseDataType(str_type);

                    LSlot slot = null;
                    if (inputs.Count > i)
                        slot = inputs[i];
                    if (slot == null)
                        slot = this.addInput(json_slot["name"]?.Deserialize<string>() ?? string.Empty, type);

                    var json_link = json_slot["link"];
                    if (json_link != null && json_link.AsValue().TryGetValue(out int linkId))
                    {
                        if (graph.links_by_id.TryGetValue(linkId, out var link))
                        {
                            slot.link = link;
                        }
                    }
                }
            }

            // Parse outputs
            var json_outputs = json_node["outputs"];
            if (json_outputs != null && json_outputs is JsonArray outputsArray)
            {
                for (int i = 0; i < outputsArray.Count; i++)
                {
                    var json_slot = outputsArray[i];
                    if (json_slot == null) continue;

                    string str_type = json_slot["type"]?.Deserialize<string>() ?? string.Empty;
                    DataType type = ParseDataType(str_type);

                    LSlot slot = null;
                    if (outputs.Count > i)
                        slot = outputs[i];
                    if (slot == null)
                        slot = this.addOutput(json_slot["name"]?.Deserialize<string>() ?? string.Empty, type);

                    var json_links = json_slot["links"];
                    if (json_links != null && json_links is JsonArray linksArray)
                    {
                        foreach (var json_link_id in linksArray)
                        {
                            if (json_link_id != null && json_link_id.AsValue().TryGetValue(out int linkId))
                            {
                                if(graph.links_by_id.TryGetValue(linkId, out var link))
                                {
                                    slot.links.Add(link);
                                }
                          
                            }
                        }
                    }
                }
            }

            // Parse custom data (properties)
            this.onConfigure(json_node);
        }

        public virtual void onConfigure(JsonNode json_node)
        {
            var propertiesNode = json_node["properties"] as JsonObject;
            if (propertiesNode == null) return;

            foreach (var prop in propertiesNode)
            {
                var curprop = this.GetType().GetProperty(prop.Key);
                if (curprop == null) continue;

                try
                {
                    object value = null;
                    if (curprop.PropertyType == typeof(string))
                    {
                        value = prop.Value?.Deserialize<string>();
                    }
                    else if (curprop.PropertyType == typeof(double))
                    {
                        if (double.TryParse(prop.Value?.ToString(), out double d))
                            value = d;
                    }
                    else if (curprop.PropertyType == typeof(bool))
                    {
                        if (bool.TryParse(prop.Value?.ToString(), out bool b))
                            value = b;
                    }
                    else if (curprop.PropertyType == typeof(int))
                    {
                        if (int.TryParse(prop.Value?.ToString(), out int i))
                            value = i;
                    }

                    if (value != null)
                    {
                        curprop.SetValue(this, value);
                    }
                }
                catch { }
            }
        }
    }

    //namespace to store global litegraph data
    public class Globals
    {
        static public Dictionary<string, DataType> stringToDataType = new Dictionary<string, DataType> {
            { "NONE", DataType.NONE }, { "", DataType.NONE }, { "0", DataType.NONE },
            { "ENUM", DataType.ENUM }, { "enum", DataType.ENUM },
            { "NUMBER", DataType.NUMBER }, { "number", DataType.NUMBER },
            { "ARRAY", DataType.ARRAY }, { "array", DataType.ARRAY },
            { "OBJECT", DataType.OBJECT }, { "object", DataType.OBJECT },
            { "BOOLEAN", DataType.BOOL }, { "boolean", DataType.BOOL },
            { "STRING", DataType.STRING }, { "string", DataType.STRING },
            { "VEC2", DataType.VEC2 }, { "vec2", DataType.VEC2 },
            { "VEC3", DataType.VEC3 }, { "vec3", DataType.VEC3 },
            { "ACTION", DataType.ACTION }, { "action", DataType.ACTION }
        };

        static public Dictionary<string, Func<LGraphNode>> node_types = new Dictionary<string, Func<LGraphNode>>();
        static public void registerType(string name, Func<LGraphNode> ctor)
        {
            node_types.Add(name, ctor);
        }
        static public LGraphNode createNodeType(string name)
        {
            if (!node_types.TryGetValue(name, out var ctor))
            {
                return null;
            }
            return ctor();
        }

    };

    //one graph
    public class LGraph
    {
        public List<LGraphNode> nodes = new List<LGraphNode>();
        public Dictionary<int, LGraphNode> nodes_by_id = new Dictionary<int, LGraphNode>();
        public List<LGraphNode> nodes_in_execution_order = new List<LGraphNode>();
        public List<LLink> links = new List<LLink>();
        public Dictionary<int, LLink> links_by_id = new Dictionary<int, LLink>();

        public bool has_errors = false;
        public int last_node_id = 0;
        public int last_link_id = 0;
        public double time = 0; //time in seconds

        public LGraph()
        {
        }

        public void add(LGraphNode node)
        {
            if (node.graph != null)
                throw (new Exception("already has graph"));

            node.graph = this;
            node.id = last_node_id++;
            node.order = node.id;
            nodes.Add(node);
            nodes_by_id.Add(node.id, node);
        }

        public void clear()
        {
            has_errors = false;
            nodes.Clear();
            nodes_by_id.Clear();
            links.Clear();
            links_by_id.Clear();
            nodes_in_execution_order.Clear();
            last_link_id = 0;
            last_node_id = 0;
        }

        public void runStep(int loop = 1, float dt = 0)
        {
            for (var ll = 0; ll < loop; ll++)
            {
                for (int i = 0; i < nodes_in_execution_order.Count; ++i)
                {
                    LGraphNode node = nodes_in_execution_order[i];
                    node.DisabledExecute();
                    foreach (var tmppac in node.PendingActions)
                    {
                        node.onAction(ll, tmppac.name, tmppac.objparams, tmppac.options, tmppac.targetslot);
                    }
                    node.PendingActions.Clear();

                    if (!node.ExistEventInput())
                    {
                        node.onExecute(ll);
                    }
                }
                time += dt;
            }
        }

        public void configure(string data)
        {
            sortByExecutionOrder();
        }

        public void sortByExecutionOrder()
        {
            nodes_in_execution_order = nodes.GetRange(0, nodes.Count);

            nodes_in_execution_order.Sort(delegate (LGraphNode a, LGraphNode b)
            {
                if (a.priority == b.priority)
                {
                    return a.order - b.order;
                }
                return a.priority - b.priority;
            });
        }

        public void fromJObject(JsonObject jobject)
        {
            clear();
            if (jobject == null) return;

            // Parse last IDs
            if (jobject.TryGetPropertyValue("last_node_id", out var lastNodeIdNode))
                last_node_id = lastNodeIdNode.Deserialize<int>();

            if (jobject.TryGetPropertyValue("last_link_id", out var lastLinkIdNode))
                last_link_id = lastLinkIdNode.Deserialize<int>();

            // Parse links
            if (jobject.TryGetPropertyValue("links", out var linksNode) && linksNode is JsonArray linksArray)
            {
                foreach (var json_link in linksArray)
                {
                    if (json_link is JsonArray linkParts && linkParts.Count >= 6)
                    {
                        int id = linkParts[0].Deserialize<int>();
                        int origin_id = linkParts[1].Deserialize<int>();
                        int origin_slot = linkParts[2].Deserialize<int>();
                        int target_id = linkParts[3].Deserialize<int>();
                        int target_slot = linkParts[4].Deserialize<int>();
                        var json_type = linkParts[5].Deserialize<string>();

                        DataType type = DataType.NONE;
                        if (!string.IsNullOrEmpty(json_type))
                        {
                            string[] typesarr = json_type.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (string ttpe in typesarr)
                            {
                                if (Globals.stringToDataType.TryGetValue(ttpe, out var t))
                                {
                                    type |= t;
                                }
                            }
                        }

                        LLink link = new LLink(id, type, origin_id, origin_slot, target_id, target_slot);
                        links.Add(link);
                        links_by_id[link.id] = link;
                    }
                }
            }

            // Parse nodes
            if (jobject.TryGetPropertyValue("nodes", out var nodesNode) && nodesNode is JsonArray nodesArray)
            {
                foreach (var json_node in nodesArray)
                {
                    if (json_node is JsonObject nodeObj &&
                        nodeObj.TryGetPropertyValue("type", out var typeNode) &&
                        typeNode != null)
                    {
                        string node_type = typeNode.Deserialize<string>();
                        LGraphNode node = Globals.createNodeType(node_type);
                        if (node == null)
                        {
                            has_errors = true;
                            continue;
                        }
                        node.graph = this;
                        nodes.Add(node);
                        node.configure(json_node);
                        nodes_by_id[node.id] = node;
                    }
                }
            }

            sortByExecutionOrder();
        }

        public void fromJSONText(string text)
        {
            try
            {
                var root = JsonNode.Parse(text) as JsonObject;
                if (root != null)
                {
                    fromJObject(root);
                }
            }
            catch (JsonException ex)
            {
                has_errors = true;
                // Handle JSON parsing error
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }
        }
    }

    public class WaitActionInfo
    {
        public string name { get; set; }
        public int targetslot { get; set; }
        public Dictionary<string, object> objparams { get; set; }
        public Dictionary<string, object> options { get; set; }
    }
}
