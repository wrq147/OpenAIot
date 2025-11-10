using AuthService;
using AuthService.Business;
using AuthService.Model;
using Common.IdGenerator;
using Common.Share;
using ProducerService.DAL;
using ProducerService.Model;
using Microsoft.Extensions.Options;
using TemplateAction.Core;
using MyAccess.Aop;
using Common.EventBus;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Expressions;
using MyAccess.DB.Builder.WhereToSql;
using Common;
using ProducerService.Controller;
using NPOI.SS.Formula.Functions;
using Minio;


namespace ProducerService.Business
{
    public class AgentBLL
    {
        private IOptions<GeneralOption> _conf;
        private ITAServiceProvider _provider;
        private OperatorHelper _operator;
        private AgentDAL _agentDAL;
        private AgentFlowDAL _agentFlowDAL;
        private SnowflakeHelper _snowflake;
        private FactoryDAL _factoryDAL;
        private GradeDAL _gradeDAL;
        private UserDAL _userDAL;
        private OrgDAL _orgDAL;
        public AgentBLL(ITAServiceProvider provider, IOptions<GeneralOption> conf, OperatorHelper operatorHelper, AgentDAL agentDAL,
            AgentFlowDAL agentFlowDAL, SnowflakeHelper snowflake, FactoryDAL factoryDAL, GradeDAL gradeDAL,
            UserDAL userDAL, OrgDAL orgDAL)
        {
            _provider = provider;
            _conf = conf;
            _operator = operatorHelper;
            _agentDAL = agentDAL;
            _agentFlowDAL = agentFlowDAL;
            _snowflake = snowflake;
            _factoryDAL = factoryDAL;
            _gradeDAL = gradeDAL;
            _userDAL = userDAL;
            _orgDAL = orgDAL;
        }

