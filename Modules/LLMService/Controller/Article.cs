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
        /// <summary>
        /// 文章列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Article>>> List(In_ArticleQuery query)
        {
            var result = await ServiceProvider.GetService<ArticleBLL>().QueryList(query, GetUser());
            return this.Success(result);
        }

        /// <summary>
        /// 栏目的文章列表
        /// </summary>
        /// <param name="kbId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<Out_ArticleItem>>> ArticleItems(string kbId)
        {
            var result = await ServiceProvider.GetService<ArticleBLL>().QueryArticleItems(kbId);
            return this.Success(result);
        }

        /// <summary>
        /// 文章详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Article>> Info(string id)
        {
            var result = await ServiceProvider.GetService<ArticleBLL>().GetById(id, GetUser());
            return this.Success(result);
        }

        /// <summary>
        /// 添加文章
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Article entity)
        {
            var rs = await ServiceProvider.GetService<ArticleBLL>().Add(entity, GetUser());
            return rs.ToAjaxResult();
        }

        /// <summary>
        /// 修改文章
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Update(MZ_Article entity)
        {
            var rs = await ServiceProvider.GetService<ArticleBLL>().Update(entity, GetUser());
            return rs.ToAjaxResult();
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Delete(string id)
        {
            var rs = await ServiceProvider.GetService<ArticleBLL>().Delete(id, GetUser());
            return rs.ToAjaxResult();
        }

        /// <summary>
        /// 增加文章的阅读次数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> View(string id)
        {
            var rs = await ServiceProvider.GetService<ArticleBLL>().IncrementViewCount(id);
            return rs.ToAjaxResult();
        }
    }
}