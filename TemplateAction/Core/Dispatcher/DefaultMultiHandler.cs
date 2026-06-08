using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Common;

namespace TemplateAction.Core
{
    /// <summary>
    /// 多个事件处理器
    /// </summary>
    public class DefaultMultiHandler<T> : ITAEventHandler where T : class
    {
        private List<Func<T, Task>> _handlers;
        public DefaultMultiHandler()
        {
            _handlers = new List<Func<T, Task>>();
        }
        public void Register(Func<T, Task> ac)
        {
            _handlers.Add(ac);
        }
        public bool UnRegister(Func<T, Task> ac)
        {
            return _handlers.Remove(ac);
        }

        public async Task OnEventAsync<T1>(T1 evt) where T1 : class
        {
            for (int i = 0; i < _handlers.Count; i++)
            {
                try
                {
                    await _handlers[i](evt as T);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(_handlers[i].Target.GetType() + "的事件处理异常," + ex.ToString());
                }
            }
        }
    }
}
