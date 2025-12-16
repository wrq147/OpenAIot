using IoTService;
using System;
using TemplateAction.Core;

namespace IoTVideoService.Business
{
    public class FixVideoBLL
    {
        private ITAServiceProvider _provider;
        public FixVideoBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }

        public virtual async Task CollectVideo()
        {
            //获取所有固定地址采集节点
            var redisHelper = _provider.GetService<IotRedisHelper>();
            var dict = await redisHelper.HashGetAllAsync<string>("FixVideoNode");
            var serverBus = _provider.GetService<ServerBusProxy>();
            List<string> offlineIds = new List<string>();
            List<string> onlineIds = new List<string>();
            //发送心跳
            foreach (var kvp in dict)
            {
                if (DateTime.TryParse(kvp.Value, out DateTime dt))
                {
                    if (dt < DateTime.Now)
                    {
                        offlineIds.Add(kvp.Key);
                    }
                    else
                    {
                        onlineIds.Add(kvp.Key);
                    }
                }
                else
                {
                    offlineIds.Add(kvp.Key);
                }
            }
            if (offlineIds.Count > 0)
            {
                await redisHelper.HashDeleteAsync("FixVideoNode", offlineIds.ToArray());
            }


            //给在线节点分配视频采集

        }

    }
}
