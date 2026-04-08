using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
            var rsp = await _videoConfigDAL.SelectPage(expression, query, string.Empty, "Id,Name");
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
