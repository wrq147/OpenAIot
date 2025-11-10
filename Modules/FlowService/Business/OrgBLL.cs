using AuthService;
using FlowService.DAL;
using FlowService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace FlowService.Business
{
    public class OrgBLL
    {
        private DAL.OrgDAL _org;
        private UserDAL _user;
        private ITAContext _context;
        public OrgBLL(DAL.OrgDAL org, UserDAL user, ITAContext context)
        {
            _org = org;
            _user = user;
            _context = context;
        }
        /// <summary>
        /// 获取当前级别的组织数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<Out_PickerItem>> GetTree(In_OrgList query)
        {
            if (query.type == "org")
            {
                var deplist = await _org.GetDeptList(query);
                var ulist = await _org.GetUserList(query);
                return deplist.Concat(ulist).ToList();
            }
            else if (query.type == "user")
            {
                var deplist = await _org.GetDeptList(query);
                var ulist = await _org.GetUserList(query);
                return deplist.Concat(ulist).ToList();
            }
            else if (query.type == "dept")
            {
                return await _org.GetDeptList(query);
            }
            else if (query.type == "role")
            {
                In_RoleList rquery = new In_RoleList();
                rquery.showAll = true;
                rquery.status = "0";
                var curuser = Data_ServerTokenInfo.From(_context);
                var rolelist = await _user.SelectRoleList(rquery, curuser);

                List<Out_PickerItem> tlist = new List<Out_PickerItem>();
                foreach (var it in rolelist.List)
                {
                    tlist.Add(new Out_PickerItem()
                    {
                        type = "role",
                        id = it.RoleID.Value,
                        name = it.RoleName
                    });
                }
                return tlist;
            }
            else
            {
                return new List<Out_PickerItem>();
            }
        }

        public async Task<List<Out_PickerItem>> SearchUserList(string key)
        {
            return await _org.SearchUserList(key);
        }
    }
}
