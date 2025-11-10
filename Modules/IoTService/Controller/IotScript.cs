using AuthService.Controller;
using Common.Share;
using IoTService.Business;
using IoTService.Models;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using Common;

namespace IoTService.Controller
{
    /// <summary>
    /// Iot脚本模板接口
    /// </summary>
    public class IotScript : AbstractLoginedController
    {
        private IotScriptBLL _scriptBLL;
        public IotScript(IotScriptBLL scriptBLL)
        {
            _scriptBLL = scriptBLL;
        }

        /// <summary>
        /// 脚本模板列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotScript>>> ListPage(In_ScriptList query)
        {
            return this.Success(await _scriptBLL.SelectPage(query));
        }

        /// <summary>
        /// 获取指定模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_IotScript>> Info(string id)
        {
            return (await _scriptBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 添加脚本模板
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Add(MZ_IotScript data)
        {
            return (await _scriptBLL.Insert(data)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑脚本模板
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_IotScript data)
        {
            return (await _scriptBLL.Update(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除脚本模板
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string[] ids)
        {
            return (await _scriptBLL.Delete(ids)).ToAjaxResult();
        }
    }
}
