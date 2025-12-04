using AuthService;
using Common.EventBus;
using CRMService.DAL;
using CRMService.Model;
using DiscussService;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CRMService.Business
{
    public class DiscussEventBLL
    {
        private ITAServiceProvider _provider;
        private ClueDAL _clueDAL;
        private UserDAL _userDAL;
        private CustomerDAL _customerDAL;
        private OpportunityDAL _opportDAL;
        public DiscussEventBLL(ITAServiceProvider provider, ClueDAL clueDAL, UserDAL userDAL, CustomerDAL customerDAL, OpportunityDAL opportDAL)
        {
            _provider = provider;
            _clueDAL = clueDAL;
            _userDAL = userDAL;
            _customerDAL = customerDAL;
            _opportDAL = opportDAL;
        }

        public virtual async Task DoEvent(DiscussEvent evt)
        {
            //默认站内通知
            var option = _provider.GetService<IOptions<DiscussOption>>();
            string[] noticeWay = option.Value.discuss_way.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (noticeWay.Length == 0)
            {
                noticeWay = new string[] { "APP" };
            }

            //通知被评论人
            switch (evt.TargetType)
            {
                case "线索":
                    {
                        MZ_Clue clue = await _clueDAL.Select(evt.TargetId);
                        if (clue != null && clue.LeaderId.Value > 0)
                        {
                            List<long> uids = new List<long>();
                            uids.Add(clue.LeaderId.Value);

                            string[] helperss = clue.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            if (helperss.Length > 0)
                            {
                                uids.AddRange(Array.ConvertAll(helperss, long.Parse));
                            }
                            var adminList = await _provider.GetService<UserDAL>().GetUserListByIds(uids);
                            List<TargetUser> targetusers = new List<TargetUser>();
                            foreach (var adminif in adminList)
                            {
                                targetusers.Add(new TargetUser()
                                {
                                    uid = adminif.Id.Value,
                                    email = adminif.Email,
                                    phone = adminif.Mobile
                                });
                            }
                            if (targetusers.Count > 0)
                            {
                                NoticeEvent nt = new NoticeEvent(2, targetusers.ToArray(), noticeWay);
                                nt.TargetType = "线索评论";
                                nt.TargetUrl = evt.TargetId;
                                nt.Label = "新增评论提醒";
                                nt.Level = 0;
                                nt.Content = $"{evt.RealName}为线索添加了一条评论";
                                nt.OrgId = clue.OrgId.Value;
                                await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                            }



                        }

                    }
                    break;
                case "客户":
                    {
                        MZ_Customer customer = await _customerDAL.Select(evt.TargetId);
                        if (customer != null && customer.LeaderId.Value > 0)
                        {
                            List<long> uids = new List<long>();
                            uids.Add(customer.LeaderId.Value);

                            string[] helperss = customer.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            if (helperss.Length > 0)
                            {
                                uids.AddRange(Array.ConvertAll(helperss, long.Parse));
                            }

                            var adminList = await _provider.GetService<UserDAL>().GetUserListByIds(uids);
                            List<TargetUser> targetusers = new List<TargetUser>();
                            foreach (var adminif in adminList)
                            {
                                targetusers.Add(new TargetUser()
                                {
                                    uid = adminif.Id.Value,
                                    email = adminif.Email,
                                    phone = adminif.Mobile
                                });
                            }
                            if (targetusers.Count > 0)
                            {
                                NoticeEvent nt = new NoticeEvent(2, targetusers.ToArray(), noticeWay);
                                nt.TargetType = "客户评论";
                                nt.TargetUrl = evt.TargetId;
                                nt.Label = "新增评论提醒";
                                nt.Level = 0;
                                nt.Content = $"{evt.RealName}为客户添加了一条评论";
                                nt.OrgId = customer.OrgId.Value;
                                await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                            }

                        }

                    }
                    break;
                case "商机":
                    {
                        MZ_Opportunity opport = await _opportDAL.Select(evt.TargetId);
                        if (opport != null && opport.LeaderId.Value > 0)
                        {
                            List<long> uids = new List<long>();
                            uids.Add(opport.LeaderId.Value);

                            string[] helperss = opport.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            if (helperss.Length > 0)
                            {
                                uids.AddRange(Array.ConvertAll(helperss, long.Parse));
                            }

                            var adminList = await _provider.GetService<UserDAL>().GetUserListByIds(uids);
                            List<TargetUser> targetusers = new List<TargetUser>();
                            foreach (var adminif in adminList)
                            {
                                targetusers.Add(new TargetUser()
                                {
                                    uid = adminif.Id.Value,
                                    email = adminif.Email,
                                    phone = adminif.Mobile
                                });
                            }
                            if (targetusers.Count > 0)
                            {
                                NoticeEvent nt = new NoticeEvent(2, targetusers.ToArray(), noticeWay);
                                nt.TargetType = "商机评论";
                                nt.TargetUrl = evt.TargetId;
                                nt.Label = "新增评论提醒";
                                nt.Level = 0;
                                nt.Content = $"{evt.RealName}为商机添加了一条评论";
                                nt.OrgId = opport.OrgId.Value;
                                await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                            }
         
                        }

                    }
                    break;
            }



        }
    }
}
