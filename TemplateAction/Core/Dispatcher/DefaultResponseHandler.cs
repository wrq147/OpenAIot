using System;
using System.Threading.Tasks;

namespace TemplateAction.Core.Dispatcher
{
    public class DefaultResponseHandler<T, R> : ITAResponseEventHandler where T : class
    {
        private Func<T, Task<R>> _configac;
        public DefaultResponseHandler(Func<T, Task<R>> ac)
        {
            _configac = ac;
        }

        public async Task<Z> OnEventWaitAsync<T1, Z>(T1 evt) where T1 : class where Z : EvtResponse
        {
            return await _configac.Invoke(evt as T) as Z;
        }
    }
}
