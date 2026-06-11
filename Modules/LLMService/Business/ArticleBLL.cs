using AuthService;
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
        private string GetStatusName(int status)
        {
            return status == 1 ? "已发布" : "草稿";
        }

        private string GetDocTypeName(int docType)
        {
            switch (docType)
            {
                case 1: return "富文本";
                case 2: return "Markdown";
                case 3: return "PDF";
                case 4: return "Word";
                default: return "未知";
            }
        }
        public ArticleBLL(KnowledgeDAL kbDal, ArticleDAL articleDal)
        {
            _kbDal = kbDal;
            _articleDal = articleDal;
        }

        public virtual async Task<PageObject<MZ_Article>> QueryList(In_ArticleQuery query, IUserInfo user)
        {
            return await _articleDal.SelectByPage(query, user);
        }

        public virtual async Task<MZ_Article> GetById(string id, IUserInfo user)
        {
            return await _articleDal.SelectById(id, user.OrgId);
        }

        public virtual async Task<List<MZ_Article>> GetByKbId(string kbId, IUserInfo user)
        {
            return await _articleDal.SelectByKbId(kbId, user.OrgId);
        }

        public virtual async Task<string> Add(MZ_Article entity, IUserInfo user)
        {
            var kb = await _kbDal.SelectByIdAsync(entity.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                throw new Exception("知识库不存在或无权操作");
            }

            entity.Status = 1;
            entity.ViewCount = 0;
            entity.SetCreateBy(user);
            await _articleDal.InsertAsync(entity);

            return entity.Id;
        }

        public virtual async Task<int> Update(MZ_Article entity, IUserInfo user)
        {
            var existing = await _articleDal.SelectByIdAsync(entity.Id);
            if (existing == null)
            {
                throw new Exception("文章不存在");
            }

            var kb = await _kbDal.SelectByIdAsync(existing.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                throw new Exception("无权修改此文章");
            }

            existing.Title = entity.Title;
            existing.Content = entity.Content;
            existing.Cover = entity.Cover;
            existing.DocType = entity.DocType;
            existing.SetUpdateBy(user);

            await _articleDal.UpdateAsync(existing);
        }

        public virtual async Task<int> Delete(string id, IUserInfo user)
        {
            var existing = await _articleDal.SelectByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("文章不存在");
            }

            var kb = await _kbDal.SelectByIdAsync(existing.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                throw new Exception("无权删除此文章");
            }

            await _articleDal.DeleteAsync(id);
        }

        public virtual async Task<int> ChangeStatus(string id, int status, IUserInfo user)
        {
            var existing = await _articleDal.SelectByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("文章不存在");
            }

            var kb = await _kbDal.SelectByIdAsync(existing.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                throw new Exception("无权修改此文章");
            }

            existing.Status = status;
            existing.SetUpdateBy(user);
            await _articleDal.UpdateAsync(existing);
        }

        public virtual async Task<int> IncrementViewCount(string id, IUserInfo user)
        {
            await _articleDal.IncrementViewCount(id);
        }
    }
}