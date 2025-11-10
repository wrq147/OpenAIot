using CRMService.DAL;
using CRMService.Model;
using AuthService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using Common.IdGenerator;
using Common.Share;

namespace CRMService.Business
{
    public class PeriodBLL
    {
        private ITAServiceProvider _provider;
        private PeriodDAL _periodDAL;
        public PeriodBLL(ITAServiceProvider provider, PeriodDAL periodDA)
        {
            _provider = provider;
            _periodDAL = periodDA;
        }

        public virtual async Task<List<MZ_Period>> SelectList()
        {
            var user = _provider.GetUser();
            return await _periodDAL.SelectList(x => x.OrgId == user.OrgId, "Sort asc");
        }


        public virtual async Task<BusResponse<string>> Add(MZ_Period data)
        {
            var user = _provider.GetUser();
            data.Id = MyAccess.Core.StringTool.GetGUID();
            data.OrgId = user.OrgId;
            data.PeriodType = "ing";
            await _periodDAL.Insert(data);
            return BusResponse<string>.Success(data.Id);
        }
        public virtual async Task<BusResponse<int>> Edit(MZ_Period data)
        {
            MZ_Period old = await _periodDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(123, "阶段不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(124, "当前用户无权限");
            }

            if (data.PeriodType != "ing")
            {
                data.Sort = null;
                data.Probability = null;
            }
            data.PeriodType = null;
            data.OrgId = null;
            return BusResponse<int>.Success(await _periodDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Delete(string id)
        {
            MZ_Period old = await _periodDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(123, "阶段不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(124, "当前用户无权限");
            }
            if (old.PeriodType != "ing")
            {
                return BusResponse<int>.Error(125, "无法删除非进行中的阶段");
            }

            return BusResponse<int>.Success(await _periodDAL.Delete(id));
        }

    }
}
