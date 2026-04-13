using Common;
using Common.Share;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Migrations
{
    [Migration(20230329001)]
    public class ReportMgrator : Migration
    {
        public override void Up()
        {


            Create.Table("mz_report").WithDescription("报表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("Name").AsString(50).WithColumnDescription("报表名称")
.WithColumn("ReportType").AsString(10).WithDefaultValue("screen").WithColumnDescription("报表类型：screen、table")
.WithColumn("OrgId").AsInt64().Indexed("ReportOrgId").WithColumnDescription("所属组织ID")
.WithColumn("GroupId").AsString(128).WithDefaultValue(string.Empty).Indexed("ReportGroupIDX").WithColumnDescription("分组Id")
.WithColumn("DesInfo").AsString(1000).WithColumnDescription("报表描述")
.WithColumn("Tag").AsString(2000).WithColumnDescription("标签")
.WithColumn("Resolution").AsString(255).WithColumnDescription("大屏分辨率")
.WithColumn("DeviceType").AsString(255).WithColumnDescription("设备类型pc、phone、double")
.WithColumn("IdGlobal").AsInt32().WithColumnDescription("组件累计Id")
.WithColumn("DrawOption").AsString(500000).WithColumnDescription("数据大屏再画数据结构")
.WithColumn("ThemeOption").AsString(500000).WithColumnDescription("主题再画数据结构")
.WithColumn("MapOption").AsString(500000).WithColumnDescription("echart图option集合")
.WithColumn("Zindex").AsInt32().WithColumnDescription("全局累计zindex图层索引")
.WithColumn("Thumbnail").AsString(255).WithColumnDescription("报表缩略图")
.WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0未发布 2已发布）")
.WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
       .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
       .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
       .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
       .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            if (Constants.General.sqltype != "Sqlite")
            {
                Execute.Sql("ALTER TABLE mz_report ADD FULLTEXT INDEX ReportTags (Tag);");
            }


            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 402,
                menu_name = "报表管理",
                parent_id = 4,
                order_num = 2,
                path = "list",
                component = "report/list",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ReportService/Report/List",
                icon = "baobiaoguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 421,
                menu_name = "新增报表",
                parent_id = 402,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ReportService/Report/Add",
                icon = "",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 422,
                menu_name = "修改报表",
                parent_id = 402,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ReportService/Report/Edit",
                icon = "",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 423,
                menu_name = "删除报表",
                parent_id = 402,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/ReportService/Report/Remove",
                icon = "",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 461,
                menu_name = "我的分享",
                parent_id = 4,
                order_num = 3,
                path = "datav/shareList",
                component = "report/datav/shareList",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ReportService/Report/MyShare",
                icon = "table",
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
