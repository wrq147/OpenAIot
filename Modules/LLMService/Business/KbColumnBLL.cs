using AuthService;
using Common.Share;
using LLMService.DAL;
using LLMService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace LLMService.Business
{
    public class KbColumnBLL
    {
        private readonly KbColumnDAL _columnDal;

        public KbColumnBLL(KbColumnDAL columnDal)
        {
            _columnDal = columnDal;
        }


        public async Task<MZ_KbColumn> GetById(string id, IUserInfo user)
        {
            return await _columnDal.SelectById(id, user.OrgId);
        }

        public async Task<List<MZ_KbColumn>> GetByKbId(string kbId)
        {
            return await _columnDal.SelectKbColumnList(kbId);
        }

        public async Task<List<MZ_KbColumn>> GetByParentId(string parentId, IUserInfo user)
        {
            return await _columnDal.SelectByParentId(parentId, user.OrgId);
        }

        public async Task<string> Add(MZ_KbColumn entity, IUserInfo user)
        {
            var kb = await _kbDal.SelectByIdAsync(entity.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                throw new Exception("知识库不存在或无权操作");
            }

            if (entity.ParentId > 0)
            {
                var parent = await _columnDal.SelectByIdAsync(entity.ParentId);
                if (parent == null || parent.KbId != entity.KbId)
                {
                    throw new Exception("父栏目不存在或不属于同一知识库");
                }
            }

            entity.Status = 1;
            entity.SortOrder = await _columnDal.GetMaxSortOrder(entity.KbId) + 1;
            entity.SetCreateBy(user);
            await _columnDal.InsertAsync(entity);

            return entity.Id;
        }

        public async Task<int> Update(MZ_KbColumn entity, IUserInfo user)
        {
            var existing = await _columnDal.SelectByIdAsync(entity.Id);
            if (existing == null)
            {
                throw new Exception("栏目不存在");
            }

            var kb = await _kbDal.SelectByIdAsync(existing.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                throw new Exception("无权修改此栏目");
            }

            existing.Name = entity.Name;
            existing.SortOrder = entity.SortOrder;
            existing.SetUpdateBy(user);

            await _columnDal.UpdateAsync(existing);
        }

        public async Task<int> Delete(string id, IUserInfo user)
        {
            var existing = await _columnDal.SelectByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("栏目不存在");
            }

            var kb = await _kbDal.SelectByIdAsync(existing.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                throw new Exception("无权删除此栏目");
            }

            var children = await _columnDal.SelectByParentId(id, user.OrgId);
            if (children.Any())
            {
                throw new Exception("请先删除子栏目");
            }

            await _columnDal.DeleteAsync(id);
        }

        public async Task<int> ChangeStatus(string id, int status, IUserInfo user)
        {
            var existing = await _columnDal.SelectByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("栏目不存在");
            }

            var kb = await _kbDal.SelectByIdAsync(existing.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                throw new Exception("无权修改此栏目");
            }

            existing.Status = status;
            existing.SetUpdateBy(user);
            await _columnDal.UpdateAsync(existing);
        }
    }
}