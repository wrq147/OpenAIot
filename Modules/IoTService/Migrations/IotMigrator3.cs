using FluentMigrator;
using System;


namespace IoTService.Migrations
{
    [Migration(20230210001)]
    public class IotMigrator3 : Migration
    {
        public override void Up()
        {


            Create.Table("mz_iot_update").WithDescription("Iot设备等待绑定表")
    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("设备Id")
    .WithColumn("OrgId").AsInt64().Indexed("IotUpdateOrgId").WithColumnDescription("所属组织ID")
    .WithColumn("Status").AsByte().WithColumnDescription("状态：0待更新，1更新中")
    .WithColumn("UpdateCount").AsInt32().WithColumnDescription("尝试更新次数")
    .WithColumn("UpdateErr").AsString(200).Nullable().WithColumnDescription("更新失败原因")
    .WithColumn("Version").AsInt32().WithColumnDescription("更新的目标版本")
    .WithColumn("UpdatedOn").AsDateTime().Nullable().WithColumnDescription("更新时间")
    .WithColumn("Level").AsInt32().WithColumnDescription("设备更新优先级（数字越低越优先）");


            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 4212,
                menu_name = "待升级设备",
                parent_id = 4000,
                order_num = 8,
                path = "deviceManage/updateList",
                component = "iot/deviceManage/updateList",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTService/IotUpdate/ListPage",
                icon = "daishengjishebei",
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
