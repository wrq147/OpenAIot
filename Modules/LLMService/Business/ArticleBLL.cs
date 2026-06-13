using AuthService;
using Common.IdGenerator;
using Common.Share;
using LLMService.DAL;
using LLMService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace LLMService.Business
{
    public class ArticleBLL
    {
        private readonly KnowledgeDAL _kbDal;
        private readonly ArticleDAL _articleDal;
        private ITAServiceProvider _provider;
        public ArticleBLL(KnowledgeDAL kbDal, ArticleDAL articleDal, ITAServiceProvider provider)
        {
            _kbDal = kbDal;
            _articleDal = articleDal;
            _provider = provider;

        }

        public virtual async Task<PageObject<MZ_Article>> QueryList(In_ArticleQuery query, IUserInfo user)
        {
            return await _articleDal.SelectByPage(query, user);
        }

        public virtual async Task<MZ_Article> GetById(string id, IUserInfo user)
        {
            var article = await _articleDal.Select(id);
            article.KbColumn = await _provider.GetService<KbColumnDAL>().Select(article.ColumnId);
            article.Kb = await _provider.GetService<KnowledgeDAL>().Select(article.KbId);
            return article;
        }


        public virtual async Task<BusResponse<string>> Add(MZ_Article entity, IUserInfo user)
        {
            var kb = await _kbDal.Select(entity.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                return BusResponse<string>.Error(111, "知识库不存在或无权操作");
            }

            var snowflake = _provider.GetService<SnowflakeHelper>();
            entity.Id = snowflake.NextId().ToString();
            entity.ViewCount = 0;
            entity.SetCreateBy(user);
            await _articleDal.Insert(entity);

            MZ_Knowledge kn = new MZ_Knowledge();
            kn.Id = entity.KbId;
            kn.DocCount = await _articleDal.Count(x => x.KbId == entity.KbId);
            await _kbDal.Update(kn, x => x.Id == entity.KbId);

            return BusResponse<string>.Success(entity.Id);
        }

        public virtual async Task<BusResponse<int>> Update(MZ_Article entity, IUserInfo user)
        {
            var existing = await _articleDal.Select(entity.Id);
            if (existing == null)
            {
                return BusResponse<int>.Error(111, "文章不存在");
            }

            var kb = await _kbDal.Select(existing.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(112, "知识库不存在或无权操作");
            }

            entity.SetUpdateBy(user);
            return BusResponse<int>.Success(await _articleDal.Update(entity));
        }

        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            var existing = await _articleDal.Select(id);
            if (existing == null)
            {
                return BusResponse<int>.Error(111, "文章不存在");
            }

            var kb = await _kbDal.Select(existing.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(112, "知识库不存在或无权操作");
            }
            int rt = await _articleDal.Delete(id);
            MZ_Knowledge kn = new MZ_Knowledge();
            kn.Id = existing.KbId;
            kn.DocCount = await _articleDal.Count(x => x.KbId == existing.KbId);
            await _kbDal.Update(kn, x => x.Id == existing.KbId);

            return BusResponse<int>.Success(rt);
        }

      

        public virtual async Task<BusResponse<int>> IncrementViewCount(string id)
        {
            return BusResponse<int>.Success(await _articleDal.IncrementViewCount(id));
        }
    }
}