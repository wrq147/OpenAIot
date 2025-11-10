using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Label.Element;

namespace TemplateAction.Core
{
    public interface ITAResponseEventHandler
    {
        Task<Z> OnEventWaitAsync<T, Z>(T evt) where T : class where Z : EvtResponse;
    }
}
