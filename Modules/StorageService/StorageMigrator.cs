using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService
{
    [Migration(20250424004)]
    public class StorageMigrator : Migration
    {
        public override void Up()
        {
            if (Schema.Table("mz_store_house").Exists())
            {
                this.Execute.Sql("delete FROM mz_menu where menu_id=8");
                this.Execute.Sql("delete FROM mz_menu where menu_id=6120");
                this.Execute.Sql("delete FROM mz_menu where menu_id=6121");
                this.Execute.Sql("delete FROM mz_menu where menu_id=6122");
                this.Execute.Sql("delete FROM mz_menu where menu_id=6123");
                this.Execute.Sql("delete FROM mz_menu where menu_id=6130");
                this.Execute.Sql("delete FROM mz_menu where menu_id=6210");
                this.Execute.Sql("delete FROM mz_menu where menu_id=6220");
                this.Execute.Sql("delete FROM mz_menu where menu_id=6230");
                this.Execute.Sql("delete FROM mz_menu where menu_id=6240");
                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 8,
                    menu_name = "数智仓储",
                    parent_id = 0,
                    order_num = 8,
                    path = "jxc",
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "M",
                    visible = "0",
                    status = "0",
                    perms = "/Stock/",
                    icon = "jinxiaocun",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6120,
                    menu_name = "仓库管理",
                    parent_id = 8,
                    order_num = 1,
                    path = "house/list",
                    component = "storage/house/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/House/List",
                    icon = "cangkuguanli",
                    scope = 1,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6121,
                    menu_name = "添加仓库",
                    parent_id = 6120,
                    order_num = 1,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/House/Add",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6122,
                    menu_name = "修改仓库",
                    parent_id = 6120,
                    order_num = 2,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/House/Edit",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6123,
                    menu_name = "删除仓库",
                    parent_id = 6120,
                    order_num = 3,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/House/Remove",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6130,
                    menu_name = "库存查询",
                    parent_id = 8,
                    order_num = 4,
                    path = "stock/list",
                    component = "storage/stock/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/Stock/List",
                    icon = "kucunchaxun",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6210,
                    menu_name = "入库记录",
                    parent_id = 8,
                    order_num = 5,
                    path = "enter/list",
                    component = "storage/enter/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/Stock/Enter",
                    icon = "rukujilu",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6220,
                    menu_name = "出库记录",
                    parent_id = 8,
                    order_num = 6,
                    path = "leave/list",
                    component = "storage/leave/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/Stock/Leave",
                    icon = "chukujilu",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6230,
                    menu_name = "盘点管理",
                    parent_id = 8,
                    order_num = 7,
                    path = "check/list",
                    component = "storage/check/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/Inventory/List",
                    icon = "a-qiyeguanli",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6240,
                    menu_name = "盘点任务",
                    parent_id = 8,
                    order_num = 8,
                    path = "check/task",
                    component = "storage/check/task",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/Inventory/Task",
                    icon = "a-qiyeguanli",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });





                this.Execute.Sql("delete FROM mz_menu where menu_id=6170");
                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 6170,
                    menu_name = "库存预警",
                    parent_id = 8,
                    order_num = 11,
                    path = "house/warn",
                    component = "storage/house/warn",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/House/WarnList",
                    icon = "cangkuguanli",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });


                this.Execute.Sql("delete FROM mz_menu where menu_id=6290");
                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 6290,
                    menu_name = "出库申请",
                    parent_id = 8,
                    order_num = 15,
                    path = "leave/applylist",
                    component = "storage/leave/applylist",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/Apply/List",
                    icon = "chukujilu",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });

                Execute.Sql("DROP VIEW IF EXISTS mz_stockrecord_v");
                Execute.Sql(@"CREATE VIEW mz_stockrecord_v as (select sp.*,b.BatchName as Name,b.PhotoUrl,b.Number as DeviceNumber,b.Unit,h.StoreName from mz_stock_record sp left join mz_product_batch_v b on sp.TargetId=b.Id left join mz_store_house h on sp.HouseId=h.Id)");

                Execute.Sql("DROP VIEW IF EXISTS mz_stockpile_v");
                Execute.Sql(@"CREATE VIEW mz_stockpile_v as (select sp.*,p.BatchName as Name,p.PhotoUrl,p.Number as DeviceNumber,p.Unit,p.SkuNumber from mz_stock_pile sp left join mz_product_batch_v p on sp.TargetId=p.Id)");

            }
            else
            {
                Create.Table("mz_store_house").WithDescription("仓库表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().WithColumnDescription("所属企业Id")
.WithColumn("StoreName").AsString(50).WithColumnDescription("仓库名称")
.WithColumn("IsSystem").AsInt32().WithColumnDescription("是否为系统仓库：0为否，1为是，系统仓库不可删除、不可停用")
.WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态：0为停用，1为正常")
.WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在、 2代表删除）")
.WithColumn("DeptId").AsInt64().WithColumnDescription("所属部门")
.WithColumn("LeaderId").AsInt64().WithColumnDescription("负责人")
.WithColumn("Remark").AsString(500).WithColumnDescription("备注")
.WithColumn("LeaveTemplateId").AsInt64().WithDefaultValue(0).WithColumnDescription("出库审核模板")
.WithColumn("EnterTemplateId").AsInt64().WithDefaultValue(0).WithColumnDescription("入库审核模板")
.WithColumn("LeaveApplyTemplateId").AsInt64().WithDefaultValue(0).WithColumnDescription("出库申请单审核模板")
.WithColumn("LeaveFlowInitJson").AsString(5000).WithDefaultValue("").WithColumnDescription("出库流程初始化json")
.WithColumn("EnterFlowInitJson").AsString(5000).WithDefaultValue("").WithColumnDescription("入库流程初始化json")
.WithColumn("LeaveApplyFlowInitJson").AsString(5000).WithDefaultValue("").WithColumnDescription("出库申请流程初始化json");


                Create.Table("mz_stock_pile").WithDescription("库存表")
    .WithColumn("HouseId").AsString(128).PrimaryKey().WithColumnDescription("所在仓库")
    .WithColumn("TargetType").AsInt32().PrimaryKey().WithColumnDescription("存储类型：0半成品、1成品")
    .WithColumn("TargetId").AsString(128).PrimaryKey().WithColumnDescription("产品批次Id")
    .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属企业Id")
    .WithColumn("Quantity").AsDecimal(10, 2).WithColumnDescription("存储数量")
    .WithColumn("Price").AsDecimal(10, 2).WithColumnDescription("成本均价")
    .WithColumn("LockQuantity").AsDecimal(10, 2).WithColumnDescription("被锁定数量");

                Create.Index("IDXStockTargetIdx").OnTable("mz_stock_pile").OnColumn("TargetType").Ascending().OnColumn("TargetId").Ascending();

                Create.Table("mz_stock_record").WithDescription("库存记录表（记录每次出入库后的数量）")
    .WithColumn("Id").AsInt64().Identity().PrimaryKey().WithColumnDescription("编码")
    .WithColumn("OrgId").AsInt64().WithColumnDescription("所属企业Id")
    .WithColumn("HouseId").AsString(128).WithColumnDescription("所在仓库")
    .WithColumn("TargetType").AsInt32().WithColumnDescription("存储类型：0半成品、1成品")
    .WithColumn("TargetId").AsString(128).WithColumnDescription("产品批次Id")
    .WithColumn("Remnant").AsInt32().WithColumnDescription("变更前存储数量")
    .WithColumn("LockRemnant").AsInt32().WithColumnDescription("变更前锁数量")
    .WithColumn("FormType").AsInt32().WithColumnDescription("单据类型：0为出库，1为入库，2为盘亏修正，3为盘盈修正")
    .WithColumn("FormId").AsString(128).WithColumnDescription("单据Id")
    .WithColumn("Quantity").AsDecimal(10, 2).WithColumnDescription("数量")
    .WithColumn("StockPrice").AsDecimal(10, 2).WithColumnDescription("库存价格")
    .WithColumn("Price").AsDecimal(10, 2).WithColumnDescription("出入价格")
    .WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间");

                Execute.Sql(@"CREATE VIEW mz_stockrecord_v as (select sp.*,b.BatchName as Name,b.PhotoUrl,b.Number as DeviceNumber,b.Unit,h.StoreName from mz_stock_record sp left join mz_product_batch_v b on sp.TargetId=b.Id left join mz_store_house h on sp.HouseId=h.Id)");


                Create.Table("mz_leave_stock").WithDescription("出库单表")
            .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
            .WithColumn("OrgId").AsInt64().WithColumnDescription("所属企业Id")
            .WithColumn("ToOrgId").AsInt64().WithColumnDescription("目标企业Id")
            .WithColumn("CustomerId").AsString(128).WithColumnDescription("对应的客户Id，非出库传空")
            .WithColumn("CustomerType").AsInt32().WithColumnDescription("客户类型：0为代理，1为直销")
            .WithColumn("SourceEnterId").AsString(128).WithColumnDescription("关联的入库单、领用申请单")
            .WithColumn("StockNumber").AsString(50).Unique().WithColumnDescription("出库单唯一编号")
            .WithColumn("FromHouseId").AsString(128).WithColumnDescription("所出仓库")
            .WithColumn("ToHouseId").AsString(128).WithColumnDescription("所入仓库")
            .WithColumn("LeaveMethod").AsInt32().WithColumnDescription("出库方式：0、出货，1、退货，2、调拨，3、领用")
            .WithColumn("ExpressNumber").AsString(255).WithColumnDescription("物流单号")
            .WithColumn("ExpressCompany").AsString(50).WithColumnDescription("物流公司")
            .WithColumn("ExpressPhone").AsString(50).WithColumnDescription("顺风用联系电话")
            .WithColumn("OutDate").AsDateTime().WithColumnDescription("出库时间")
            .WithColumn("Remark").AsString(5000).WithColumnDescription("备注")
            .WithColumn("Status").AsInt32().WithColumnDescription("提交状态：0、待提交；1、待审批；2、出库成功；3、出库失败；")
            .WithColumn("FlowId").AsInt64().WithColumnDescription("关联的流程Id")
            .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
            .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
            .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
            .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

                Create.Table("mz_leave_detail").WithDescription("出库单明细表")
                    .WithColumn("StockId").AsString(128).PrimaryKey().WithColumnDescription("出库单Id")
                    .WithColumn("TargetType").AsInt32().PrimaryKey().WithColumnDescription("存储类型：0半成品、1成品")
                    .WithColumn("TargetId").AsString(128).PrimaryKey().WithColumnDescription("产品批次Id")
                    .WithColumn("Quantity").AsDecimal(10, 2).WithColumnDescription("数量")
                    .WithColumn("Price").AsDecimal(10, 2).WithColumnDescription("价格");

                Create.Table("mz_enter_stock").WithDescription("入库单表")
                    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
            .WithColumn("OrgId").AsInt64().WithColumnDescription("所属企业Id")
            .WithColumn("FromOrgId").AsInt64().WithColumnDescription("来源企业Id")
            .WithColumn("StockNumber").AsString(50).Unique().WithColumnDescription("入库单唯一编号")
            .WithColumn("FromHouseId").AsString(128).WithColumnDescription("所出仓库")
            .WithColumn("ToHouseId").AsString(128).WithColumnDescription("所入仓库")
            .WithColumn("EnterMethod").AsInt32().WithColumnDescription("入库方式：0、入货，1、退货，2、调拨，3、手动")
            .WithColumn("ExpressNumber").AsString(255).WithColumnDescription("物流单号")
            .WithColumn("ExpressCompany").AsString(50).WithColumnDescription("物流公司")
            .WithColumn("ExpressPhone").AsString(50).WithColumnDescription("顺风用联系电话")
            .WithColumn("InDate").AsDateTime().WithColumnDescription("入库时间")
            .WithColumn("Remark").AsString(5000).WithColumnDescription("备注")
            .WithColumn("Status").AsInt32().WithColumnDescription("提交状态：0、待提交；1、待审批；2、入库成功；3、待退货；4、已退货；")
            .WithColumn("FlowId").AsInt64().WithColumnDescription("关联的流程Id")
            .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
            .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
            .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
            .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

                Create.Table("mz_enter_detail").WithDescription("入库单明细表")
                     .WithColumn("StockId").AsString(128).PrimaryKey().WithColumnDescription("入库单Id")
                    .WithColumn("TargetType").AsInt32().PrimaryKey().WithColumnDescription("存储类型：0半成品、1成品")
                    .WithColumn("TargetId").AsString(128).PrimaryKey().WithColumnDescription("产品批次Id")
                    .WithColumn("Quantity").AsDecimal(10, 2).WithColumnDescription("数量")
                    .WithColumn("Price").AsDecimal(10, 2).WithColumnDescription("价格");


                Create.Table("mz_inventory").WithDescription("盘点表")
                     .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                     .WithColumn("OrgId").AsInt64().WithColumnDescription("所属企业Id")
                    .WithColumn("Name").AsString(50).WithColumnDescription("盘点名称")
                    .WithColumn("HouseId").AsString(128).WithColumnDescription("盘点仓库")
                    .WithColumn("Status").AsInt32().WithColumnDescription("盘点状态：0、待提交；1、待开始；2、初盘；3、复盘；4、结束；5、已修正；6、取消")
                    .WithColumn("StartOn").AsDateTime().Nullable().WithColumnDescription("初盘时间")
                    .WithColumn("CheckOn").AsDateTime().Nullable().WithColumnDescription("复盘时间")
                    .WithColumn("EndOn").AsDateTime().Nullable().WithColumnDescription("结束时间")
                    .WithColumn("Remark").AsString(5000).WithColumnDescription("备注")
                    .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                    .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                    .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                    .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

                Create.Table("mz_inventory_item").WithDescription("盘点项表")
                    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("GUID编码")
                    .WithColumn("OrgId").AsInt64().WithColumnDescription("所属企业Id")
                    .WithColumn("InventoryId").AsString(128).WithColumnDescription("盘点Id")
                    .WithColumn("HouseId").AsString(128).WithColumnDescription("盘点仓库")
                    .WithColumn("SnapQuantity").AsDecimal(10, 2).Nullable().WithColumnDescription("快照数量")
                    .WithColumn("TargetType").AsInt32().WithColumnDescription("存储类型：0半成品、1成品")
                    .WithColumn("TargetId").AsString(128).WithColumnDescription("产品批次Id")
                    .WithColumn("FirstCount").AsDecimal(10, 2).Nullable().WithColumnDescription("初盘数量")
                    .WithColumn("CheckCount").AsDecimal(10, 2).Nullable().WithColumnDescription("复盘数量")
                    .WithColumn("Count").AsDecimal(10, 2).Nullable().WithColumnDescription("最终数量")
                    .WithColumn("DiffCount").AsDecimal(10, 2).Nullable().WithColumnDescription("最终差异数量")
                    .WithColumn("FirstUserId").AsInt64().WithColumnDescription("初盘人员")
                    .WithColumn("FirstAtTime").AsDateTime().Nullable().WithColumnDescription("初盘时间")
                    .WithColumn("CheckUserId").AsInt64().WithColumnDescription("复盘人员")
                    .WithColumn("CheckAtTime").AsDateTime().Nullable().WithColumnDescription("复盘时间");

                Create.Index("IDXInventoryItemUni").OnTable("mz_inventory_item").WithOptions().Unique().OnColumn("InventoryId").Ascending().OnColumn("TargetId").Ascending();
                Create.Index("IDXInventoryHouseTargetItem").OnTable("mz_inventory_item").OnColumn("HouseId").Ascending().OnColumn("TargetId").Ascending();

                Create.Table("mz_inventory_user").WithDescription("盘点人表")
        .WithColumn("InventoryId").AsString(128).WithColumnDescription("盘点Id")
        .WithColumn("TimeIn").AsInt32().WithColumnDescription("0表示初盘人员、1表示复盘人员")
        .WithColumn("UserId").AsInt64().WithColumnDescription("盘点人");


                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 8,
                    menu_name = "数智仓储",
                    parent_id = 0,
                    order_num = 8,
                    path = "jxc",
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "M",
                    visible = "0",
                    status = "0",
                    perms = "/Stock/",
                    icon = "jinxiaocun",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6120,
                    menu_name = "仓库管理",
                    parent_id = 8,
                    order_num = 1,
                    path = "house/list",
                    component = "storage/house/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/House/List",
                    icon = "cangkuguanli",
                    scope = 1,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6121,
                    menu_name = "添加仓库",
                    parent_id = 6120,
                    order_num = 1,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/House/Add",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6122,
                    menu_name = "修改仓库",
                    parent_id = 6120,
                    order_num = 2,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/House/Edit",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6123,
                    menu_name = "删除仓库",
                    parent_id = 6120,
                    order_num = 3,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/House/Remove",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6130,
                    menu_name = "库存查询",
                    parent_id = 8,
                    order_num = 4,
                    path = "stock/list",
                    component = "storage/stock/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/Stock/List",
                    icon = "kucunchaxun",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6210,
                    menu_name = "入库记录",
                    parent_id = 8,
                    order_num = 5,
                    path = "enter/list",
                    component = "storage/enter/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/Stock/Enter",
                    icon = "rukujilu",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6220,
                    menu_name = "出库记录",
                    parent_id = 8,
                    order_num = 6,
                    path = "leave/list",
                    component = "storage/leave/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/Stock/Leave",
                    icon = "chukujilu",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6230,
                    menu_name = "盘点管理",
                    parent_id = 8,
                    order_num = 7,
                    path = "check/list",
                    component = "storage/check/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/Inventory/List",
                    icon = "a-qiyeguanli",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 6240,
                    menu_name = "盘点任务",
                    parent_id = 8,
                    order_num = 8,
                    path = "check/task",
                    component = "storage/check/task",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/Inventory/Task",
                    icon = "a-qiyeguanli",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });





                this.Execute.Sql("delete FROM mz_menu where menu_id=6170");
                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 6170,
                    menu_name = "库存预警",
                    parent_id = 8,
                    order_num = 11,
                    path = "house/warn",
                    component = "storage/house/warn",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/House/WarnList",
                    icon = "cangkuguanli",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });


                Alter.Table("mz_stock_pile").AddColumn("MinNum").AsInt32().WithDefaultValue(-1).WithColumnDescription("库存下限，-1不预警")
                    .AddColumn("MaxNum").AsInt32().WithDefaultValue(-1).WithColumnDescription("库存上限，-1不预警")
                    .AddColumn("IsTrigger").AsBoolean().WithDefaultValue(false).WithColumnDescription("是否已发送提醒");

                Create.Index("IDXPileNum").OnTable("mz_stock_pile").OnColumn("IsTrigger").Ascending().OnColumn("MinNum").Ascending().OnColumn("MaxNum").Ascending();


                Execute.Sql("DROP VIEW IF EXISTS mz_stockpile_v");
                Execute.Sql(@"CREATE VIEW mz_stockpile_v as (select sp.*,p.BatchName as Name,p.PhotoUrl,p.Number as DeviceNumber,p.Unit,p.SkuNumber from mz_stock_pile sp left join mz_product_batch_v p on sp.TargetId=p.Id)");



                this.Execute.Sql("delete FROM mz_menu where menu_id=6290");
                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 6290,
                    menu_name = "出库申请",
                    parent_id = 8,
                    order_num = 15,
                    path = "leave/applylist",
                    component = "storage/leave/applylist",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/StorageService/Apply/List",
                    icon = "chukujilu",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });

                Execute.Sql("DROP TABLE IF EXISTS mz_leave_apply");
                Create.Table("mz_leave_apply").WithDescription("出库申请单")
                    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                    .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织")
                    .WithColumn("ApplyNumber").AsString(50).Unique().WithColumnDescription("申请单唯一编号")
                    .WithColumn("HouseId").AsString(128).WithColumnDescription("出库仓库")
                    .WithColumn("ApplyType").AsString(50).WithColumnDescription("申请类型,参考字典apply_type")
                    .WithColumn("ApplyWorkId").AsString(128).Indexed().Nullable().WithColumnDescription("关联的申请类型工单")
                    .WithColumn("ApplyUserId").AsInt64().WithColumnDescription("申请人")
                    .WithColumn("ApplyDeptId").AsInt64().WithColumnDescription("申请部门")
                    .WithColumn("ApplyOn").AsDateTime().WithColumnDescription("申请时间")
                    .WithColumn("Reason").AsString(500).WithColumnDescription("申请原因")
                    .WithColumn("FlowId").AsInt64().WithColumnDescription("关联的流程Id")
                    .WithColumn("Status").AsInt32().WithColumnDescription("提交状态：0、待提交；1、待审批；2、申请成功；3、申请失败；4、已取消;")
                    .WithColumn("OutStatus").AsInt32().WithDefaultValue(0).WithColumnDescription("0、待出库;1、出库中;2、已出库;3、出库失败");



                Execute.Sql("DROP TABLE IF EXISTS mz_leave_apply_detail");
                Create.Table("mz_leave_apply_detail").WithDescription("出库申请单明细")
                    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                    .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织")
                    .WithColumn("ApplyId").AsString(128).WithColumnDescription("申请单编码")
                    .WithColumn("HouseId").AsString(128).WithColumnDescription("所在仓库")
                    .WithColumn("TargetType").AsInt32().WithColumnDescription("存储类型：0半成品、1成品")
                    .WithColumn("TargetId").AsString(128).WithColumnDescription("产品批次Id")
                    .WithColumn("Quantity").AsDecimal(10, 2).WithColumnDescription("数量");

                Create.Index("IDXUNILEAVEAPPLY").OnTable("mz_leave_apply_detail").WithOptions().Unique().OnColumn("ApplyId").Ascending().OnColumn("TargetType").Ascending().OnColumn("TargetId").Ascending();


                Execute.Sql("delete from mz_dict_type where dict_type='apply_type'");
                Insert.IntoTable("mz_dict_type").Row(new
                {
                    dict_name = "申请类型",
                    dict_type = "apply_type",
                    status = "0",
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0,
                    remark = "出库申请类型列表"
                });

                Execute.Sql("delete from mz_dict_data where dict_type='apply_type'");
                Insert.IntoTable("mz_dict_data").Row(new
                {
                    dict_sort = 0,
                    dict_label = "生产领用",
                    dict_value = "0",
                    dict_type = "apply_type",
                    css_class = string.Empty,
                    list_class = "default",
                    is_default = "Y",
                    status = "0",
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0,
                    remark = string.Empty
                }).Row(new
                {
                    dict_sort = 1,
                    dict_label = "办公领用",
                    dict_value = "1",
                    dict_type = "apply_type",
                    css_class = string.Empty,
                    list_class = "default",
                    is_default = "N",
                    status = "0",
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0,
                    remark = string.Empty
                });


                //代理添加数智仓储
                Insert.IntoTable("mz_role_permission").Row(new
                {
                    RoleID = 4,
                    MenuId = 8
                }).Row(new
                {
                    RoleID = 4,
                    MenuId = 6120
                }).Row(new
                {
                    RoleID = 4,
                    MenuId = 6121
                }).Row(new
                {
                    RoleID = 4,
                    MenuId = 6122
                }).Row(new
                {
                    RoleID = 4,
                    MenuId = 6123
                }).Row(new
                {
                    RoleID = 4,
                    MenuId = 6130
                }).Row(new
                {
                    RoleID = 4,
                    MenuId = 6210
                }).Row(new
                {
                    RoleID = 4,
                    MenuId = 6220
                }).Row(new
                {
                    RoleID = 4,
                    MenuId = 6230
                }).Row(new
                {
                    RoleID = 4,
                    MenuId = 6240
                });
            }

        }

        public override void Down()
        {
        }

    }
}
