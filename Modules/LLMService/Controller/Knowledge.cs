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
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Knowledge>>> List(In_KnowledgeQuery query)
        {
            var result = await ServiceProvider.GetService<KnowledgeBLL>().QueryList(query, GetUser());
            return this.Success(result);
        }

        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Knowledge>> Info(string id)
        {
            var result = await ServiceProvider.GetService<KnowledgeBLL>().GetById(id, GetUser());
            return this.Success(result);
        }

        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Knowledge entity)
        {
            var id = await ServiceProvider.GetService<KnowledgeBLL>().Add(entity, GetUser());
            return this.Success(id);
        }

        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Update(MZ_Knowledge entity)
        {
            int rs = await ServiceProvider.GetService<KnowledgeBLL>().Update(entity, GetUser());
            return this.Success(rs);
        }

        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Delete(string id)
        {
            int rs = await ServiceProvider.GetService<KnowledgeBLL>().Delete(id, GetUser());
            return this.Success(rs);
        }

        [HttpPost]
        public async Task<DefaultAjaxResult<int>> ChangeStatus(string id, int status)
        {
            int rs = await ServiceProvider.GetService<KnowledgeBLL>().ChangeStatus(id, status, GetUser());
            return this.Success(rs);
        }
    }
}