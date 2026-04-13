using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService
{
    [Migration(20250407001)]
    public class AfterMigrator : Migration
    {
        public override void Up()
        {
            if (Schema.Table("mz_room").Exists())
            {
                this.Execute.Sql("delete FROM mz_menu where menu_id=7060");
                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 7060,
                    menu_name = "我的设备",
                    parent_id = 7,
                    order_num = 3,
                    path = "dev/list",
                    component = "after/dev/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/Dev/List",
                    icon = "wodeshebei",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });


                this.Execute.Sql("delete FROM mz_menu where menu_id=7050");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7051");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7052");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7053");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7058");
                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 7050,
                    menu_name = "房间管理",
                    parent_id = 7,
                    order_num = 5,
                    path = "dev/room",
                    component = "after/dev/room",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/Room/List",
                    icon = "baobiaoguanli",
                    scope = 1,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7051,
                    menu_name = "添加房间",
                    parent_id = 7050,
                    order_num = 1,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/Room/Add",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7052,
                    menu_name = "修改房间",
                    parent_id = 7050,
                    order_num = 2,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/Room/Edit",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7053,
                    menu_name = "删除房间",
                    parent_id = 7050,
                    order_num = 3,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/Room/Remove",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7058,
                    menu_name = "房间分类",
                    parent_id = 7050,
                    order_num = 8,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/RoomCategory/Man",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });


                this.Execute.Sql("delete FROM mz_menu where menu_id=7020");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7021");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7022");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7023");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7030");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7031");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7032");

                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 7020,
                    menu_name = "计划类型",
                    parent_id = 7,
                    order_num = 1,
                    path = "devplane/list",
                    component = "after/devplane/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlane/List",
                    icon = "baobiaoguanli",
                    scope = 1,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7021,
                    menu_name = "添加计划",
                    parent_id = 7020,
                    order_num = 2,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlane/Add",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7022,
                    menu_name = "修改计划",
                    parent_id = 7020,
                    order_num = 3,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlane/Edit",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7023,
                    menu_name = "删除计划",
                    parent_id = 7020,
                    order_num = 4,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlane/Remove",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7030,
                    menu_name = "计划任务",
                    parent_id = 7,
                    order_num = 2,
                    path = "devplane/task",
                    component = "after/devplane/task",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlaneTask/List",
                    icon = "baobiaoguanli",
                    scope = 1,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7031,
                    menu_name = "发起任务",
                    parent_id = 7030,
                    order_num = 1,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlaneTask/Add",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7032,
                    menu_name = "作废任务",
                    parent_id = 7030,
                    order_num = 2,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlaneTask/Cancel",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });


                this.Execute.Sql("delete FROM mz_menu where menu_id=7033");
                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 7033,
                    menu_name = "查看记录",
                    parent_id = 7030,
                    order_num = 3,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlaneTask/Records",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });

            }
            else
            {
                this.Execute.Sql("delete FROM mz_menu where menu_id=7060");

                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 7060,
                    menu_name = "我的设备",
                    parent_id = 7,
                    order_num = 3,
                    path = "dev/list",
                    component = "after/dev/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/Dev/List",
                    icon = "wodeshebei",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });



                this.Execute.Sql("delete FROM mz_menu where menu_id=7050");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7051");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7052");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7053");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7058");
                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 7050,
                    menu_name = "房间管理",
                    parent_id = 7,
                    order_num = 5,
                    path = "dev/room",
                    component = "after/dev/room",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/Room/List",
                    icon = "baobiaoguanli",
                    scope = 1,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7051,
                    menu_name = "添加房间",
                    parent_id = 7050,
                    order_num = 1,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/Room/Add",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7052,
                    menu_name = "修改房间",
                    parent_id = 7050,
                    order_num = 2,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/Room/Edit",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7053,
                    menu_name = "删除房间",
                    parent_id = 7050,
                    order_num = 3,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/Room/Remove",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7058,
                    menu_name = "房间分类",
                    parent_id = 7050,
                    order_num = 8,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/RoomCategory/Man",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });



                Execute.Sql("DROP TABLE IF EXISTS mz_room");
                Create.Table("mz_room").WithDescription("房间表")
    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
    .WithColumn("Name").AsString(50).WithColumnDescription("房间名称")
    .WithColumn("TargetOrgId").AsInt64().Indexed().WithColumnDescription("房间所属企业Id，为0则无所属")
    .WithColumn("CustomerId").AsString().WithColumnDescription("客户Id")
    .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
    .WithColumn("DeptId").AsInt64().WithColumnDescription("所属部门")
    .WithColumn("LeaderId").AsInt64().WithColumnDescription("负责人")
    .WithColumn("Helper").AsString(500).WithDefaultValue("").WithColumnDescription("协作者（多个,号分隔）")
    .WithColumn("CategoryId").AsString(128).WithColumnDescription("所属分类")
    .WithColumn("Sort").AsInt32().WithColumnDescription("排序值：越小越前面")
    .WithColumn("Remark").AsString(255).WithColumnDescription("备注说明")
    .WithColumn("AutoAdd").AsBoolean().WithColumnDescription("是否自动从关联客户添加设备到房间")
    .WithColumn("ReportToken").AsString(2000).Nullable().WithColumnDescription("房间的监控报表")
    .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
    .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
    .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
    .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

                Execute.Sql("ALTER TABLE mz_room ADD FULLTEXT INDEX RoomHelper (Helper);");


                Execute.Sql("DROP TABLE IF EXISTS mz_room_category");
                Create.Table("mz_room_category").WithDescription("房间分类")
                    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                    .WithColumn("TargetOrgId").AsInt64().Indexed().WithColumnDescription("房间所属企业Id，为0则无所属")
                    .WithColumn("CustomerId").AsString().WithColumnDescription("客户Id")
                        .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                        .WithColumn("Name").AsString(50).WithColumnDescription("分类名称")
                        .WithColumn("Sort").AsInt32().WithColumnDescription("排序值：越小越前面")
                        .WithColumn("ParentId").AsString(128).WithColumnDescription("父分类Id")
                        .WithColumn("Path").AsString(800).Indexed().WithColumnDescription("分类层级");


                Execute.Sql("DROP TABLE IF EXISTS mz_room_device");
                Create.Table("mz_room_device").WithDescription("房间设备关联表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("房间编码")
                .WithColumn("TargetId").AsString(128).PrimaryKey().WithColumnDescription("设备Id")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属企业Id");


                Execute.Sql("DROP VIEW IF EXISTS mz_room_device_v;");
                Execute.Sql("CREATE VIEW mz_room_device_v as select rd.Id,rd.TargetId,rd.OrgId,r.LeaderId,r.DeptId,r.TargetOrgId,r.CategoryId,r.Name,r.Helper from mz_room_device rd left join mz_room r on rd.Id=r.Id");



                this.Execute.Sql("delete FROM mz_menu where menu_id=7020");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7021");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7022");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7023");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7030");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7031");
                this.Execute.Sql("delete FROM mz_menu where menu_id=7032");

                Insert.IntoTable("mz_menu").Row(new
                {
                    menu_id = 7020,
                    menu_name = "计划类型",
                    parent_id = 7,
                    order_num = 1,
                    path = "devplane/list",
                    component = "after/devplane/list",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlane/List",
                    icon = "baobiaoguanli",
                    scope = 1,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7021,
                    menu_name = "添加计划",
                    parent_id = 7020,
                    order_num = 2,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlane/Add",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7022,
                    menu_name = "修改计划",
                    parent_id = 7020,
                    order_num = 3,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlane/Edit",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7023,
                    menu_name = "删除计划",
                    parent_id = 7020,
                    order_num = 4,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlane/Remove",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7030,
                    menu_name = "计划任务",
                    parent_id = 7,
                    order_num = 2,
                    path = "devplane/task",
                    component = "after/devplane/task",
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 0,
                    menu_type = "C",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlaneTask/List",
                    icon = "baobiaoguanli",
                    scope = 1,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7031,
                    menu_name = "发起任务",
                    parent_id = 7030,
                    order_num = 1,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlaneTask/Add",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                }).Row(new
                {
                    menu_id = 7032,
                    menu_name = "作废任务",
                    parent_id = 7030,
                    order_num = 2,
                    path = string.Empty,
                    component = string.Empty,
                    query = string.Empty,
                    is_frame = 0,
                    is_cache = 1,
                    menu_type = "F",
                    visible = "0",
                    status = "0",
                    perms = "/AfterService/DevPlaneTask/Cancel",
                    icon = "#",
                    scope = 0,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });


                Execute.Sql("DROP TABLE IF EXISTS mz_plane_type");
                Create.Table("mz_plane_type").WithDescription("计划类型")
    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
    .WithColumn("Name").AsString(50).WithColumnDescription("计划名称")
    .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
    .WithColumn("StartWay").AsInt32().WithColumnDescription("0为手动发起、1为定时发起、2为设备事件发起")
    .WithColumn("TimerCron").AsString(50).WithColumnDescription("Cron表达式")
    .WithColumn("PlaneDays").AsInt32().WithColumnDescription("计划执行天数，为0不限制")
    .WithColumn("ExcludeHoliday").AsBoolean().WithColumnDescription("是否排除假期")
    .WithColumn("IsFilterLeader").AsBoolean().Indexed().WithColumnDescription("是否限制权限发起")
    .WithColumn("Remark").AsString(500).WithColumnDescription("备注")
    .WithColumn("FlowTemplateId").AsInt64().WithColumnDescription("派工流程模板Id")
    .WithColumn("FlowInitJson").AsString(5000).WithColumnDescription("流程初始化json")
    .WithColumn("ExpireNotices").AsString(2000).WithColumnDescription("超期提醒Json:格式为{'day':3,'way':1,'userid':1}")
    .WithColumn("FlowCreatedUserId").AsInt64().WithColumnDescription("流程的发起人")
    .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
    .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
    .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
    .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

                Execute.Sql("DROP TABLE IF EXISTS mz_plane_event");
                Create.Table("mz_plane_event").WithDescription("计划的设备事件")
    .WithColumn("Id").AsInt64().Identity().PrimaryKey().WithColumnDescription("编码")
    .WithColumn("PlaneId").AsString(128).Indexed().WithColumnDescription("计划类型编码")
    .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("事件所属组织")
    .WithColumn("EventId").AsString(50).Indexed().WithColumnDescription("事件标识符")
    .WithColumn("EventName").AsString(50).WithColumnDescription("事件名称");

                Create.Index("IDXPlaneEvent").OnTable("mz_plane_event").OnColumn("OrgId").Ascending().OnColumn("EventId").Ascending();


                Execute.Sql("DROP TABLE IF EXISTS mz_plane_target");
                Create.Table("mz_plane_target").WithDescription("计划的设备关联")
                    .WithColumn("PlaneId").AsString(128).PrimaryKey().WithColumnDescription("计划类型编码")
                    .WithColumn("TargetId").AsString(128).PrimaryKey().WithColumnDescription("目标设备Id")
                    .WithColumn("TargetType").AsInt32().WithColumnDescription("0为设备，1为产品");


                Execute.Sql("DROP TABLE IF EXISTS mz_plane_task");
                Create.Table("mz_plane_task").WithDescription("计划任务")
    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
    .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
    .WithColumn("PlaneNumber").AsString(50).Unique().WithColumnDescription("任务单号")
    .WithColumn("PlanName").AsString(50).WithColumnDescription("计划名称")
    .WithColumn("PlanTypeId").AsString(128).WithColumnDescription("设备计划Id")
    .WithColumn("FlowId").AsInt64().WithColumnDescription("关联流程任务Id")
    .WithColumn("TaskStatus").AsByte().WithColumnDescription("状态：0未开始、1待执行、2执行中、3已完成、4已过期、5已验收、6验收失败、7已作废")
    .WithColumn("TargetId").AsString(128).WithColumnDescription("设备Id")
    .WithColumn("NoticeCount").AsInt32().WithColumnDescription("超时已提醒次数")
    .WithColumn("DeptId").AsInt64().WithColumnDescription("任务发起人所在部门")
    .WithColumn("UserId").AsInt64().WithColumnDescription("任务的发起人")
    .WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间")
    .WithColumn("StartOn").AsDateTime().WithColumnDescription("开始时间")
    .WithColumn("EndOn").AsDateTime().WithColumnDescription("截止时间")
    .WithColumn("DispatchOn").AsDateTime().Nullable().WithColumnDescription("派工时间")
    .WithColumn("DispatchUserId").AsInt64().WithColumnDescription("派工人")
    .WithColumn("ExecutedOn").AsDateTime().Nullable().WithColumnDescription("执行时间")
    .WithColumn("ExeUserId").AsInt64().WithColumnDescription("执行人")
    .WithColumn("FinishedOn").AsDateTime().Nullable().WithColumnDescription("完成时间")
    .WithColumn("CheckOn").AsDateTime().Nullable().WithColumnDescription("验收时间")
    .WithColumn("CheckUserId").AsInt64().WithColumnDescription("验收人");



                Execute.Sql("delete from mz_dict_type where dict_type='device_run'");
                Insert.IntoTable("mz_dict_type").Row(new
                {
                    dict_name = "运行状态",
                    dict_type = "device_run",
                    status = "0",
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0,
                    remark = "设备运行状态过滤用"
                });

                Execute.Sql("delete from mz_dict_data where dict_type='device_run'");
                Insert.IntoTable("mz_dict_data").Row(new
                {
                    dict_sort = 0,
                    dict_label = "正常",
                    dict_value = "正常",
                    dict_type = "device_run",
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
                    dict_label = "维修",
                    dict_value = "维修",
                    dict_type = "device_run",
                    css_class = string.Empty,
                    list_class = "default",
                    is_default = "N",
                    status = "0",
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0,
                    remark = string.Empty
                }).Row(new
                {
                    dict_sort = 2,
                    dict_label = "保养",
                    dict_value = "保养",
                    dict_type = "device_run",
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


                this.Execute.Sql("delete FROM mz_config where config_id=15");
                Insert.IntoTable("mz_config").Row(new
                {
                    config_id = 15,
                    config_name = "启用运行状态",
                    config_key = "device.runstate",
                    config_value = "false",
                    config_type = "Y",
                    remark = "是否启用设备运行状态过滤列表",
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });
                this.Execute.Sql("delete FROM mz_config where config_id=30");
                Insert.IntoTable("mz_config").Row(new
                {
                    config_id = 30,
                    config_name = "隐藏设备计划",
                    config_key = "device.plane",
                    config_value = "false",
                    config_type = "Y",
                    remark = "配置设备详情是否隐藏计划",
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    createId = 0,
                    updateId = 0
                });

           
            }

        }
        public override void Down()
        {
        }
    }
}
