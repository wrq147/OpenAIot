using System;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    /// <summary>
    /// 单个事件处理器
    /// </summary>
    public class DefaultHandler<T> : ITAEventHandler where T : class
    {
        private Func<T, Task> _configac;
        public DefaultHandler(Func<T, Task> ac)
        {
            _configac = ac;
        }

        public async Task OnEventAsync<K>(K evt) where K : class
        {
            await _configac.Invoke(evt as T);
        }

    }
}
