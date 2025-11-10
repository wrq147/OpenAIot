using AuthService;
using Common.IdGenerator;
using Common.Share;
using CRMService.DAL;
using CRMService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TemplateAction.Core;

namespace CRMService.Business
{
    public class ContactBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private ContactDAL _contactDAL;
        private CustomerDAL _customerDAL;
        private OrgDAL _orgDAL;
        private UserDAL _userDAL;
        public ContactBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, ContactDAL contactDAL,
            CustomerDAL customerDAL, OrgDAL orgDAL, UserDAL userDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _contactDAL = contactDAL;
            _customerDAL = customerDAL;
            _orgDAL = orgDAL;
            _userDAL = userDAL;
        }
        public virtual async Task<PageObject<MZ_Contact>> SelectList(In_ContactList query)
        {
            var user = _provider.GetUser();
            var scope = await user.GetScope(_provider);
            var tpage = await _contactDAL.SelectByPage(query, scope, user);
            //初始化客户字典
            var customerdict = await _customerDAL.NavigateDict<MZ_Contact, string>(tpage.List, x => !string.IsNullOrEmpty(x.CustomerId), x => x.CustomerId);
            //初始化协助人字典
            List<string> allusers = new List<string>();
            foreach (var opp in tpage.List)
            {
                if (!string.IsNullOrEmpty(opp.Helper))
                {
                    var tids = opp.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    allusers.AddRange(tids);
                }
                if (opp.LeaderId != null && opp.LeaderId > 0)
                {
                    allusers.Add(opp.LeaderId.ToString());
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
            foreach (var opp in tpage.List)
            {
                if (!string.IsNullOrEmpty(opp.Helper))
                {
                    opp.HelperUsers = new List<MZ_AdminInfo>();
                    var tids = opp.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var u in tids)
                    {
                        MZ_AdminInfo tmpuu;
                        if (adminDict.TryGetValue(u, out tmpuu))
                        {
                            opp.HelperUsers.Add(tmpuu);
                        }
                    }
                }
                if (opp.LeaderId != null && opp.LeaderId > 0)
                {
                    MZ_AdminInfo tmpuu;
                    if (adminDict.TryGetValue(opp.LeaderId.ToString(), out tmpuu))
                    {
                        opp.LeaderUser = tmpuu;
                    }
                }

                MZ_Customer tmpCustomer;
                if (customerdict.TryGetValue(opp.CustomerId, out tmpCustomer))
                {
                    opp.CustomerName = tmpCustomer.CustomerName;
                }

            }

            return tpage;
        }
        public virtual async Task<BusResponse<MZ_Contact>> Info(string id)
        {
            var info = await _contactDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_Contact>.Error(111, "联系人信息不存在");
            }
            if (!string.IsNullOrEmpty(info.Helper))
            {
                var tmpstrs = info.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var ids = Array.ConvertAll(tmpstrs, s => long.Parse(s));
                var users = await _userDAL.GetUserListByIds(ids.ToList());
                info.HelperUsers = users;
            }
            if(!string.IsNullOrEmpty(info.CustomerId))
            {
                var custom = await _customerDAL.Select(info.CustomerId);
                if (custom != null)
                {
                    info.CustomerName = custom.CustomerName;
                }
            }
            if (info.LeaderId != null && info.LeaderId > 0)
            {
                info.LeaderUser = await _userDAL.GetAdminById(info.LeaderId.Value);
            }
            return BusResponse<MZ_Contact>.Success(info);
        }
        public virtual async Task<BusResponse<string>> Add(MZ_Contact data)
        {

            MZ_Customer customer = await _customerDAL.Select(data.CustomerId);
            if (customer == null)
            {
                return BusResponse<string>.Error(123, "客户不存在");
            }
            var user = _provider.GetUser();
            data.Id = _snowflake.NextId().ToString();

            var leaderDept = await _orgDAL.SelectUserOrg(data.LeaderId.Value, user.OrgId);
            data.DeptId = leaderDept.dept_id;
            data.del_flag = "0";
            data.OrgId = user.OrgId;

            data.Email ??= string.Empty;
            data.Helper ??= string.Empty;
            data.Mobile ??= string.Empty;
            data.PostName ??= string.Empty;
            data.RealName ??= string.Empty;
            data.Remark ??= string.Empty;
            if (string.IsNullOrEmpty(data.Sex))
            {
                data.Sex = "2";
            }
            data.WxNumber ??= string.Empty;

            data.SetCreateBy(user);
            await _contactDAL.Insert(data);
            return BusResponse<string>.Success(data.Id);
        }
        public virtual async Task<BusResponse<string>> Edit(MZ_Contact data)
        {
            MZ_Contact old = await _contactDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "联系人不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            if (data.LeaderId != null)
            {
                var leaderDept = await _orgDAL.SelectUserOrg(data.LeaderId.Value, user.OrgId);
                data.DeptId = leaderDept.dept_id;
            }
            else
            {
                data.DeptId = null;
            }
            data.SetUpdateBy(user);
            data.OrgId = null;
            data.del_flag = null;
            await _contactDAL.Update(data);
            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> Delete(string id)
        {
            var user = _provider.GetUser();
            MZ_Contact old = await _contactDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "联系人不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            MZ_Contact contact = new MZ_Contact();
            contact.Id = id;
            contact.del_flag = "2";
            contact.SetUpdateBy(user);
            await _contactDAL.Update(contact);
            return BusResponse<string>.Success();
        }
    }
}
