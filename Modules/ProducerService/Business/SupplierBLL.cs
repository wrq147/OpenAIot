using AuthService.Fields;
using Common;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using ProducerService.DAL;
using ProducerService.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace ProducerService.Business
{
    public class SupplierBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private SupplierDAL _supplierDAL;
        public SupplierBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, SupplierDAL supplierDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _supplierDAL = supplierDAL;
        }
        public virtual async Task<PageObject<MZ_Supplier>> SelectList(In_SupplierList query, IUserInfo user)
        {
            if (query.Items != null && query.Items.Length > 0)
            {
                var typeNameItem = query.Items.Where(x => x.field == "StatusName").FirstOrDefault();
                if (typeNameItem != null)
                {
                    typeNameItem.field = "Status";
                    if (typeNameItem.val == "停用")
                    {
                        typeNameItem.val = "0";
                    }
                    else if (typeNameItem.val == "正常")
                    {
                        typeNameItem.val = "1";
                    }
                    else
                    {
                        typeNameItem.val = null;
                    }
                }
            }
            var tpage = await _supplierDAL.SelectByPage(query, user.OrgId);
            await FieldUtility.GenerateExtValList(_provider, tpage.List);
            foreach (var item in tpage.List)
            {
                item.StatusName = item.Status == "0" ? "停用" : "正常";
            }
            return tpage;
        }
        public virtual async Task<string> GenerateNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("GY");
        }
        public virtual async Task<BusResponse<MZ_Supplier>> Info(string id)
        {
            var supplierInfo = await _supplierDAL.Select(id);
            if (supplierInfo == null)
            {
                return BusResponse<MZ_Supplier>.Error(111, "供应商不存在");
            }
            await FieldUtility.GenerateExtObject(_provider, supplierInfo, supplierInfo.OrgId.Value);
            await FieldUtility.GenerateExtVals(_provider, supplierInfo);
            return BusResponse<MZ_Supplier>.Success(supplierInfo);
        }
        public virtual async Task<BusResponse<string>> Add(MZ_Supplier data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(133, "请切换到企业账号");
            }
            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;

            if (string.IsNullOrEmpty(data.SupplierName))
            {
                return BusResponse<string>.Error(111, "供应商名称不能为空");
            }

            var checkRsp = await FieldUtility.CheckAddForm(_provider, data, user.OrgId, "供应商");
            if (!checkRsp.IsSuccess())
            {
                return checkRsp;
            }

            if (string.IsNullOrEmpty(data.Number))
            {
                data.Number = await GenerateNumber();
            }
            else
            {
                if (await _supplierDAL.Some(x => x.Number == data.Number))
                {
                    return BusResponse<string>.Error(117, "供应商编号已被使用");
                }
            }
            data.FullName ??= string.Empty;
            data.PayTerm ??= 0;
            data.ContactName ??= string.Empty;
            data.Tel ??= string.Empty;
            if (data.Lng != null && data.Lat != null)
            {
                data.Geo = MyAccess.Core.GeoHash.Encode(data.Lat.Value, data.Lng.Value);
            }
            data.Lng ??= 0;
            data.Lat ??= 0;
            data.Geo ??= string.Empty;
            data.AddressCode ??= string.Empty;
            data.AddressName ??= string.Empty;
            data.AddressDetail ??= string.Empty;
            data.SetCreateBy(user);
            await FieldUtility.UpdateFieldEntity(_provider, data, user.OrgId);
            await _supplierDAL.Insert(data);
            return BusResponse<string>.Success(data.Id);
        }

        public virtual async Task<BusResponse<int>> Edit(MZ_Supplier data, IUserInfo user)
        {
            var tmpsupplier = await _supplierDAL.Select(data.Id);
            if (tmpsupplier == null)
            {
                return BusResponse<int>.Error(111, "供应商不存在");
            }
            if (tmpsupplier.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(112, "供应商所属组织错误");
            }
            if (string.IsNullOrEmpty(data.SupplierName))
            {
                return BusResponse<int>.Error(113, "供应商名称不能为空");
            }
            var checkRsp = await FieldUtility.CheckEditForm(_provider, data, user.OrgId, "供应商");
            if (!checkRsp.IsSuccess())
            {
                return checkRsp;
            }
            data.OrgId = null;
            data.Number = null;
            if (data.Lng != null && data.Lat != null)
            {
                data.Geo = MyAccess.Core.GeoHash.Encode(data.Lat.Value, data.Lng.Value);
            }
            await FieldUtility.UpdateFieldEntity(_provider, data, user.OrgId);
            return BusResponse<int>.Success(await _supplierDAL.Update(data));
        }

        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            MZ_Supplier old = await _supplierDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(111, "供应商不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(112, "供应商所属组织错误");
            }
            await FieldUtility.DeleteFieldEntity(_provider, old);
            return BusResponse<int>.Success(await _supplierDAL.Delete(id));
        }

    }
}
