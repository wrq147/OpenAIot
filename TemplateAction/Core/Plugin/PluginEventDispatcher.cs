using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    public class PluginEventDispatcher : AbstractEventDispatcher
    {
        private Dictionary<string, ITAEventHandler> _handlers;
        private Dictionary<string, ITAResponseEventHandler> _responseHandlers;
        public PluginEventDispatcher()
        {
            _handlers = new Dictionary<string, ITAEventHandler>();
            _responseHandlers = new Dictionary<string, ITAResponseEventHandler>();
        }

        public override void Register(string key, ITAEventHandler handler)
        {
            _handlers[key] = handler;
        }
        public override void RegisterReponse(string key, ITAResponseEventHandler handler)
        {
            _responseHandlers[key] = handler;
        }
        public override async Task Dispatch<T>(string key, T evt)
        {
            ITAEventHandler val;
            if (_handlers.TryGetValue(key, out val))
            {
                await val.OnEventAsync(evt);
            }
        }

        public override async Task<Z> DispathWait<T, Z>(string key, T evt)
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
