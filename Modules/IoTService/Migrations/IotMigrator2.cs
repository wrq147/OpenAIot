using FluentMigrator;
using System;


namespace IoTService.Migrations
{
    [Migration(20230206001)]
    public class IotMigrator2 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 4207,
                menu_name = "修改协议",
                parent_id = 4205,
                order_num = 4,
                path = "physicalModel/productAdd/:id",
                component = "iot/physicalModel/productAdd",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "1",
                status = "0",
                perms = "/IoTService/IotProduct/Edit",
                icon = string.Empty,
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 4208,
                menu_name = "新增协议",
                parent_id = 4205,
                order_num = 5,
                path = "physicalModel/productAddSteps",
                component = "iot/physicalModel/productAddSteps",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "1",
                status = "0",
                perms = "/IoTService/IotProduct/Add",
                icon = string.Empty,
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
