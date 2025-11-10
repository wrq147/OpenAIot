using AuthService.Controller;
using Common;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using System.Collections.Generic;
using Common.Share;

namespace MonitorService.Controller
{
    /// <summary>
    /// 缓存监控
    /// </summary>
    [About]
    public class Cache : AbstractLoginedController
    {
        public Cache()
        {
        }
        /// <summary>
        /// 删除指定缓存键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Del(string key)
        {
            GeneralRedisHelper redis = this.ServiceProvider.GetService<GeneralRedisHelper>();
            await redis.KeyDeleteAsync(key);
            return this.Success<string>();
        }

        [HttpGet]
        public async Task<AjaxResult> Info()
        {
            GeneralRedisHelper redis = this.ServiceProvider.GetService<GeneralRedisHelper>();
            var info = await redis.ExecuteAsync("info");
            var commandStats = await redis.ExecuteAsync("info", "commandstats");
            var dbsize = await redis.ExecuteAsync("DBSIZE");

            string[] infoitems = ((string)info).Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, string> infoDict = new Dictionary<string, string>();
            foreach (string it in infoitems)
            {
                string[] tvals = it.Split(":", StringSplitOptions.RemoveEmptyEntries);
                if (tvals.Length > 1)
                {
                    infoDict.Add(tvals[0], tvals[1]);
                }
            }

            string[] statsItems = ((string)commandStats).Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            List<ObjectItem> statsList = new List<ObjectItem>();
            foreach (string sit in statsItems)
            {
                string[] tvals = sit.Split(":", StringSplitOptions.RemoveEmptyEntries);
                if (tvals.Length > 1)
                {
                    statsList.Add(new ObjectItem()
                    {
                        name = tvals[0].RemoveStart("cmdstat_"),
                        value = tvals[1].SubstringBetween("calls=", ",usec")
                    });
                }
            }

            return this.Success(new
            {
                info = infoDict,
                dbSize = (int)dbsize,
                commandStats = statsList
            });
        }
    }
}
