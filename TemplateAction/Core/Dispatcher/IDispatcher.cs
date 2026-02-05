using System;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    public interface IDispatcher
    {
        Task Dispatch<T>(string key, T evt) where T : class;
        Task<Z> DispathWait<T, Z>(string key, T evt) where T : ResponseEvent where Z : EvtResponse, new();
    }
}
