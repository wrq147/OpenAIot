using FluentMigrator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Migrators
{
    [Migration(20240829102)]
    public class AuthMigrator5 : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_menu where menu_id=120");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 120,
                menu_name = "主题管理",
                parent_id = 1,
                order_num = 11,
                path = "theme/index",
                component = "system/theme/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/AuthService/StyleMan/List",
                icon = "baobiaoguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Execute.Sql("DROP TABLE IF EXISTS mz_app_style");
            Create.Table("mz_app_style").WithDescription("应用主题")
.WithColumn("Id").AsString(50).PrimaryKey().WithColumnDescription("主题Id")
.WithColumn("Name").AsString(50).WithColumnDescription("主题名称")
.WithColumn("PhotoUrl").AsString(255).WithColumnDescription("主题展示图")
.WithColumn("StyleJson").AsString(10000).WithColumnDescription("主题Json")
.WithColumn("Remark").AsString(500).WithColumnDescription("主题说明")
.WithColumn("IsPublic").AsBoolean().WithColumnDescription("是否公开")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间")
.WithColumn("createId").AsInt64().WithColumnDescription("归属人Id")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id");

            Execute.Sql("DROP TABLE IF EXISTS mz_org_style");
            Create.Table("mz_org_style").WithDescription("组织的应用主题")
.WithColumn("OrgId").AsInt64().PrimaryKey().WithColumnDescription("组织编号")
.WithColumn("StyleId").AsString(50).PrimaryKey().WithColumnDescription("主题Id")
.WithColumn("FrowWay").AsInt32().WithColumnDescription("来源方式：0自购，1为继承")
.WithColumn("IsUsing").AsBoolean().WithColumnDescription("是否使用中")
.WithColumn("CreatedOn").AsDateTime().WithColumnDescription("添加时间");



            this.Execute.Sql("delete FROM mz_config where config_id=14");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 14,
                config_name = "默认主题",
                config_key = "org.style",
                config_value = "1",
                config_type = "Y",
                remark = "默认为系统组织",
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
