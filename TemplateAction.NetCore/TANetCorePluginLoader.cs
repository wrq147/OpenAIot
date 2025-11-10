using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCorePluginLoader : PluginLoader
    {
        private Dictionary<string, AssemblyLoadContext> _assemblyContexts = new Dictionary<string, AssemblyLoadContext>();
        private bool _useUnload = false;
        public void UseMemoryUnload()
        {
            if (!_useUnload)
            {
                _useUnload = true;
            }
        }
        public TANetCorePluginLoader(PluginCollection collection) : base(collection)
        {
        }
        private string Assembly2Key(Assembly assembly)
        {
            string guid = assembly.ManifestModule.ModuleVersionId.ToString();
            return assembly.GetHashCode() + guid;
        }
        public override void UnloadAssembly(Assembly ass)
        {
            base.UnloadAssembly(ass);
            string tkey = Assembly2Key(ass);
            AssemblyLoadContext asscontext;
            if (_assemblyContexts.TryGetValue(tkey, out asscontext))
            {
                asscontext.Unload();
                _assemblyContexts.Remove(tkey);
            }
        }
        protected override Assembly Path2Assembly(string path)
        {
            AssemblyLoadContext alc = new AssemblyLoadContext(Guid.NewGuid().ToString("N"), _useUnload);
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Assembly ass = alc.LoadFromStream(fs);
                _assemblyContexts.Add(Assembly2Key(ass), alc);
                return ass;
            }
        }
    }
}
