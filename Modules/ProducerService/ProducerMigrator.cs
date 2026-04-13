using Common;
using FluentMigrator;
using System;

namespace ProducerService
{
    [Migration(20250522003)]
    public class ProducerMigrator : Migration
    {
        public override void Up()
        {
            Create.Table("mz_factory").WithDescription("生产商表")
        .WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("生产商Id")
        .WithColumn("GradeWay").AsString(10).WithColumnDescription("代理方式：auto表示逐级自动分配代理级别，manual表示符合代理条件后手动升级")
        .WithColumn("CertTemplateId").AsString(128).WithColumnDescription("授权证书打印模板Id")
        .WithColumn("PHNumPrefix").AsString(5).WithColumnDescription("自定义批次前缀")
         .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
         .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
         .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
         .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Create.Table("mz_grade").WithDescription("代理级别")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("GradeName").AsString(50).WithColumnDescription("代理级别")
.WithColumn("Sort").AsInt32().WithColumnDescription("排序")
.WithColumn("IsSystem").AsInt32().WithColumnDescription("0为普通，1为系统")
.WithColumn("FactoryId").AsInt64().WithColumnDescription("生产商Id");

            Create.Table("mz_agent").WithDescription("企业代理信息表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("ParentOrgId").AsInt64().WithColumnDescription("上级企业Id")
.WithColumn("OrgId").AsInt64().WithColumnDescription("企业Id")
.WithColumn("GradeId").AsString(128).WithColumnDescription("代理级别")
.WithColumn("LevelPath").AsString(500).WithColumnDescription("代理层级信息，每一层,号分隔")
.WithColumn("FactoryId").AsInt64().WithColumnDescription("生产商Id")
.WithColumn("Regions").AsString(100).WithColumnDescription("代理区域代码（多个,号分隔）")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Create.Index("IDXAgentFactoryOrgID").OnTable("mz_agent").WithOptions().Unique().OnColumn("FactoryId").Ascending().OnColumn("OrgId").Ascending();
            if (Constants.General.sqltype != "Sqlite")
            {
                Execute.Sql("ALTER TABLE mz_agent ADD FULLTEXT INDEX AgentRegions (Regions);");
            }

            Create.Table("mz_agent_flow").WithDescription("邀请、申请、升级流程")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("ParentOrgId").AsInt64().WithColumnDescription("上级代理Id")
.WithColumn("BelowOrgId").AsInt64().WithColumnDescription("下级企业Id")
.WithColumn("GradeId").AsString(128).WithColumnDescription("代理级别，当为邀请时，空则为直销")
.WithColumn("FactoryId").AsInt64().WithColumnDescription("生产商Id")
.WithColumn("Regions").AsString(100).WithColumnDescription("代理区域代码（多个,号分隔）")
.WithColumn("FromType").AsFixedLengthAnsiString(1).WithColumnDescription("I为邀请，A为申请，U为升级")
.WithColumn("BindCustomerId").AsString(128).WithColumnDescription("绑定的客户Id")
.WithColumn("PhoneCode").AsString(128).Nullable().WithColumnDescription("短信验证码")
.WithColumn("Status").AsInt32().WithColumnDescription("状态：0为处理中，1为同意，2为拒绝")
.WithColumn("OverTime").AsDateTime().WithColumnDescription("过期时间")
            .WithColumn("OrgName").AsString(50).Nullable().WithColumnDescription("组织名称")
            .WithColumn("Logo").AsString(255).Nullable().WithColumnDescription("组织Logo")
            .WithColumn("Industry").AsInt32().Nullable().WithColumnDescription("行业类型")
            .WithColumn("Size").AsInt32().Nullable().WithColumnDescription("员工规模")
            .WithColumn("ContactName").AsString(50).Nullable().WithColumnDescription("联系人")
            .WithColumn("Tel").AsString(255).Nullable().WithColumnDescription("联系电话")
            .WithColumn("Lng").AsDouble().Nullable().WithColumnDescription("经度")
            .WithColumn("Lat").AsDouble().Nullable().WithColumnDescription("纬度")
            .WithColumn("AddressCode").AsString(6).Nullable().WithColumnDescription("省市区代码")
            .WithColumn("AddressName").AsString(255).Nullable().WithColumnDescription("地址名称")
            .WithColumn("AddressDetail").AsString(255).Nullable().WithColumnDescription("详细地址")
      .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
      .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
      .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
      .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            //创建代理视图
            Execute.Sql("CREATE VIEW mz_agent_v as select a.*,p.OrgName as ParentOrgName,o.OrgName,o.Logo,g.GradeName from mz_agent a left join mz_org p on a.ParentOrgId=p.Id left join mz_org o on a.OrgId=o.Id left join mz_grade g on a.GradeId=g.Id");


            Create.Table("mz_product_type").WithDescription("产品分组表")
            .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
.WithColumn("Name").AsString(50).WithColumnDescription("分组名称")
.WithColumn("PhotoUrl").AsString(255).WithColumnDescription("图片地址")
.WithColumn("PropList").AsString(500).WithColumnDescription("属性列表，多个用','分隔")
.WithColumn("Sort").AsInt32().WithColumnDescription("排序值：越小越前面")
.WithColumn("ConditionJson").AsString(10000).WithColumnDescription("过滤条件的json")
.WithColumn("ListFieldsJson").AsString(10000).WithColumnDescription("列表字段的json");

            Create.Table("mz_product").WithDescription("产品表（半成品、成品）")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                .WithColumn("SkuNumber").AsString(50).Indexed().WithColumnDescription("唯一编号(sku编码)")
                .WithColumn("IOTProductId").AsString(128).Nullable().Indexed().WithColumnDescription("关联的物联协议Id")
                .WithColumn("ProductLabel").AsFixedLengthAnsiString(1).Indexed().WithColumnDescription("产品标签：M原材料，U半成品，F成品")
                .WithColumn("TypeId").AsString(128).Indexed().WithColumnDescription("产品分组Id，为空所属全部")
                .WithColumn("Prop").AsString(255).WithColumnDescription("产品分组对应的产品属性，可为空")
                .WithColumn("ProductFrom").AsString(10).WithColumnDescription("生产来源：自制，外购，委外")
                .WithColumn("ProductName").AsString(80).WithColumnDescription("产品名称")
                .WithColumn("PhotoUrl").AsString(255).WithColumnDescription("图片地址")
                .WithColumn("Unit").AsString(10).WithColumnDescription("包装单位")
                .WithColumn("Specs").AsString(255).WithColumnDescription("产品规格")
                .WithColumn("Total").AsDecimal(10, 4).WithColumnDescription("总计量")
                .WithColumn("MinUnit").AsString(10).WithDefaultValue("").WithColumnDescription("最小计量单位")
                .WithColumn("Price").AsDecimal(10, 2).WithColumnDescription("成本单价")
                .WithColumn("SalesPrice").AsDecimal(10, 2).WithColumnDescription("销售单价")
                .WithColumn("Route").AsString(128).Indexed().WithColumnDescription("工艺路线")
                .WithColumn("Supplier").AsString(128).Indexed().WithColumnDescription("供应商")
                .WithColumn("Remark").AsString(255).WithColumnDescription("备注说明")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Create.Index().OnTable("mz_product").WithOptions().Unique().OnColumn("OrgId").Ascending().OnColumn("SkuNumber").Ascending();

            Create.Table("mz_product_batch").WithDescription("产品批次表（代表半成品批次、成品设备）")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("批次id或设备id")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                .WithColumn("BatchName").AsString(80).WithColumnDescription("批次名称")
                .WithColumn("PhotoUrl").AsString(255).WithColumnDescription("图片地址")
                .WithColumn("Number").AsString(50).Indexed().WithColumnDescription("唯一编号")
                .WithColumn("ProductId").AsString(128).Indexed().WithColumnDescription("所属产品Id");


            Execute.Sql("CREATE VIEW mz_product_batch_v as select a.*,c.Name as TypeName,b.SkuNumber,b.IOTProductId,b.ProductLabel,b.TypeId,b.Prop,b.ProductName,b.Unit,b.Specs,b.Total,b.Price,b.SalesPrice,b.Route,b.Supplier,b.Remark from mz_product_batch a left join mz_product b on a.ProductId=b.Id left join mz_product_type c on b.TypeId=c.Id");

            //单位表
            Create.Table("mz_unit").WithDescription("单位表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id编号")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
.WithColumn("UnitName").AsString(10).WithColumnDescription("单位名称")
.WithColumn("Remark").AsString(500).WithColumnDescription("备注");


            //供应商
            Create.Table("mz_supplier").WithDescription("供应商")
            .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
            .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
            .WithColumn("Number").AsString(50).Indexed().WithColumnDescription("唯一编号")
            .WithColumn("SupplierName").AsString(50).WithColumnDescription("供应商名称")
            .WithColumn("FullName").AsString(128).WithColumnDescription("供应商全称")
            .WithColumn("PayTerm").AsInt32().WithColumnDescription("付款期限")
            .WithColumn("ContactName").AsString(50).WithColumnDescription("联系人")
            .WithColumn("Tel").AsString(255).WithColumnDescription("联系电话")
            .WithColumn("Lng").AsDouble().WithColumnDescription("经度")
            .WithColumn("Lat").AsDouble().WithColumnDescription("纬度")
            .WithColumn("Geo").AsString(30).WithColumnDescription("经纬度的geo编码")
            .WithColumn("AddressCode").AsString(6).WithColumnDescription("省市区代码")
            .WithColumn("AddressName").AsString(255).WithColumnDescription("地址名称")
            .WithColumn("AddressDetail").AsString(255).WithColumnDescription("详细地址")
            .WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态：0为停用，1为正常")
            .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
            .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
            .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
            .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Create.Index().OnTable("mz_supplier").WithOptions().Unique().OnColumn("OrgId").Ascending().OnColumn("Number").Ascending();



            //系统管理生产商
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 117,
                menu_name = "生产商管理",
                parent_id = 1,
                order_num = 9,
                path = "factory/list",
                component = "manufac/factory/list",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ProducerService/Factory/List",
                icon = "a-qiyeguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 301,
                menu_name = "添加生产商",
                parent_id = 117,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ProducerService/Factory/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 302,
                menu_name = "编辑生产商",
                parent_id = 117,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ProducerService/Factory/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 303,
                menu_name = "移除生产商",
                parent_id = 117,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ProducerService/Factory/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Insert.IntoTable("mz_role_permission").Row(new
            {
                RoleID = 1,
                MenuId = 117
            }).Row(new
            {
                RoleID = 1,
                MenuId = 301
            }).Row(new
            {
                RoleID = 1,
                MenuId = 302
            }).Row(new
            {
                RoleID = 1,
                MenuId = 303
            });


            //生产商权限
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 5,
                menu_name = "生产商资料",
                parent_id = 0,
                order_num = 8,
                path = "factory",
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "M",
                visible = "0",
                status = "0",
                perms = "/AgentMan/",
                icon = "wodegongchang",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 5003,
                menu_name = "代理商",
                parent_id = 5,
                order_num = 3,
                path = "factory/allagent",
                component = "manufac/factory/allagent",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ProducerService/Agent/AllList",
                icon = "dailishang",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 5110,
                menu_name = "供应商",
                parent_id = 5,
                order_num = 4,
                path = "factory/supplierlist",
                component = "manufac/factory/supplierlist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ProducerService/Supplier/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 5103,
                menu_name = "邀请客户",
                parent_id = 5003,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ProducerService/Agent/AddAllInvite",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 5440,
                menu_name = "产品管理",
                parent_id = 5,
                order_num = 5,
                path = "factory/productlist",
                component = "manufac/factory/productlist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ProducerService/Product/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 5450,
                menu_name = "产品批次",
                parent_id = 5,
                order_num = 6,
                path = "factory/batchlist",
                component = "manufac/factory/batchlist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ProducerService/ProductBatch/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 5600,
                menu_name = "生产商设置",
                parent_id = 5,
                order_num = 7,
                path = "factory/gradelist",
                component = "manufac/factory/gradelist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ProducerService/Grade/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 5700,
                menu_name = "单位管理",
                parent_id = 5,
                order_num = 8,
                path = "factory/unitlist",
                component = "manufac/factory/unitlist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ProducerService/Unit/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 210,
                menu_name = "我的授权",
                parent_id = 107,
                order_num = 5,
                path = "org/myauth",
                component = "manufac/org/myauth",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ProducerService/Agent/AuthList",
                icon = "wodeshouquan",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });



