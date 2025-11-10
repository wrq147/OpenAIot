using FluentMigrator;
using System;

namespace AuthService.Migrators
{
    [Migration(20241212007)]
    public class AuthMigrator6 : Migration
    {
        public override void Up()
        {

            this.Execute.Sql("delete FROM mz_config where config_id=31");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 31,
                config_name = "隐藏手机号登录",
                config_key = "login.code",
                config_value = "false",
                config_type = "Y",
                remark = "配置是否隐藏手机号登录",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            this.Execute.Sql("delete FROM mz_config where config_id=32");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 32,
                config_name = "隐藏企业微信登录",
                config_key = "login.corp",
                config_value = "false",
                config_type = "Y",
                remark = "配置是否隐藏企业微信登录",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            this.Execute.Sql("delete FROM mz_config where config_id=33");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 33,
                config_name = "隐藏邮箱登录",
                config_key = "login.email",
                config_value = "false",
                config_type = "Y",
                remark = "配置是否隐藏邮箱登录",
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
