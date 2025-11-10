using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Infrastructure;

namespace Common.FluentMigrator
{
    public class TAEmbeddedResourceProvider : IEmbeddedResourceProvider
    {
        private List<Assembly> _assembiles;
        public TAEmbeddedResourceProvider()
        {
            _assembiles = new List<Assembly>();
        }
        public void AddAssembly(Assembly ass)
        {
            _assembiles.Add(ass);
        }
        public IEnumerable<(string name, Assembly assembly)> GetEmbeddedResources()
        {
            if (_assembiles == null)
                yield break;

            foreach (var assembly in _assembiles)
            {
                foreach (var resourceName in assembly.GetManifestResourceNames())
                {
                    yield return (resourceName, assembly);
                }
            }
        }
    }
}
