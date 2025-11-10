using AuthService;
using Common.IdGenerator;
using Common.Share;
using ProducerService.DAL;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace ProducerService.Business
{
    public class UnitBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private UnitDAL _unitDAL;
        public UnitBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, UnitDAL unitDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _unitDAL = unitDAL;
        }

        public virtual async Task<List<MZ_Unit>> SelectList(In_UnitList query, IUserInfo user)
        {
            Expression<Func<MZ_Unit, bool>> expression = x => x.OrgId == user.OrgId;
            return await _unitDAL.SelectList(expression);
        }

        public virtual async Task<BusResponse<MZ_Unit>> Info(string id)
        {
            var unitInfo = await _unitDAL.Select(id);
            if (unitInfo == null)
            {
                return BusResponse<MZ_Unit>.Error(111, "单位不存在");
            }
            return BusResponse<MZ_Unit>.Success(unitInfo);
        }

        public virtual async Task<BusResponse<string>> Add(MZ_Unit data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(133, "请切换到企业账号");
            }
            if (string.IsNullOrEmpty(data.UnitName))
            {
                return BusResponse<string>.Error(111, "单位名称不能为空");
            }
            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.Remark ??= string.Empty;

            await _unitDAL.Insert(data);
            return BusResponse<string>.Success(data.Id);
        }
        public virtual async Task<BusResponse<int>> Edit(MZ_Unit data, IUserInfo user)
        {
            MZ_Unit old = await _unitDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(123, "单位不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(124, "当前用户无权限");
            }
            data.OrgId = null;
            return BusResponse<int>.Success(await _unitDAL.Update(data));
        }

        public virtual async Task<BusResponse<string>> Delete(string id, IUserInfo user)
        {
            MZ_Unit old = await _unitDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "单位不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            await _unitDAL.Delete(id);
            return BusResponse<string>.Success();
        }

    }
}
