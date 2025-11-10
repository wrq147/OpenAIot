
using TemplateAction.NetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TemplateAction.Core;
using Common;
using ProducerService.Business;
using ProducerService.DAL;
using AuthService.Fields;
using Common.EventBus;
using System;
using System.Collections.Generic;
using Common.IdGenerator;
using ProducerService.Model;
using Microsoft.Extensions.Options;

namespace ProducerService
{
    /// <summary>
    /// 生产商资料模块
    /// </summary>
    public class PluginConfig : TANetCorePluginConfig
    {
        private ILogger<PluginConfig> _log;
        public override string[] DependOn => new string[] { "AuthService", "DeveloperService", "DictService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<AgentBLL>();
            services.AddBLL<FactoryBLL>();
            services.AddBLL<GradeBLL>();
            services.AddBLL<ProductBLL>();
            services.AddBLL<ProductTypeBLL>();
            services.AddBLL<ProductBatchBLL>();
            services.AddBLL<UnitBLL>();
            services.AddBLL<SupplierBLL>();


            services.AddDAL<AgentDAL>();
            services.AddDAL<AgentFlowDAL>();
            services.AddDAL<FactoryDAL>();
            services.AddDAL<GradeDAL>();
            services.AddDAL<ProductBatchDAL>();
            services.AddDAL<ProductDAL>();
            services.AddDAL<ProductTypeDAL>();
            services.AddDAL<UnitDAL>();
            services.AddDAL<SupplierDAL>();
        }
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            var redis = app.ServiceProvider.GetService<GeneralRedisHelper>();
            List<FieldBase> fields = new List<FieldBase>();
            fields.Add(new TextField()
            {
                mapid = "SkuNumber",
                name = "产品编号",
                type = "文本"
            });
            fields.Add(new TextField()
            {
                mapid = "ProductLabel",
                name = "产品标签",
                type = "文本"
            });
            fields.Add(new TextField()
            {
                mapid = "TypeName",
                name = "产品分组",
                type = "文本"
            });
            fields.Add(new TextField()
            {
                mapid = "Prop",
                name = "产品属性",
                type = "文本"
            });
            fields.Add(new TextField()
            {
                mapid = "ProductFrom",
                name = "生产来源",
                type = "文本"
            });
            fields.Add(new NumberField()
            {
                mapid = "Total",
                name = "总计量",
                type = "数字"
            });
            fields.Add(new NumberField()
            {
                mapid = "Price",
                name = "成本单价",
                type = "数字"
            });
            fields.Add(new NumberField()
            {
                mapid = "SalesPrice",
                name = "销售单价",
                type = "数字"
            });
            redis.HashSet("FixedFields", "产品", fields);

            List<FieldBase> supplierfields = new List<FieldBase>();
            supplierfields.Add(new TextField()
            {
                mapid = "Number",
                name = "供应商编号",
                type = "文本"
            });
            supplierfields.Add(new TextField()
            {
                mapid = "SupplierName",
                name = "供应商名称",
                type = "文本"
            });
            supplierfields.Add(new TextField()
            {
                mapid = "FullName",
                name = "供应商全称",
                type = "文本"
            });
            supplierfields.Add(new TextField()
            {
                mapid = "ContactName",
                name = "联系人",
                type = "文本"
            });
            supplierfields.Add(new TextField()
            {
                mapid = "Tel",
                name = "联系电话",
                type = "文本"
            });
            supplierfields.Add(new NumberField()
            {
                mapid = "Lng",
                name = "经度",
                type = "数字"
            });
            supplierfields.Add(new NumberField()
            {
                mapid = "Lat",
                name = "纬度",
                type = "数字"
            });
            supplierfields.Add(new TextField()
            {
                mapid = "AddressCode",
                name = "省市区代码",
                type = "文本"
            });
            supplierfields.Add(new TextField()
            {
                mapid = "AddressName",
                name = "地址名称",
                type = "文本"
            });
            supplierfields.Add(new TextField()
            {
                mapid = "AddressDetail",
                name = "详细地址",
                type = "文本"
            });
            supplierfields.Add(new RadioField()
            {
                mapid = "StatusName",
                name = "状态",
                type = "单选框",
                optionals = new[]
                {
                    "停用",
                    "正常"
                },
                is_add = false,
                defval = "正常",
                show_way = "平铺"
            });
            redis.HashSet("FixedFields", "供应商", supplierfields);


            plg.RegisterBus("IOTDeviceDel", async (bs) =>
            {
                try
                {
                    //删除批次
                    string tId = bs.GetValue("DevId");
                    var productBatchDAL = app.ServiceProvider.GetService<ProductBatchDAL>();
                    await productBatchDAL.Delete(x => x.Id == tId);
                }
                catch { }

            });

            plg.RegisterBus("AddIOTDevice", async (bs) =>
            {
                //生成产品批次
                string tId = bs.GetValue("Id");
                long tOrgId = bs.GetLong("OrgId");
                string tName = bs.GetValue("Name");
                string tNumber = bs.GetValue("Number");
                string tLNumber = bs.GetValue("LNumber");
                string tPhotoUrl = bs.GetValue("PhotoUrl");
                string tProductId = bs.GetValue("ProductId");

                var productDAL = app.ServiceProvider.GetService<ProductDAL>();
                var productBatchDAL = app.ServiceProvider.GetService<ProductBatchDAL>();
                var tprolist = await productDAL.SelectList(x => x.OrgId == tOrgId && x.IOTProductId == tProductId);
                if (tprolist.Count == 1)
                {
                    //新增
                    MZ_ProductBatch probb = new MZ_ProductBatch();
                    probb.Id = tId;
                    probb.Number = tNumber;
                    probb.LNumber = tLNumber;
                    probb.BatchName = tName;
                    probb.OrgId = tOrgId;
                    probb.PhotoUrl = tPhotoUrl;
                    probb.ProductId = tprolist[0].Id;
                    await productBatchDAL.Insert(probb);
                }
                else
                {
                    MZ_ProductBatch probb = new MZ_ProductBatch();
                    probb.Id = tId;
                    probb.Number = tNumber;
                    probb.LNumber = tLNumber;
                    probb.BatchName = tName;
                    probb.OrgId = tOrgId;
                    probb.PhotoUrl = tPhotoUrl;
                    probb.ProductId = "1";
                    await productBatchDAL.Insert(probb);
                }
            });


            plg.RegisterBus("UpdateIOTDevice", async (bs) =>
            {
                try
                {
                    string tId = bs.GetValue("Id");
                    string tNumber = bs.GetValue("Number");
                    string tLNumber = bs.GetValue("LNumber");
                    string tName = bs.GetValue("Name");
                    string tPhotoUrl = bs.GetValue("PhotoUrl");
                    var productBatchDAL = app.ServiceProvider.GetService<ProductBatchDAL>();
                    var pb = await productBatchDAL.Select(tId);
                    if (pb != null)
                    {
                        MZ_ProductBatch probb = new MZ_ProductBatch();
                        probb.Id = tId;
                        probb.LNumber = tLNumber;
                        probb.BatchName = tName;
                        probb.PhotoUrl = tPhotoUrl;
                        probb.Number = tNumber;
                        await productBatchDAL.Update(probb);
                    }
                }
                catch { }

            });


        }
    }
}
