using AuthService.Controller;
using Common;
using Common.Share;
using LLMService.Business;
using LLMService.DAL;
using LLMService.Model;
using System.Threading.Tasks;
using TemplateAction.Route;
using TemplateAction.Core;
namespace LLMService.Controller
{
    public class Knowledge : AbstractLoginedController
    {
        /// <summary>
        /// 获取知识库列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Knowledge>>> List(In_KnowledgeQuery query)
        {
            var result = await ServiceProvider.GetService<KnowledgeBLL>().QueryList(query, GetUser());
            return this.Success(result);
        }

        /// <summary>
        /// 获取知识库信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Knowledge>> Info(string id)
        {
            var result = await ServiceProvider.GetService<KnowledgeBLL>().GetById(id, GetUser());
            return this.Success(result);
        }

        /// <summary>
        /// 添加知识库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Knowledge entity)
        {
            var rs = await ServiceProvider.GetService<KnowledgeBLL>().Add(entity, GetUser());
            return rs.ToAjaxResult();
        }

        /// <summary>
        /// 更新知识库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Update(MZ_Knowledge entity)
        {
            var rs = await ServiceProvider.GetService<KnowledgeBLL>().Update(entity, GetUser());
            return rs.ToAjaxResult();
        }

        /// <summary>
        /// 删除知识库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Delete(string id)
        {
            var rs = await ServiceProvider.GetService<KnowledgeBLL>().Delete(id, GetUser());
            return rs.ToAjaxResult();
        }

        /// <summary>
        /// 发布知识库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Enable(string id)
        {
            var rs = await ServiceProvider.GetService<KnowledgeBLL>().EnableKnowledge(id, GetUser());
            return rs.ToAjaxResult();
        }

        /// <summary>
        /// 设置为草稿
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Disable(string id)
        {
            var rs = await ServiceProvider.GetService<KnowledgeBLL>().DisableKnowledge(id, GetUser());
            return rs.ToAjaxResult();
        }
    }
}