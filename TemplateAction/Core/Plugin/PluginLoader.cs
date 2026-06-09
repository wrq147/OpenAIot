using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TemplateAction.Common;
using System.Collections.Concurrent;
using System.Linq;

namespace TemplateAction.Core
{
    public class PluginLoader : IPluginLoader
    {
        private PluginCollection _collection;
        private string _pluginPath;
        /// <summary>
        /// 加载的程序集
        /// </summary>
        private ConcurrentDictionary<string, Assembly> _allAssembly;
        private ReferenceTree _tree;

        public PluginLoader(PluginCollection collection)
        {
            _collection = collection;
            _allAssembly = new ConcurrentDictionary<string, Assembly>();
            _tree = new ReferenceTree();
        }

        protected virtual Assembly Path2Assembly(string path)
        {
            return Assembly.Load(File.ReadAllBytes(path));
        }

        /// <summary>
        /// 程序集引用其它程序集时，保证加载的程序集都是唯一的
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Assembly LoadEmbeddedAssembly(object sender, ResolveEventArgs args)
        {
            string name = args.Name;
            string[] tarr = name.Split(',');
            if (tarr.Length == 0) return null;
            name = tarr[0];
            string filepath = _pluginPath + Path.DirectorySeparatorChar + name + TAUtility.ModExt;
            if (!File.Exists(filepath))
            {
                return null;
            }

            return _allAssembly.GetOrAdd(filepath, path =>
            {
                return Path2Assembly(path);
            });
        }


        public void LoadFrom(Assembly entry, string path)
        {
            //加载静态模块
            List<PluginObject> tlist = new List<PluginObject>();
            HashSet<string> hslist = new HashSet<string>();
            //从服务文件services.txt里加载
            string serviceFilePath = System.AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "services.txt";
            if (File.Exists(serviceFilePath))
            {
                System.IO.FileStream fs = new System.IO.FileStream(serviceFilePath, System.IO.FileMode.Open);
                System.IO.StreamReader sr = new System.IO.StreamReader(fs, Encoding.UTF8);
                string tempText = "";
                while ((tempText = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(tempText))
                    {
                        Assembly tmpAss = Assembly.Load(tempText);
                        if (hslist.Contains(tmpAss.FullName))
                        {
                            continue;
                        }
                        PluginObject tmpobj = _collection.NewPlugin(tmpAss, null);
                        if (tmpobj != null)
                        {
                            tmpobj.IsService = true;
                            tlist.Add(tmpobj);
                            hslist.Add(tmpAss.FullName);
                        }
                    }
                }
                sr.Close();
                fs.Close();
            }


            //加载动态模块
            AppDomain.CurrentDomain.AssemblyResolve += LoadEmbeddedAssembly;
            _pluginPath = path;
            HashSet<string> pluginSet = new HashSet<string>();
            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists)
            {
                //加载模块
                FileInfo[] tmodfiles = info.GetFiles();
                foreach (FileInfo fi in tmodfiles)
                {
                    if (TAUtility.ModExt.Equals(fi.Extension, StringComparison.OrdinalIgnoreCase))
                    {
                        string fullpath = fi.FullName;
                        if (!_allAssembly.ContainsKey(fullpath))
                        {
                            pluginSet.Add(fullpath);
                            _allAssembly.TryAdd(fullpath, Path2Assembly(fullpath));
                        }
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }



            foreach (KeyValuePair<string, Assembly> kvp in _allAssembly)
            {
                var tmpplugin = _collection.NewPlugin(kvp.Value, kvp.Key);
                if (pluginSet.Contains(kvp.Key))
                {
                    tmpplugin.IsService = true;
                }
                tlist.Add(tmpplugin);
            }

            //生成引用节点树,并调用各个插件的load
            _tree.Append(tlist);
        }
        public void UpdateAssembly(List<string> pathList)
        {
            HashSet<string> needUpdates = new HashSet<string>();
            string[] fileNames = new string[pathList.Count];
            List<ReferenceNode> needUpdateNodes = new List<ReferenceNode>();
            for (int i = 0; i < pathList.Count; i++)
            {
                string path = pathList[i];
                string fileName = Path.GetFileNameWithoutExtension(path);
                needUpdates.Add(fileName);
                ReferenceNode node = _tree.GetReferenceNode(fileName);
                if (node != null)
                {
                    needUpdateNodes.Add(node);
                }

            }

            //先更新已存在插件
            needUpdateNodes.Sort((a, b) =>
            {
                return a.Level - b.Level;
            });

            foreach (ReferenceNode node in needUpdateNodes)
            {
                if (needUpdates.Contains(node.Name))
                {
                    UpdateReferenceNode(node, needUpdates);
                }
            }
            //最后更新新加入的插件
            List<PluginObject> tlist = new List<PluginObject>();
            foreach (string s in needUpdates)
            {
                string filepath = _pluginPath + Path.DirectorySeparatorChar + s + TAUtility.ModExt;
                Assembly ass = Path2Assembly(filepath);
                if (_allAssembly.TryAdd(filepath, ass))
                {
                    tlist.Add(_collection.NewPlugin(ass, filepath));
                }
            }
            _tree.Append(tlist);
        }
        private void UpdateReferenceNode(ReferenceNode node, HashSet<string> needUpdates)
        {
            string filepath = _pluginPath + Path.DirectorySeparatorChar + node.Name + TAUtility.ModExt;
            Assembly ass = Path2Assembly(filepath);
            _allAssembly[filepath] = ass;
            PluginObject target = _collection.NewPlugin(ass, filepath);
            _tree.UpdateReferenceNode(node, target);
            needUpdates.Remove(node.Name);
            //初始化插件
            TAEventDispatcher.Instance.DispathPluginLoad(target);

            foreach (ReferenceNode nextNode in node.Children)
            {
                UpdateReferenceNode(nextNode, needUpdates);
            }
        }
        public virtual void UnloadAssembly(Assembly ass)
        {
        }

    }
}
