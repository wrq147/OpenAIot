using AuthService;
using AuthService.DAL;
using AuthService.Fields;
using System;
using Common;
using Common.IdGenerator;
using Common.Share;
using ProducerService.DAL;
using ProducerService.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using System.Linq;

namespace ProducerService.Business
{
    public class ProductBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private ProductDAL _productDAL;
        public ProductBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, ProductDAL productDAL)
        {
            _snowflake = snowflake;
            _provider = provider;
            _productDAL = productDAL;
        }
        public virtual async Task<PageObject<MZ_Product>> SelectList(In_ProductList query, IUserInfo user, bool isAgent)
        {
            if (query.Items != null && query.Items.Length > 0)
            {
                var typeNameItem = query.Items.Where(x => x.field == "TypeName" && x.compare == "等于").FirstOrDefault();
                if (typeNameItem != null)
                {
                    var tmpTypeList = await _provider.GetService<ProductTypeDAL>().SelectList(x => x.OrgId == user.OrgId && x.Name == typeNameItem.val);
                    if (tmpTypeList.Count > 0)
                    {
                        query.TypeId = tmpTypeList[0].Id;
                    }
                    typeNameItem.val = null;
                }
            }
            var tpage = await _productDAL.SelectByPage(query, user.OrgId, isAgent);

            await FieldUtility.GenerateExtValList(_provider, tpage.List);

            //初始化供应商
            var supplierIds = tpage.List.Select(x => x.Supplier).Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (supplierIds.Count > 0)
            {
                var supplierDAL = _provider.GetService<SupplierDAL>();
                var suplierlist = await supplierDAL.SelectList(x => supplierIds.Contains(x.Id));
                foreach (var item in tpage.List)
                {
                    var supitem = suplierlist.Where(x => x.Id == item.Supplier).FirstOrDefault();
                    if (supitem != null)
                    {
                        item.SupplierName = supitem.SupplierName;
                    }
                }
            }

            //初始化物联产品
            var iotIds = tpage.List.Select(x => x.IOTProductId).Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (iotIds.Count > 0)
            {
                var iotprolist = await _productDAL.SelectIotProductList(iotIds);
                foreach (var item in tpage.List)
                {
                    var iotitem = iotprolist.Where(x => x.Id == item.IOTProductId).FirstOrDefault();
                    if (iotitem != null)
                    {
                        item.ProductName = iotitem.Name;
                    }
                }
            }

            //初始化工艺路线
            var routeIds = tpage.List.Select(x => x.Route).Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (routeIds.Count > 0)
            {
                var routelist = await _productDAL.SelectProductRouteList(routeIds);
                foreach (var item in tpage.List)
                {
                    var routeitem = routelist.Where(x => x.Id == item.Route).FirstOrDefault();
                    if (routeitem != null)
                    {
                        item.RouteName = routeitem.RouteName;
                    }
                }
            }

            return tpage;
        }
        public virtual async Task<BusResponse<string>> Add(MZ_Product data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(133, "请切换到企业账号");
            }
            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            if (string.IsNullOrEmpty(data.ProductName))
            {
                return BusResponse<string>.Error(111, "产品名称不能为空");
            }
            if (string.IsNullOrEmpty(data.ProductLabel))
            {
                return BusResponse<string>.Error(112, "产品标签不能为空");
            }
            if (string.IsNullOrEmpty(data.ProductFrom))
            {
                return BusResponse<string>.Error(113, "生产来源不能为空");
            }



            var checkRsp = await FieldUtility.CheckAddForm(_provider, data, user.OrgId, "产品");
            if (!checkRsp.IsSuccess())
            {
                return checkRsp;
            }

            if (string.IsNullOrEmpty(data.SkuNumber))
            {
                data.SkuNumber = await GenerateNumber();
            }
            else
            {
                if (await _productDAL.Some(x => x.SkuNumber == data.SkuNumber))
                {
                    return BusResponse<string>.Error(117, "产品sku编码已被使用");
                }
            }

            if (data.ProductLabel != "F")
            {
                data.IOTProductId = string.Empty;
            }
            data.Route ??= string.Empty;
            data.PhotoUrl ??= string.Empty;
            data.Remark ??= string.Empty;
            data.TypeId ??= string.Empty;
            if (string.IsNullOrEmpty(data.MinUnit))
            {
                data.MinUnit = data.Unit;
            }
            data.SetCreateBy(user);
            await FieldUtility.UpdateFieldEntity(_provider, data, user.OrgId);
            await _productDAL.Insert(data);


            return BusResponse<string>.Success(data.Id);
        }
        public virtual async Task<BusResponse<int>> Edit(MZ_Product data, IUserInfo user)
        {
            var tmpproduct = await _productDAL.Select(data.Id);
            if (tmpproduct == null)
            {
                return BusResponse<int>.Error(111, "产品不存在");
            }
            if (tmpproduct.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(112, "产品所属组织错误");
            }
            if (data.ProductName == "")
            {
                return BusResponse<int>.Error(113, "产品名称不能为空");
            }


            var checkRsp = await FieldUtility.CheckEditForm(_provider, data, user.OrgId, "产品");
            if (!checkRsp.IsSuccess())
            {
                return checkRsp;
            }

            data.OrgId = null;
            data.SkuNumber = null;
            data.ProductLabel = null;
            if (tmpproduct.ProductLabel != "F")
            {
                data.IOTProductId = null;
            }
            await FieldUtility.UpdateFieldEntity(_provider, data, user.OrgId);
            return BusResponse<int>.Success(await _productDAL.Update(data));
        }

        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            var tmpproduct = await _productDAL.Select(id);
            if (tmpproduct == null)
            {
                return BusResponse<int>.Error(111, "产品不存在");
            }
            if (tmpproduct.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(112, "产品所属组织错误");
            }
            await FieldUtility.DeleteFieldEntity(_provider, tmpproduct);
            var res = await _productDAL.Delete(id);
            await _provider.GetService<ProductBatchDAL>().Delete(x => x.ProductId == id);
            return BusResponse<int>.Success(res);
        }
        public virtual async Task<BusResponse<MZ_Product>> Info(string id)
        {
            var tmpproduct = await _productDAL.SelectWithTypeById(id);
            if (tmpproduct == null)
            {
                return BusResponse<MZ_Product>.Error(111, "产品不存在");
            }

            if (!string.IsNullOrEmpty(tmpproduct.Supplier))
            {
                var supplierDAL = _provider.GetService<SupplierDAL>();
                var suppitem = await supplierDAL.Select(tmpproduct.Supplier);
                if (suppitem != null)
                {
                    tmpproduct.SupplierName = suppitem.SupplierName;
                }
            }

            if (!string.IsNullOrEmpty(tmpproduct.IOTProductId))
            {
                var tmpiot = await _productDAL.SelectIotProductById(tmpproduct.IOTProductId);
                if (tmpiot != null)
                {
                    tmpproduct.ProductName = tmpiot.Name;
                }
            }

            if (!string.IsNullOrEmpty(tmpproduct.Route))
            {
                var tmproute = await _productDAL.SelectProductRouteById(tmpproduct.Route);
                if (tmproute != null)
                {
                    tmpproduct.RouteName = tmproute.RouteName;
                }
            }

            await FieldUtility.GenerateExtObject(_provider, tmpproduct, tmpproduct.OrgId.Value);
            await FieldUtility.GenerateExtVals(_provider, tmpproduct);
            return BusResponse<MZ_Product>.Success(tmpproduct);
        }

        public virtual async Task<string> GenerateNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("PD");
        }
    }
}
