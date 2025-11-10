using FluentMigrator;
using System;

namespace ReportService.Migrations
{
    [Migration(20240809007)]
    public class ReportMgrator2 : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_report_com");
            //            Create.Table("mz_report_com").WithDescription("报表组件")
            //.WithColumn("Id").AsString().PrimaryKey().WithColumnDescription("编号")
            //.WithColumn("Name").AsString(50).WithColumnDescription("组件名称（唯一）")
            //.WithColumn("OrgId").AsInt64().Indexed("ReportComOrgId").WithColumnDescription("所属组织ID")
            //.WithColumn("Tag").AsString(2000).WithColumnDescription("标签")
            //.WithColumn("Option").AsString(2000).WithColumnDescription("组件option配置")
            //.WithColumn("Graph").AsString(255).WithColumnDescription("所属图表")
            //.WithColumn("Component").AsString(255).WithColumnDescription("所属组件")
            //.WithColumn("DesInfo").AsString(1000).WithColumnDescription("组件描述")
            //.WithColumn("Thumbnail").AsString(255).WithColumnDescription("缩略图")
            //.WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0正常 1停用 2已发布）")
            //.WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
            //       .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
            //       .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
            //       .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
            //       .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //            Execute.Sql("ALTER TABLE mz_report_com ADD FULLTEXT INDEX ReportComTags (Tag);");

            this.Execute.Sql("delete FROM mz_menu where menu_id=404");
            this.Execute.Sql("delete FROM mz_menu where menu_id=405");
            this.Execute.Sql("delete FROM mz_menu where menu_id=406");
            this.Execute.Sql("delete FROM mz_menu where menu_id=407");
            //Insert.IntoTable("mz_menu").Row(new
            //{
            //    menu_id = 404,
            //    menu_name = "组件管理",
            //    parent_id = 4,
            //    order_num = 4,
            //    path = "widgetManger",
            //    component = "report/widgetManger",
            //    query = string.Empty,
            //    is_frame = 0,
            //    is_cache = 1,
            //    menu_type = "C",
            //    visible = "0",
            //    status = "0",
            //    perms = "/ReportService/Widget/List",
            //    icon = "table",
            //    scope = 0,
            //    create_time = DateTime.Now,
            //    update_time = DateTime.Now,
            //    createId = 0,
            //    updateId = 0
            //}).Row(new
            //{
            //    menu_id = 405,
            //    menu_name = "新增组件",
            //    parent_id = 404,
            //    order_num = 5,
            //    path = string.Empty,
            //    component = string.Empty,
            //    query = string.Empty,
            //    is_frame = 0,
            //    is_cache = 1,
            //    menu_type = "F",
            //    visible = "0",
            //    status = "0",
            //    perms = "/ReportService/Widget/Add",
            //    icon = "",
            //    scope = 0,
            //    create_time = DateTime.Now,
            //    update_time = DateTime.Now,
            //    createId = 0,
            //    updateId = 0
            //}).Row(new
            //{
            //    menu_id = 406,
            //    menu_name = "修改组件",
            //    parent_id = 404,
            //    order_num = 6,
            //    path = string.Empty,
            //    component = string.Empty,
            //    query = string.Empty,
            //    is_frame = 0,
            //    is_cache = 1,
            //    menu_type = "F",
            //    visible = "0",
            //    status = "0",
            //    perms = "/ReportService/Widget/Edit",
            //    icon = "",
            //    scope = 0,
            //    create_time = DateTime.Now,
            //    update_time = DateTime.Now,
            //    createId = 0,
            //    updateId = 0
            //}).Row(new
            //{
            //    menu_id = 407,
            //    menu_name = "删除组件",
            //    parent_id = 404,
            //    order_num = 7,
            //    path = string.Empty,
            //    component = string.Empty,
            //    query = string.Empty,
            //    is_frame = 0,
            //    is_cache = 1,
            //    menu_type = "F",
            //    visible = "0",
            //    status = "0",
            //    perms = "/ReportService/Widget/Remove",
            //    icon = "",
            //    scope = 0,
            //    create_time = DateTime.Now,
            //    update_time = DateTime.Now,
            //    createId = 0,
            //    updateId = 0
            //});
        }
        public override void Down()
        {
        }
    }
}
