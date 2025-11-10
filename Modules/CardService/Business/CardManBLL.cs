using AuthService;
using CardService.DAL;
using CardService.Model;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CardService.Business
{
    public class CardManBLL
    {
        private ITAServiceProvider _provider;
        private ITAContext _context;
        private UserDAL _userDAL;
        private OrgDAL _orgDAL;
        public CardManBLL(ITAServiceProvider serviceProvider, UserDAL userDAL,OrgDAL orgDAL, ITAContext context)
        {
            _provider = serviceProvider;
            _context = context;
            _userDAL = userDAL;
            _orgDAL = orgDAL;
        }
        public async Task<List<MZ_AdminInfo>> Select(IUserInfo user)
        {
            return await _userDAL.SelectManUsers(user.OrgId);
        }
        public async Task<BusResponse<bool>> ExistMan(long id)
        {
            IUserInfo userInfo = Data_ServerTokenInfo.From(_context);
            bool isMan = await _orgDAL.CheckExistOrg(userInfo.UserId, id);
            return BusResponse<bool>.Success(isMan);
        }
        public async Task<BusResponse<int>> Delete(long uid, IUserInfo user)
        {
            //判断是否有当前组织的管理员权限
            if (!await _orgDAL.CheckExistOrg(user.UserId, user.OrgId))
            {
                return BusResponse<int>.Error(102, "没有当前组织的管理员权限");
            }
            var manlist = await _userDAL.SelectManUserIds(user.OrgId);
            if (manlist.Count < 2)
            {
                return BusResponse<int>.Error(103, "删除失败,当前组织的管理员人员不足");
            }
            try
            {
                await _userDAL.DeleteUserMan(uid, user.OrgId);
                return BusResponse<int>.Success();
            }
            catch
            {
                return BusResponse<int>.ErrorBusy();
            }
        }

        public async Task<BusResponse<int>> Insert(long uid, IUserInfo user)
        {
            //判断是否有当前组织的管理员权限
            if (!await _orgDAL.CheckExistOrg(user.UserId, user.OrgId))
            {
                return BusResponse<int>.Error(102, "没有当前组织的管理员权限");
            }
            if(await _orgDAL.CheckExistOrg(uid, user.OrgId))
            {
                return BusResponse<int>.Error(103, "当前员工已经是管理员");
            }
            MZ_UserRole man = new MZ_UserRole();
            man.UserId = uid;
            man.RoleID = 2;
            man.OrgId = user.OrgId;
            try
            {
                await _userDAL.AddUserRoleItem(man);
                return BusResponse<int>.Success();
            }
            catch
            {
                return BusResponse<int>.ErrorBusy();
            }
        }

    }
}
