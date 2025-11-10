using FluentMigrator;
using System;

namespace IoTService.Migrations
{
    [Migration(20240802025)]
    public class IotMigrator4 : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_menu where menu_id=7");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 7,
                menu_name = "设备中台",
                parent_id = 0,
                order_num = 4,
                path = "after",
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "M",
                visible = "0",
                status = "0",
                perms = "/After/",
                icon = "dashboard",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Execute.Sql("DROP TABLE IF EXISTS mz_iot_warning");

            Create.Table("mz_iot_warning").WithDescription("Iot告警工单表")
.WithColumn("Id").AsInt64().PrimaryKey().Identity().WithColumnDescription("Id编号")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("工单所属企业")
.WithColumn("WarnNumber").AsString(50).Unique().WithColumnDescription("工单唯一编号")
.WithColumn("Name").AsString(50).WithColumnDescription("告警名称")
.WithColumn("Code").AsString(50).WithColumnDescription("事件标识")
.WithColumn("Description").AsString(500).WithColumnDescription("告警描述")
.WithColumn("Level").AsInt32().WithColumnDescription("报警级别：普通、告警、紧急")
.WithColumn("DeviceId").AsString().Indexed("IDXWarningDevice").WithColumnDescription("设备Id")
.WithColumn("MsgInfo").AsString(2000).WithColumnDescription("消息内容")
.WithColumn("Status").AsByte().WithColumnDescription("状态：0待处理，1已处理，2待派工")
.WithColumn("ClearOn").AsDateTime().Nullable().WithColumnDescription("处理时间")
.WithColumn("ClearId").AsInt64().Nullable().WithColumnDescription("处理人员")
.WithColumn("ClearRemark").AsString(500).WithColumnDescription("处理备注")
.WithColumn("CreateOn").AsDateTime().WithColumnDescription("告警时间")
.WithColumn("FlowId").AsInt64().WithColumnDescription("关联的流程Id");


            this.Execute.Sql("delete FROM mz_menu where menu_id=7010");
            this.Execute.Sql("delete FROM mz_menu where menu_id=7011");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 7010,
                menu_name = "告警工单",
                parent_id = 7,
                order_num = 7,
                path = "deviceManage/physicalModel/warnList",
                component = "iot/physicalModel/warnList",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTService/IotWarning/ListPage",
                icon = "baojingliebiao",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 7011,
                menu_name = "处理工单",
                parent_id = 7010,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/IoTService/IotWarning/Clear",
                icon = "#",
                scope = 0,
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
