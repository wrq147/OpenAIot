using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService
{
    [Migration(20250617003)]
    public class AfterMigrator2 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 7070,
                menu_name = "设备分布",
                parent_id = 7,
                order_num = 4,
                path = "dev/maplist",
                component = "after/dev/maplist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/AfterService/DevMap/List",
                icon = "haocaiguanli",
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
