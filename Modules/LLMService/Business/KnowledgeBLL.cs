using AuthService;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using LLMService.DAL;
using LLMService.Model;
using NPOI.Util;
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


        public virtual async Task<PageObject<MZ_Knowledge>> QueryList(In_KnowledgeQuery query, IUserInfo user)
        {
            var result = await _kbDal.SelectByPage(query, user);
            return result;
        }

        public virtual async Task<MZ_Knowledge> GetById(string id, IUserInfo user)
        {
            var result = await _kbDal.SelectList(x => x.Id == id && (x.IsPublic == true || x.OrgId == user.OrgId));
            return result.FirstOrDefault();
        }

        public virtual async Task<BusResponse<string>> Add(MZ_Knowledge entity, IUserInfo user)
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

        public virtual async Task<BusResponse<int>> Update(MZ_Knowledge entity, IUserInfo user)
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
            entity.DocCount = await _provider.GetService<ArticleDAL>().Count(x => x.KbId == entity.Id);
            entity.SetUpdateBy(user);
            return BusResponse<int>.Success(await _kbDal.Update(existing));
        }

        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            var entity = await _kbDal.Select(id);
            if (entity == null || entity.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(111, "知识库不存在或无权修改");
            }
            var rs = await _kbDal.Delete(id);
            await _provider.GetService<KnowledgeRagBLL>().DelKnowledgeFromMilvus(id);
            return BusResponse<int>.Success(rs);
        }

        public virtual async Task<BusResponse<int>> EnableKnowledge(string id, IUserInfo user)
        {
            var entity = await _kbDal.Select(id);
            if (entity == null || entity.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(111, "知识库不存在或无权修改");
            }
            if (entity.Status != 0 && entity.Status != 3)
            {
                return BusResponse<int>.Error(112, "当前状态无法发布");
            }
            MZ_Knowledge updateEntity = new MZ_Knowledge();
            updateEntity.Id = entity.Id;
            if (user.OrgId == 1)
            {
                updateEntity.Status = 1;
            }
            else
            {
                if (entity.IsPublic == true)
                {
                    updateEntity.Status = 2;

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
                    nt.Label = "知识库发布申请";
                    await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                }
                else
                {
                    updateEntity.Status = 1;
                }
            }
            updateEntity.SetUpdateBy(user);
            var rs = await _kbDal.Update(updateEntity);

            if (updateEntity.Status == 1)
            {
                //同步张量数据库
                var columnList = await _provider.GetService<KbColumnDAL>().SelectList(x => x.KbId == entity.Id);
                var artList = await _provider.GetService<ArticleDAL>().SelectList(x => x.KbId == entity.Id);
                await _provider.GetService<KnowledgeRagBLL>().SyncArticleToMilvus(entity, columnList, artList);
            }
            return BusResponse<int>.Success(rs);
        }
        public virtual async Task<BusResponse<int>> DisableKnowledge(string id, IUserInfo user)
        {
            var entity = await _kbDal.Select(id);
            if (entity == null || entity.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(111, "知识库不存在或无权修改");
            }
            if (entity.Status != 1 && entity.Status != 2)
            {
                return BusResponse<int>.Error(112, "当前状态无法取消");
            }
            MZ_Knowledge updateEntity = new MZ_Knowledge();
            updateEntity.Id = entity.Id;
            updateEntity.Status = 0;
            updateEntity.SetUpdateBy(user);
            var rs = await _kbDal.Update(updateEntity);
            await _provider.GetService<KnowledgeRagBLL>().DelKnowledgeFromMilvus(id);
            return BusResponse<int>.Success(rs);
        }
        public virtual async Task<BusResponse<int>> AgreeKnowledge(string id, IUserInfo user)
        {
            var entity = await _kbDal.Select(id);
            if (entity == null || entity.Status != 2 || entity.OrgId != 1)
            {
                return BusResponse<int>.Error(111, "知识库不存在或无审核权");
            }

            var userDAL = _provider.GetService<UserDAL>();
            var recvList = await userDAL.SelectManUsers(entity.OrgId.Value);
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
            nt.Content = $"知识库【{entity.Name}】发布申请通过";
            nt.Label = "知识库发布申请通过";
            await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);

            MZ_Knowledge updateEntity = new MZ_Knowledge();
            updateEntity.Id = entity.Id;
            updateEntity.Status = 1;
            var rs = await _kbDal.Update(updateEntity);
            if (entity.Status == 1)
            {
                //同步张量数据库
                var columnList = await _provider.GetService<KbColumnDAL>().SelectList(x => x.KbId == entity.Id);
                var artList = await _provider.GetService<ArticleDAL>().SelectList(x => x.KbId == entity.Id);
                await _provider.GetService<KnowledgeRagBLL>().SyncArticleToMilvus(entity, columnList, artList);
            }
            return BusResponse<int>.Success(rs);
        }

        public virtual async Task<BusResponse<int>> RefuseKnowledge(string id, string reason, IUserInfo user)
        {
            var entity = await _kbDal.Select(id);
            if (entity == null || entity.Status != 2 || entity.OrgId != 1)
            {
                return BusResponse<int>.Error(111, "知识库不存在或无审核权");
            }

            var userDAL = _provider.GetService<UserDAL>();
            var recvList = await userDAL.SelectManUsers(entity.OrgId.Value);
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
            nt.Content = $"知识库【{entity.Name}】发布申请被拒，原因{reason}";
            nt.Label = "知识库发布申请被拒";
            await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);

            MZ_Knowledge updateEntity = new MZ_Knowledge();
            updateEntity.Id = entity.Id;
            updateEntity.Status = 3;
            return BusResponse<int>.Success(await _kbDal.Update(updateEntity));
        }
    }
}