using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace TemplateAction.Core
{
    public class PluginEventDispatcher : IDispatcher, IEventRegister
    {
        private Dictionary<string, ITAEventHandler> _handlers;
        private Dictionary<string, ITAResponseEventHandler> _responseHandlers;
        public PluginEventDispatcher()
        {
            _handlers = new Dictionary<string, ITAEventHandler>();
            _responseHandlers = new Dictionary<string, ITAResponseEventHandler>();
        }
        public void Register<T>(string key, Func<T, Task> ac) where T : class
        {
            Register(key, new DefaultHandler<T>(ac));
        }

        public void Register(string key, ITAEventHandler handler)
        {
            _handlers[key] = handler;
        }
        public void RegisterReponse(string key, ITAResponseEventHandler handler)
        {
            _responseHandlers[key] = handler;
        }
        public async Task Dispatch<T>(string key, T evt) where T : class
        {
            ITAEventHandler val;
            if (_handlers.TryGetValue(key, out val))
            {
                await val.OnEventAsync(evt);
            }
        }

        public async Task<Z> DispathWait<T, Z>(string key, T evt) where T : ResponseEvent where Z : EvtResponse
        {
            ITAResponseEventHandler val;
            if (_responseHandlers.TryGetValue(key, out val))
            {
                return await val.OnEventWaitAsync<T, Z>(evt);
            }
            return null;
        }


    }
}
