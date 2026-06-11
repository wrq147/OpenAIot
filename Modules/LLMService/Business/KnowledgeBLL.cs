using AuthService;
using Common.Share;
using LLMService.DAL;
using LLMService.Model;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace LLMService.Business
{
    public class KnowledgeBLL
    {
        private readonly KnowledgeDAL _kbDal;

        public KnowledgeBLL(KnowledgeDAL kbDal)
        {
            _kbDal = kbDal;
        }

        private string GetPermissionTypeName(long orgId)
        {
            if (orgId > 0)
            {
                return "私有文库";
            }
            else
            {
                return "公开文库";
            }
        }
        public async Task<PageObject<MZ_Knowledge>> QueryList(In_KnowledgeQuery query, IUserInfo user)
        {
            var result = await _kbDal.SelectByPage(query, user);
            return result;
        }

        public async Task<MZ_Knowledge> GetById(string id, IUserInfo user)
        {
            var result = await _kbDal.SelectList(x => x.Id == id && (x.OrgId == 0 || x.OrgId == user.OrgId));
            if (result != null)
            {
                result.DocCount = await _kbDal.GetDocCount(id);
            }
            return result;
        }

        public async Task<string> Add(MZ_Knowledge entity, IUserInfo user)
        {
            entity.OrgId = user.OrgId;
            entity.Status = 1;
            entity.DocCount = 0;
            entity.SetCreateBy(user);
            await _kbDal.InsertAsync(entity);
            return entity.Id;
        }

        public async Task<int> Update(MZ_Knowledge entity, IUserInfo user)
        {
            var existing = await _kbDal.SelectByIdAsync(entity.Id);
            if (existing == null || existing.OrgId != user.OrgId)
            {
                throw new Exception("知识库不存在或无权修改");
            }

            existing.Cover = entity.Cover;
            existing.Name = entity.Name;
            existing.Description = entity.Description;
            existing.PermissionType = entity.PermissionType;
            existing.SetUpdateBy(user);

            await _kbDal.UpdateAsync(existing);
        }

        public async Task<int> Delete(string id, IUserInfo user)
        {
            var entity = await _kbDal.SelectByIdAsync(id);
            if (entity == null || entity.OrgId != user.OrgId)
            {
                throw new Exception("知识库不存在或无权删除");
            }

            await _kbDal.DeleteAsync(id);
        }

        public async Task<int> ChangeStatus(string id, int status, IUserInfo user)
        {
            var entity = await _kbDal.SelectByIdAsync(id);
            if (entity == null || entity.OrgId != user.OrgId)
            {
                throw new Exception("知识库不存在或无权修改");
            }

            entity.Status = status;
            entity.SetUpdateBy(user);
            await _kbDal.UpdateAsync(entity);
        }
    }
}