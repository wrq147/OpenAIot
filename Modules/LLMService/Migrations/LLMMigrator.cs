using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Migrations
{
    [Migration(20260608001)]
    public class LLMMigrator : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_menu where menu_id=55");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 55,
                menu_name = "AI助手",
                parent_id = 4,
                order_num = 4,
                path = "ai/assistant",
                component = "llm/ai/assistant",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "1",
                status = "0",
                perms = "/LLMService/AI/Assistant",
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
