using AuthService;
using Common.IdGenerator;
using Common.Share;
using CRMService.DAL;
using CRMService.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CRMService.Business
{
    public class PlanBLL
    {
        private ITAServiceProvider _provider;
        private OperatorHelper _operator;
        private SnowflakeHelper _snowflake;
        private PlanDAL _planDAL;
        private OrgDAL _orgDAL;
        public PlanBLL(ITAServiceProvider provider, OperatorHelper operatorHelper, SnowflakeHelper snowflake, PlanDAL planDAL, OrgDAL orgDAL)
        {
            _provider = provider;
            _operator = operatorHelper;
            _snowflake = snowflake;
            _planDAL = planDAL;
            _orgDAL = orgDAL;
        }
        public virtual async Task<BusResponse<MZ_FollowPlan>> Info(string id)
        {
            var info = await _planDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_FollowPlan>.Error(111, "跟进计划不存在");
            }

            if (!string.IsNullOrEmpty(info.CustomerId))
            {
                var customer = await _provider.GetService<CustomerDAL>().Select(info.CustomerId);
                if (customer != null)
                {
                    info.CustomerName = customer.CustomerName;
                    info.CustomerType = customer.CustomerType;
                }
            }
            if (!string.IsNullOrEmpty(info.Executor))
            {
                var tmpstrs = info.Executor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var ids = Array.ConvertAll(tmpstrs, s => long.Parse(s));
                var users = await _provider.GetService<UserDAL>().GetUserListByIds(ids.ToList());
                info.ExecutorUsers = users;
            }
            return BusResponse<MZ_FollowPlan>.Success(info);
        }
        public virtual async Task<PageObject<MZ_FollowPlan>> SelectList(In_FollowPlanList query)
        {
            var user = _provider.GetUser();
            var scope = await user.GetScope(_provider);
            var pagelist = await _planDAL.SelectByPage(query, scope, user);
            List<long> exeUsers = new List<long>();
            foreach (var item in pagelist.List)
            {
                string[] executors = item.Executor.Split(',');
                long[] executorll = Array.ConvertAll(executors, (x) => long.Parse(x));
                exeUsers.AddRange(executorll);
            }
            exeUsers = exeUsers.Distinct().ToList();
            if (exeUsers.Count > 0)
            {
                var userDict = new Dictionary<long, MZ_AdminInfo>();
                var userDAL = _provider.GetService<UserDAL>();
                var userList = await userDAL.GetUserListByIds(exeUsers);
                foreach (var userItem in userList)
                {
                    userDict.Add(userItem.Id.Value, userItem);
                }
                foreach (var item in pagelist.List)
                {
                    string[] executors = item.Executor.Split(',');
                    long[] executorll = Array.ConvertAll(executors, (x) => long.Parse(x));

                    item.ExecutorUsers = new List<MZ_AdminInfo>();
                    foreach (long exid in executorll)
                    {
                        MZ_AdminInfo tmpadmin;
                        if (userDict.TryGetValue(exid, out tmpadmin))
                        {
                            item.ExecutorUsers.Add(tmpadmin);
                        }
                    }
                }
            }
            return pagelist;
        }

        public virtual async Task<BusResponse<string>> Add(MZ_FollowPlan data)
        {
            var user = _provider.GetUser();
            data.Id = _snowflake.NextId().ToString();
            data.Status = "A";
            data.OrgId = user.OrgId;
            data.Remark = new MyAccess.Filter.HtmlFilter().FilterHtml(data.Remark);

            data.SetCreateBy(user);

            string[] executors = data.Executor.Split(',');
            long[] executorll = Array.ConvertAll(executors, (x) => long.Parse(x));
            List<long> deplist = new List<long>();
            foreach (var ll in executorll)
            {
                var leaderDept = await _orgDAL.SelectUserOrg(executorll[0], user.OrgId);
                deplist.Add(leaderDept.dept_id.Value);
            }

            data.DeptIds = string.Join(',', deplist);


            await _planDAL.Insert(data);
            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> Edit(MZ_FollowPlan data)
        {
            MZ_FollowPlan old = await _planDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "跟进计划不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            if (data.Remark != null)
            {
                data.Remark = new MyAccess.Filter.HtmlFilter().FilterHtml(data.Remark);
            }
            if (data.Executor == null)
            {
                data.DeptIds = null;
            }
            else
            {
                string[] executors = data.Executor.Split(',');
                long[] executorll = Array.ConvertAll(executors, (x) => long.Parse(x));
                List<long> deplist = new List<long>();
                foreach (var ll in executorll)
                {
                    var leaderDept = await _orgDAL.SelectUserOrg(executorll[0], user.OrgId);
                    deplist.Add(leaderDept.dept_id.Value);
                }

                data.DeptIds = string.Join(',', deplist);
            }
            data.OrgId = null;
            data.Status = null;
            data.SetUpdateBy(user);
            await _planDAL.Update(data);
            return BusResponse<string>.Success();
        }

        public virtual async Task<BusResponse<string>> Delete(string id)
        {
            var user = _provider.GetUser();
            MZ_FollowPlan old = await _planDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "跟进计划不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            await _planDAL.Delete(x => x.Id == id);
            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> Finish(string id)
        {
            var user = _provider.GetUser();
            MZ_FollowPlan old = await _planDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "跟进计划不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            MZ_FollowPlan data = new MZ_FollowPlan();
            data.Id = id;
            data.Status = "F";
            data.SetUpdateBy(user);
            await _planDAL.Update(data);
            return BusResponse<string>.Success();

        }
    }
}
