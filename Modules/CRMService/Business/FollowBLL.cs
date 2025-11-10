using AuthService;
using Common.IdGenerator;
using Common.Share;
using CRMService.DAL;
using CRMService.Model;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CRMService.Business
{
    public class FollowBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private FollowDAL _followDAL;
        private OrgDAL _orgDAL;
        private ClueDAL _clueDAL;
        private CustomerDAL _customerDAL;
        private OpportunityDAL _opportunityDAL;
        public FollowBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, FollowDAL followDAL,
            OrgDAL orgDAL, ClueDAL clueDAL, CustomerDAL customerDAL, OpportunityDAL opportunityDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _followDAL = followDAL;
            _orgDAL = orgDAL;
            _clueDAL = clueDAL;
            _customerDAL = customerDAL;
            _opportunityDAL = opportunityDAL;
        }
        public virtual async Task<BusResponse<MZ_Follow>> Info(string id)
        {
            var info = await _followDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_Follow>.Error(111, "跟进记录不存在");
            }
            if (info.TargetType == 0)
            {
                var customer = await _customerDAL.Select(info.TargetId);
                if (customer != null)
                {
                    info.TargetName = customer.CustomerName;
                }
            }
            else if (info.TargetType == 1)
            {
                var clue = await _clueDAL.Select(info.TargetId);
                if (clue != null)
                {
                    info.TargetName = clue.CompanyName;
                }
            }
            if (!string.IsNullOrEmpty(info.OpportId))
            {
                var tmpOpport = await _opportunityDAL.Select(info.OpportId);
                info.OpportName = tmpOpport.OpportName;
            }

            if (info.FollowUser != null && info.FollowUser > 0)
            {
                var userDAL = _provider.GetService<UserDAL>();
                info.FollowUserInfo = await userDAL.Select(info.FollowUser);
            }

            if(!string.IsNullOrEmpty(info.ContactId))
            {
                var contactDAL = _provider.GetService<ContactDAL>();
                info.ContactInfo = await contactDAL.Select(info.ContactId);
            }

            return BusResponse<MZ_Follow>.Success(info);
        }
        public virtual async Task<PageObject<MZ_Follow>> SelectList(In_FollowList query)
        {
            var user = _provider.GetUser();
            var scope = await user.GetScope(_provider);
            var followList = await _followDAL.SelectByPage(query, scope, user);
            var opportdict = await _opportunityDAL.NavigateDict<MZ_Follow, string>(followList.List, x => !string.IsNullOrEmpty(x.OpportId), x => x.OpportId);
            var userDAL = _provider.GetService<UserDAL>();
            var followUserDict = await userDAL.NavigateDict(followList.List, x => x.FollowUser > 0, x => x.FollowUser.Value);
            var contactDAL = _provider.GetService<ContactDAL>();
            var contactDict = await contactDAL.NavigateDict(followList.List, x => !string.IsNullOrEmpty(x.ContactId), x => x.ContactId);

            foreach (var item in followList.List)
            {
                MZ_Opportunity tmpOpport;
                if (opportdict.TryGetValue(item.OpportId, out tmpOpport))
                {
                    item.OpportName = tmpOpport.OpportName;
                }
                MZ_AdminInfo tmpuser;
                if (followUserDict.TryGetValue(item.FollowUser.Value, out tmpuser))
                {
                    item.FollowUserInfo = tmpuser;
                }
                MZ_Contact tmpcontact;
                if(contactDict.TryGetValue(item.ContactId,out tmpcontact))
                {
                    item.ContactInfo = tmpcontact;
                }
            }

            return followList;
        }


        public virtual async Task<BusResponse<string>> Add(MZ_Follow data)
        {
            var user = _provider.GetUser();
            data.Id = _snowflake.NextId().ToString();
            data.del_flag = "0";
            data.OrgId = user.OrgId;

            data.OpportId ??= string.Empty;
            data.ContactId ??= string.Empty;
            data.FollowWay ??= string.Empty;
            data.Remark = new MyAccess.Filter.HtmlFilter().FilterHtml(data.Remark);

            data.SetCreateBy(user);

            var leaderDept = await _orgDAL.SelectUserOrg(data.FollowUser.Value, user.OrgId);
            data.DeptId = leaderDept.dept_id;

            await _followDAL.Insert(data);
            return BusResponse<string>.Success(data.Id);
        }
        public virtual async Task<BusResponse<string>> Edit(MZ_Follow data)
        {
            MZ_Follow old = await _followDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "跟进不存在");
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
            if (data.FollowUser == null)
            {
                data.DeptId = null;
            }
            else
            {
                var leaderDept = await _orgDAL.SelectUserOrg(data.FollowUser.Value, user.OrgId);
                data.DeptId = leaderDept.dept_id;
            }

            data.OrgId = null;
            data.SetUpdateBy(user);
            data.del_flag = null;
            await _followDAL.Update(data);
            return BusResponse<string>.Success();
        }

        public virtual async Task<BusResponse<string>> Delete(string id)
        {
            var user = _provider.GetUser();
            MZ_Follow old = await _followDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "跟进不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            MZ_Follow data = new MZ_Follow();
            data.Id = id;
            data.del_flag = "2";
            data.SetUpdateBy(user);
            await _followDAL.Update(data);
            return BusResponse<string>.Success();
        }
    }
}
