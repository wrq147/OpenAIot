using AuthService.Fields;
using Common.IdGenerator;
using Common.Share;
using MESService.DAL;
using MESService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MESService.Business
{
    public class OperBLL
    {
        private ITAServiceProvider _provider;
        private OperDAL _operDAL;
        public OperBLL(ITAServiceProvider provider, OperDAL operDAL)
        {
            _provider = provider;
            _operDAL = operDAL;
        }
        public virtual async Task<PageObject<MZ_ProductOper>> SelectList(In_OperList query, IUserInfo user)
        {
            var tpage = await _operDAL.SelectByPage(query, user.OrgId);
            await FieldUtility.GenerateExtValList(_provider, tpage.List);
            return tpage;
        }

        public virtual async Task<BusResponse<MZ_ProductOper>> Info(string id)
        {
            var info = await _operDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_ProductOper>.Error(111, "工序不存在");
            }
            await FieldUtility.GenerateExtObject(_provider, info, info.OrgId.Value);
            await FieldUtility.GenerateExtVals(_provider, info);
            return BusResponse<MZ_ProductOper>.Success(info);
        }
        public virtual async Task<BusResponse<MZ_ProductRouteOper>> RouteInfo(string id)
        {
            var info = await _provider.GetService<RouteOperDAL>().Select(id);
            if (info == null)
            {
                return BusResponse<MZ_ProductRouteOper>.Error(111, "工艺路线明细不存在");
            }
            await FieldUtility.GenerateExtObject(_provider, info, info.OrgId.Value);
            await FieldUtility.GenerateExtVals(_provider, info);
            return BusResponse<MZ_ProductRouteOper>.Success(info);
        }

        public virtual async Task<BusResponse<string>> Add(MZ_ProductOper data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(133, "请切换到企业账号");
            }
            var checkRsp = await FieldUtility.CheckAddForm(_provider, data, user.OrgId, "工序");
            if (!checkRsp.IsSuccess())
            {
                return checkRsp;
            }
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.DeviceIds ??= string.Empty;
            data.DefectJson ??= string.Empty;
            data.ReportFields ??= string.Empty;
            data.FieldsInit ??= string.Empty;
            data.SetCreateBy(user);
            await FieldUtility.UpdateFieldEntity(_provider, data, user.OrgId);
            await _operDAL.Insert(data);
            return BusResponse<string>.Success(data.Id);
        }

        public virtual async Task<BusResponse<int>> Edit(MZ_ProductOper data, IUserInfo user)
        {
            var old = await _operDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(111, "工序不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(112, "所属组织错误");
            }

            var checkRsp = await FieldUtility.CheckEditForm(_provider, data, user.OrgId, "产品");
            if (!checkRsp.IsSuccess())
            {
                return checkRsp;
            }

            data.SetUpdateBy(user);
            await FieldUtility.UpdateFieldEntity(_provider, data, user.OrgId);
            return BusResponse<int>.Success(await _operDAL.Update(data));
        }

        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            MZ_ProductOper old = await _operDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(111, "工序不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(112, "所属组织错误");
            }
            await FieldUtility.DeleteFieldEntity(_provider, old);
            return BusResponse<int>.Success(await _operDAL.Delete(id));
        }
    }
}
