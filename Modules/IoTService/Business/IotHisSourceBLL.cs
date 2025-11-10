using Common.IdGenerator;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using JiebaNet.Segmenter;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotHisSourceBLL
    {
        private ITAServiceProvider _provider;
        private IotHisSourceDAL _hisSourceDAL;
        public IotHisSourceBLL(ITAServiceProvider provider, IotHisSourceDAL hisSourceDAL)
        {
            _provider = provider;
            _hisSourceDAL = hisSourceDAL;
        }
        public virtual async Task<PageObject<MZ_IotHisSource>> SelectPage(In_HisSourceList query, IUserInfo user)
        {
            Expression<Func<MZ_IotHisSource, bool>> expression = x => x.OrgId == user.OrgId || x.OrgId == 0;
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And(x => x.Name.Contains(query.Key) || x.Remark.Contains(query.Key));
            }
            var rsp = await _hisSourceDAL.SelectPage(expression, query, "create_time desc");
            return rsp;
        }

        public virtual async Task<BusResponse<MZ_IotHisSource>> Info(string id)
        {
            var info = await _hisSourceDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_IotHisSource>.Error(111, "数据源不存在");
            }
            return BusResponse<MZ_IotHisSource>.Success(info);
        }
        public virtual async Task<BusResponse<int>> Update(MZ_IotHisSource data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法修改数据源");
            }
            var old = await _hisSourceDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(113, "数据源不存在");
            }
            if (user.OrgId != 1)
            {
                if (old.OrgId != user.OrgId)
                {
                    return BusResponse<int>.Error(114, "无权修改当前数据源");
                }
            }
      
            data.OrgId = null;
            data.SetUpdateBy(user);
            return BusResponse<int>.Success(await _hisSourceDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Insert(MZ_IotHisSource data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法添加数据源");
            }
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            if (user.OrgId == 1)
            {
                data.OrgId = 0;
            }
            else
            {
                data.OrgId = user.OrgId;
            }
            data.SetCreateBy(user);

            return BusResponse<int>.Success(await _hisSourceDAL.Insert(data));
        }
        public virtual async Task<BusResponse<int>> Delete(string[] ids, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法删除数据源");
            }
            try
            {
                if (user.OrgId == 1)
                {
                    var num = await _hisSourceDAL.Delete(x => ids.Contains(x.Id));
                    return BusResponse<int>.Success(num);
                }
                else
                {
                    var num = await _hisSourceDAL.Delete(x => x.OrgId == user.OrgId && ids.Contains(x.Id));
                    return BusResponse<int>.Success(num);
                }
    
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }
    }
}
