using AuthService;
using Common.IdGenerator;
using Common.Share;
using IoTRulesService.DAL;
using IoTRulesService.Model;
using IoTService.DAL;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService.Business
{
    public class RuleGroupBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private RuleGroupDAL _groupDAL;
        public RuleGroupBLL(ITAServiceProvider provider, RuleGroupDAL groupDAL, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _snowflake = snowflake;
            _groupDAL = groupDAL;
        }

        public virtual async Task<List<MZ_RuleGroup>> SelectAllOfOrg(IUserInfo user)
        {
            return await _groupDAL.SelectList((x) => x.OrgId == user.OrgId, "Sort asc");
        }

        public virtual async Task<MZ_RuleGroup> Info(string id)
        {
            return await _groupDAL.Select(id);
        }
        public virtual async Task<BusResponse<int>> UpdateSort(List<string> idList)
        {
            try
            {
                return BusResponse<int>.Success(await _groupDAL.UpdateSort(idList));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(112, ex.Message);
            }
        }
        private async Task _UpdateChildren(string newPath, string oldPath)
        {
            List<MZ_RuleGroup> children = await _groupDAL.SelectList(x => x.Path.StartsWith(oldPath));
            foreach (MZ_RuleGroup child in children)
            {
                child.Path = newPath + child.Path.Substring(oldPath.Length);
            }
            if (children.Count > 0)
            {
                await _groupDAL.UpdateChildrenPath(children);
            }
        }
        public virtual async Task<BusResponse<int>> Update(MZ_RuleGroup data)
        {
            data.OrgId = null;
            if (data.ParentId != null)
            {
                if (data.ParentId == data.Id)
                {
                    return BusResponse<int>.Error(111, "父级分组错误");
                }
                var old = await _groupDAL.Select(data.Id);
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
                    var parent = await _groupDAL.Select(data.ParentId);
                    if (parent == null)
                    {
                        return BusResponse<int>.Error(113, "父分组不存在");
                    }
                    data.Path = parent.Path + data.Id + ",";
                }

                await _UpdateChildren(data.Path, old.Path);
            }
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            data.SetUpdateBy(user);
            return BusResponse<int>.Success(await _groupDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Insert(MZ_RuleGroup data)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法添加设备分组");
            }
            data.Id = _snowflake.NextId().ToString();
            data.ParentId ??= string.Empty;
            data.Remark ??= string.Empty;
            data.OrgId = user.OrgId;
            if (!string.IsNullOrEmpty(data.ParentId))
            {
                var parent = await _groupDAL.Select(data.ParentId);
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
            return BusResponse<int>.Success(await _groupDAL.Insert(data));
        }

        public virtual async Task<BusResponse<int>> Remove(string id)
        {
            var old = await _groupDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(114, "分组不存在");
            }
            var ruleDAL = _provider.GetService<RuleTemplateDAL>();
            //判断分组下面是否有规则
            if (await ruleDAL.ExistRule(old.Path))
            {
                return BusResponse<int>.Error(115, "分组存在设备，无法删除");
            }
            return BusResponse<int>.Success(await _groupDAL.Delete(x => x.Path.StartsWith(old.Path)));
        }

    }
}
