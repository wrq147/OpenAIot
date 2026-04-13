using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Migrations
{
    [Migration(20230624001)]
    public class ReportMgrator4 : Migration
    {
        public override void Up()
        {
            Create.Table("mz_print_template").WithDescription("打印模板")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("模板ID")
.WithColumn("OrgId").AsInt64().Indexed("IDXPrintOrgId").WithColumnDescription("所属组织ID")
.WithColumn("Name").AsString(50).WithColumnDescription("模板名称")
.WithColumn("Content").AsString(500000).WithColumnDescription("模板内容")
.WithColumn("PaperDirection").AsString(1).WithColumnDescription("纸张方向")
.WithColumn("PaperMarginTop").AsInt32().WithColumnDescription("页面上边距")
.WithColumn("PaperMarginBottom").AsInt32().WithColumnDescription("页面下边距")
.WithColumn("Background").AsString(20).WithColumnDescription("背景颜色")
.WithColumn("FontFamily").AsString(20).WithColumnDescription("默认字体")
.WithColumn("LineHeight").AsInt32().WithColumnDescription("默认行高")
.WithColumn("PaperName").AsString(20).WithColumnDescription("纸张类型名")
.WithColumn("PaperWidth").AsInt32().WithColumnDescription("纸张宽")
.WithColumn("PaperHeight").AsInt32().WithColumnDescription("纸张高")
.WithColumn("DataId").AsString(128).WithColumnDescription("数据源Id")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().Nullable().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Create.Table("mz_print_data").WithDescription("打印模板数据源")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("ID")
.WithColumn("Name").AsString(50).WithColumnDescription("数据源名称")
.WithColumn("DataMap").AsString(20000).WithColumnDescription("数据源标识")
.WithColumn("ApiId").AsString(128).WithColumnDescription("接口数据源Id");

            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 481,
                menu_name = "打印模板",
                parent_id = 4,
                order_num = 1,
                path = "print/index",
                component = "report/print/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ReportService/Print/List",
                icon = "dayinmoban",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 482,
                menu_name = "新增模板",
                parent_id = 481,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ReportService/Print/Add",
                icon = "",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 483,
                menu_name = "修改模板",
                parent_id = 481,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ReportService/Print/Edit",
                icon = "",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 484,
                menu_name = "删除模板",
                parent_id = 481,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ReportService/Print/Remove",
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
