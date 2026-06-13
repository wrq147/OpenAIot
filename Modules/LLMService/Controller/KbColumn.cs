using AuthService.Controller;
using Common;
using Common.Share;
using LLMService.Business;
using LLMService.DAL;
using LLMService.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;
namespace LLMService.Controller
{
    public class KbColumn : AbstractLoginedController
    {


        /// <summary>
        /// 栏目树
        /// </summary>
        /// <param name="kbId">知识库Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_KbColumn>>> ListTree(string kbId)
        {
            var allcls = await ServiceProvider.GetService<KbColumnBLL>().SelectAllClass(kbId);
            return this.Success(MZ_KbColumn.BuildTree(allcls));
        }

        /// <summary>
        /// 栏目排序
        /// </summary>
        /// <param name="ids">排序Id列表</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Sort(List<string> ids)
        {
            return (await ServiceProvider.GetService<KbColumnBLL>().UpdateSort(ids)).ToAjaxResult();
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