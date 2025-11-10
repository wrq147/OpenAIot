using System;
using System.Collections.Generic;
namespace TemplateAction.Core
{
    public class ServiceCollection : IServiceCollection
    {
        private Dictionary<string, ServiceDescriptorList> _services;
        private string _pluginName;
        public string PluginName
        {
            get { return _pluginName; }
        }
        private ITAServiceProvider _provider;
        public ServiceCollection(ITAServiceProvider provider, string plugin)
        {
            _provider = provider;
            _pluginName = plugin;
            _services = new Dictionary<string, ServiceDescriptorList>();
        }
        public IServiceDescriptorEnumerable this[string key]
        {
            get
            {
                ServiceDescriptorList sd;
                if (_services.TryGetValue(key, out sd))
                {
                    return sd;
                }
                return null;
            }
        }
        public void Clear(string key)
        {
            _provider.ClearService(key);
        }
        public void Remove(string key)
        {
            _services.Remove(key);
        }
        public void Add(string key, ServiceDescriptor des)
        {
            if (des == null) return;
            ServiceDescriptorList sd;
            if (_services.TryGetValue(key, out sd))
            {
                des.PluginName = _pluginName;
                sd.Add(des);
            }
            else
            {
                des.PluginName = _pluginName;
                _services.Add(key, ServiceDescriptorList.Create(des));
            }
        }
        public bool TryAdd(string key, ServiceDescriptor des)
        {
            if (des == null) return false;
            if (!_provider.ExistService(key))
            {
                des.PluginName = _pluginName;
                _services.Add(key, ServiceDescriptorList.Create(des));
                return true;
            }
            return false;
        }

        public void CopyTo(ServiceDescriptorList list)
        {
            foreach(var kvp in _services)
            {
                foreach(var item in kvp.Value)
                {
                    list.Add(item);
                }
            }
        }
    }
}
