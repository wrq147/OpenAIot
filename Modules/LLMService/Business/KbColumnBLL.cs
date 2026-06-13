using AuthService;
using Common.IdGenerator;
using Common.Share;
using LLMService.DAL;
using LLMService.Model;
using MyAccess.DB;
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
        private readonly KnowledgeDAL _knowledgeDal;
        private ITAServiceProvider _provider;
        public KbColumnBLL(KbColumnDAL columnDal, KnowledgeDAL knowledgeDal, ITAServiceProvider provider)
        {
            _columnDal = columnDal;
            _knowledgeDal = knowledgeDal;
            _provider = provider;
        }


        public virtual async Task<MZ_KbColumn> GetById(string id, IUserInfo user)
        {
            return await _columnDal.Select(id);
        }

        public virtual async Task<List<MZ_KbColumn>> SelectAllClass(string kbId)
        {
            var user = _provider.GetUser();
            return await _columnDal.SelectKbColumnList(kbId);
        }
        public virtual async Task<BusResponse<int>> UpdateSort(List<string> idList)
        {
            try
            {
                return BusResponse<int>.Success(await _columnDal.UpdateSort(idList));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(112, ex.Message);
            }
        }

        private async Task _UpdateChildren(string newPath, string oldPath)
        {
            List<MZ_KbColumn> children = await _columnDal.SelectList(x => x.Path.StartsWith(oldPath));
            foreach (MZ_KbColumn child in children)
            {
                child.Path = newPath + child.Path.Substring(oldPath.Length);
            }
            if (children.Count > 0)
            {
                await _columnDal.UpdateChildrenPath(children);
            }
        }
        public virtual async Task<BusResponse<string>> Add(MZ_KbColumn entity, IUserInfo user)
        {
            var kb = await _knowledgeDal.Select(entity.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                return BusResponse<string>.Error(111, "知识库不存在或无权操作");
            }

            if (!string.IsNullOrEmpty(entity.ParentId))
            {
                var parent = await _columnDal.Select(entity.ParentId);
                if (parent == null || parent.KbId != entity.KbId)
                {
                    return BusResponse<string>.Error(112, "父栏目不存在或不属于同一知识库");
                }
            }
            var snowflake = _provider.GetService<SnowflakeHelper>();
            entity.Id = snowflake.NextId().ToString();
            entity.SetCreateBy(user);
            if (entity.SortOrder != null)
            {
                entity.SortOrder = 0;
            }
            await _columnDal.SortIncrease(entity.KbId, entity.SortOrder.Value);
            if (!string.IsNullOrEmpty(entity.ParentId))
            {
                var parent = await _columnDal.Select(entity.ParentId);
                if (parent == null)
                {
                    return BusResponse<string>.Error(113, "父分类不存在");
                }
                entity.Path = parent.Path + entity.Id + ",";
            }
            else
            {
                entity.Path = entity.Id + ",";
            }
            await _columnDal.Insert(entity);

            return BusResponse<string>.Success(entity.Id);
        }

        public virtual async Task<BusResponse<int>> Update(MZ_KbColumn entity, IUserInfo user)
        {
            var existing = await _columnDal.Select(entity.Id);
            if (existing == null)
            {
                return BusResponse<int>.Error(111, "栏目不存在");
            }

            var kb = await _knowledgeDal.Select(existing.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(112, "知识库不存在或无权操作");
            }
            if (entity.ParentId != null)
            {
                if (entity.ParentId == "")
                {
                    entity.Path = entity.Id + ",";
                }
                else
                {
                    var parent = await _columnDal.Select(entity.ParentId);
                    if (parent == null)
                    {
                        return BusResponse<int>.Error(113, "父分类不存在");
                    }
                    entity.Path = parent.Path + entity.Id + ",";
                }

                await _UpdateChildren(entity.Path, existing.Path);
            }
            if (entity.SortOrder != null)
            {
                await _columnDal.SortIncrease(existing.KbId, entity.SortOrder.Value);
            }
            entity.SetUpdateBy(user);
            return BusResponse<int>.Success(await _columnDal.Update(entity));
        }

        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            var existing = await _columnDal.Select(id);
            if (existing == null)
            {
                return BusResponse<int>.Error(111, "栏目不存在");
            }

            var kb = await _knowledgeDal.Select(existing.KbId);
            if (kb == null || kb.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(112, "知识库不存在或无权操作");
            }

            var children = await _columnDal.SelectList(x => x.ParentId == id);
            if (children.Any())
            {
                return BusResponse<int>.Error(113, "请先删除子栏目");
            }

            return BusResponse<int>.Success(await _columnDal.Delete(id));
        }

     
    }
}