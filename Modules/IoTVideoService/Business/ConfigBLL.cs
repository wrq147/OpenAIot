using AuthService;
using Common.IdGenerator;
using Common.Share;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Linq.Expressions;
using TemplateAction.Core;

namespace IoTVideoService.Business
{
    public class ConfigBLL
    {
        private ITAServiceProvider _provider;
        private VideoConfigDAL _videoConfigDAL;
        public ConfigBLL(ITAServiceProvider provider, VideoConfigDAL videoConfigDAL)
        {
            _provider = provider;
            _videoConfigDAL = videoConfigDAL;
        }

        public virtual async Task<PageObject<MZ_IotVideoConfig>> SelectPage(In_VideoConfigPage query, IUserInfo user)
        {
            Expression<Func<MZ_IotVideoConfig, bool>> expression = x => x.OrgId == user.OrgId;
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And(x => x.Name.Contains(query.Key));
            }
            var rsp = await _videoConfigDAL.SelectPage(expression, query, string.Empty, "Id,Name,OrgId,createId,updateId,create_time,update_time");


            if (rsp.List.Count > 0)
            {
                var createdIds = rsp.List.Select(x => x.createId.Value).ToList();
                var updatedIds = rsp.List.Select(x => x.updateId.Value).ToList();
                List<long> concatList = createdIds.Concat(updatedIds).ToList();
                var tmpUsers = await _provider.GetService<UserDAL>().GetUserListByIds(concatList);
                foreach (var item in rsp.List)
                {
                    item.updateName = tmpUsers.Where(x => x.Id == item.updateId).FirstOrDefault()?.RealName;
                    item.createName = tmpUsers.Where(x => x.Id == item.createId).FirstOrDefault()?.RealName;
                }
            }
            return rsp;
        }

        public virtual async Task<MZ_IotVideoConfig> Info(string id)
        {
            return await _videoConfigDAL.Select(id);
        }
        public virtual async Task<BusResponse<int>> Update(MZ_IotVideoConfig data, IUserInfo user)
        {
            var old = await _videoConfigDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(113, "策略不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(114, "所属组织错误");
            }
            data.OrgId = null;
            data.SetUpdateBy(user);
            return BusResponse<int>.Success(await _videoConfigDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Insert(MZ_IotVideoConfig data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法添加策略");
            }
            data.Id = _provider.GetService<SnowflakeHelper>().NextId().ToString();
            data.OrgId = user.OrgId;
            data.SetCreateBy(user);
            return BusResponse<int>.Success(await _videoConfigDAL.Insert(data));
        }

        public virtual async Task<BusResponse<int>> Remove(string id, IUserInfo user)
        {
            var old = await _videoConfigDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(114, "策略不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(115, "所属组织错误");
            }
            if (await _provider.GetService<VideoSourceDAL>().Some(x => x.ConfigId == id))
            {
                return BusResponse<int>.Error(116, "策略正被视频源使用，无法删除！");
            }
            return BusResponse<int>.Success(await _videoConfigDAL.Delete(x => x.Id == id));
        }
    }
}
