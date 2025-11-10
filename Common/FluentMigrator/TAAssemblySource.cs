using System;
using System.Collections.Generic;
using System.Reflection;
using FluentMigrator.Runner.Initialization;
using TemplateAction.Core;

namespace Common.FluentMigrator
{
    public class TAAssemblySource : IAssemblySource
    {
        private IAssemblyEnumerable _curEnumerable;

        public TAAssemblySource()
        {
            _curEnumerable = null;
        }
        public void SetAssemblySource(Assembly ass)
        {
            _curEnumerable = new SingleAssemblyEnumerable(ass);
        }
        public void SetAllAssemblySource(ITAServiceProvider provider)
        {
            _curEnumerable = new CollectionAssemblyEnumerable(provider);
        }
        public IReadOnlyCollection<Assembly> Assemblies
        {
            get
            {
                return _curEnumerable.Assemblies;
            }
        }
    }
}
