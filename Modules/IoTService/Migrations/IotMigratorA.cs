using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Migrations
{
    [Migration(20260703001)]
    public class IotMigratorA : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 4601,
                menu_name = "数据修正",
                parent_id = 4000,
                order_num = 8,
                path = "data/index",
                component = "iot/data/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTService/IotData/Update",
                icon = "a-guizeliebiao",
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
