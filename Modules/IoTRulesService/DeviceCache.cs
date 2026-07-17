using ChannelUtility;
using Common;
using IoTService;
using IoTService.DAL;
using IoTService.Models;
using Quartz.Impl.AdoJobStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService
{
    /// <summary>
    /// 设备本地缓存
    /// </summary>
    public class DeviceCache
    {
        private ITAServiceProvider _provider;
        private CacheHelper _cacheHelper;
        public DeviceCache(ITAServiceProvider provider, CacheHelper cacheHelper)
        {
            _provider = provider;
            _cacheHelper = cacheHelper;
        }
        public async Task<string> GetId(string deviceId)
        {
            string tkey = "idxmem#" + deviceId;
            string tid = _cacheHelper.GetCache<string>(tkey);
            if (string.IsNullOrEmpty(tid))
            {
                var deviceDAL = _provider.GetService<IotDeviceDAL>();
                var devicelist = await deviceDAL.SelectList(x => x.DeviceId == deviceId, string.Empty, "Id");
                if (devicelist.Count > 0)
                {
                    tid = devicelist[0].Id;
                    _cacheHelper.SetCache(tkey, tid, DateTime.Now.AddMinutes(5));
                }
                else
                {
                    return null;
                }
            }
            return tid;
        }
        public async Task<IDictionary<string, DevicePropertyValue>> GetDevice(string deviceId)
        {
            string tkey = "mem#" + deviceId;
            IDictionary<string, DevicePropertyValue> dict = _cacheHelper.GetCache<IDictionary<string, DevicePropertyValue>>(tkey);
            if (dict == null)
            {
                var redis = _provider.GetService<IotRedisHelper>();
                var redisdict = await redis.HashGetAllAsync<string>("Device:" + deviceId);
                if (redisdict != null && redisdict.Count > 0)
                {
                    dict = DevicePropertyValue.FromDictStr(redisdict);
                }
            }
            return dict;
        }
        public void SetDevice(string deviceId, IDictionary<string, DevicePropertyValue> dict)
        {
            string tkey = "mem#" + deviceId;
            _cacheHelper.SetCache(tkey, dict, DateTime.Now.AddMinutes(5));
        }
        public void ClearDevice(string deviceId)
        {
            string tkey = "mem#" + deviceId;
            _cacheHelper.RemoveCache(tkey);
        }
        public void SetStartReadAll(string dtuId, Dictionary<string, WaitCache> val)
        {
            string tkey = "readall#" + dtuId;
            _cacheHelper.SetCache(tkey, val, DateTime.Now.AddSeconds(10));
        }
        public Dictionary<string, WaitCache> GetStartReadAll(string dtuId)
        {
            string tkey = "readall#" + dtuId;
            return _cacheHelper.GetCache<Dictionary<string, WaitCache>>(tkey);
        }
    }

    public class WaitCache
    {
        public List<string> props { get; set; }
        public HashSet<string> needs { get; set; }
    }
}