        public virtual async Task<MZ_AgentFlow> AgentFlowInfo(string id)
        {
            return await _agentFlowDAL.Select(id);
        }
        public virtual async Task<PageObject<MZ_AgentFlow>> SelectAgentFlowList(In_AgentFlowList query, IUserInfo user)
        {
            Expression<Func<MZ_AgentFlow, bool>> expression = x => x.ParentOrgId == user.OrgId;
            if (query.YQFrom == 1)
            {
                expression = expression.And(x => x.BindCustomerId == "");
            }
            else if (query.YQFrom == 2)
            {
                expression = expression.And(x => x.BindCustomerId != "");
            }
            return await _agentFlowDAL.SelectPage(expression, query, "create_time desc");
        }
        public virtual async Task<Out_Agent> Info(string id, bool ext = false)
        {
            Out_Agent info = new Out_Agent();
            var agent = await _agentDAL.Select(id);
            info.Id = agent.Id;
            info.createId = agent.createId;
            info.create_time = agent.create_time;
            info.FactoryId = agent.FactoryId;
            info.Regions = agent.Regions;
            info.GradeId = agent.GradeId;
            info.ParentOrgId = agent.ParentOrgId;
            info.OrgId = agent.OrgId;
            info.LevelPath = agent.LevelPath;
            if (ext)
            {
                MZ_Org factory = await _orgDAL.SelectById(agent.FactoryId.Value);
                if (factory != null)
                {
                    info.FactoryName = factory.OrgName;
                }
                MZ_Org parent = await _orgDAL.SelectById(agent.ParentOrgId.Value);
                if (parent != null)
                {
                    info.ParentOrgName = parent.OrgName;
                }
                var tmpcodeDict = await _provider.GetService<CodeBLL>().SelectAreaDict();
                if (!string.IsNullOrEmpty(info.Regions))
                {
                    info.RegionsName = tmpcodeDict.MultiToName(info.Regions);
                }
                var grade = await _gradeDAL.Select(agent.GradeId);
                if (grade != null)
                {
                    info.GradeName = grade.GradeName;
                }
            }

            return info;
        }
        public virtual async Task<BusResponse<OutCertInfo>> CertInfo(string id)
        {
            var user = _provider.GetUser();
            if (string.IsNullOrEmpty(id) || user.OrgId <= 0 || !await _agentDAL.Some(x => x.OrgId == user.OrgId))
            {
                return new BusResponse<OutCertInfo>(1, string.Empty, OutCertInfo.DemoData());
            }

            var tmpcodeDict = await _provider.GetService<CodeBLL>().SelectAreaDict();
            OutCertInfo certInfo = await _agentDAL.SelectCertInfo(id, user.OrgId);

            if (!string.IsNullOrEmpty(certInfo.Regions))
            {
                certInfo.RegionsName = tmpcodeDict.MultiToName(certInfo.Regions);
            }

            return BusResponse<OutCertInfo>.Success(certInfo);
        }
        public virtual async Task<PageObject<Out_Agent>> SelectList(In_AgentList query)
        {
            var user = _provider.GetUser();
            var tmpcodeDict = await _provider.GetService<CodeBLL>().SelectAreaDict();
            var pager = await _agentDAL.SelectByPage(query, user);
            foreach (var item in pager.List)
            {
                if (string.IsNullOrEmpty(item.Regions))
                {
                    continue;
                }

                item.RegionsName = tmpcodeDict.MultiToName(item.Regions);
            }
            return pager;
        }
        public virtual async Task<List<Out_AgentFactory>> SelectFactory(long id)
        {
            var outList = await _agentDAL.SelectFactory(id);
            if (outList.Count > 0)
            {
                var tmpGradeDict = await _provider.GetService<GradeDAL>().NavigateDict(outList, x => !string.IsNullOrEmpty(x.GradeId), x => x.GradeId);
                var tmpcodeDict = await _provider.GetService<CodeBLL>().SelectAreaDict();
                var tmpFactoryDict = await _factoryDAL.NavigateDict(outList, x => x.FactoryId != null, x => x.FactoryId);
                foreach (var item in outList)
                {
                    MZ_Grade tmpgrade;
                    if (tmpGradeDict.TryGetValue(item.GradeId, out tmpgrade))
                    {
                        item.GradeName = tmpgrade.GradeName;
                    }
                    if (!string.IsNullOrEmpty(item.Regions))
                    {
                        item.RegionsName = tmpcodeDict.MultiToName(item.Regions);
                    }
                    MZ_Factory fact;
                    if (tmpFactoryDict.TryGetValue(item.FactoryId, out fact))
                    {
                        item.CertTemplateId = fact.CertTemplateId;
                    }
                }
            }

            if (!outList.Exists(x => x.FactoryId == id))
            {
                var factory = await _factoryDAL.Select(id);
                if (factory != null)
                {
                    MZ_Org orgFactory = await _orgDAL.SelectById(id);
                    if (orgFactory != null)
                    {
                        outList.Add(new Out_AgentFactory()
                        {
                            Id = string.Empty,
                            FactoryId = factory.Id,
                            ParentOrgId = 0,
                            OrgId = factory.Id,
                            GradeId = string.Empty,
                            Regions = string.Empty,
                            FactoryName = orgFactory.OrgName,
                            Logo = orgFactory.Logo,
                            ParentOrgName = string.Empty,
                            GradeName = "生产商",
                            RegionsName = "无限制",
                            CertTemplateId = factory.CertTemplateId
                        });
                    }

                }

            }

            return outList;
        }
        public virtual async Task<BusResponse<int>> CancelProxy(string id, bool cancelDown)
        {
            try
            {
                var user = _provider.GetUser();
                if (user.OrgId <= 0)
                {
                    return BusResponse<int>.Error(133, "请切换到企业账号");
                }
                MZ_Agent agent = await _agentDAL.Select(id);
                if (agent == null)
                {
                    return BusResponse<int>.Error(134, "代理不存在");
                }
                if (agent.ParentOrgId != user.OrgId)
                {
                    return BusResponse<int>.Error(134, "无权删除此代理");
                }
                if (cancelDown)
                {
                    //删除该代理和所有子代理
                    return BusResponse<int>.Success(await _agentDAL.Delete(x => x.LevelPath.StartsWith(agent.LevelPath)));
                }
                else
                {
                    return BusResponse<int>.Success(await _agentDAL.Delete(x => x.Id == id));
                }

            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }

        }

        public virtual async Task<BusResponse<string>> GenerateAllInvitLink(In_AllInvite data)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(133, "请切换到企业账号");
            }
            var factory = await _factoryDAL.Select(user.OrgId);
            if (factory == null)
            {
                return BusResponse<string>.Error(134, "您还没有成为生产商，暂无法添加代理");
            }

            //生成邀请单
            MZ_AgentFlow flow = new MZ_AgentFlow();
            flow.SetCreateBy(user);
            flow.Id = _snowflake.NextId().ToString();
            flow.ParentOrgId = user.OrgId;
            flow.BelowOrgId = 0;

            if (factory.GradeWay == "auto")
            {
                var gradelist = await _gradeDAL.SelectList(x => x.FactoryId == user.OrgId, "Sort asc");
                if (gradelist.Count == 0)
                {
                    return BusResponse<string>.Error(136, "请先添加代理级别");
                }
                flow.GradeId = gradelist[0].Id;

            }
            else
            {
                if (string.IsNullOrEmpty(data.GradeId))
                {
                    return BusResponse<string>.Error(135, "请输入代理级别");
                }
                flow.GradeId = data.GradeId;
            }

