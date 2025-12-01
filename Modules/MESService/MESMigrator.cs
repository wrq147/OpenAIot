using Common;
using FluentMigrator;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService
{
    [Migration(20251112002)]
    public class MESMigrator : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_menu where menu_id=5720");
            this.Execute.Sql("delete FROM mz_menu where menu_id=5730");
            this.Execute.Sql("delete FROM mz_menu where menu_id=5740");
            this.Execute.Sql("delete FROM mz_menu where menu_id=5750");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 5720,
                menu_name = "物料清单",
                parent_id = 5,
                order_num = 9,
                path = "factory/bomlist",
                component = "mes/factory/bomlist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MESService/Bom/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 5730,
                menu_name = "不良品项",
                parent_id = 5,
                order_num = 10,
                path = "factory/defect",
                component = "mes/factory/defect",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MESService/Defect/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 5740,
                menu_name = "生产工序",
                parent_id = 5,
                order_num = 11,
                path = "factory/operlist",
                component = "mes/factory/operlist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MESService/Oper/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 5750,
                menu_name = "工艺路线",
                parent_id = 5,
                order_num = 12,
                path = "factory/routelist",
                component = "mes/factory/routelist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MESService/Route/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Execute.Sql("DROP TABLE IF EXISTS mz_bom_header");
            Create.Table("mz_bom_header").WithDescription("物料清单头表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("BOM 头唯一标识（主键）")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
.WithColumn("ProductId").AsString(128).Indexed().WithColumnDescription("所属产品ID")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");



            Execute.Sql("DROP TABLE IF EXISTS mz_bom_line");
            Create.Table("mz_bom_line").WithDescription("物料清单明细表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("明细Id")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                .WithColumn("HeaderId").AsString(128).Indexed().WithColumnDescription("所属BOM头Id")
                .WithColumn("ParentProductId").AsString(128).Indexed().WithColumnDescription("父项产品Id")
                .WithColumn("ProductId").AsString(128).Indexed().WithColumnDescription("子项产品Id")
                .WithColumn("Sort").AsInt32().WithColumnDescription("排序值：越小越前面")
                .WithColumn("Quantity").AsDecimal(10, 4).WithColumnDescription("用量")
                .WithColumn("ProcessStepId").AsString(128).Indexed().WithColumnDescription("关联工序ID")
                .WithColumn("Remark").AsString(500).WithColumnDescription("备注");


            Execute.Sql("DROP TABLE IF EXISTS mz_defect_type");
            Create.Table("mz_defect_type").WithDescription("不良品项表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id编号")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
.WithColumn("DefectName").AsString(50).WithColumnDescription("不良品项名称")
.WithColumn("DefectCategory").AsString(50).WithColumnDescription("不良类别(外观/功能/性能/其它等)");


            Execute.Sql("DROP TABLE IF EXISTS mz_product_route");
            Create.Table("mz_product_route").WithDescription("产品工艺路线表")
     .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id编号")
     .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
     .WithColumn("RouteName").AsString(50).WithColumnDescription("工艺路线名称")
     .WithColumn("ToHouseId").AsString(128).Nullable().WithColumnDescription("产品入库的目标仓库，没有则不自动入库")
     .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
     .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
     .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
     .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Execute.Sql("DROP TABLE IF EXISTS mz_product_route_oper");
            Create.Table("mz_product_route_oper").WithDescription("产品工艺路线明细表")
             .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id编号")
             .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
             .WithColumn("RouteId").AsString(128).Indexed().WithColumnDescription("工艺路线Id")
             .WithColumn("OperId").AsString(128).Indexed().WithColumnDescription("工序Id")
                        .WithColumn("PropOf").AsDecimal(10, 2).WithColumnDescription("报工数配比")
                        .WithColumn("WorkTime").AsDecimal(10, 2).WithColumnDescription("工时(分钟)")
                        .WithColumn("Sequence").AsInt32().WithColumnDescription("工序顺序序号")
                        .WithColumn("StrExt1").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段1")
                        .WithColumn("StrExt2").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段2")
                        .WithColumn("StrExt3").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段3")
                        .WithColumn("StrExt4").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段4")
                        .WithColumn("StrExt5").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段5")
                        .WithColumn("StrExt6").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段6")
                        .WithColumn("StrExt7").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段7")
                        .WithColumn("StrExt8").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段8")
                        .WithColumn("StrExt9").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段9")
                        .WithColumn("StrExt10").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段10")
                        .WithColumn("StrExt11").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段11")
                        .WithColumn("StrExt12").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段12")
                        .WithColumn("StrExt13").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段13")
                        .WithColumn("StrExt14").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段14")
                        .WithColumn("StrExt15").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段15")
                        .WithColumn("StrExt16").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段16")
                        .WithColumn("StrExt17").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段17")
                        .WithColumn("StrExt18").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段18")
                        .WithColumn("StrExt19").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段19")
                        .WithColumn("StrExt20").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段20")
                        .WithColumn("StrExt21").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段21")
                        .WithColumn("StrExt22").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段22")
                        .WithColumn("StrExt23").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段23")
                        .WithColumn("StrExt24").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段24")
                        .WithColumn("StrExt25").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段25")
                        .WithColumn("StrExt26").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段26")
                        .WithColumn("StrExt27").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段27")
                        .WithColumn("StrExt28").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段28")
                        .WithColumn("StrExt29").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段29")
                        .WithColumn("StrExt30").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段30")
                        .WithColumn("NumExt1").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段1")
                        .WithColumn("NumExt2").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段2")
                        .WithColumn("NumExt3").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段3")
                        .WithColumn("NumExt4").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段4")
                        .WithColumn("NumExt5").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段5")
                        .WithColumn("NumExt6").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段6")
                        .WithColumn("NumExt7").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段7")
                        .WithColumn("NumExt8").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段8")
                        .WithColumn("NumExt9").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段9")
                        .WithColumn("NumExt10").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段10");

            Execute.Sql("DROP TABLE IF EXISTS mz_product_oper");
            Create.Table("mz_product_oper").WithDescription("产品工序表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id编号")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                .WithColumn("DeviceIds").AsString(500).WithColumnDescription("工序使用的设备，多台设备用逗号分隔")
                .WithColumn("OperName").AsString(50).WithColumnDescription("工序名称")
                .WithColumn("AssignedUser").AsString(20000).WithColumnDescription("允许提交的人员")
                .WithColumn("PropOf").AsDecimal(10, 2).WithColumnDescription("报工数配比:‌生产计划数 × 报工数配比 = 工序计划数‌")
                .WithColumn("WorkTime").AsDecimal(10, 2).WithColumnDescription("预计工时(分钟)")
                .WithColumn("PriceMethod").AsString(6).WithColumnDescription("计件、计时")
                .WithColumn("UnitPrice").AsDecimal(10, 2).WithColumnDescription("工资单价")
                .WithColumn("DefectJson").AsString(20000).WithColumnDescription("不良品项列表")
                .WithColumn("ReportFields").AsString(20000).WithColumnDescription("工序的报工表单权限")
                .WithColumn("FieldsInit").AsString(20000).WithColumnDescription("报工表单初始化配置")
                           .WithColumn("StrExt1").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段1")
                           .WithColumn("StrExt2").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段2")
                           .WithColumn("StrExt3").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段3")
                           .WithColumn("StrExt4").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段4")
                           .WithColumn("StrExt5").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段5")
                           .WithColumn("StrExt6").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段6")
                           .WithColumn("StrExt7").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段7")
                           .WithColumn("StrExt8").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段8")
                           .WithColumn("StrExt9").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段9")
                           .WithColumn("StrExt10").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段10")
                           .WithColumn("StrExt11").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段11")
                           .WithColumn("StrExt12").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段12")
                           .WithColumn("StrExt13").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段13")
                           .WithColumn("StrExt14").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段14")
                           .WithColumn("StrExt15").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段15")
                           .WithColumn("StrExt16").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段16")
                           .WithColumn("StrExt17").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段17")
                           .WithColumn("StrExt18").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段18")
                           .WithColumn("StrExt19").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段19")
                           .WithColumn("StrExt20").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段20")
                           .WithColumn("StrExt21").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段21")
                           .WithColumn("StrExt22").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段22")
                           .WithColumn("StrExt23").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段23")
                           .WithColumn("StrExt24").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段24")
                           .WithColumn("StrExt25").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段25")
                           .WithColumn("StrExt26").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段26")
                           .WithColumn("StrExt27").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段27")
                           .WithColumn("StrExt28").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段28")
                           .WithColumn("StrExt29").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段29")
                           .WithColumn("StrExt30").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段30")
                           .WithColumn("NumExt1").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段1")
                           .WithColumn("NumExt2").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段2")
                           .WithColumn("NumExt3").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段3")
                           .WithColumn("NumExt4").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段4")
                           .WithColumn("NumExt5").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段5")
                           .WithColumn("NumExt6").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段6")
                           .WithColumn("NumExt7").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段7")
                           .WithColumn("NumExt8").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段8")
                           .WithColumn("NumExt9").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段9")
                           .WithColumn("NumExt10").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段10")
     .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
     .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
     .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
     .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            if (Constants.General.sqltype != "Sqlite")
            {
                Execute.Sql("ALTER TABLE mz_product_oper ADD FULLTEXT INDEX OperDeviceIds (DeviceIds);");
            }


            this.Execute.Sql("delete FROM mz_menu where menu_id=10");
            this.Execute.Sql("delete FROM mz_menu where menu_id=10100");
            this.Execute.Sql("delete FROM mz_menu where menu_id=10200");
            this.Execute.Sql("delete FROM mz_menu where menu_id=10300");
            this.Execute.Sql("delete FROM mz_menu where menu_id=10400");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 10,
                menu_name = "生产管理",
                parent_id = 0,
                order_num = 10,
                path = "mes",
                component = "",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "M",
                visible = "0",
                status = "0",
                perms = "/MES/",
                icon = "jinxiaocun",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 10100,
                menu_name = "生产计划",
                parent_id = 10,
                order_num = 1,
                path = "prod/planlist",
                component = "mes/prod/planlist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MESService/Plan/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 10200,
                menu_name = "生产工单",
                parent_id = 10,
                order_num = 2,
                path = "prod/orderlist",
                component = "mes/prod/orderlist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MESService/Order/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 10300,
                menu_name = "生产任务",
                parent_id = 10,
                order_num = 3,
                path = "prod/tasklist",
                component = "mes/prod/tasklist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MESService/Task/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 10400,
                menu_name = "我的报工",
                parent_id = 10,
                order_num = 4,
                path = "prod/reportlist",
                component = "mes/prod/reportlist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MESService/Report/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });


            Execute.Sql("DROP TABLE IF EXISTS mz_factory_mes");
            Create.Table("mz_factory_mes").WithDescription("生产商配置mes信息")
                .WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("生产商Id")
                .WithColumn("PlanTemplateId").AsInt64().WithDefaultValue(0).WithColumnDescription("生产计划审核模板")
                .WithColumn("ReportTemplateId").AsInt64().WithDefaultValue(0).WithColumnDescription("报工审核模板")
                .WithColumn("PlanFlowInitJson").AsString(5000).WithDefaultValue("").WithColumnDescription("生产计划流程初始化json")
                .WithColumn("ReportFlowInitJson").AsString(5000).WithDefaultValue("").WithColumnDescription("报工流程初始化json");

            Execute.Sql("DROP TABLE IF EXISTS mz_product_plan");
            Create.Table("mz_product_plan").WithDescription("生产计划表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id编号")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                .WithColumn("Number").AsString(50).Unique().WithColumnDescription("唯一编号")
                .WithColumn("PlanName").AsString(50).WithColumnDescription("计划名称")
                .WithColumn("Status").AsInt32().Indexed().WithColumnDescription("状态：0、待提交；1、待审批；2、待执行；3、执行中；4、已完成；5、已取消；6、已驳回")
                .WithColumn("Priority").AsInt32().WithColumnDescription("优先级：1、优先安排；2、加急处理；3、正常排产")
                .WithColumn("FlowId").AsInt64().WithColumnDescription("关联的审核流程Id")
                .WithColumn("OverTime").AsDateTime().WithColumnDescription("超期时间")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Execute.Sql("DROP TABLE IF EXISTS mz_product_plan_item");
            Create.Table("mz_product_plan_item").WithDescription("生产计划的产品信息表")
                    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id编号")
                    .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                    .WithColumn("PlanId").AsString(128).Indexed().WithColumnDescription("生产计划Id")
                    .WithColumn("ProductId").AsString(128).Indexed().WithColumnDescription("所属产品ID")
                    .WithColumn("Quantity").AsDecimal(10, 2).WithColumnDescription("计划数量")
                    .WithColumn("PlannedStartOn").AsDateTime().WithColumnDescription("计划开始时间")
                    .WithColumn("PlannedEndOn").AsDateTime().WithColumnDescription("计划结束时间");



            Execute.Sql("DROP TABLE IF EXISTS mz_work_order");
            Create.Table("mz_work_order").WithDescription("生产工单表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id编号")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                .WithColumn("PlanId").AsString(128).Indexed().WithColumnDescription("生产计划Id")
                .WithColumn("WorkNumber").AsString(50).Unique().WithColumnDescription("唯一编号")
                .WithColumn("ParentWorkOrderId").AsString(128).Indexed().WithColumnDescription("父工单Id")
                .WithColumn("ParentPath").AsString(800).Indexed().WithColumnDescription("父层级")
                .WithColumn("Status").AsInt32().Indexed().WithColumnDescription("状态：0、待生产；1、生产中；2、已完成；3、已取消；")
                .WithColumn("Priority").AsInt32().WithColumnDescription("优先级：1、优先安排；2、加急处理；3、正常排产")
                .WithColumn("OverTime").AsDateTime().WithColumnDescription("超期时间")
                .WithColumn("Quantity").AsDecimal(10, 2).WithColumnDescription("计划产量")
                .WithColumn("ProductId").AsString(128).Indexed().WithColumnDescription("所属产品ID")
                .WithColumn("RouteId").AsString(128).Indexed().WithColumnDescription("工艺路线Id")
                .WithColumn("PlannedStartOn").AsDateTime().WithColumnDescription("计划开始时间")
                .WithColumn("PlannedEndOn").AsDateTime().WithColumnDescription("计划结束时间")
                .WithColumn("StartOn").AsDateTime().Nullable().WithColumnDescription("实际开始时间")
                .WithColumn("EndOn").AsDateTime().Nullable().WithColumnDescription("实际结束时间")
                .WithColumn("BatchCount").AsDecimal(10, 2).WithColumnDescription("实际产量")
                .WithColumn("CancelReason").AsString(500).WithColumnDescription("取消原因")
                .WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("UpdatedOn").AsDateTime().WithColumnDescription("更新时间");


            Execute.Sql("DROP TABLE IF EXISTS mz_work_bom");
            Create.Table("mz_work_bom").WithDescription("生产工单物料表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id编号")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                .WithColumn("ProductId").AsString(128).Indexed().WithColumnDescription("产品Id")
                .WithColumn("WorkOrderId").AsString(128).Indexed().WithColumnDescription("关联的工单Id")
                .WithColumn("OperId").AsString(128).WithColumnDescription("关联的工序Id")
                .WithColumn("NeedQuantity").AsDecimal(10, 4).WithColumnDescription("所需用量")
                .WithColumn("UsedQuantity").AsDecimal(10, 4).Nullable().WithColumnDescription("实际用量")
                .WithColumn("Quantity").AsDecimal(10, 4).WithColumnDescription("单个用量");

            Execute.Sql("DROP TABLE IF EXISTS mz_work_task");
            Create.Table("mz_work_task").WithDescription("生产任务表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id编号")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                .WithColumn("PlanId").AsString(128).Indexed().WithColumnDescription("生产计划Id")
                .WithColumn("WorkOrderId").AsString(128).Indexed().WithColumnDescription("关联的工单Id")
                .WithColumn("ProductId").AsString(128).Indexed().WithColumnDescription("所属产品ID")
                .WithColumn("OperId").AsString(128).WithColumnDescription("关联的工序Id")
                .WithColumn("RouteOperId").AsString(128).WithColumnDescription("关联的工艺路线明细Id")
                .WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("StartOn").AsDateTime().Nullable().WithColumnDescription("开始时间")
                .WithColumn("FinishOn").AsDateTime().Nullable().WithColumnDescription("完成时间")
                .WithColumn("IsFinish").AsBoolean().Indexed().WithColumnDescription("任务是否完成")
                .WithColumn("Priority").AsInt32().WithColumnDescription("优先级：1、优先安排；2、加急处理；3、正常排产")
                .WithColumn("OverTime").AsDateTime().WithColumnDescription("超期时间")
                .WithColumn("PropOf").AsDecimal(10, 2).WithColumnDescription("报工数配比")
                .WithColumn("WorkTime").AsDecimal(10, 2).WithColumnDescription("工时(分钟)")
                .WithColumn("WorkTimeTotal").AsDecimal(10, 2).WithColumnDescription("总工时(分钟)")
                .WithColumn("PlanNum").AsDecimal(10, 2).WithColumnDescription("计划数")
                .WithColumn("GoodNum").AsDecimal(10, 2).WithColumnDescription("良品数")
                .WithColumn("DefectNum").AsDecimal(10, 2).WithColumnDescription("不良品数")
                .WithColumn("Sequence").AsInt32().WithColumnDescription("工序顺序序号")
                .WithColumn("Remark").AsString(5000).WithColumnDescription("任务说明");


            Execute.Sql("DROP TABLE IF EXISTS mz_work_batch");
            Create.Table("mz_work_batch").WithDescription("生产批次表")
                .WithColumn("Id").AsString(50).PrimaryKey().WithColumnDescription("批次编号")
                .WithColumn("OrgId").AsInt64().PrimaryKey().WithColumnDescription("所属组织ID")
                .WithColumn("LNumber").AsString(50).Nullable().WithColumnDescription("通讯编号")
                .WithColumn("WorkOrderId").AsString(128).Indexed().WithColumnDescription("关联的工单Id")
                .WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("UpdatedOn").AsDateTime().WithColumnDescription("更新时间")
                              .WithColumn("StrExt1").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段1")
                              .WithColumn("StrExt2").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段2")
                              .WithColumn("StrExt3").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段3")
                              .WithColumn("StrExt4").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段4")
                              .WithColumn("StrExt5").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段5")
                              .WithColumn("StrExt6").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段6")
                              .WithColumn("StrExt7").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段7")
                              .WithColumn("StrExt8").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段8")
                              .WithColumn("StrExt9").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段9")
                              .WithColumn("StrExt10").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段10")
                              .WithColumn("StrExt11").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段11")
                              .WithColumn("StrExt12").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段12")
                              .WithColumn("StrExt13").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段13")
                              .WithColumn("StrExt14").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段14")
                              .WithColumn("StrExt15").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段15")
                              .WithColumn("StrExt16").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段16")
                              .WithColumn("StrExt17").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段17")
                              .WithColumn("StrExt18").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段18")
                              .WithColumn("StrExt19").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段19")
                              .WithColumn("StrExt20").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段20")
                              .WithColumn("StrExt21").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段21")
                              .WithColumn("StrExt22").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段22")
                              .WithColumn("StrExt23").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段23")
                              .WithColumn("StrExt24").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段24")
                              .WithColumn("StrExt25").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段25")
                              .WithColumn("StrExt26").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段26")
                              .WithColumn("StrExt27").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段27")
                              .WithColumn("StrExt28").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段28")
                              .WithColumn("StrExt29").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段29")
                              .WithColumn("StrExt30").AsString(500).Indexed().Nullable().WithColumnDescription("扩展字符串字段30")
                              .WithColumn("NumExt1").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段1")
                              .WithColumn("NumExt2").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段2")
                              .WithColumn("NumExt3").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段3")
                              .WithColumn("NumExt4").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段4")
                              .WithColumn("NumExt5").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段5")
                              .WithColumn("NumExt6").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段6")
                              .WithColumn("NumExt7").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段7")
                              .WithColumn("NumExt8").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段8")
                              .WithColumn("NumExt9").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段9")
                              .WithColumn("NumExt10").AsDouble().Indexed().Nullable().WithColumnDescription("扩展数字字段10");

            Execute.Sql("DROP TABLE IF EXISTS mz_batch_dev_his");
            Create.Table("mz_batch_dev_his").WithDescription("生产批次数据采集的历史数据表")
                .WithColumn("Id").AsInt64().Identity().PrimaryKey().WithColumnDescription("Id编号")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                .WithColumn("BatchNo").AsString(50).Indexed().WithColumnDescription("批次编号")
                .WithColumn("OperId").AsString(128).Indexed().WithColumnDescription("关联的工序Id")
                .WithColumn("PropName").AsString(50).WithColumnDescription("采集的属性名称")
                .WithColumn("PropCode").AsString(50).Indexed().WithColumnDescription("采集的属性标识符")
                .WithColumn("PropType").AsString(50).WithColumnDescription("采集的属性类型")
                .WithColumn("PropVal").AsDouble().Indexed().WithColumnDescription("采集的数值型属性值")
                .WithColumn("PropStrVal").AsString().Indexed().WithColumnDescription("采集的字符串属性值")
                .WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间");


            Execute.Sql("DROP TABLE IF EXISTS mz_work_report");
            Create.Table("mz_work_report").WithDescription("报工表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id编号")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                .WithColumn("WorkOrderId").AsString(128).Indexed().WithColumnDescription("关联的工单Id")
                .WithColumn("WorkTaskId").AsString(128).Indexed().WithColumnDescription("关联的任务Id")
                .WithColumn("OperId").AsString(128).Indexed().WithColumnDescription("关联的工序Id")
                .WithColumn("RouteOperId").AsString(128).WithColumnDescription("关联的工艺路线明细Id")
                .WithColumn("Number").AsString(50).Unique().WithColumnDescription("唯一编号")
                .WithColumn("BatchNo").AsString(50).Indexed().WithColumnDescription("批次编号")
                .WithColumn("GoodNum").AsDecimal(10, 2).WithColumnDescription("良品数")
                .WithColumn("DefectNum").AsDecimal(10, 2).WithColumnDescription("总不良品数")
                .WithColumn("StartWork").AsDateTime().Nullable().WithColumnDescription("开始时间")
                .WithColumn("EndWork").AsDateTime().Nullable().WithColumnDescription("结束时间")
                .WithColumn("WorkTime").AsDecimal(10, 2).WithColumnDescription("报工时长(分钟)")
                .WithColumn("OverReason").AsString(500).WithColumnDescription("超时原因，当报工时长超过标准时间时需要填写")
                .WithColumn("Status").AsInt32().Indexed().WithColumnDescription("状态：0、待提交；1、待审核；2、已审核；3、已取消；4、已驳回；")
                .WithColumn("FlowId").AsInt64().WithColumnDescription("关联的审核流程Id")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间")
                .WithColumn("submitTime").AsDateTime().Nullable().WithColumnDescription("提交时间");


            Execute.Sql("DROP TABLE IF EXISTS mz_work_defect");
            Create.Table("mz_work_defect").WithDescription("报工不良品表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id编号")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                .WithColumn("WorkOrderId").AsString(128).Indexed().WithColumnDescription("关联的工单Id")
                .WithColumn("WorkTaskId").AsString(128).Indexed().WithColumnDescription("关联的任务Id")
                .WithColumn("OperId").AsString(128).Indexed().WithColumnDescription("关联的工序Id")
                .WithColumn("ReportId").AsString(128).Indexed().WithColumnDescription("报工Id")
                .WithColumn("DefectId").AsString(128).WithColumnDescription("不良品编号")
                .WithColumn("DefectName").AsString(50).WithColumnDescription("不良品项名称")
                .WithColumn("DefectCategory").AsString(50).WithColumnDescription("不良类别(外观/功能/性能/其它等)")
                .WithColumn("DefectNum").AsDecimal(10, 2).WithColumnDescription("不良品数");

        }
        public override void Down()
        {
        }
    }
}
