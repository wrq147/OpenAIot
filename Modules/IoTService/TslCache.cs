using ChannelUtility;
using ChannelUtility.Tsl;
using Common;
using IoTService.DAL;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService
{
    public static class TslCache
    {
        public static async Task<TslReturn> GetTslModelByDtuId(string dtuId, bool sendconn, ITAServiceProvider provider)
        {
            IotRedisHelper redis = provider.GetService<IotRedisHelper>();
            var serverBus = provider.GetService<ServerBusProxy>();
            var cache = provider.GetService<CacheHelper>();
            var productId = cache.GetCache<string>("Device:" + dtuId + "$ProductId");
            if (string.IsNullOrEmpty(productId))
            {
                productId = await redis.HashGetAsync<string>("Device:" + dtuId, "$ProductId").ConfigureAwait(false);
                if (string.IsNullOrEmpty(productId))
                {
                    if (sendconn)
                    {
                        await serverBus.SendConnect(string.Empty, dtuId);
                    }
                    var deviceDAL = provider.GetService<IotDeviceDAL>();
                    var devicelist = await deviceDAL.SelectList(x => x.DeviceId == dtuId);
                    if (devicelist.Count > 0)
                    {
                        productId = devicelist[0].ProductId;
                    }

                    if (productId == null)
                    {
                        return null;
                    }
                    else
                    {
                        cache.SetCache("Device:" + dtuId + "$ProductId", productId, DateTime.Now.AddMinutes(60));
                    }
                }
            }
            else
            {
                cache.SetCache("Device:" + dtuId + "$ProductId", productId, DateTime.Now.AddMinutes(60));
            }
            return await GetTslModel(productId, redis, provider);
        }
        public static async Task<TslReturn> GetTslModel(string productId, ITAServiceProvider provider)
        {
            IotRedisHelper redis = provider.GetService<IotRedisHelper>();
            return await GetTslModel(productId, redis, provider);
        }
        public static async Task<TslReturn> GetTslModel(string productId, IotRedisHelper redis, ITAServiceProvider provider)
        {
            var cache = provider.GetService<CacheHelper>();
            var alltsl = cache.GetCache<TslReturn>("ProductSys:" + productId);
            if (alltsl == null)
            {
                Dictionary<string, string> proDict;
                proDict = await redis.HashGetAllAsync<string>("ProductSys:" + productId);
                if (proDict.Count == 0)
                {
                    var pro = await provider.GetService<IotProductDAL>().Select(productId);
                    if (pro != null)
                    {
                        proDict = await provider.GetService<ServerBusProxy>().DownUpdateProductSys(pro);
                    }
                }
                string modeltsl;
                proDict.TryGetValue("$ModelTSL", out modeltsl);

                string modelscript;
                proDict.TryGetValue("$Script", out modelscript);

                string status;
                proDict.TryGetValue("$Status", out status);

                string netway;
                proDict.TryGetValue("$NetworkWay", out netway);

                var model = TslModel.CreateFrom(modeltsl);
                alltsl = new TslReturn(productId, model, status, modelscript, netway);
                cache.SetCache("ProductSys:" + productId, alltsl, DateTime.Now.AddMinutes(30));
            }
            return alltsl;
        }
    }
}
