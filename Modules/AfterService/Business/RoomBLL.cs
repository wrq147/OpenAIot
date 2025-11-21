using AuthService;
using Common.IdGenerator;
using Common.Share;
using AfterService.DAL;
using AfterService.Model;
using System.Linq.Expressions;
using TemplateAction.Core;
using MyAccess.DB.Builder.WhereToSql;
using MyAccess.Aop;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AfterService.Business
{
    public class RoomBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private RoomDAL _roomDAL;
        public RoomBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, RoomDAL roomDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _roomDAL = roomDAL;
        }
        public virtual async Task<List<MZ_Room>> QueryList(In_RoomList query, IUserInfo user, DataScope scope = null)
        {
            Expression<Func<MZ_Room, bool>> expression = x => x.OrgId == user.OrgId;
            if (query.TargetOrgId != null)
            {
                expression = expression.And(x => x.TargetOrgId == query.TargetOrgId);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expression = expression.And(x => x.Name.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.CategoryId))
            {
                var categorydata = await _provider.GetService<RoomCategoryDAL>().Select(query.CategoryId);
                if (categorydata != null)
                {
                    string isexistCategory = "EXISTS(select Id from mz_room_category where mz_room.CategoryId=Id and Path like '" + categorydata.Path + "%')";
                    expression = expression.And(x => SonSqlFun.SqlCondition(isexistCategory));
                }
            }

            if (scope != null)
            {
                List<string> keys = new List<string>();
                keys.Add(user.UserId.ToString());
                string tsqlmatch = _roomDAL.GetUsingDbHelp().CreateCompatible().FullSearch("Helper", keys);

                string scopestr = scope.GenerateFilter("DeptId", "LeaderId", "LeaderId=" + user.UserId + " or " + tsqlmatch, false, false, false);
                if (!string.IsNullOrEmpty(scopestr))
                {
                    expression = expression.And(x => SonSqlFun.SqlCondition(scopestr));
                }
            }
            var tlist = await _roomDAL.SelectList(expression);

            var categoryDict = await _provider.GetService<RoomCategoryDAL>().NavigateDict<MZ_Room, string>(tlist, x => true, x => x.CategoryId);
            var leaderDict = await _provider.GetService<UserDAL>().NavigateDict<MZ_Room, long?>(tlist, x => true, x => x.LeaderId);
            foreach (var item in tlist)
            {
                MZ_AdminInfo tmpuser;
                if (leaderDict.TryGetValue(item.LeaderId, out tmpuser))
                {
                    item.LeaderInfo = tmpuser;
                }

                MZ_RoomCategory tmpcategory;
                if (categoryDict.TryGetValue(item.CategoryId, out tmpcategory))
                {
                    item.CategoryName = tmpcategory.Name;
                }
            }
            return tlist;
        }

        public virtual async Task<BusResponse<string>> Add(MZ_Room data)
        {
            try
            {
                var user = _provider.GetUser();
                if (user.OrgId <= 0)
                {
                    return BusResponse<string>.Error(112, "非企业用户无法添加房间");
                }
                data.Id = _snowflake.NextId().ToString();
                data.OrgId = user.OrgId;
                data.TargetOrgId ??= 0;
                data.Remark ??= string.Empty;
                data.Helper ??= string.Empty;
                if (data.LeaderId == null)
                {
                    data.LeaderId = 0;
                    data.DeptId = 0;
                }
                else
                {
                    if (data.LeaderId <= 0)
                    {
                        return BusResponse<string>.Error(121, "请选择正确的负责人");
                    }
                    var userOrg = await _provider.GetService<AuthService.OrgDAL>().SelectUserOrg(data.LeaderId.Value, user.OrgId);
                    if (userOrg == null)
                    {
                        data.DeptId = 0;
                    }
                    else
                    {
                        data.DeptId = userOrg.dept_id;
                    }
                }
                data.SetCreateBy(user);
                await _roomDAL.Insert(data);

                if (data.AutoAdd == true && data.TargetOrgId > 0)
                {
                    //添加客户设备到房间
                    KFDeviceDAL deviceDAL = _provider.GetService<KFDeviceDAL>();
                    var tlist = await deviceDAL.SelectKFDeviceList(user, data.TargetOrgId.Value);
                    if (tlist.Count > 0)
                    {
                        List<MZ_RoomDevice> troomDeviceList = new List<MZ_RoomDevice>();
                        foreach (var t in tlist)
                        {
                            MZ_RoomDevice roomDevice = new MZ_RoomDevice();
                            roomDevice.Id = data.Id;
                            roomDevice.TargetId = t.Id;
                            roomDevice.OrgId = data.OrgId;
                            troomDeviceList.Add(roomDevice);
                        }
                        await _provider.GetService<RoomDeviceDAL>().InsertOrIgnore(troomDeviceList);
                    }

                }


                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }
        public virtual async Task<BusResponse<string>> Edit(MZ_Room data)
        {
            try
            {
                MZ_Room old = await _roomDAL.Select(data.Id);
                if (old == null)
                {
                    return BusResponse<string>.Error(123, "房间不存在");
                }
                var user = _provider.GetUser();
                if (user.OrgId != old.OrgId)
                {
                    return BusResponse<string>.Error(124, "当前用户无权限");
                }
                if (user.OrgId <= 0)
                {
                    return BusResponse<string>.Error(112, "非企业用户无法添加房间");
                }
                if (data.LeaderId != null)
                {
                    if (data.LeaderId > 0)
                    {
                        var userOrg = await _provider.GetService<AuthService.OrgDAL>().SelectUserOrg(data.LeaderId.Value, user.OrgId);
                        data.DeptId = userOrg.dept_id;
                    }
                    else
                    {
                        data.LeaderId = 0;
                        data.DeptId = 0;
                    }
                }
                data.OrgId = null;
                data.SetUpdateBy(user);
                await _roomDAL.Update(data);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }

        }

        public virtual async Task<BusResponse<string>> Delete(string id)
        {
            var user = _provider.GetUser();
            MZ_Room old = await _roomDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "房间不存在");
            }

            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            var deviceDAL = _provider.GetService<RoomDeviceDAL>();

            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await deviceDAL.Delete(x => x.Id == id);
                    await _roomDAL.Delete(id);
                    // 完成
                    await scope.CompleteAsync();
                }
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<MZ_Room>> Info(string id)
        {
            var info = await _roomDAL.GetRoomInfo(id);
            if (info == null)
            {
                return BusResponse<MZ_Room>.Error(123, "房间不存在");
            }
            if (!string.IsNullOrEmpty(info.CustomerId))
            {
                var tmpname = await _roomDAL.GetRoomCustomerName(info.CustomerId);
                if (!string.IsNullOrEmpty(tmpname))
                {
                    info.TargetName = tmpname;
                }
            }
            else
            {
                if (info.TargetOrgInfo != null)
                {
                    info.TargetName = info.TargetOrgInfo.OrgName;
                }
            }
            if (!string.IsNullOrEmpty(info.Helper))
            {
                var tmpstrs = info.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var ids = Array.ConvertAll(tmpstrs, s => long.Parse(s));
                info.HelperUsers = await _provider.GetService<UserDAL>().GetUserListByIds(ids.ToList());
            }
            return BusResponse<MZ_Room>.Success(info);
        }
    }
}
