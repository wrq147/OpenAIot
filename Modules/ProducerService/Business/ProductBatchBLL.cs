using AuthService;
using AuthService.DAL;
using AuthService.Fields;
using Common;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using MailKit.Search;
using ProducerService.DAL;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;


namespace ProducerService.Business
{
    public class ProductBatchBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private ProductBatchDAL _productBatchDAL;
        public ProductBatchBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, ProductBatchDAL productBatchDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _productBatchDAL = productBatchDAL;
        }
        public virtual async Task<string> GenerateNumber(IUserInfo user)
        {
            var factoryInfo = await _provider.GetService<FactoryDAL>().Select(user.OrgId);
            if (factoryInfo == null)
            {
                GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
                return await tmpredis.GenerateNumber("PH");
            }
            else
            {
                GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
                return await tmpredis.GenerateNumber(string.IsNullOrEmpty(factoryInfo.PHNumPrefix) ? "PH" : factoryInfo.PHNumPrefix);
            }
        }
        public virtual async Task<PageObject<V_ProductBatch>> SelectByPage(In_ProductBatchList query, IUserInfo user)
        {
            var listpage = await _productBatchDAL.SelectByPage(query, user.OrgId);
            var supplierIds = listpage.List.Select(x => x.Supplier).ToList();
            if (supplierIds.Count > 0)
            {
                var supplierDAL = _provider.GetService<SupplierDAL>();
                var suplierlist = await supplierDAL.SelectList(x => supplierIds.Contains(x.Id));
                foreach (var item in listpage.List)
                {
                    var supitem = suplierlist.Where(x => x.Id == item.Supplier).FirstOrDefault();
                    if (supitem != null)
                    {
                        item.SupplierName = supitem.SupplierName;
                    }

                }
            }

            return listpage;
        }
        public virtual async Task SyncFromDevice(IUserInfo user)
        {
            var tmplist = await _productBatchDAL.NoExistInIOT(user.OrgId);
            List<MZ_ProductBatch> batchList = new List<MZ_ProductBatch>();
            foreach (var item in tmplist)
            {
                MZ_ProductBatch batch = new MZ_ProductBatch();
                batch.OrgId = item.OrgId;
                batch.BatchName = item.Name;
                batch.PhotoUrl = item.PhotoUrl;
                batch.Number = item.DeviceNumber;
                batch.Id = item.Id;
                batch.ProductId = "1";
                batchList.Add(batch);
            }
            if (batchList.Count > 0)
            {
                await _productBatchDAL.Insert(batchList);
            }
        }
        public virtual async Task<PageObject<T_IotDevice>> NoExistDevPage(In_NoExistDevParam query, IUserInfo user)
        {
            return await _productBatchDAL.NoExistDevPage(query, user.OrgId);
        }
        public virtual async Task<MZ_ProductBatch> SelectByNumber(long orgId, string number)
        {
            var tmplist = await _productBatchDAL.SelectList(x => x.OrgId == orgId && x.Number == number);
            if (tmplist.Count > 0)
            {
                return tmplist[0];
            }
            else
            {
                return null;
            }
        }
        public virtual async Task<V_ProductBatch> SelectVByNumber(long orgId, string number)
        {
            return await _productBatchDAL.SelectProductVByNumber(orgId, number);
        }

        public virtual async Task<BusResponse<int>> InsertList(In_AddBatchList data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(133, "请切换到企业账号");
            }
            var devlist = await _productBatchDAL.SelectIOTList(data.Ids);
            List<MZ_ProductBatch> batchList = new List<MZ_ProductBatch>();
            foreach (var item in devlist)
            {
                MZ_ProductBatch batch = new MZ_ProductBatch();
                batch.OrgId = user.OrgId;
                batch.BatchName = item.Name;
                batch.PhotoUrl = item.PhotoUrl;
                batch.Number = item.DeviceNumber;
                batch.Id = item.Id;
                batch.ProductId = data.ProductId;
                batchList.Add(batch);
            }
            await _productBatchDAL.Insert(batchList);
            return BusResponse<int>.Success();
        }
        public virtual async Task<BusResponse<int>> Insert(In_AddProductBatch addData, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(133, "请切换到企业账号");
            }
            MZ_ProductBatch data = new MZ_ProductBatch();
            data.OrgId = user.OrgId;
            var pro = await _provider.GetService<ProductDAL>().Select(addData.ProductId);
            if (pro == null)
            {
                return BusResponse<int>.Error(134, "产品不存在");
            }
            if (string.IsNullOrEmpty(addData.Number))
            {
                return BusResponse<int>.Error(135, "批次编号不能为空");
            }
            data.BatchName = pro.ProductName;
            data.PhotoUrl = pro.PhotoUrl;
            data.ProductId = addData.ProductId;
            data.Number = addData.Number;
            if (!string.IsNullOrEmpty(pro.IOTProductId))
            {
                var iotdev = await _productBatchDAL.SelectIOTNumber(data.OrgId.Value, data.Number);
                if (iotdev != null)
                {
                    data.Id = iotdev.Id;
                    data.BatchName = iotdev.Name;
                    data.PhotoUrl = iotdev.PhotoUrl;
                }
                else
                {

                    var tmprsp = await BusUtility.Call("FromMesBatch", new
                    {
                        UserId = 2,
                        OrgId = pro.OrgId,
                        PhotoUrl = pro.PhotoUrl,
                        DeviceNumber = data.Number,
                        ProductId = pro.IOTProductId,
                        MesProductId = pro.Id,
                        DeviceId = addData.DtuId,
                        Name = data.BatchName
                    });
                    if (tmprsp.IsSuccess())
                    {
                        data.Id = tmprsp.Result;
                    }
                    else
                    {
                        return BusResponse<int>.Error(tmprsp.Code, tmprsp.Message);
                    }
                }
            }
            else
            {
                data.Id = _snowflake.NextId().ToString();
            }

            return BusResponse<int>.Success(await _productBatchDAL.Insert(data));
        }
        public virtual async Task<BusResponse<int>> Edit(In_EditProductBatch data, IUserInfo user)
        {
            var old = await _productBatchDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(111, "批次不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(112, "产品所属组织错误");
            }
            if (string.IsNullOrEmpty(data.ProductId))
            {
                data.ProductId = null;
            }
            else
            {
                var pro = await _provider.GetService<ProductDAL>().Select(data.ProductId);
                if (!string.IsNullOrEmpty(pro.IOTProductId))
                {
                    var tmprsp = await BusUtility.Call("FromMesBatch", new
                    {
                        UserId = 2,
                        OrgId = pro.OrgId,
                        DeviceNumber = old.Number,
                        ProductId = pro.IOTProductId,
                        MesProductId = pro.Id,
                        DeviceId = data.DtuId
                    });

                    if (tmprsp.IsSuccess())
                    {
                        data.Id = tmprsp.Result;
                    }
                    else
                    {
                        return BusResponse<int>.Error(tmprsp.Code, tmprsp.Message);
                    }
                }
            }

            MZ_ProductBatch newdata = new MZ_ProductBatch();
            newdata.Id = data.Id;
            newdata.ProductId = data.ProductId;
            return BusResponse<int>.Success(await _productBatchDAL.Update(newdata));
        }
        public virtual async Task<BusResponse<int>> Remove(string id, IUserInfo user)
        {
            return BusResponse<int>.Success(await _productBatchDAL.Delete(id));
        }
        public virtual async Task<BusResponse<int>> RemoveList(string[] ids, IUserInfo user)
        {
            try
            {
                var num = await _productBatchDAL.Delete(x => x.OrgId == user.OrgId && ids.Contains(x.Id));
                return BusResponse<int>.Success(num);
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }

        }
    }
}
