using System;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    public abstract class AbstractEventDispatcher : IDispatcher, IEventRegister
    {

        public void Register<T>(string key, Func<T, Task> ac) where T : class
        {
            Register(key, new DefaultHandler<T>(ac));
        }

        public void Register<T>(DefaultMultiHandler<T> handler) where T : class
        {
            Register(typeof(T).ToString(), handler);
        }
        public abstract Task Dispatch<T>(string key, T evt) where T : class;
        public abstract Task<Z> DispathWait<T, Z>(string key, T evt)
    where T : ResponseEvent
    where Z : EvtResponse;
        public abstract void Register(string key, ITAEventHandler handler);

        public abstract void RegisterReponse(string key, ITAResponseEventHandler handler);
    }
}
