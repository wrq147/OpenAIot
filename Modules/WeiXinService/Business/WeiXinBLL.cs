using AuthService;
using AuthService.Model;
using Common;
using Common.IdGenerator;
using Common.Share;
using MyAccess.Aop;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using WeiXinService.DAL;
using WeiXinService.Model;

namespace WeiXinService.Business
{
    public class WeiXinBLL
    {
        protected ITAServiceProvider _serviceProvider;
        protected ITAContext _context;
        protected UserDAL _userDAL;
        protected UserWxDAL _usrWxDAL;
        protected UserCropDAL _usrCropDAL;
        private SnowflakeHelper _snowflake;
        public WeiXinBLL(ITAServiceProvider serviceProvider, SnowflakeHelper snowflake, UserDAL userDAL, UserWxDAL usrWxDAL, UserCropDAL usrCropDAL, ITAContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
            _userDAL = userDAL;
            _usrWxDAL = usrWxDAL;
            _usrCropDAL = usrCropDAL;
            _snowflake = snowflake;
        }
        public async Task<BusResponse<int>> WxAppletUnBind(string appid, IUserInfo user)
        {
            var rs= await _usrWxDAL.Delete(x => x.Id == user.UserId && x.AppId == appid);
            return BusResponse<int>.Success(rs);
        }
        public async Task<BusResponse<string>> WxAppletBind(string appid, string code, IUserInfo user)
        {
            var apiHelper = _serviceProvider.GetService<WxApiHelper>();
            string usappid = await apiHelper.DefaultAppId(appid);

            var request = new SnsJsCode2SessionRequest() { GrantType = "authorization_code", JsCode = code };
            var client = await apiHelper.CreateWxClient(usappid);
            var response = await client.ExecuteSnsJsCode2SessionAsync(request);
            if (response == null || response.ErrorCode != 0)
            {
                return BusResponse<string>.Error(110, response.ErrorMessage);
            }
            if (string.IsNullOrEmpty(response.UnionId))
            {
                return BusResponse<string>.Error(102, "需要授权,请升级版本");
            }

            var tmpadmin = await _userDAL.GetAdminById(user.UserId);
            if (tmpadmin == null)
            {
                return BusResponse<string>.Error(118, "用户不存在");
            }
            var newThird = new MZ_AdminWx();
            newThird.Id = tmpadmin.Id;
            newThird.AppId = appid;
            newThird.UnionId = response.UnionId;
            newThird.UpdatedOn = DateTime.Now;
            await _usrWxDAL.CreateOrUpdate(newThird);

            return BusResponse<string>.Success();
        }
        public async Task<BusResponse<string>> MobileBind(string appid, string code)
        {
            var apiHelper = _serviceProvider.GetService<WxApiHelper>();
            appid = await apiHelper.DefaultAppId(appid);
            var rsp = await apiHelper.GetWxMobile(appid, code);

            if (rsp.IsSuccess())
            {
                try
                {
                    var usr = Data_ServerTokenInfo.From(_context);
                    MZ_AdminInfo upad = new MZ_AdminInfo();
                    upad.Id = usr.UserId;
                    upad.Mobile = rsp.Data;
                    await _userDAL.UpdateUser(upad);
                    return BusResponse<string>.Success(rsp.Data);
                }
                catch
                {
                    return BusResponse<string>.Error(113, "业务繁忙,请重试");
                }
            }
            return rsp;
        }
        public virtual async Task<BusResponse<Out_Login>> AppletLogin(In_LoginApplet ipt, ITAContext context)
        {
            var apiHelper = _serviceProvider.GetService<WxApiHelper>();
            ipt.appid = await apiHelper.DefaultAppId(ipt.appid);

            var request = new SnsJsCode2SessionRequest() { GrantType = "authorization_code", JsCode = ipt.code };
            var client = await apiHelper.CreateWxClient(ipt.appid);
            var response = await client.ExecuteSnsJsCode2SessionAsync(request);
            if (response == null || response.ErrorCode != 0)
            {
                return BusResponse<Out_Login>.Error(110, response.ErrorMessage);
            }
            if (string.IsNullOrEmpty(response.UnionId))
            {
                return BusResponse<Out_Login>.Error(102, "需要授权,请升级版本");
            }

            WeiXinDAL wxDAL = _serviceProvider.GetService<WeiXinDAL>();
            //记录openid与unionid关系
            var oldwx = await wxDAL.SelectById(response.UnionId, ipt.appid);
            if (oldwx == null)
            {
                MZ_WeiXin mz_wx = new MZ_WeiXin();
                mz_wx.UnionId = response.UnionId;
                mz_wx.AppId = ipt.appid;
                mz_wx.OpenId = response.OpenId;

                await wxDAL.Insert(mz_wx);
            }


            var redis = _serviceProvider.GetService<GeneralRedisHelper>();
            string lockstr = "UnionBindLock" + response.UnionId;
            if (await redis.WaitLockTakeAsync(lockstr))
            {
                try
                {
                    var thirdlist = await _usrWxDAL.SelectList(x => x.UnionId == response.UnionId, "UpdatedOn desc");
                    if (thirdlist.Count == 0)
                    {
                        if (ipt.created != true)
                        {
                            return BusResponse<Out_Login>.Error(5, "微信号未绑定");
                        }
                        //不存在,则创建账号
                        MZ_AdminInfo adminInfo = new MZ_AdminInfo();
                        adminInfo.Id = _snowflake.NextId();
                        adminInfo.OrgId = 0;
                        adminInfo.Salt = MyAccess.Core.StringTool.GetEnglishChar(16);
                        adminInfo.Password = MyAccess.Core.Crypter.MD5(string.Concat(response.UnionId, adminInfo.Salt));
                        adminInfo.RealName = string.Empty;
                        adminInfo.Avatar = string.Empty;
                        adminInfo.UserName = "$" + adminInfo.Id;
                        adminInfo.Mobile = string.Empty;
                        adminInfo.Email = string.Empty;
                        adminInfo.Sex = "2";
                        adminInfo.Introduction = string.Empty;
                        adminInfo.Signature ??= string.Empty;
                        adminInfo.WaitSignature ??= string.Empty;
                        adminInfo.status = "0";
                        adminInfo.del_flag = "0";
                        adminInfo.createId = 0;
                        adminInfo.updateId = 0;
                        adminInfo.create_time = DateTime.Now;
                        adminInfo.update_time = DateTime.Now;

                        //添加第三方用户信息
                        var thirdInfo = new MZ_AdminWx();
                        thirdInfo.Id = adminInfo.Id;
                        thirdInfo.AppId = ipt.appid;
                        thirdInfo.UnionId = response.UnionId;
                        thirdInfo.UpdatedOn = DateTime.Now;


                        using (BLLTranScope scope = new BLLTranScope())
                        {
                            // 新增用户角色关联
                            var roleVal = await _serviceProvider.GetService<ConfigBLL>().SelectConfigByKey("sys.reg.roleId");
                            MZ_UserRole ur = new MZ_UserRole();
                            ur.UserId = adminInfo.Id;
                            ur.RoleID = long.Parse(roleVal);
                            ur.OrgId = 0;
                            List<MZ_UserRole> userRoles = new List<MZ_UserRole>();
                            userRoles.Add(ur);
                            await _userDAL.AddUserRole(userRoles);
                            await _userDAL.AddUser(adminInfo);
                            await _usrWxDAL.CreateOrUpdate(thirdInfo);
                            // 完成
                            await scope.CompleteAsync();
                        }
                        var res = await _serviceProvider.GetService<AuthBLL>().Login(adminInfo, context);
                        res.Data.isnew = true;
                        return res;
                    }
                    else
                    {
                        var tmpadmin = await _userDAL.GetAdminById(thirdlist[0].Id.Value);
                        //存在,则登录
                        return await _serviceProvider.GetService<AuthBLL>().Login(tmpadmin, context);
                    }

                }
                finally
                {
                    await redis.LockReleaseAsync(lockstr);
                }
            }
            else
            {
                return BusResponse<Out_Login>.ErrorBusy();
            }

        }
        public virtual async Task<BusResponse<Out_Login>> CorpLogin(In_LoginCorp ipt, ITAContext context)
        {
            var apiHelper = _serviceProvider.GetService<WxApiHelper>();
            var rs = await apiHelper.GetCorpUserInfo(ipt.appid, ipt.code);
            if (!rs.IsSuccess())
            {
                return BusResponse<Out_Login>.Error(101, rs.Message);
            }
            var orgDAL = _serviceProvider.GetService<OrgDAL>();

            string lockstr = "CorpUserBindLock" + ipt.appid + ":" + rs.Data.userid;
            var redis = _serviceProvider.GetService<GeneralRedisHelper>();
            if (await redis.WaitLockTakeAsync(lockstr))
            {
                try
                {
                    var thirdlist = await _usrCropDAL.SelectList(x => x.AppId == ipt.appid && x.CropUserId == rs.Data.userid, "UpdatedOn desc");
                    if (thirdlist.Count == 0)
                    {
                        if (ipt.created != true)
                        {
                            return BusResponse<Out_Login>.Error(117, "企业微信未绑定");
                        }
                        var wxrsp = await apiHelper.GetCorpUserDetail(ipt.appid, rs.Data.userid);
                        if (!wxrsp.IsSuccess())
                        {
                            return BusResponse<Out_Login>.Error(118, wxrsp.Message);
                        }
                        var wxUserDetail = wxrsp.Data;
                        //不存在,则创建账号
                        MZ_AdminInfo adminInfo = new MZ_AdminInfo();
                        adminInfo.Id = _snowflake.NextId();
                        adminInfo.OrgId = rs.Data.OrgId;
                        adminInfo.Salt = MyAccess.Core.StringTool.GetEnglishChar(16);
                        adminInfo.Password = MyAccess.Core.Crypter.MD5(string.Concat(rs.Data.user_ticket, adminInfo.Salt));
                        adminInfo.RealName = wxUserDetail.name;
                        if (string.IsNullOrEmpty(wxUserDetail.avatar))
                        {
                            adminInfo.Avatar = string.Empty;
                        }
                        else
                        {
                            adminInfo.Avatar = await _serviceProvider.GetService<FileHelper>().DownFileToLocal(wxUserDetail.avatar);
                        }
                        adminInfo.UserName = "$" + adminInfo.Id;
                        adminInfo.Mobile = string.Empty;
                        adminInfo.Email = string.Empty;
                        adminInfo.EmailActive = false;
                        adminInfo.Sex = "2";
                        adminInfo.Introduction = string.Empty;
                        adminInfo.Signature ??= string.Empty;
                        adminInfo.WaitSignature ??= string.Empty;
                        adminInfo.status = "0";
                        adminInfo.del_flag = "0";
                        adminInfo.createId = 0;
                        adminInfo.updateId = 0;
                        adminInfo.create_time = DateTime.Now;
                        adminInfo.update_time = DateTime.Now;

                        //添加第三方用户信息
                        var thirdInfo = new MZ_AdminCrop();
                        thirdInfo.Id = adminInfo.Id;
                        thirdInfo.AppId = ipt.appid;
                        thirdInfo.CropUserId = rs.Data.userid;
                        thirdInfo.UpdatedOn = DateTime.Now;

                        MZ_Dept jointDept = null;
                        if (rs.Data.OrgId > 0)
                        {
                            var deptDAL = _serviceProvider.GetService<DeptDAL>();
                            var depList = await deptDAL.SearchDeptByName(wxUserDetail.deptname, rs.Data.OrgId);

                            if (depList.Count > 0)
                            {
                                jointDept = depList[0];
                            }
                            else
                            {
                                jointDept = await deptDAL.SelectRoot(rs.Data.OrgId);
                            }

                        }

                        using (BLLTranScope scope = new BLLTranScope())
                        {
                            // 新增用户角色关联
                            var roleVal = await _serviceProvider.GetService<ConfigBLL>().SelectConfigByKey("sys.reg.roleId");
                            MZ_UserRole ur = new MZ_UserRole();
                            ur.UserId = adminInfo.Id;
                            ur.RoleID = long.Parse(roleVal);
                            ur.OrgId = 0;
                            List<MZ_UserRole> userRoles = new List<MZ_UserRole>();
                            userRoles.Add(ur);
                            await _userDAL.AddUserRole(userRoles);
                            await _userDAL.AddUser(adminInfo);
                            await _usrCropDAL.CreateOrUpdate(thirdInfo);

                            if (rs.Data.OrgId > 0)
                            {
                                //加入企业
                                MZ_User_Org userOrg = new MZ_User_Org();
                                userOrg.dept_id = jointDept.dept_id;
                                userOrg.OrgId = rs.Data.OrgId;
                                userOrg.post_name = wxUserDetail.postname;
                                userOrg.UserId = adminInfo.Id;
                                userOrg.IsLeader = false;
                                userOrg.IsPrimary = true;
                                await orgDAL.InsertUserOrg(userOrg);
                            }
                            // 完成
                            await scope.CompleteAsync();
                        }
                        var res = await _serviceProvider.GetService<AuthBLL>().Login(adminInfo, context);
                        res.Data.isnew = true;
                        return res;
                    }
                    else
                    {
                        var tmpadmin = await _userDAL.GetAdminById(thirdlist[0].Id.Value);
                        if (rs.Data.OrgId > 0)
                        {
                            if (!await orgDAL.CheckExistOrg(tmpadmin.Id.Value, rs.Data.OrgId))
                            {
                                //自动加入企业
                                var wxrsp = await apiHelper.GetCorpUserDetail(ipt.appid, rs.Data.userid);
                                if (wxrsp.IsSuccess())
                                {
                                    var wxUserDetail = wxrsp.Data;

                                    MZ_Dept jointDept = null;
                                    var deptDAL = _serviceProvider.GetService<DeptDAL>();
                                    var depList = await deptDAL.SearchDeptByName(wxUserDetail.deptname, rs.Data.OrgId);

                                    if (depList.Count > 0)
                                    {
                                        jointDept = depList[0];
                                    }
                                    else
                                    {
                                        jointDept = await deptDAL.SelectRoot(rs.Data.OrgId);
                                    }

                                    //加入企业
                                    MZ_User_Org userOrg = new MZ_User_Org();
                                    userOrg.dept_id = jointDept.dept_id;
                                    userOrg.OrgId = rs.Data.OrgId;
                                    userOrg.post_name = wxUserDetail.postname;
                                    userOrg.UserId = tmpadmin.Id;
                                    userOrg.IsLeader = false;
                                    userOrg.IsPrimary = true;
                                    await orgDAL.InsertUserOrg(userOrg);
                                }
                            }
                        }

                        //存在,则登录
                        return await _serviceProvider.GetService<AuthBLL>().Login(tmpadmin, context);
                    }

                }
                catch (Exception ex)
                {
                    return BusResponse<Out_Login>.Error(131, ex.Message);
                }
                finally
                {
                    await redis.LockReleaseAsync(lockstr);
                }
            }
            else
            {
                return BusResponse<Out_Login>.ErrorBusy();
            }
        }

    }
}