            flow.ContactName = data.ContactName;
            flow.Tel = data.Tel;
            flow.OrgName = data.OrgName;
            flow.Industry = data.Industry;
            flow.Logo = data.Logo;
            flow.Size = data.Size;
            flow.Lng = data.Lng;
            flow.Lat = data.Lat;
            flow.AddressCode = data.AddressCode;
            flow.AddressName = data.AddressName;
            flow.AddressDetail = data.AddressDetail;

            flow.FactoryId = user.OrgId;
            flow.Regions = data.Regions;
            flow.FromType = "I";
            flow.BindCustomerId = string.Empty;
            flow.Status = 0;
            flow.OverTime = DateTime.Now.AddDays(7);
            await _agentFlowDAL.Insert(flow);
            return BusResponse<string>.Success(flow.Id);
        }
        /// <summary>
        /// 通过邀请链接加入
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<string>> JoinByInvitLink(In_JoinInvite data)
        {
            var user = _provider.GetUser();

            var flow = await _agentFlowDAL.Select(data.Id);
            if (flow == null)
            {
                return BusResponse<string>.Error(132, "邀请不存在");
            }
            if (DateTime.Now > flow.OverTime)
            {
                return BusResponse<string>.Error(133, "邀请已超时");
            }
            if (flow.Status != 0)
            {
                return BusResponse<string>.Error(134, "邀请状态错误");
            }
            var factory = await _factoryDAL.Select(flow.FactoryId);
            if (factory == null)
            {
                return BusResponse<string>.Error(138, "生产商已不存在");
            }

            MZ_AgentFlow newflow = new MZ_AgentFlow();
            newflow.BelowOrgId = data.OrgId;
            MZ_Agent agent = null;
            MZ_UserRole ur = null;
            if (!string.IsNullOrEmpty(flow.GradeId))
            {
                if (!await _agentDAL.Some(x => x.FactoryId == flow.FactoryId && x.OrgId == data.OrgId))
                {
                    //新增代理
                    agent = new MZ_Agent();
                    agent.Id = _snowflake.NextId().ToString();
                    agent.ParentOrgId = flow.ParentOrgId;
                    agent.OrgId = data.OrgId;
                    agent.GradeId = flow.GradeId;
                    if (flow.FactoryId == flow.ParentOrgId)
                    {
                        agent.LevelPath = flow.FactoryId + "," + data.OrgId + ",";
                    }
                    else
                    {
                        var parentAgent = (await _agentDAL.SelectList(x => x.FactoryId == flow.FactoryId && x.OrgId == flow.ParentOrgId)).FirstOrDefault();
                        if (parentAgent == null)
                        {
                            return BusResponse<string>.Error(141, "上级代理权限已失效，请联系您的上级");
                        }
                        agent.LevelPath = parentAgent.LevelPath + data.OrgId + ",";
                    }
                    agent.FactoryId = flow.FactoryId;
                    agent.Regions = flow.Regions;

                    //分配代理角色权限
                    if (!await _userDAL.ExistUserRole(4, user.UserId, agent.OrgId.Value))
                    {
                        ur = new MZ_UserRole();
                        ur.UserId = user.UserId;
                        ur.RoleID = 4;
                        ur.OrgId = agent.OrgId;
                    }
                }

            }

            //从生产商继承企业主题
            MZ_OrgStyle style = null;
            var orgStyleDAL = _provider.GetService<OrgStyleDAL>();
            var orgstylelist = await orgStyleDAL.SelectList(x => x.OrgId == flow.FactoryId && x.IsUsing == true);
            if (orgstylelist.Count > 0)
            {
                style = new MZ_OrgStyle();
                style.OrgId = data.OrgId;
                style.StyleId = orgstylelist[0].StyleId;
                style.CreatedOn = DateTime.Now;
                style.FrowWay = 1;
                style.IsUsing = true;
            }


            try
            {
                await BusUtility.Dispatch("JoinBy", new
                {
                    UserId = user.UserId,
                    BindCustomerId = flow.BindCustomerId,
                    OrgId = data.OrgId.Value
                });

                using (BLLTranScope scope = new BLLTranScope())
                {
                    if (style != null)
                    {
                        await orgStyleDAL.ClearUsing(style.OrgId.Value);
                        await orgStyleDAL.CreateOrUpdate(style);
                    }


                    newflow.Status = 1;
                    newflow.SetUpdateBy(user);
                    await _agentFlowDAL.Update(newflow, x => x.Id == data.Id && x.Status == 0);
                    if (agent != null)
                    {
                        agent.SetCreateBy(user);
                        await _agentDAL.Insert(agent);
                        if (ur != null)
                        {
                            await _userDAL.AddUserRoleItem(ur);
                        }
                    }

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


        public async Task<BusResponse<string>> SendYaoQing(string phone, string url, string code, IUserInfo user)
        {
            var flow = await _agentFlowDAL.Select(code);
            if (flow == null)
            {
                return BusResponse<string>.Error(132, "邀请不存在");
            }
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            string tkey = "agyq_count_" + user.UserId;
            Tx_AgYq_Count cc = await redis.StringGetAsync<Tx_AgYq_Count>(tkey);
            if (cc == null)
            {
                cc = new Tx_AgYq_Count();
                cc.first_send = new DateTime();
                cc.count = 1;
            }
            else
            {
                cc.count++;
            }
            if (cc.count > 20)
            {
                return BusResponse<string>.Error(110, "短时间内发送太多邀请短信,请30分钟后重试");
            }
            await redis.StringSetAsync(tkey, cc, TimeSpan.FromMinutes(30));


            string phonecode = Guid.NewGuid().ToString("N");
            MZ_AgentFlow newflow = new MZ_AgentFlow();
            newflow.Id = code;
            newflow.PhoneCode = phonecode;
            await _agentFlowDAL.Update(newflow);

            //发送短信
            List<TargetUser> users = new List<TargetUser>();
            users.Add(new TargetUser()
            {
                phone = phone
            });
            var nt = new NoticeEvent(2, users.ToArray(), new string[] { "SMS" });
            nt.TargetType = "邀请短信";
            nt.TargetUrl = phone;
            if (url.IndexOf("?") != -1)
            {
                nt.Content = $"{url}&yqcode={code}&node={phonecode}";
            }
            else
            {
                nt.Content = $"{url}?yqcode={code}&node={phonecode}";
            }

            await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
            return BusResponse<string>.Success();
        }

        public async Task<BusResponse<Out_Login>> RegByYq(In_RegTelYqData data, ITAContext context)
        {
            if (string.IsNullOrEmpty(data.RealName))
            {
                return BusResponse<Out_Login>.Error(111, "真实姓名不能为空");
            }
            if (string.IsNullOrEmpty(data.Mobile))
            {
                return BusResponse<Out_Login>.Error(112, "手机号不能为空");
            }
            if (!StringHelper.IsMobile(data.Mobile))
            {
                return BusResponse<Out_Login>.Error(113, "手机格式错误");
            }
            if (string.IsNullOrEmpty(data.Password))
            {
                return BusResponse<Out_Login>.Error(114, "密码不能为空");
            }
            if (data.Password.Length < 3 || data.Password.Length > 20)
            {
                return BusResponse<Out_Login>.Error(115, "请输入3-20长度的密码");
            }

            data.Email = string.Empty;

            if (string.IsNullOrEmpty(data.YqCode))
            {
                return BusResponse<Out_Login>.Error(122, "手机邀请码不能为空");
            }
            var flow = await _agentFlowDAL.Select(data.YqCode);
            if (flow == null)
            {
                return BusResponse<Out_Login>.Error(122, "手机邀请码不存在");
            }

            if (data.MobileCode != flow.PhoneCode)
            {
                return BusResponse<Out_Login>.Error(122, "手机验证码错误");
            }

            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            string userMobileLock = "UserMobileLock" + data.Mobile;
            if (await redis.WaitLockTakeAsync(userMobileLock))
            {
                try
                {
                    MZ_AdminInfo info = await _userDAL.GetAdminByMobile(data.Mobile);
                    if (info != null)
                    {
                        return BusResponse<Out_Login>.Error(116, "手机号已被注册");
                    }

                    UserBLL userBLL = _provider.GetService<UserBLL>();
                    return await userBLL.Reg(data, context);
                }
                finally
                {
                    redis.LockRelease(userMobileLock);
                }
            }
            else
            {
                return BusResponse<Out_Login>.ErrorBusy();
            }
        }

        public async Task<BusResponse<Out_Login>> LoginByYq(In_YqLogin data, ITAContext context)
        {
            if (string.IsNullOrEmpty(data.YqCode))
            {
                return BusResponse<Out_Login>.Error(122, "手机邀请码不能为空");
            }
            var flow = await _agentFlowDAL.Select(data.YqCode);
            if (flow == null)
            {
                return BusResponse<Out_Login>.Error(122, "手机邀请码不存在");
            }

            if (string.IsNullOrEmpty(flow.Tel))
            {
                return BusResponse<Out_Login>.Error(122, "邀请信息没有手机号");
            }
            if (data.MobileCode != flow.PhoneCode)
            {
                return BusResponse<Out_Login>.Error(122, "手机验证码错误");
            }
            MZ_AdminInfo account = await _userDAL.GetAdminByMobile(flow.Tel);
            if (account == null) return BusResponse<Out_Login>.Error(1, "用户不存在");
            if (account.status == "1") return BusResponse<Out_Login>.Error(2, "用户已被停用");
            return await _provider.GetService<AuthBLL>().Login(account, context);

        }

    }
}
