using AuthService;
using Common.IdGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using DiscussService.DAL;
using Common.Share;
using DiscussService.Model;
using DictService.Business;
using MyAccess.Filter;
using Common.EventBus;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Components;

namespace DiscussService.Business
{
    public class CommentBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private CommentDAL _commentDAL;
        private SubjectDAL _subjectDAL;
        public CommentBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, CommentDAL commentDAL, SubjectDAL subjectDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _commentDAL = commentDAL;
            _subjectDAL = subjectDAL;
        }
        public virtual async Task<PageObject<MZ_Comment>> SelectList(In_CommentList query)
        {
            var user = _provider.GetUser();
            return await _commentDAL.SelectByPage(query, user);
        }

        public virtual async Task<BusResponse<string>> Add(In_AddComment data)
        {
            var dictTypeBLL = _provider.GetService<DictTypeBLL>();
            var dictConverter = await dictTypeBLL.SelectDictConverter("comment_type");
            if (dictConverter.ToName(data.TargetType) == null)
            {
                return BusResponse<string>.Error(122, "评论类型不存在");
            }

            var user = _provider.GetUser();
            data.Id = _snowflake.NextId().ToString();
            data.del_flag = "0";
            data.OrgId = user.OrgId;
            data.CreateOn = DateTime.Now;
            data.UserId = user.UserId;
            HtmlFilter filter = new HtmlFilter();
            data.Content = filter.FilterHtml(data.Content);
            if (!string.IsNullOrEmpty(data.TargetId))
            {
                var tmpList = await _subjectDAL.SelectList(x => x.TargetId == data.TargetId && x.TargetType == data.TargetType);
                if (tmpList.Count == 0)
                {
                    return BusResponse<string>.Error(133, "评论的主题不存在");
                }
                data.SubjectId = tmpList[0].Id;
            }
            data.ParentCommentId ??= string.Empty;
            data.ParentCommentUserId ??= 0;
            data.PraiseNum = 0;

            await _commentDAL.Insert(data);


            //发送@通知
            var option = _provider.GetService<IOptions<DiscussOption>>();
            string[] noticeWay = option.Value.discuss_way.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (noticeWay.Length == 0)
            {
                noticeWay = new string[] { "APP" };
            }
            var spanConfig = filter.GetTag("span");
            List<TagItem> mentionTags = new List<TagItem>();
            foreach (var tag in spanConfig.Tags)
            {
                for (int i = 0; i < tag.PropName.Count; i++)
                {
                    if (tag.PropName[i] == "data-w-e-type" && tag.PropVal[i] == "mention")
                    {
                        mentionTags.Add(tag);
                    }
                }
            }
            List<long> atUsers = new List<long>();
            foreach (var mention in mentionTags)
            {
                int idx = mention.PropName.FindIndex(x => x == "data-info");
                long uid = Convert.ToInt64(mention.PropVal[idx]);
                atUsers.Add(uid);
            }

            var adminList = await _provider.GetService<UserDAL>().GetUserListByIds(atUsers);
            List<TargetUser> targetusers = new List<TargetUser>();
            foreach(var adminif in adminList)
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
                NoticeEvent atnt = new NoticeEvent(2, targetusers.ToArray(), noticeWay);
                atnt.TargetType = data.TargetType + "@";
                atnt.TargetUrl = data.TargetId ?? data.SubjectId;
                atnt.Label = "有新的评论@了您";
                atnt.Level = 0;
                atnt.Content = $"{user.UserName}在评论里@了您";
                atnt.OrgId = user.OrgId;
                await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, atnt);
            }


            //发送评论通知消息
            await BusUtility.Dispatch("NewDiscuss", new
            {
                UserId = user.UserId,
                RealName = user.UserName,
                TargetId = data.TargetId ?? data.SubjectId,
                TargetType = data.TargetType,
                CommentId = data.Id
            });

            return BusResponse<string>.Success();
        }


        public virtual async Task<BusResponse<string>> Delete(string id)
        {
            var user = _provider.GetUser();
            MZ_Comment old = await _commentDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "评论不存在");
            }
            if (user.UserId != old.UserId)
            {
                return BusResponse<string>.Error(124, "无权限删除这条评论");
            }
            MZ_Comment newupdate = new MZ_Comment();
            newupdate.Id = id;
            newupdate.del_flag = "2";
            await _commentDAL.Update(newupdate);

            return BusResponse<string>.Success();
        }

    }
}
