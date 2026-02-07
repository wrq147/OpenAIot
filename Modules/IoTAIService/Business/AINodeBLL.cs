using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using ChannelUtility.Redis;
namespace IoTAIService.Business
{
    public class AINodeBLL
    {
        private ITAServiceProvider _provider;
        public AINodeBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// 定时心跳验证iot节点是否存活
        /// </summary>
        /// <returns></returns>
        public virtual async Task ExecuteSendHeartbeat()
        {
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            var dict = redis.HashGetAll<string>("AIExeNodes");
            var serverBus = _provider.GetService<AIBusProxy>();
            var isneedupdate = false;
            //发送心跳
            foreach (var kvp in dict)
            {
                if (DateTime.TryParse(kvp.Value, out DateTime dt))
                {
                    if (dt < DateTime.Now)
                    {
                        isneedupdate = true;
                        await redis.HashDeleteAsync("RuleExeNodes", kvp.Key);
                    }
                }
                await serverBus.TestUpNode(kvp.Key);
            }

            //判断是否存在超时
            if (isneedupdate)
            {
                //通知更新所有节点监听者
                await serverBus.PublishNodeChange();
            }
        }
    }
}