            Insert.IntoTable("mz_role").Row(new
            {
                RoleID = 3,
                RoleName = "生产商",
                RoleSort = 0,
                RoleDesc = "生产商的角色",
                IsSystem = "1",
                Status = "0",
                OrgId = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            //代理商角色
            Insert.IntoTable("mz_role").Row(new
            {
                RoleID = 4,
                RoleName = "代理商",
                RoleSort = 0,
                RoleDesc = "代理商的角色",
                IsSystem = "1",
                Status = "0",
                OrgId = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });


            //添加生产商权限
            Insert.IntoTable("mz_role_permission").Row(new
            {
                RoleID = 3,
                MenuId = 5
            }).Row(new
            {
                RoleID = 3,
                MenuId = 5201
            }).Row(new
            {
                RoleID = 3,
                MenuId = 5002
            }).Row(new
            {
                RoleID = 3,
                MenuId = 5003
            }).Row(new
            {
                RoleID = 3,
                MenuId = 5103
            });





            Insert.IntoTable("mz_role_permission").Row(new
            {
                RoleID = 3,
                MenuId = 4
            }).Row(new
            {
                RoleID = 3,
                MenuId = 481
            }).Row(new
            {
                RoleID = 3,
                MenuId = 482
            }).Row(new
            {
                RoleID = 3,
                MenuId = 483
            }).Row(new
            {
                RoleID = 3,
                MenuId = 484
            });

            this.Execute.Sql("delete FROM mz_product where Id=1");
            Insert.IntoTable("mz_product").Row(new
            {
                Id = 1,
                OrgId = 0,
                SkuNumber = string.Empty,
                IOTProductId = string.Empty,
                ProductLabel = "F",
                TypeId = string.Empty,
                Prop = string.Empty,
                ProductFrom = "自制",
                ProductName = "物联设备",
                PhotoUrl = string.Empty,
                Unit = string.Empty,
                Specs = string.Empty,
                Total = 1,
                Price = 0,
                SalesPrice = 0,
                Route = string.Empty,
                Supplier = string.Empty,
                Remark = string.Empty,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

        }

        public override void Down()
        {
        }
    }
}
