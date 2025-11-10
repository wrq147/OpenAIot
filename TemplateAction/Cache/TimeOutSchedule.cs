using System;
using System.Threading.Tasks;

namespace TemplateAction.Cache
{
    public class TimeOutSchedule : TimerTask
    {
        /// <summary>
        /// 缓存池
        /// </summary>
        public CachePool Pool { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string ItemKey { get; set; }
        public void Run(IWheelTimeout timeout)
        {
            Task.Run(() => {
                Pool.Remove(ItemKey);
            });
      
        }
    }
}
