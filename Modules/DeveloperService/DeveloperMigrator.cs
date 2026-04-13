using FluentMigrator;
using System;
using System.Threading;

namespace DeveloperService
{
    [Migration(20221028001)]
    public class DeveloperMigrator : Migration
    {

        public override void Up()
        {
            if (Schema.Table("mz_developer").Exists())
            {
                return;
            }
            Create.Table("mz_developer").WithDescription("开发者表")
      .WithColumn("DevId").AsString(64).PrimaryKey().WithColumnDescription("开发者Id")
      .WithColumn("SecKey").AsString(128).Unique().WithColumnDescription("开发者SecKey")
      .WithColumn("KeyType").AsByte().WithColumnDescription("0为简单验证，1为OAuth验证")
      .WithColumn("UserType").AsByte().WithColumnDescription("0为个人开发者，1为企业开发者")
      .WithColumn("UserId").AsInt64().Indexed("DeveloperUserId").WithColumnDescription("开发者关联用户Id")
      .WithColumn("OrgId").AsInt64().Indexed("DeveloperOrgIdId").WithColumnDescription("开发者关联企业Id")
      .WithColumn("CreateOn").AsDateTime().WithColumnDescription("创建时间")
      .WithColumn("UpdatedOn").AsDateTime().WithColumnDescription("更新时间");

            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 108,
                menu_name = "开发者管理",
                parent_id = 1,
                order_num = 8,
                path = "Dev",
                component = "system/dev/list",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/DeveloperService/Developer/ListPage",
                icon = "user",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 801,
                menu_name = "新增开发者",
                parent_id = 108,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/DeveloperService/Developer/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 802,
                menu_name = "修改开发者",
                parent_id = 108,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/DeveloperService/Developer/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 803,
                menu_name = "删除开发者",
                parent_id = 108,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/DeveloperService/Developer/Remove",
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
