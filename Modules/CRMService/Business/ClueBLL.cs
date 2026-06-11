using AuthService;
using Common.IdGenerator;
using Common.Share;
using CRMService.DAL;
using CRMService.Model;
using DiscussService.DAL;
using DiscussService.Model;
using Microsoft.Extensions.DependencyInjection;
using MyAccess.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CRMService.Business
{
    public class ClueBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private ClueDAL _clueDAL;
        private UserDAL _userDAL;
        private FollowDAL _followDAL;
        private OrgDAL _orgDAL;
        private SubjectDAL _subjectDAL;

        public ClueBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, ClueDAL clueDAL, UserDAL userDAL, FollowDAL followDAL, OrgDAL orgDAL, SubjectDAL subjectDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _clueDAL = clueDAL;
            _userDAL = userDAL;
            _followDAL = followDAL;
            _orgDAL = orgDAL;
            _subjectDAL = subjectDAL;
        }

        public virtual async Task<PageObject<MZ_Clue>> SelectList(In_ClueList query)
        {
            var user = _provider.GetUser();
            var scope = await user.GetScope(_provider);
            var clueList = await _clueDAL.SelectByPage(query, scope, user);

            List<string> allusers = new List<string>();
            foreach (var clue in clueList.List)
            {
                if (!string.IsNullOrEmpty(clue.Helper))
                {
                    var tids = clue.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    allusers.AddRange(tids);
                }
                if (clue.LeaderId != null && clue.LeaderId > 0)
                {
                    allusers.Add(clue.LeaderId.ToString());
                }
            }
            Dictionary<string, MZ_AdminInfo> adminDict = new Dictionary<string, MZ_AdminInfo>();
            if (allusers.Count > 0)
            {
                var tmpids = allusers.ConvertAll(t => long.Parse(t));
                var users = await _userDAL.GetUserListByIds(tmpids);
                foreach (var u in users)
                {
                    adminDict.Add(u.Id.ToString(), u);
                }
            }
            foreach (var clue in clueList.List)
            {
                if (!string.IsNullOrEmpty(clue.Helper))
                {
                    List<MZ_AdminInfo> tmpusers = new List<MZ_AdminInfo>();
                    var tids = clue.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var u in tids)
                    {
                        MZ_AdminInfo tmpuu;
                        if (adminDict.TryGetValue(u, out tmpuu))
                        {
                            tmpusers.Add(tmpuu);
                        }
                    }

                    clue.HelperName = string.Join(',', tmpusers.Select(x => x.RealName).ToList());
                }
                if (clue.LeaderId != null && clue.LeaderId > 0)
                {
                    MZ_AdminInfo tmpuu;
                    if (adminDict.TryGetValue(clue.LeaderId.ToString(), out tmpuu))
                    {
                        clue.LeaderName = tmpuu.RealName;
                    }

                }
            }

            return clueList;
        }


        public virtual async Task<BusResponse<MZ_Clue>> Info(long id)
        {
            var clueInfo = await _clueDAL.Select(id);
            if (clueInfo == null)
            {
                return BusResponse<MZ_Clue>.Error(111, "线索不存在");
            }
            if (string.IsNullOrEmpty(clueInfo.Helper))
            {
                clueInfo.HelperName = string.Empty;
            }
            else
            {
                var tids = clueInfo.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                var tmpids = tids.ConvertAll(t => long.Parse(t));
                var users = await _userDAL.GetUserListByIds(tmpids);
                clueInfo.HelperUsers = users;
                clueInfo.HelperName = string.Join(',', users.Select(x => x.RealName).ToList());
            }
            if (clueInfo.LeaderId != null && clueInfo.LeaderId > 0)
            {
                var leader = await _userDAL.GetAdminById(clueInfo.LeaderId.Value);
                clueInfo.LeaderName = leader.RealName;
            }
            return BusResponse<MZ_Clue>.Success(clueInfo);
        }
        public virtual async Task<BusResponse<string>> Delete(string id, bool isPublic)
        {
            MZ_Clue old = await _clueDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "线索不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            if (isPublic)
            {
                if (old.LeaderId > 0)
                {
                    return BusResponse<string>.Error(125, "无法删除私海数据");
                }
            }
            else
            {
                if (old.LeaderId == 0)
                {
                    return BusResponse<string>.Error(125, "无法删除公海数据");
                }
            }
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    MZ_Clue clue = new MZ_Clue();
                    clue.SetUpdateBy(user);
                    clue.del_flag = "2";
                    if (await _clueDAL.Update(clue, x => x.Id == id && x.del_flag == "0") <= 0)
                    {
                        BusResponse<string>.Error(121, "线索状态错误");
                    }

                    MZ_Follow follow = new MZ_Follow();
                    follow.del_flag = "2";
                    follow.SetUpdateBy(user);
                    await _followDAL.Update(follow, x => x.TargetType == 1 && x.TargetId == id);

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
        public virtual async Task<BusResponse<string>> Edit(MZ_Clue data, bool isPublic)
        {
            try
            {
                MZ_Clue old = await _clueDAL.Select(data.Id);
                if (old == null)
                {
                    return BusResponse<string>.Error(123, "线索不存在");
                }
                var user = _provider.GetUser();
                if (user.OrgId != old.OrgId)
                {
                    return BusResponse<string>.Error(124, "当前用户无权限");
                }

                data.OrgId = null;
                data.SetUpdateBy(user);
                if (isPublic)
                {
                    if (old.LeaderId > 0)
                    {
                        return BusResponse<string>.Error(125, "无法编辑私海数据");
                    }
                }
                else
                {
                    if (old.LeaderId == 0)
                    {
                        return BusResponse<string>.Error(125, "无法编辑公海数据");
                    }
                }


                if (data.CompanyName != null)
                {
                    MZ_Subject subj = new MZ_Subject();
                    subj.Title = data.CompanyName;
                    await _subjectDAL.Update(subj, x => x.TargetId == data.Id);
                }


                data.LastFollowId = null;
                data.LastFollowDate = null;
                data.StartFollowDate = null;
                data.ChangeDate = null;
                data.ReturnReason = null;
                data.del_flag = null;
                data.SyncFollow = null;
                await _clueDAL.Update(data);

                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<string>> Draw(string id)
        {
            MZ_Clue old = await _clueDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "线索不存在");
            }
            if (old.del_flag != "0")
            {
                return BusResponse<string>.Error(125, "线索状态错误");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            var userOrg = await _orgDAL.SelectUserOrg(user.UserId, user.OrgId);
            MZ_Clue data = new MZ_Clue();
            data.Id = id;
            data.SetUpdateBy(user);
            data.LeaderId = user.UserId;
            data.DeptId = userOrg.dept_id;
            data.LastFollowDate = data.StartFollowDate = DateTime.Now;
            await _clueDAL.Update(data);

            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> Transform(In_ClueTrans query)
        {
            MZ_Clue old = await _clueDAL.Select(query.ClueId);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "线索不存在");
            }
            if (old.del_flag != "0")
            {
                return BusResponse<string>.Error(125, "线索状态错误");
            }

            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            if (old.LeaderId != 0 && user.UserId != old.LeaderId)
            {
                return BusResponse<string>.Error(126, "不是线索负责人无法转换");
            }
            bool isAdd = query.Customer.Id == null;
            MZ_Clue data = new MZ_Clue();
            data.Id = query.ClueId;
            data.SetUpdateBy(user);
            data.del_flag = "1";
            if (query.Customer.Id == null)
            {
                //新建客户
                query.Customer.Id = _snowflake.NextId().ToString();
                var tmpbll = _provider.GetService<CustomerBLL>();
                query.Customer.CustomerNumber = await tmpbll.GenerateNumber();
            }
            data.ChangeId = query.Customer.Id;
            data.ChangeDate = DateTime.Now;
            data.SyncFollow = query.SyncFollow;

            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    if (isAdd)
                    {
                        var res = await _provider.GetService<CustomerBLL>().Add(query.Customer, false);
                        if (!res.IsSuccess()) return res;
                    }
                    await _clueDAL.Update(data);

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
        public virtual async Task<BusResponse<string>> Return(string id, string reason)
        {
            MZ_Clue old = await _clueDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "线索不存在");
            }
            if (old.del_flag != "0")
            {
                return BusResponse<string>.Error(125, "线索状态错误");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            MZ_Clue data = new MZ_Clue();
            data.Id = id;
            data.SetUpdateBy(user);
            data.LeaderId = 0;
            data.DeptId = 0;
            data.StartFollowDate = null;
            data.ReturnReason = reason;
            await _clueDAL.Update(data);
            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> Add(MZ_Clue data, bool isPublic)
        {
            var user = _provider.GetUser();

            data.Id = _snowflake.NextId().ToString();
            data.SetCreateBy(user);
            data.OrgId = user.OrgId;
            if (isPublic)
            {
                data.LeaderId = 0;
                data.DeptId = 0;
                data.LastFollowDate = null;
                data.StartFollowDate = null;
            }
            else
            {
                data.LeaderId = user.UserId;
                var userOrg = await _orgDAL.SelectUserOrg(user.UserId, user.OrgId);
                data.DeptId = userOrg.dept_id;
                data.LastFollowDate = data.StartFollowDate = DateTime.Now;
            }
            data.LastFollowId = string.Empty;
            data.ChangeDate = null;
            data.ReturnReason = string.Empty;
            data.del_flag = "0";
            data.SyncFollow = false;
            data.Remark ??= string.Empty;


            //添加关联的主题
            MZ_Subject subj = new MZ_Subject();
            subj.SetCreateBy(user);
            subj.Id = _snowflake.NextId().ToString();
            subj.OrgId = data.OrgId;
            subj.Title = data.CompanyName;
            subj.SubjectContent = string.Empty;
            subj.TargetId = data.Id;
            subj.TargetType = "线索";

            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await _clueDAL.Insert(data);
                    await _subjectDAL.Insert(subj);
                    // 完成
                    await scope.CompleteAsync();
                }

                return BusResponse<string>.Success(data.Id);
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }
    }
}
