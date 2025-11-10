using AuthService.DAL;
using AuthService.Model;
using Common.IdGenerator;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService.Business
{
    public class GroupViewBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private GroupViewDAL _groupViewDAL;
        public GroupViewBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, GroupViewDAL groupViewDAL)
        {
            _snowflake = snowflake;
            _provider = provider;
            _groupViewDAL = groupViewDAL;
        }

        public virtual async Task<List<MZ_GroupView>> SelectGroupList(string table, IUserInfo user)
        {
            return await _groupViewDAL.SelectList((x) => x.OrgId == user.OrgId && x.TableName == table, "Sort asc", "Id,OrgId,Name,LevelCode,PhotoUrl,Sort");
        }
        public virtual async Task<MZ_GroupView> Info(string id)
        {
            return await _groupViewDAL.Select(id);
        }

        public virtual async Task<BusResponse<int>> UpdateSort(List<string> idList)
        {
            try
            {
                return BusResponse<int>.Success(await _groupViewDAL.UpdateSort(idList));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(112, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> Insert(MZ_GroupView data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(133, "请切换到企业账号");
            }

            if (string.IsNullOrEmpty(data.Name))
            {
                return BusResponse<int>.Error(113, "分组名称不能为空");
            }

            var tmplist = await _groupViewDAL.SelectList(x => x.OrgId == user.OrgId && x.Name == data.Name);
            if (tmplist.Count > 0)
            {
                return BusResponse<int>.Error(114, "分组名称已存在");
            }

            data.Id = _snowflake.NextId().ToString();
            data.ConditionJson ??= string.Empty;
            data.ListFieldsJson ??= string.Empty;
            data.PhotoUrl ??= string.Empty;
            data.OrgId = user.OrgId;
            if (data.Sort == null)
            {
                data.Sort = 0;
            }
            await _groupViewDAL.SortIncrease(data.OrgId.Value, data.Sort.Value);
            return BusResponse<int>.Success(await _groupViewDAL.Insert(data));
        }

        public virtual async Task<BusResponse<int>> Update(MZ_GroupView data, IUserInfo user)
        {
            var old = await _groupViewDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(113, "分组不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(114, "分组所属组织错误");
            }
            if (data.Name != null)
            {
                var tmplist = await _groupViewDAL.SelectList(x => x.OrgId == user.OrgId && x.Id != data.Id && x.Name == data.Name);
                if (tmplist.Count > 0)
                {
                    return BusResponse<int>.Error(115, "分组名称已存在");
                }
            }
            data.OrgId = null;
            if (data.Sort != null)
            {
                await _groupViewDAL.SortIncrease(old.OrgId.Value, data.Sort.Value);
            }
            return BusResponse<int>.Success(await _groupViewDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Remove(string id, IUserInfo user)
        {
            var old = await _groupViewDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(113, "分组不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(114, "分组所属组织错误");
            }
            return BusResponse<int>.Success(await _groupViewDAL.Delete(x => x.Id == id));
        }
    }
}
