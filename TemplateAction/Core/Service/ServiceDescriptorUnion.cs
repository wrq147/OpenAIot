using System;
using System.Collections;
using System.Collections.Generic;


namespace TemplateAction.Core
{
    public class ServiceDescriptorUnion : IServiceDescriptorEnumerable
    {
        private List<IServiceDescriptorEnumerable> _unionList = new List<IServiceDescriptorEnumerable>();

        public ServiceDescriptor First
        {
            get
            {
                return _unionList.Count > 0 ? _unionList[0].First : null;
            }
        }

        public void Union(IServiceDescriptorEnumerable list)
        {
            _unionList.Add(list);
        }
        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            foreach (IEnumerable<ServiceDescriptor> enuitem in _unionList)
            {
                foreach(ServiceDescriptor sd in enuitem)
                {
                    yield return sd;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (IEnumerable<ServiceDescriptor> enuitem in _unionList)
            {
                foreach (ServiceDescriptor sd in enuitem)
                {
                    yield return sd;
                }
            }
        }
    }
}
