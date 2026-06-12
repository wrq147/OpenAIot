using AuthService.Controller;
using Common;
using Common.Share;
using LLMService.Business;
using LLMService.DAL;
using LLMService.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Route;
using TemplateAction.Core;
namespace LLMService.Controller
{
    public class KbColumn : AbstractLoginedController
    {
        /// <summary>
        /// 获取知识库的栏目列表
        /// </summary>
        /// <param name="kbId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_KbColumn>>> ListByKbId(string kbId)
        {
            var result = await ServiceProvider.GetService<KbColumnBLL>().GetByKbId(kbId);
            return this.Success(result);
        }

        /// <summary>
        /// 获取栏目信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_KbColumn>> Info(string id)
        {
            var result = await ServiceProvider.GetService<KbColumnBLL>().GetById(id, GetUser());
            return this.Success(result);
        }

        /// <summary>
        /// 新增栏目
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_KbColumn entity)
        {
            var rs = await ServiceProvider.GetService<KbColumnBLL>().Add(entity, GetUser());
            return rs.ToAjaxResult();
        }

        /// <summary>
        /// 编辑栏目
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Update(MZ_KbColumn entity)
        {
            var rs = await ServiceProvider.GetService<KbColumnBLL>().Update(entity, GetUser());
            return rs.ToAjaxResult();
        }

        /// <summary>
        /// 删除栏目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Delete(string id)
        {
            var rs = await ServiceProvider.GetService<KbColumnBLL>().Delete(id, GetUser());
            return rs.ToAjaxResult();
        }

    }
}