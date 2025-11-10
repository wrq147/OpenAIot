using Common;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace AuthService.Controller
{
    public class Config : AbstractLoginedController
    {
        private ConfigBLL _config;
        public Config(ConfigBLL config)
        {
            _config = config;
        }


        /// <summary>
        /// 获取参数配置列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<AjaxResult> List(In_ConfigList query)
        {
            return this.Success(await _config.SelectConfigList(query));
        }



        /// <summary>
        /// 根据参数编号获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(int id)
        {
            return this.Success(await _config.SelectConfigById(id));
        }



        /// <summary>
        /// 新增参数配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_Config config)
        {
            config.SetCreateBy(GetUser());
            return (await _config.InsertConfig(config)).ToAjaxResult();
        }



        /// <summary>
        /// 修改参数配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_Config config)
        {
            config.SetUpdateBy(GetUser());
            return (await _config.UpdateConfig(config)).ToAjaxResult();
        }



        /// <summary>
        /// 删除参数配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<AjaxResult> Remove(int[] id)
        {
            return (await _config.DeleteConfigByIds(id)).ToAjaxResult();
        }

        /// <summary>
        /// 刷新参数缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> RefreshCache()
        {
            await _config.ResetConfigCache();
            return this.Success(1);
        }
    }
}
