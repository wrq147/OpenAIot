using FluentMigrator;
using System;

namespace AuthService.Migrators
{
    [Migration(20221206001)]
    public class AuthMigrator2 : Migration
    {
        public override void Up()
        {
      
            Insert.IntoTable("mz_role").Row(new
            {
                RoleID = 11,
                RoleName = "基础会员",
                RoleSort = 0,
                RoleDesc = "用户注册的初始角色",
                IsSystem = "1",
                Status = "0",
                OrgId = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 7,
                config_name = "用户注册-默认角色",
                config_key = "sys.reg.roleId",
                config_value = "11",
                config_type = "Y",
                remark = "用户注册使用的初始角色",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Insert.IntoTable("mz_user_role").Row(new
            {
                UserId = 1,
                RoleID = 11,
                OrgId = 0
            });

            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 4,
                menu_name = "统计报表",
                parent_id = 0,
                order_num = 4,
                path = "report",
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "M",
                visible = "0",
                status = "0",
                perms = "Default",
                icon = "dashboard",
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
