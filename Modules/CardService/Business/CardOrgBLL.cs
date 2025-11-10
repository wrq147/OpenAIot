using AuthService;
using AuthService.Model;
using CardService.DAL;
using CardService.Model;
using Common;
using Common.IdGenerator;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CardService.Business
{
    public class CardOrgBLL : OrgBLL
    {

        private DeptDAL _deptDAL;
        private CardOrgDAL _cardOrgDAL;
        private CardDAL _cardDAL;

        public CardOrgBLL(ITAServiceProvider serviceProvider, CardDAL cardDAL, UserDAL userDAL, DeptDAL deptDAL, CardOrgDAL cardOrgDAL,
            OrgDAL orgDAL, SnowflakeHelper snowflake, ITAContext context) : base(serviceProvider, orgDAL, userDAL, snowflake, context)
        {
            _cardDAL = cardDAL;
            _deptDAL = deptDAL;
            _cardOrgDAL = cardOrgDAL;
        }
        public virtual async Task<List<MZ_User_Org_V>> SelectUserOrgListById(bool man)
        {
            IUserInfo user = Data_ServerTokenInfo.From(_context);
            return await _cardOrgDAL.SelectUserOrgListById(user.UserId, man);
        }
        protected override async Task OnEditUserOrg(MZ_User_Org uo)
        {
            Data_ServerTokenInfo user = Data_ServerTokenInfo.From(_context);
            //判断是否有当前组织的管理员权限
            if (!await _orgDAL.CheckManOrg(user.UserId, uo.OrgId.Value))
            {
                throw new Exception("没有当前组织的管理员权限");
            }
        }
        public virtual async Task<BusResponse<long>> JudgeBindId(string developId, string srcId)
        {
            string bindId = developId + srcId;
            var cardorg = await _cardOrgDAL.SelectByBindId(bindId);
            if (cardorg == null)
            {
                return BusResponse<long>.Success(0);
            }
            else
            {
                return BusResponse<long>.Success(cardorg.OrgId.Value);
            }
        }
        public override async Task<BusResponse<string>> ExitOrg(long id)
        {
            IUserInfo user = Data_ServerTokenInfo.From(_context);
            var rs = await base.ExitOrg(id);
            if (rs.IsSuccess())
            {
                //清除名片的组织信息
                await _cardDAL.ClearOrg(user.UserId, id);
            }
            return rs;
        }
        public async Task<BusResponse<string>> UpdateCardOrg(MZ_Card_Org cardOrg)
        {
            IUserInfo user = Data_ServerTokenInfo.From(_context);
            //判断是否有当前组织的管理员权限
            if (!await _orgDAL.CheckManOrg(user.UserId, cardOrg.OrgId.Value))
            {
                return BusResponse<string>.Error(102, "没有当前组织的管理员权限");
            }
            try
            {
                if (await _cardOrgDAL.ExistCardOrg(cardOrg.OrgId.Value))
                {
                    await _cardOrgDAL.UpdateCardOrg(cardOrg);
                    return BusResponse<string>.Success();
                }
                else
                {
                    cardOrg.CaseConfig ??= string.Empty;
                    cardOrg.ProConfig ??= string.Empty;
                    await _cardOrgDAL.Insert(cardOrg);
                    return BusResponse<string>.Success();
                }
            }
            catch
            {
                return BusResponse<string>.Error(112, "数据繁忙,请重试");
            }
        }
        public override async Task OnDeleteUserFromOrg(long uid, long orgId)
        {
            Data_ServerTokenInfo user = Data_ServerTokenInfo.From(_context);
            //判断是否有当前组织的管理员权限
            if (!await _orgDAL.CheckManOrg(uid, user.OrgId))
            {
                throw new Exception("没有当前组织的管理员权限");
            }
        }
        protected override async Task OnDelete(MZ_Org org, Data_ServerTokenInfo user)
        {
            await _cardOrgDAL.DeleteCardOrg(org.Id.Value);
        }

        protected override async Task<BusResponse<long>> OnCreate(MZ_Org org, IUserInfo user)
        {
            MZ_Card_Org cardOrg = new MZ_Card_Org();
            cardOrg.CaseConfig = string.Empty;
            cardOrg.OrgId = org.Id;
            cardOrg.ProConfig = string.Empty;
            if (org is In_Org inorg)
            {
                cardOrg.BindId = inorg.BindId;
            }
            await _cardOrgDAL.Insert(cardOrg);

            return await base.OnCreate(org, user);
        }

        public override async Task<MZ_Org> SelectById(long id)
        {
            return await _cardOrgDAL.SelectCardOrgById(id);
        }
        public virtual async Task<BusResponse<MZ_User_Org>> SelectUserOrg(long orgId)
        {
            IUserInfo user = Data_ServerTokenInfo.From(_context);
            MZ_User_Org userOrg = await _orgDAL.SelectUserOrg(user.UserId, orgId);
            return BusResponse<MZ_User_Org>.Success(userOrg);
        }
        public virtual async Task<BusResponse<MZ_YQCode>> ParseYqCode(string code)
        {
            var yq = await _provider.GetService<GeneralRedisHelper>().StringGetAsync<MZ_YQCode>("yq_" + code);
            if (yq == null)
            {
                return BusResponse<MZ_YQCode>.Error(111, "已超时,请重新操作");
            }
            if (yq.OrgId != null)
            {
                var org = await _orgDAL.SelectById(yq.OrgId.Value);
                yq.OrgName = org.OrgName;
            }
            if (yq.DeptId != null)
            {
                var dept = await _deptDAL.SelectById(yq.DeptId.Value);
                yq.DeptName = dept.dept_name;
            }
            await _provider.GetService<GeneralRedisHelper>().KeyExpireAsync("yq_" + code, TimeSpan.FromMinutes(5));
            return BusResponse<MZ_YQCode>.Success(yq);
        }
        public virtual async Task<BusResponse<string>> Join(In_Join data)
        {
            IUserInfo user = Data_ServerTokenInfo.From(_context);
            if (data.depId != null)
            {
                if (!_deptDAL.CheckDeptId(data.depId.Value, data.orgId))
                {
                    return BusResponse<string>.Error(103, "要加入的部门不存在");
                }
            }
            if (await _orgDAL.CheckExistOrg(user.UserId, data.orgId))
            {
                return BusResponse<string>.Error(105, "无法重复加入企业");
            }


            MZ_User_Org userOrg = new MZ_User_Org();
            if (data.depId != null)
            {
                userOrg.dept_id = data.depId;
            }
            else
            {
                var rootDept = await _deptDAL.SelectRoot(data.orgId);
                userOrg.dept_id = rootDept.dept_id;
            }
            userOrg.OrgId = data.orgId;
            userOrg.UserId = user.UserId;
            userOrg.post_name = data.postName ?? "";
            userOrg.IsLeader = false;
            userOrg.IsPrimary = true;
            MZ_AdminInfo upAdmin = new MZ_AdminInfo();
            upAdmin.Id = user.UserId;
            upAdmin.OrgId = userOrg.OrgId;
            await _orgDAL.InsertUserOrg(userOrg);
            upAdmin.SetUpdateBy(user);
            if (!string.IsNullOrEmpty(data.realName))
            {
                upAdmin.RealName = data.realName;
            }
            await _userDAL.UpdateUser(upAdmin);
            return BusResponse<string>.Success();
        }
        /// <summary>
        /// 邀请加入企业
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<string>> Join(In_JoinOrg data)
        {
            IUserInfo user = Data_ServerTokenInfo.From(_context);
            if (data.deptId != null)
            {
                if (!_deptDAL.CheckDeptId(data.deptId.Value, user.OrgId))
                {
                    return BusResponse<string>.Error(103, "要加入的部门不存在");
                }
            }

            var redis = _provider.GetService<GeneralRedisHelper>();
            var yqlockkey = "yq_lock_" + data.code;
            if (await redis.WaitLockTakeAsync(yqlockkey))
            {
                try
                {
                    var yq = await _provider.GetService<GeneralRedisHelper>().StringGetAsync<MZ_YQCode>("yq_" + data.code);
                    if (yq == null)
                    {
                        return BusResponse<string>.Error(104, "邀请码已过期");
                    }
                    if (yq.Limit)
                    {
                        await _provider.GetService<GeneralRedisHelper>().KeyDeleteAsync(data.code);
                    }

                    if (yq.OrgId <= 0)
                    {
                        if (await _orgDAL.CheckOrgNameUnique(yq.OrgName))
                        {
                            return BusResponse<string>.Error(111, "公司名称已经被认证");
                        }
                        //更新邀请人数
                        yq.YQCount++;
                        await _provider.GetService<GeneralRedisHelper>().StringSetAsync("yq_" + data.code, yq, TimeSpan.FromMinutes(5));

                        //创建企业
                        MZ_Org org = new MZ_Org();
                        org.AddressCode = string.Empty;
                        org.AddressDetail = string.Empty;
                        org.Intro = string.Empty;
                        org.Industry = 0;
                        org.OrgName = yq.OrgName;
                        org.Lng = 0;
                        org.Lat = 0;
                        org.del_flag = "0";
                        org.status = "0";
                        org.Size = 0;

                        MZ_User_Org userOrg = new MZ_User_Org();
                        userOrg.post_name = data.postName ?? "";
                        await Create(org, false, userOrg);

                        MZ_AdminInfo upAdmin = new MZ_AdminInfo();
                        upAdmin.Id = user.UserId;
                        upAdmin.OrgId = userOrg.OrgId;
                        upAdmin.SetUpdateBy(user);
                        upAdmin.RealName = data.realName ?? "";
                        await _userDAL.UpdateUser(upAdmin);

                        return BusResponse<string>.Success();
                    }
                    else
                    {
                        if (await _orgDAL.CheckExistOrg(user.UserId, yq.OrgId.Value))
                        {
                            return BusResponse<string>.Error(105, "无法重复加入企业");
                        }
                        //更新邀请人数
                        yq.YQCount++;
                        await _provider.GetService<GeneralRedisHelper>().StringSetAsync("yq_" + data.code, yq, TimeSpan.FromMinutes(5));
                        MZ_User_Org userOrg = new MZ_User_Org();
                        if (data.deptId == null)
                        {
                            if (yq.DeptId > 0)
                            {
                                userOrg.dept_id = yq.DeptId;
                            }
                        }
                        else
                        {
                            userOrg.dept_id = data.deptId;
                        }
                        userOrg.OrgId = yq.OrgId;
                        userOrg.UserId = user.UserId;
                        userOrg.post_name = data.postName ?? "";
                        userOrg.IsLeader = false;
                        userOrg.IsPrimary = true;
                        MZ_AdminInfo upAdmin = new MZ_AdminInfo();
                        upAdmin.Id = user.UserId;
                        upAdmin.OrgId = userOrg.OrgId;
                        await _orgDAL.InsertUserOrg(userOrg);
                        upAdmin.SetUpdateBy(user);
                        if (!string.IsNullOrEmpty(data.realName))
                        {
                            upAdmin.RealName = data.realName;
                        }
                        await _userDAL.UpdateUser(upAdmin);

                        return BusResponse<string>.Success();
                    }

                }
                catch (Exception ex)
                {
                    return BusResponse<string>.Error(110, ex.Message);
                }
                finally
                {
                    await redis.LockReleaseAsync(yqlockkey);
                }

            }
            else
            {
                return BusResponse<string>.ErrorBusy();
            }
        }

        public virtual async Task<BusResponse<string>> Invite(MZ_YQCode data, IUserInfo user)
        {
            if (data.OrgId != null)
            {
                //判断是否有当前组织的管理员权限
                if (!await _orgDAL.CheckManOrg(user.UserId, data.OrgId.Value))
                {
                    return BusResponse<string>.Error(102, "没有当前组织的管理员权限");
                }
                MZ_Org org = await _orgDAL.SelectById(data.OrgId.Value);
                if (org == null)
                {
                    return BusResponse<string>.Error(104, "邀请的组织不存在");
                }
                data.OrgName = org.OrgName;
            }
            if (data.DeptId != null)
            {
                MZ_Dept dpt = await _deptDAL.SelectById(data.DeptId.Value);
                if (dpt.OrgId != data.OrgId)
                {
                    return BusResponse<string>.Error(103, "要加入的部门不存在");
                }
                data.DeptName = dpt.dept_name;
            }
            data.YQCount = 0;
            var redis = _provider.GetService<GeneralRedisHelper>();
            string successCode = string.Empty;
            int maxCount = 20;
            while (maxCount > 0)
            {
                maxCount--;

                string code = MyAccess.Core.StringTool.GetDigitChar(4);
                if (data.YQCodeType == 1)
                {
                    code = MyAccess.Core.StringTool.GetGUID();
                }

                string yqkey = "yq_" + code;
                var yq = await redis.StringGetAsync<MZ_YQCode>(yqkey);
                if (yq == null)
                {
                    string lockkey = "yq_lock_" + code;
                    if (await redis.LockTakeAsync(lockkey))
                    {
                        try
                        {
                            yq = await redis.StringGetAsync<MZ_YQCode>(yqkey);
                            if (yq != null)
                            {
                                continue;
                            }
                            await redis.StringSetAsync<MZ_YQCode>(yqkey, data, TimeSpan.FromMinutes(5));
                            successCode = code;
                            break;
                        }
                        finally
                        {
                            await redis.LockReleaseAsync(lockkey);
                        }

                    }
                }
            }
            if (!string.IsNullOrEmpty(successCode))
            {
                return BusResponse<string>.Success(successCode);
            }
            else
            {
                return BusResponse<string>.ErrorBusy();
            }

        }

    }
}
