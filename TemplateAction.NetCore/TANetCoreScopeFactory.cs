using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreScopeFactory : IServiceScopeFactory
    {
        private ITAServiceProvider _provider;
        public TANetCoreScopeFactory(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public IServiceScope CreateScope()
        {
            return new TANetCoreScope(_provider);
        }
    }
}
