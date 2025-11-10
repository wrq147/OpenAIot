using AuthService.Model;
using Common;
using Common.IdGenerator;
using Common.Share;
using MESService.DAL;
using MESService.Model;
using System.Linq.Expressions;
using System;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using MyAccess.DB.Builder.WhereToSql;

namespace MESService.Business
{
    public class DefectBLL
    {
        private ITAServiceProvider _provider;
        private DefectDAL _defectDAL;
        public DefectBLL(ITAServiceProvider provider, DefectDAL defectDAL)
        {
            _provider = provider;
            _defectDAL = defectDAL;
        }


        public virtual async Task<PageObject<MZ_DefectType>> SelectList(In_DefectList query, IUserInfo user)
        {
            Expression<Func<MZ_DefectType, bool>> expression = x => x.OrgId == user.OrgId;
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And(x => x.DefectName.Contains(query.Key) || x.DefectCategory.Contains(query.Key));
            }
            var tpage = await _defectDAL.SelectPage(expression, query, string.Empty);
            return tpage;
        }

        public virtual async Task<BusResponse<MZ_DefectType>> Info(string id)
        {
            var info = await _defectDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_DefectType>.Error(111, "不良品不存在");
            }
            return BusResponse<MZ_DefectType>.Success(info);
        }
        public virtual async Task<string> GenerateNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("DF");
        }
        public virtual async Task<BusResponse<string>> Add(MZ_DefectType data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(133, "请切换到企业账号");
            }
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            data.OrgId = user.OrgId;

            if (string.IsNullOrEmpty(data.DefectName))
            {
                return BusResponse<string>.Error(111, "名称不能为空");
            }

            if (string.IsNullOrEmpty(data.DefectCategory))
            {
                return BusResponse<string>.Error(111, "类别不能为空");
            }

            await _defectDAL.Insert(data);
            return BusResponse<string>.Success(data.Id);
        }

        public virtual async Task<BusResponse<int>> Edit(MZ_DefectType data, IUserInfo user)
        {
            var old = await _defectDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(111, "不良品不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(112, "所属组织错误");
            }
            if (string.IsNullOrEmpty(data.DefectName))
            {
                return BusResponse<int>.Error(113, "名称不能为空");
            }

            if (string.IsNullOrEmpty(data.DefectCategory))
            {
                return BusResponse<int>.Error(114, "类别不能为空");
            }

            return BusResponse<int>.Success(await _defectDAL.Update(data));
        }

        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            MZ_DefectType old = await _defectDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(111, "不良项不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(112, "所属组织错误");
            }

            return BusResponse<int>.Success(await _defectDAL.Delete(id));
        }
    }
}
