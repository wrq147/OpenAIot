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
    public class Article : AbstractLoginedController
    {
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Article>>> List(In_ArticleQuery query)
        {
            var result = await ServiceProvider.GetService<ArticleBLL>().QueryList(query, GetUser());
            return this.Success(result);
        }

        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Article>> Info(string id)
        {
            var result = await ServiceProvider.GetService<ArticleBLL>().GetById(id, GetUser());
            return this.Success(result);
        }

        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_Article>>> GetByKbId(string kbId)
        {
            var result = await ServiceProvider.GetService<ArticleBLL>().GetByKbId(kbId, GetUser());
            return this.Success(result);
        }

        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Article entity)
        {
            var id = await ServiceProvider.GetService<ArticleBLL>().Add(entity, GetUser());
            return this.Success(id);
        }

        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Update(MZ_Article entity)
        {
            int rs = await ServiceProvider.GetService<ArticleBLL>().Update(entity, GetUser());
            return this.Success(rs);
        }

        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Delete(string id)
        {
            int rs = await ServiceProvider.GetService<ArticleBLL>().Delete(id, GetUser());
            return this.Success(rs);
        }

        [HttpPost]
        public async Task<DefaultAjaxResult<int>> ChangeStatus(string id, int status)
        {
            int rs = await ServiceProvider.GetService<ArticleBLL>().ChangeStatus(id, status, GetUser());
            return this.Success(rs);
        }

        [HttpGet]
        public async Task<DefaultAjaxResult<int>> View(string id)
        {
            int rs = await ServiceProvider.GetService<ArticleBLL>().IncrementViewCount(id, GetUser());
            return this.Success(rs);
        }
    }
}