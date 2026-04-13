using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Migrations
{
    [Migration(20230424001)]
    public class ReportMgrator3 : Migration
    {
        public override void Up()
        {

            Create.Table("mz_report_theme").WithDescription("报表主题")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("ThemeName").AsString(50).WithColumnDescription("主题名称")
.WithColumn("ThemeOption").AsString(20000).WithColumnDescription("主题配置json")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Execute.EmbeddedScript("theme.sql");



            Create.Table("mz_file_source").WithDescription("文件数据源")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("FileSourceOrgId").WithColumnDescription("所属组织ID")
.WithColumn("FileName").AsString(255).WithColumnDescription("文件名称")
.WithColumn("FileUrl").AsString(255).WithColumnDescription("文件连接")
.WithColumn("FileData").AsString(20000).WithColumnDescription("解析后的表格json数据")
.WithColumn("ChartType").AsString(255).WithColumnDescription("图表类型")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Create.Table("mz_data_source").WithDescription("数据库源")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("SQLSourceOrgId").WithColumnDescription("所属组织ID")
.WithColumn("LinkName").AsString(50).WithColumnDescription("连接名称")
.WithColumn("DatabaseType").AsString(255).WithColumnDescription("数据库类型")
.WithColumn("IpAddress").AsString(255).WithColumnDescription("ip地址")
.WithColumn("Port").AsInt32().WithColumnDescription("端口号")
.WithColumn("DatabaseName").AsString(255).WithColumnDescription("数据库名称")
.WithColumn("UserName").AsString(255).WithColumnDescription("用户名")
.WithColumn("Password").AsString(255).WithColumnDescription("密码")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Create.Table("mz_api_source").WithDescription("api数据源")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("ApiSourceOrgId").WithColumnDescription("所属组织ID")
.WithColumn("InterfaceName").AsString(50).WithColumnDescription("接口名称")
.WithColumn("ApiType").AsFixedLengthAnsiString(1).WithColumnDescription("接口类型：0为BI报表、1为打印模板")
.WithColumn("Url").AsString(255).WithColumnDescription("接口地址")
.WithColumn("Method").AsString(10).WithColumnDescription("接口Method")
.WithColumn("ParamJson").AsString(1000).WithColumnDescription("请求的Param")
.WithColumn("ParamType").AsString(20).WithColumnDescription("参数类型:PARAM、JSON、FORM")
.WithColumn("HeaderJson").AsString(1000).WithColumnDescription("请求的Header")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 471,
                menu_name = "数据源",
                parent_id = 4,
                order_num = 4,
                path = "source",
                component = "report/source",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ReportService/Source/List",
                icon = "baobiaoguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 472,
                menu_name = "新增数据源",
                parent_id = 471,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ReportService/Source/Add",
                icon = "",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 473,
                menu_name = "修改数据源",
                parent_id = 471,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ReportService/Source/Edit",
                icon = "",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 474,
                menu_name = "删除数据源",
                parent_id = 471,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ReportService/Source/Remove",
                icon = "",
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
