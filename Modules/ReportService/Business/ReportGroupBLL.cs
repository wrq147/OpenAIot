using AuthService;
using Common.IdGenerator;
using Common.Share;
using Quartz.Impl.AdoJobStore.Common;
using ReportService.DAL;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
namespace ReportService.Business
{
    public class ReportGroupBLL
    {
        private SnowflakeHelper _snowflake;
        private ReportGroupDAL _group;
        private ITAServiceProvider _provider;
        public ReportGroupBLL(ReportGroupDAL group, SnowflakeHelper snowflake, ITAServiceProvider provider)
        {
            _snowflake = snowflake;
            _group = group;
            _provider = provider;
        }
        public virtual async Task<List<MZ_ReportGroup>> SelectAllOfOrg(IUserInfo user)
        {
            return await _group.SelectList((x) => x.OrgId == user.OrgId, "Sort asc");
        }
        public virtual async Task<MZ_ReportGroup> Info(string id)
        {
            return await _group.Select(id);
        }
        public virtual async Task<BusResponse<int>> InsertGroup(MZ_ReportGroup data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法添加报表分组");
            }
            data.Id = _snowflake.NextId().ToString();
            data.ParentId ??= string.Empty;
            data.OrgId = user.OrgId;
            if (!string.IsNullOrEmpty(data.ParentId))
            {
                var parent = await _group.Select(data.ParentId);
                if (parent == null)
                {
                    return BusResponse<int>.Error(113, "父分组不存在");
                }
                data.Path = parent.Path + data.Id + ",";
            }
            else
            {
                data.Path = data.Id + ",";
            }
            data.SetCreateBy(user);
            return BusResponse<int>.Success(await _group.Insert(data));
        }



        public virtual async Task<BusResponse<int>> UpdateGroup(MZ_ReportGroup data, IUserInfo user)
        {
            data.OrgId = null;
            if (data.ParentId != null)
            {
                if (data.ParentId == data.Id)
                {
                    return BusResponse<int>.Error(111, "父级分组错误");
                }
                var old = await _group.Select(data.Id);
                if (old == null)
                {
                    return BusResponse<int>.Error(112, "分组不存在");
                }
                if (data.ParentId == "")
                {
                    data.Path = data.Id + ",";
                }
                else
                {
                    var parent = await _group.Select(data.ParentId);
                    if (parent == null)
                    {
                        return BusResponse<int>.Error(113, "父分组不存在");
                    }
                    data.Path = parent.Path + data.Id + ",";
                }

                await _UpdateChildren(data.Path, old.Path);
            }
            data.SetUpdateBy(user);
            return BusResponse<int>.Success(await _group.Update(data));
        }

        public virtual async Task<BusResponse<int>> DeleteGroup(string id, IUserInfo user)
        {
            var old = await _group.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(114, "分组不存在");
            }
            var reportDAL = _provider.GetService<ReportDAL>();
            //判断分组下面是否有报表
            if (await reportDAL.ExistReport(old.Path))
            {
                return BusResponse<int>.Error(115, "分组存在报表，无法删除");
            }
            return BusResponse<int>.Success(await _group.Delete(x => x.Path.StartsWith(old.Path)));
        }
        private async Task _UpdateChildren(string newPath, string oldPath)
        {
            List<MZ_ReportGroup> children = await _group.SelectList(x => x.Path.StartsWith(oldPath));
            foreach (MZ_ReportGroup child in children)
            {
                child.Path = newPath + child.Path.Substring(oldPath.Length);
            }
            if (children.Count > 0)
            {
                await _group.UpdateChildrenPath(children);
            }
        }
    }
}
