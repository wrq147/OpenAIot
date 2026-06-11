using CRMService.DAL;
using CRMService.Model;
using AuthService;
using Common;
using Common.Share;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using CRMService.Controller;
using Common.IdGenerator;
using DiscussService.Model;
using DiscussService.DAL;
using AuthService.DAL;
using MyAccess.Aop;
using ProducerService.DAL;
using ProducerService.Business;

namespace CRMService.Business
{
    public class CustomerBLL
    {
        private IOptions<GeneralOption> _conf;
        private ITAServiceProvider _provider;
        private OperatorHelper _operator;
        private CustomerDAL _customerDAL;
        private OrgDAL _orgDAL;
        private FollowDAL _followDAL;
        private ContactDAL _contactDAL;
        private SnowflakeHelper _snowflake;
        private OpportunityDAL _opportunityDAL;
        private UserDAL _userDAL;
        private SubjectDAL _subjectDAL;
        private OrgExtDAL _orgExtDAL;
        public CustomerBLL(ITAServiceProvider provider, IOptions<GeneralOption> conf, OperatorHelper operatorHelper,
            OpportunityDAL opportunityDAL, CustomerDAL customerDAL, OrgDAL orgDAL, FollowDAL followDAL, ContactDAL contactDAL,
            SnowflakeHelper snowflake, UserDAL userDAL, SubjectDAL subjectDAL, OrgExtDAL orgExtDAL)
        {
            _provider = provider;
            _conf = conf;
            _operator = operatorHelper;
            _customerDAL = customerDAL;
            _orgDAL = orgDAL;
            _followDAL = followDAL;
            _contactDAL = contactDAL;
            _snowflake = snowflake;
            _opportunityDAL = opportunityDAL;
            _userDAL = userDAL;
            _subjectDAL = subjectDAL;
            _orgExtDAL = orgExtDAL;
        }
        public async Task<string> GenerateNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("KF");
        }
        public virtual async Task<BusResponse<MZ_Customer>> Info(string id)
        {
            var info = await _customerDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_Customer>.Error(111, "客户信息不存在");
            }
            if (!string.IsNullOrEmpty(info.Helper))
            {
                var tmpstrs = info.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var ids = Array.ConvertAll(tmpstrs, s => long.Parse(s));
                var users = await _userDAL.GetUserListByIds(ids.ToList());
                info.HelperUsers = users;
                info.HelperName = string.Join(',', users.Select(x => x.RealName));
            }
            if (info.LeaderId != null && info.LeaderId > 0)
            {
                var leader = await _userDAL.GetAdminById(info.LeaderId.Value);
                info.LeaderName = leader.RealName;
            }
            return BusResponse<MZ_Customer>.Success(info);
        }
        public virtual async Task<PageObject<MZ_Customer>> SelectList(In_CustomerList query)
        {
            var user = _provider.GetUser();
            var scope = await user.GetScope(_provider);

            int overDay = 0;
            if (query.IsFollowOver == true)
            {
                overDay = Convert.ToInt32(await _orgExtDAL.SelectList(x => x.OrgId == user.OrgId && x.ExtField == "FollowReturnDay"));
            }

            var customerList = await _customerDAL.SelectByPage(query, scope, user, overDay);
            List<string> allusers = new List<string>();
            foreach (var customer in customerList.List)
            {
                if (!string.IsNullOrEmpty(customer.Helper))
                {
                    var tids = customer.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    allusers.AddRange(tids);
                }
                if (customer.LeaderId != null && customer.LeaderId > 0)
                {
                    allusers.Add(customer.LeaderId.ToString());
                }
            }
            allusers = allusers.Distinct().ToList();
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
            foreach (var customer in customerList.List)
            {
                if (!string.IsNullOrEmpty(customer.Helper))
                {
                    List<MZ_AdminInfo> tmpusers = new List<MZ_AdminInfo>();
                    var tids = customer.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var u in tids)
                    {
                        MZ_AdminInfo tmpuu;
                        if (adminDict.TryGetValue(u, out tmpuu))
                        {
                            tmpusers.Add(tmpuu);
                        }
                    }

                    customer.HelperName = string.Join(',', tmpusers.Select(x => x.RealName).ToList());
                }
                if (customer.LeaderId != null && customer.LeaderId > 0)
                {
                    MZ_AdminInfo tmpuu;
                    if (adminDict.TryGetValue(customer.LeaderId.ToString(), out tmpuu))
                    {
                        customer.LeaderName = tmpuu.RealName;
                    }

                }
            }

            return customerList;
        }

        public virtual async Task<BusResponse<string>> Add(MZ_Customer data, bool isPublic)
        {
            var user = _provider.GetUser();
            data.Id = _snowflake.NextId().ToString();
            if (isPublic)
            {
                data.LeaderId = 0;
                data.DeptId = 0;
            }
            else
            {
                if (data.LeaderId <= 0)
                {
                    return BusResponse<string>.Error(121, "请选择负责人");
                }

                var userOrg = await _orgDAL.SelectUserOrg(data.LeaderId.Value, user.OrgId);
                data.DeptId = userOrg.dept_id;
            }


            data.del_flag = "0";
            data.OrgId = user.OrgId;

            if (string.IsNullOrEmpty(data.CustomerNumber))
            {
                data.CustomerNumber = await GenerateNumber();
            }

            if (data.Lng != null && data.Lat != null)
            {
                data.Geo = MyAccess.Core.GeoHash.Encode(data.Lat.Value, data.Lng.Value);
            }
            data.Lng ??= 0;
            data.Lat ??= 0;
            data.AddressCode ??= string.Empty;
            data.AddressDetail ??= string.Empty;
            data.AddressName ??= string.Empty;
            data.Geo ??= string.Empty;
            data.Industry ??= 0;
            data.CompanyTel ??= string.Empty;
            data.CompanyUrl ??= string.Empty;
            data.FromType ??= "other";
            data.FromId ??= string.Empty;
            data.Helper ??= string.Empty;
            data.LastFollowDate = null;
            data.LastFollowId = string.Empty;
            data.Remark ??= string.Empty;
            data.ReturnReason ??= string.Empty;
            data.SetCreateBy(user);
            data.StartFollowDate = null;

            //添加关联的主题
            MZ_Subject subj = new MZ_Subject();
            subj.SetCreateBy(user);
            subj.Id = _snowflake.NextId().ToString();
            subj.OrgId = data.OrgId;
            subj.Title = data.CustomerName;
            subj.SubjectContent = string.Empty;
            subj.TargetId = data.Id;
            subj.TargetType = "客户";

            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await _customerDAL.Insert(data);
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

        public virtual async Task<BusResponse<string>> Edit(MZ_Customer data, bool isPublic)
        {
            MZ_Customer old = await _customerDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "客户不存在");
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

            if (old.BindOrgId > 0)
            {
                data.CustomerType = null;
            }

            if (data.CustomerName != null)
            {
                MZ_Subject subj = new MZ_Subject();
                subj.Title = data.CustomerName;
                await _subjectDAL.Update(subj, x => x.TargetId == data.Id);
            }

            data.SetUpdateBy(user);
            data.OrgId = null;
            data.LeaderId = null;
            data.DeptId = null;
            data.LastFollowId = null;
            data.LastFollowDate = null;
            data.StartFollowDate = null;
            data.ReturnReason = null;
            data.del_flag = null;
            await _customerDAL.Update(data);
            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> UnBind(string id, bool cancelAgent)
        {
            MZ_Customer old = await _customerDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "客户不存在");
            }
            if (old.BindOrgId <= 0)
            {
                return BusResponse<string>.Error(124, "客户未邀请");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            if (cancelAgent)
            {
                var agentDAL = _provider.GetService<AgentDAL>();
                var agentList = await agentDAL.SelectList(x => x.ParentOrgId == user.OrgId && x.OrgId == old.BindOrgId);
                if (agentList.Count > 0)
                {
                    foreach (var agent in agentList)
                    {
                        //删除该代理和所有子代理
                        await agentDAL.Delete(x => x.LevelPath.StartsWith(agent.LevelPath));
                    }

                }
            }
            else
            {
                var agentDAL = _provider.GetService<AgentDAL>();
                await agentDAL.Delete(x => x.ParentOrgId == user.OrgId && x.OrgId == old.BindOrgId);
            }

            MZ_Customer newcustomer = new MZ_Customer();
            newcustomer.Id = id;
            newcustomer.BindOrgId = 0;
            newcustomer.SetUpdateBy(user);
            await _customerDAL.Update(newcustomer);

            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> Draw(string id)
        {
            MZ_Customer old = await _customerDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "客户不存在");
            }
            if (old.del_flag != "0")
            {
                return BusResponse<string>.Error(125, "客户状态错误");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            var userOrg = await _orgDAL.SelectUserOrg(user.UserId, user.OrgId);
            MZ_Customer data = new MZ_Customer();
            data.Id = id;
            data.SetUpdateBy(user);
            data.LeaderId = user.UserId;
            data.DeptId = userOrg.dept_id;
            data.LastFollowDate = data.StartFollowDate = DateTime.Now;

            MZ_Contact contact = new MZ_Contact();
            contact.LeaderId = user.UserId;
            contact.DeptId = userOrg.dept_id;
            contact.SetUpdateBy(user);

            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await _customerDAL.Update(data);
                    await _contactDAL.Update(contact, x => x.CustomerId == id);

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
            MZ_Customer old = await _customerDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "客户不存在");
            }
            if (old.del_flag != "0")
            {
                return BusResponse<string>.Error(125, "客户状态错误");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            MZ_Customer data = new MZ_Customer();
            data.Id = id;
            data.SetUpdateBy(user);
            data.LeaderId = 0;
            data.DeptId = 0;
            data.StartFollowDate = null;
            data.ReturnReason = reason;

            MZ_Contact contact = new MZ_Contact();
            contact.LeaderId = 0;
            contact.DeptId = 0;
            contact.SetUpdateBy(user);

            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await _customerDAL.Update(data);
                    await _contactDAL.Update(contact, x => x.CustomerId == id);

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

        public virtual async Task<BusResponse<string>> Delete(string id, bool isPublic)
        {
            MZ_Customer old = await _customerDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "客户不存在");
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
                    MZ_Customer data = new MZ_Customer();
                    data.SetUpdateBy(user);
                    data.del_flag = "2";
                    if (await _customerDAL.Update(data, x => x.Id == id && x.del_flag == "0") <= 0)
                    {
                        BusResponse<string>.Error(121, "客户状态错误");
                    }

                    MZ_Follow follow = new MZ_Follow();
                    follow.del_flag = "2";
                    follow.SetUpdateBy(user);
                    await _followDAL.Update(follow, x => x.TargetType == 0 && x.TargetId == id);

                    MZ_Contact contact = new MZ_Contact();
                    contact.del_flag = "2";
                    contact.SetUpdateBy(user);
                    await _contactDAL.Update(contact, x => x.CustomerId == id);

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
    }
}
