using AuthService;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using LLMService.DAL;
using LLMService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace LLMService.Business
{
    public class KnowledgeBLL
    {
        private readonly KnowledgeDAL _kbDal;
        private ITAServiceProvider _provider;
        public KnowledgeBLL(KnowledgeDAL kbDal, ITAServiceProvider provider)
        {
            _kbDal = kbDal;
            _provider = provider;
        }


        public async Task<PageObject<MZ_Knowledge>> QueryList(In_KnowledgeQuery query, IUserInfo user)
        {
            var result = await _kbDal.SelectByPage(query, user);
            return result;
        }

        public async Task<MZ_Knowledge> GetById(string id, IUserInfo user)
        {
            var result = await _kbDal.SelectList(x => x.Id == id && (x.IsPublic == true || x.OrgId == user.OrgId));
            return result.FirstOrDefault();
        }

        public async Task<BusResponse<string>> Add(MZ_Knowledge entity, IUserInfo user)
        {
            entity.OrgId = user.OrgId;
            var snowflake = _provider.GetService<SnowflakeHelper>();
            entity.Id = snowflake.NextId().ToString();
            entity.Status = 0;
            entity.DocCount = 0;
            entity.SetCreateBy(user);
            await _kbDal.Insert(entity);
            return BusResponse<string>.Success(entity.Id);
        }

        public async Task<BusResponse<int>> Update(MZ_Knowledge entity, IUserInfo user)
        {
            var existing = await _kbDal.Select(entity.Id);
            if (existing == null || existing.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(111, "知识库不存在或无权修改");
            }
            if (existing.Status != 0)
            {
                return BusResponse<int>.Error(112, "知识库状态错误，无法修改");
            }
            entity.Id = null;
            entity.OrgId = null;
            entity.Status = null;
            entity.DocCount = null;
            entity.SetUpdateBy(user);
            return BusResponse<int>.Success(await _kbDal.Update(existing));
        }

        public async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            var entity = await _kbDal.Select(id);
            if (entity == null || entity.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(111, "知识库不存在或无权修改");
            }

            return BusResponse<int>.Success(await _kbDal.Delete(id));
        }

        public async Task<BusResponse<int>> EnableKnowledge(string id, IUserInfo user)
        {
            var entity = await _kbDal.Select(id);
            if (entity == null || entity.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(111, "知识库不存在或无权修改");
            }
            if (user.OrgId == 1)
            {
                entity.Status = 1;
            }
            else
            {
                if (entity.IsPublic == true && entity.Status != 1)
                {
                    entity.Status = 2;

                    var userDAL = _provider.GetService<UserDAL>();
                    var recvList = await userDAL.SelectManUsers(1);
                    List<TargetUser> targets = new List<TargetUser>();
                    foreach (var recvId in recvList)
                    {
                        targets.Add(new TargetUser()
                        {
                            uid = recvId.Id.Value,
                            email = recvId.Email,
                            phone = recvId.Mobile
                        });
                    }
                    var nt = new NoticeEvent(2, targets.ToArray(), new string[] { "APP" });
                    nt.OrgId = 1;
                    nt.TargetType = "Knowledge";
                    nt.TargetUrl = string.Empty;
                    nt.Content = $"知识库【{entity.Name}】需要您的审核";
                    nt.Label = "知识库发布申请消息";
                    await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                }
                else
                {
                    entity.Status = 1;
                }
            }
            entity.SetUpdateBy(user);
            return BusResponse<int>.Success(await _kbDal.Update(entity));
        }
        public async Task<BusResponse<int>> DisableKnowledge(string id, IUserInfo user)
        {
            var entity = await _kbDal.Select(id);
            if (entity == null || entity.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(111, "知识库不存在或无权修改");
            }
            entity.Status = 0;
            entity.SetUpdateBy(user);
            return BusResponse<int>.Success(await _kbDal.Update(entity));
        }

    }
}