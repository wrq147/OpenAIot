using AuthService;
using Common.IdGenerator;
using Common.Share;
using CRMService.DAL;
using CRMService.Model;
using Microsoft.Extensions.Options;
using ProducerService.DAL;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TemplateAction.Core;


namespace CRMService.Business
{
    public class CRMAgentBLL
    {
        private IOptions<GeneralOption> _conf;
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private CustomerDAL _customerDAL;
        private PeriodDAL _periodDAL;
        public CRMAgentBLL(ITAServiceProvider provider, IOptions<GeneralOption> conf, SnowflakeHelper snowflake, CustomerDAL customerDAL, PeriodDAL periodDAL)
        {
            _provider = provider;
            _conf = conf;
            _snowflake = snowflake;
            _customerDAL = customerDAL;
            _periodDAL = periodDAL;
        }


        public virtual async Task<BusResponse<string>> GenerateInvitLink(In_Invite data)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(133, "请切换到企业账号");
            }
            var agentDAL = _provider.GetService<AgentDAL>();
            var gradeDAL = _provider.GetService<GradeDAL>();
            var agentFlowDAL = _provider.GetService<AgentFlowDAL>();
            if (!await agentDAL.Some(x => x.OrgId == user.OrgId) && data.FactoryId != user.OrgId)
            {
                return BusResponse<string>.Error(135, "企业不是代理商或生产商");
            }


            MZ_Customer customer = await _customerDAL.Select(data.BindCustomerId);
            if (customer == null)
            {
                return BusResponse<string>.Error(138, "邀请的客户不存在");
            }
            if (customer.BindOrgId != null && customer.BindOrgId != 0)
            {
                return BusResponse<string>.Error(137, "客户已被绑定");
            }

            //生成邀请单
            MZ_AgentFlow flow = new MZ_AgentFlow();
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
            MZ_Agent curAgent = null;
            if (customer.CustomerType == 0)
            {
                var agentlist = await agentDAL.SelectList(x => x.FactoryId == data.FactoryId && x.OrgId == user.OrgId);
                if (agentlist.Count == 0)
                {
                    if (user.OrgId == data.FactoryId)
                    {
                        var gradelist = await gradeDAL.SelectList(x => x.FactoryId == data.FactoryId, "Sort asc");
                        if (gradelist.Count == 0)
                        {
                            return BusResponse<string>.Error(136, "您无权添加代理商");
                        }
                        flow.GradeId = gradelist[0].Id;
                    }
                    else
                    {
                        return BusResponse<string>.Error(139, "代理权限已失效，请联系您的上级");
                    }
                }
                else
                {
                    curAgent = agentlist[0];
                    var gradelist = await gradeDAL.SelectList(x => x.FactoryId == data.FactoryId, "Sort asc");
                    if (gradelist.Count == 0)
                    {
                        return BusResponse<string>.Error(136, "您无权添加代理商");
                    }
                    int curIdx = -1;
                    for (int i = 0; i < gradelist.Count; i++)
                    {
                        if (gradelist[i].Id == curAgent.GradeId)
                        {
                            curIdx = i;
                            break;
                        }
                    }
                    if (curIdx < 0)
                    {
                        return BusResponse<string>.Error(140, "上级代理权限已失效，请联系您的上级");
                    }
                    if (curIdx + 1 < gradelist.Count)
                    {
                        flow.GradeId = gradelist[curIdx + 1].Id;
                    }
                    else
                    {
                        return BusResponse<string>.Error(141, "您的代理级别无法添加代理商");
                    }
                }
            }
            else
            {
                flow.GradeId = string.Empty;
            }

            flow.SetCreateBy(user);
            flow.Id = _snowflake.NextId().ToString();
            flow.ParentOrgId = user.OrgId;
            flow.BelowOrgId = 0;
            flow.FactoryId = data.FactoryId;
            flow.Regions = data.Regions;
            flow.FromType = "I";
            flow.BindCustomerId = data.BindCustomerId;
            flow.Status = 0;
            flow.OverTime = DateTime.Now.AddDays(7);
            await agentFlowDAL.Insert(flow);
            return BusResponse<string>.Success(flow.Id);
        }


        public virtual async Task JoinByOtherMod(JoinEventData data)
        {
            var agentFlowDAL = _provider.GetService<AgentFlowDAL>();
            MZ_Customer customer = null;
            if (!string.IsNullOrEmpty(data.BindCustomerId))
            {
                //绑定客户
                customer = new MZ_Customer();
                customer.Id = data.BindCustomerId;
                customer.BindOrgId = data.OrgId;
                customer.updateId = data.UserId;
                customer.update_time = DateTime.Now;
                await _customerDAL.Update(customer);

                //使其它相同BindCustomerId的邀请单过期
                MZ_AgentFlow upflow = new MZ_AgentFlow();
                upflow.OverTime = DateTime.Now.AddHours(-1);
                await agentFlowDAL.Update(upflow, x => x.BindCustomerId == data.BindCustomerId && x.OverTime > DateTime.Now);
            }


            if (!await _periodDAL.Some(x => x.OrgId == data.OrgId))
            {
                //初始化销售阶段
                MZ_Period p1 = new MZ_Period();
                p1.PeriodName = "需求发现";
                p1.Sort = 0;
                p1.PeriodType = "ing";
                p1.Id = _snowflake.NextId().ToString();
                p1.OrgId = data.OrgId;
                p1.Probability = 20;
                await _periodDAL.Insert(p1);

                MZ_Period p2 = new MZ_Period();
                p2.PeriodName = "需求确认";
                p2.Sort = 1;
                p2.PeriodType = "ing";
                p2.Id = _snowflake.NextId().ToString();
                p2.OrgId = data.OrgId;
                p2.Probability = 40;
                await _periodDAL.Insert(p2);

                MZ_Period p3 = new MZ_Period();
                p3.PeriodName = "方案报价";
                p3.Sort = 2;
                p3.PeriodType = "ing";
                p3.Id = _snowflake.NextId().ToString();
                p3.OrgId = data.OrgId;
                p3.Probability = 60;
                await _periodDAL.Insert(p3);

                MZ_Period p4 = new MZ_Period();
                p4.PeriodName = "商务谈判";
                p4.Sort = 3;
                p4.PeriodType = "ing";
                p4.Id = _snowflake.NextId().ToString();
                p4.OrgId = data.OrgId;
                p4.Probability = 80;
                await _periodDAL.Insert(p4);

                MZ_Period p5 = new MZ_Period();
                p5.PeriodName = "赢单";
                p5.Sort = 4;
                p5.PeriodType = "win";
                p5.Id = _snowflake.NextId().ToString();
                p5.OrgId = data.OrgId;
                p5.Probability = 100;
                await _periodDAL.Insert(p5);

                MZ_Period p6 = new MZ_Period();
                p6.PeriodName = "输单";
                p6.Sort = 5;
                p6.PeriodType = "lose";
                p6.Id = _snowflake.NextId().ToString();
                p6.OrgId = data.OrgId;
                p6.Probability = 0;
                await _periodDAL.Insert(p6);

                MZ_Period p7 = new MZ_Period();
                p7.PeriodName = "无效";
                p7.Sort = 6;
                p7.PeriodType = "invalid";
                p7.Id = _snowflake.NextId().ToString();
                p7.OrgId = data.OrgId;
                p7.Probability = 0;
                await _periodDAL.Insert(p7);
            }

        }
    }
}
