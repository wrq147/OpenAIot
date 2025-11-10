using Common;
using Common.EventBus;
using Common.Share;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService
{
    public class ConfigBLL
    {
        private ITAServiceProvider _provider;
        private ConfigDAL _configDAL;
        public ConfigBLL(ITAServiceProvider provider, ConfigDAL configDAL)
        {
            _provider = provider;
            _configDAL = configDAL;
        }


        /// <summary>
        /// 缓存配置数据
        /// </summary>
        private async Task _LoadingConfigCache()
        {
            In_ConfigList query = new In_ConfigList();
            query.showAll = true;
            List<MZ_Config> configsList = (await _configDAL.SelectConfigList(query)).List;
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            foreach (MZ_Config config in configsList)
            {
                await redis.StringSetAsync(AuthConstant.CONFIG_CACHE_PRE + config.config_key, config.config_value);
            }
        }


        /// <summary>
        /// 查询参数配置信息
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task<MZ_Config> SelectConfigById(int configId)
        {
            MZ_Config config = new MZ_Config();
            config.config_id = configId;
            return await _configDAL.SelectConfig(config);
        }

        /// <summary>
        /// 根据键名查询参数配置信息
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        public async Task<string> SelectConfigByKey(string configKey)
        {
            //使用本地获取配置
            var configCache = _provider.GetService<ConfigCache>();
            string configValue = configCache.GetCache<string>(configKey);
            if (configValue != null)
            {
                return configValue;
            }

            //使用redis获取配置
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            configValue = await redis.StringGetAsync(AuthConstant.CONFIG_CACHE_PRE + configKey);
            if (configValue != null)
            {
                configCache.SetCache(configKey, configValue, DateTime.Now.AddMinutes(5));
                return configValue;
            }

            //使用数据库获取配置
            MZ_Config config = new MZ_Config();
            config.config_key = configKey;
            MZ_Config retConfig = await _configDAL.SelectConfig(config);
            if (retConfig != null)
            {
                await redis.StringSetAsync(AuthConstant.CONFIG_CACHE_PRE + configKey, retConfig.config_value);
                return retConfig.config_value;
            }
            return string.Empty;
        }
        public async Task<List<ObjectItem>> SelectConfigList(string[] keylist)
        {
            //使用本地获取配置
            List<ObjectItem> retlist = new List<ObjectItem>();
            var configCache = _provider.GetService<ConfigCache>();
            bool isLocal = true;
            for (int i = 0; i < keylist.Length; i++)
            {
                var tmpval = configCache.GetCache<string>(keylist[i]);
                if (tmpval == null)
                {
                    isLocal = false;
                    break;
                }
                retlist.Add(new ObjectItem()
                {
                    name = keylist[i],
                    value = tmpval
                });
            }
            if (isLocal)
            {
                return retlist;
            }
            else
            {
                retlist.Clear();
            }

            //使用redis获取配置
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            List<string> rqkey = new List<string>();
            for (int i = 0; i < keylist.Length; i++)
            {
                rqkey.Add(AuthConstant.CONFIG_CACHE_PRE + keylist[i]);
            }
            var configValues = await redis.StringGetAsync(rqkey);
            for (int i = 0; i < keylist.Length; i++)
            {
                string configKey = keylist[i];
                if (configValues[i].IsNull)
                {
                    MZ_Config config = new MZ_Config();
                    config.config_key = configKey;
                    MZ_Config retConfig = await _configDAL.SelectConfig(config);
                    if (retConfig != null)
                    {
                        await redis.StringSetAsync(AuthConstant.CONFIG_CACHE_PRE + configKey, retConfig.config_value);
                        retlist.Add(new ObjectItem()
                        {
                            name = configKey,
                            value = retConfig.config_value
                        });
                    }
                }
                else
                {
                    configCache.SetCache(configKey, (string)configValues[i], DateTime.Now.AddMinutes(5));
                    retlist.Add(new ObjectItem()
                    {
                        name = configKey,
                        value = configValues[i]
                    });
                }
            }

            return retlist;
        }

        /// <summary>
        /// 查询参数配置列表
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<PageObject<MZ_Config>> SelectConfigList(In_ConfigList query)
        {
            return await _configDAL.SelectConfigList(query);
        }

        /// <summary>
        /// 新增参数配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<BusResponse<long>> InsertConfig(MZ_Config config)
        {
            if (config.config_value == null)
            {
                config.config_value = string.Empty;
            }
            if (!await CheckConfigKeyUnique(config))
            {
                return BusResponse<long>.Error(31, "新增参数'" + config.config_key + "'失败，参数键名已存在");
            }
            config.remark ??= string.Empty;
            long row = await _configDAL.Insert(config);
            if (row > 0)
            {
                GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
                await redis.StringSetAsync(AuthConstant.CONFIG_CACHE_PRE + config.config_key, config.config_value);
            }
            return BusResponse<long>.Success(row);
        }

        /// <summary>
        /// 修改参数配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<BusResponse<int>> UpdateConfig(MZ_Config config)
        {
            if (!await CheckConfigKeyUnique(config))
            {
                return BusResponse<int>.Error(31, "修改参数'" + config.config_key + "'失败，参数键名已存在");
            }
            int row = await _configDAL.Update(config);
            if (row > 0)
            {
                GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
                await redis.StringSetAsync(AuthConstant.CONFIG_CACHE_PRE + config.config_key, config.config_value);
            }
            return BusResponse<int>.Success(row);
        }

        /// <summary>
        /// 批量删除参数信息
        /// </summary>
        /// <param name="configIds"></param>
        public async Task<BusResponse<string>> DeleteConfigByIds(int[] configIds)
        {
            foreach (int configId in configIds)
            {
                MZ_Config config = await SelectConfigById(configId);
                if ("Y" == config.config_type)
                {
                    return BusResponse<string>.Error(31, string.Format("内置参数【{0}】不能删除", config.config_key));
                }
                await _configDAL.DeleteConfigById(configId);
                GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
                await redis.KeyDeleteAsync(AuthConstant.CONFIG_CACHE_PRE + config.config_key);
            }
            return BusResponse<string>.Success();
        }

        /// <summary>
        /// 清空参数缓存数据
        /// </summary>
        private async Task _ClearConfigCache()
        {
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            List<string> keys = await redis.KeysAsync(AuthConstant.CONFIG_CACHE_PRE + "*");
            await redis.KeyDeleteAsync(keys);
        }

        /// <summary>
        /// 重置参数缓存数据
        /// </summary>
        public async Task ResetConfigCache()
        {
            await BusUtility.Dispatch("ClearConfig", null);
            await _ClearConfigCache();
            await _LoadingConfigCache();
        }

        /// <summary>
        /// 校验参数键名是否唯一
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private async Task<bool> CheckConfigKeyUnique(MZ_Config config)
        {
            MZ_Config info = await _configDAL.CheckConfigKeyUnique(config.config_key);
            if (info != null && info.config_id != config.config_id)
            {
                return false;
            }
            return true;
        }

    }
}
