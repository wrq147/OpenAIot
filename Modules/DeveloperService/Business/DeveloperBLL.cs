using AuthService;
using Common;
using Common.IdGenerator;
using Common.Share;
using DeveloperService.DAL;
using DeveloperService.Model;
using MyAccess.Aop;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TemplateAction.Core;

namespace DeveloperService.Business
{
    public class DeveloperBLL
    {
        private ITAServiceProvider _provider;
        private DeveloperDAL _developer;
        private SnowflakeHelper _snowflake;
        public DeveloperBLL(ITAServiceProvider provider, DeveloperDAL developer, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _developer = developer;
            _snowflake = snowflake;
        }
        public virtual async Task<PageObject<MZ_Developer>> ListPage(In_DeveloperPage query)
        {
            return await _developer.SelectListPage(query);
        }
        public virtual async Task<BusResponse<MZ_Developer>> Profile()
        {
            var user = _provider.GetUser();
            var orgDAL = _provider.GetService<OrgDAL>();
            if (user.OrgId > 0)
            {
                if (await orgDAL.CheckRoleOrg(user.UserId, user.OrgId, 5))
                {
                    var entlist = await _developer.SelectList(x => x.UserType == 1 && x.OrgId == user.OrgId);
                    if (entlist.Count > 0)
                    {
                        return BusResponse<MZ_Developer>.Success(entlist.FirstOrDefault());
                    }
                }
            }

            var tmplist = await _developer.SelectList(x => x.UserType == 0 && x.UserId == user.UserId);
            return BusResponse<MZ_Developer>.Success(tmplist.FirstOrDefault());
        }
        public virtual async Task<MZ_Developer> InfoByOrg(long orgId)
        {
            var infolist = await _developer.SelectList(x => x.OrgId == orgId);
            if (infolist.Count > 0)
            {
                return infolist.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
        public virtual async Task<BusResponse<MZ_Developer>> Info(string id)
        {
            var info = await _developer.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_Developer>.Error(111, "开发者不存在");
            }

            return BusResponse<MZ_Developer>.Success(info);
        }

        public virtual async Task<BusResponse<int>> Insert(MZ_Developer data)
        {
            data.DevId = _snowflake.NextId().ToString();
            data.SecKey = MyAccess.Core.StringTool.GetGUID();
            data.CreateOn = DateTime.Now;
            data.UpdatedOn = data.CreateOn;

            MZ_UserRole ur = new MZ_UserRole();
            if (data.UserType == 0)
            {
                if (data.UserId == null || data.UserId <= 0)
                {
                    return BusResponse<int>.Error(111, "开发者不能为空");
                }
                if (await _developer.Some(x => x.UserId == data.UserId))
                {
                    return BusResponse<int>.Error(113, "无法重复添加个人为开发者");
                }
                data.OrgId = 0;
                ur.UserId = data.UserId;
                ur.RoleID = 6;
                ur.OrgId = 0;

            }
            else if (data.UserType == 1)
            {
                if (data.OrgId == null || data.OrgId <= 0)
                {
                    return BusResponse<int>.Error(112, "开发者不能为空");
                }
                if (await _developer.Some(x => x.OrgId == data.OrgId))
                {
                    return BusResponse<int>.Error(113, "无法重复添加组织为开发者");
                }
                MZ_Org tmpOrg = await _provider.GetService<OrgBLL>().SelectById(data.OrgId.Value);
                data.UserId = tmpOrg.createId;
                ur.UserId = data.UserId;
                ur.RoleID = 5;
                ur.OrgId = data.OrgId;
            }
            else
            {
                return BusResponse<int>.Error(122, "开发者类型错误");
            }
            var userDAL = _provider.GetService<UserDAL>();
            using (BLLTranScope scope = new BLLTranScope())
            {
                await userDAL.DeleteUserRoleById(ur.RoleID.Value, ur.UserId.Value, ur.OrgId.Value);
                await _provider.GetService<UserDAL>().AddUserRoleItem(ur);
                var rt = await _developer.Insert(data);
                // 完成
                await scope.CompleteAsync();
                return BusResponse<int>.Success(rt);
            }

        }
        public virtual async Task<BusResponse<int>> Edit(MZ_Developer data)
        {
            if (data.KeyType != null)
            {
                data.SecKey = MyAccess.Core.StringTool.GetGUID();
                await _provider.GetService<GeneralRedisHelper>().KeyDeleteAsync("developer:" + data.DevId);
            }
            data.UserId = null;
            data.OrgId = null;
            data.UserType = null;
            data.UpdatedOn = DateTime.Now;
            var rt = await _developer.Update(data);
            return BusResponse<int>.Success(rt);
        }
        public virtual async Task<BusResponse<int>> Remove(string id)
        {
            var info = await _developer.Select(id);
            if (info == null)
            {
                return BusResponse<int>.Error(111, "开发者不存在");
            }
            if (info.UserType == 0)
            {
                await _provider.GetService<UserDAL>().DeleteUserRoleById(6, info.UserId.Value, 0);
            }
            else if (info.UserType == 1)
            {
                await _provider.GetService<UserDAL>().DeleteUserRoleByOrg(5, info.OrgId.Value);
            }
            await _provider.GetService<GeneralRedisHelper>().KeyDeleteAsync("developer:" + id);

            return BusResponse<int>.Success(await _developer.Delete(id));
        }
        public virtual async Task ChangeOrgUser(long orgId, long uid)
        {
            MZ_Developer data = new MZ_Developer();
            data.UserId = uid;
            await _developer.Update(data, x => x.OrgId == orgId);
        }

    }
}
