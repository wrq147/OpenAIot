using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Migrators
{
    [Migration(20250507003)]
    public class CustomerMigrator2 : Migration
    {

        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_menu where menu_id=6820");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 6820,
                menu_name = "CRM设置",
                parent_id = 6,
                order_num = 9,
                path = "config/customConfig",
                component = "crm/config/customConfig",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/CRMService/Config/SetCrm",
                icon = "baobiaoguanli",
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
