using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    [Migration(20250513013)]
    public class EmailMigrator : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_config where config_id=70");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 70,
                config_name = "邮箱配置参数",
                config_key = "system.email",
                config_value = "{\r\n    \"email_from_name\": \"OpenAIot官方邮箱\",\r\n    \"email_from\": \"OpenAIot@126.com\",\r\n    \"email_password\": \"CXJCQKQTBNEUGWEJ\",\r\n    \"ssl\": 1,\r\n    \"email_host\": \"smtp.126.com\",\r\n    \"email_post\": 25,\r\n    \"email_bind_title\": \"OpenAIot - 邮箱验证\"\r\n  }",
                config_type = "Y",
                remark = "配置服务端使用的邮箱服务器信息",
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
