using System;
using System.Collections.Generic;
using System.Reflection;

namespace TemplateAction.Core
{
    internal class ReferenceTree
    {
        private ReferenceNode _rootNode;
        private Dictionary<string, MappingObject> _mapping = new Dictionary<string, MappingObject>();
        class MappingObject
        {
            public PluginObject Target { get; set; }
            public ReferenceNode Node { get; set; }
        }
        public ReferenceTree()
        {
            //构建引用树
            _rootNode = new ReferenceNode();
            _rootNode.Children = new List<ReferenceNode>();
        }
        public void UpdateReferenceNode(ReferenceNode node, PluginObject target)
        {
            _mapping[node.Name].Target = target;
            //更新树
            HashSet<string> tmp = new HashSet<string>();
            string[] dependOn = target.GenerateDependOn();
            foreach (string d in dependOn)
            {
                tmp.Add(d);
            }
            //被移除的引用
            foreach (ReferenceNode parentNode in node.Parents)
            {
                if (tmp.Contains(parentNode.Name))
                {
                    tmp.Remove(parentNode.Name);
                }
                else
                {
                    parentNode.Children.Remove(node);
                    node.Parents.Remove(parentNode);
                }
            }
            //新增的引用
            foreach (string s in tmp)
            {
                ReferenceNode n = _mapping[s].Node;
                n.Children.Add(node);
                node.Parents.Add(n);
            }
            int maxLevel = node.Level;
            foreach (ReferenceNode n in node.Parents)
            {
                maxLevel = Math.Max(maxLevel, n.Level);
            }
            node.Level = maxLevel + 1;
        }

        public ReferenceNode GetReferenceNode(string name)
        {
            MappingObject mapObj;
            if (_mapping.TryGetValue(name, out mapObj))
            {
                return mapObj.Node;
            }
            else
            {
                return null;
            }
        }

        public void Append(List<PluginObject> plugins, Func<string, PluginObject> plgCreator)
        {
            foreach (PluginObject plg in plugins)
            {
                _mapping.Add(plg.Name, new MappingObject()
                {
                    Target = plg
                });
            }
            foreach (PluginObject plg in plugins)
            {
                GenerateReferenceNode(_mapping[plg.Name], plgCreator);
            }
        }
        private ReferenceNode GenerateReferenceNode(MappingObject mapObj, Func<string, PluginObject> plgCreator)
        {
            if (mapObj.Node == null)
            {
                mapObj.Node = new ReferenceNode();
                mapObj.Node.Name = mapObj.Target.Name;
                mapObj.Node.Parents = new List<ReferenceNode>();
                mapObj.Node.Children = new List<ReferenceNode>();
                int maxLevel = 0;
                string[] dependOn = mapObj.Target.GenerateDependOn();
                foreach (string s in dependOn)
                {
                    ReferenceNode n = null;
                    if (!_mapping.TryGetValue(s, out MappingObject tmpobj))
                    {
                        PluginObject target = plgCreator(s);
                        if (target != null)
                        {
                            n = GenerateReferenceNode(new MappingObject()
                            {
                                Target = target
                            }, plgCreator);
                        }
                    }
                    else
                    {
                        n = GenerateReferenceNode(_mapping[s], plgCreator);
                    }
                    if (n != null)
                    {
                        n.Children.Add(mapObj.Node);
                        maxLevel = Math.Max(maxLevel, n.Level);
                        mapObj.Node.Parents.Add(n);
                    }
                }
                //初始化本节点
                mapObj.Node.Level = maxLevel + 1;

                //无引用节点的，则添加进根节点
                if (mapObj.Node.Level == 1)
                {
                    _rootNode.Children.Add(mapObj.Node);
                }

                //初始化插件
                TAEventDispatcher.Instance.DispathPluginLoad(mapObj.Target);
                return mapObj.Node;
            }
            else
            {
                return mapObj.Node;
            }
        }
    }
}
