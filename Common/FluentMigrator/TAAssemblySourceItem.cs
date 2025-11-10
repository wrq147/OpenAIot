using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TemplateAction.Core;

namespace Common.FluentMigrator
{
    public interface IAssemblyEnumerable
    {
        IReadOnlyCollection<Assembly> Assemblies { get; }
    }
    public class SingleAssemblyEnumerable : IAssemblyEnumerable
    {
        private List<Assembly> _singleass;
        public SingleAssemblyEnumerable(Assembly ass)
        {
            _singleass = new List<Assembly>();
            _singleass.Add(ass);
        }
        public IReadOnlyCollection<Assembly> Assemblies
        {
            get
            {
                return _singleass;
            }
        }
    }
    public class CollectionAssemblyEnumerable : IAssemblyEnumerable
    {
        private List<Assembly> _rt;
        public CollectionAssemblyEnumerable(ITAServiceProvider provider)
        {
            PluginCollection plgColl = provider as PluginCollection;
            PluginObject[] plgArray = plgColl.GetAllPlugin();
            _rt = plgArray.Select(x => x.TargetAssembly).ToList();

        }
        public IReadOnlyCollection<Assembly> Assemblies
        {
            get
            {
                return _rt;
            }
        }
    }
}
