using System;
using System.Collections;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public interface IServiceDescriptorEnumerable: IEnumerable<ServiceDescriptor>, IEnumerable
    {
        ServiceDescriptor First { get; }
    }
}
