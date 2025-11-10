using FluentMigrator;
using System;

namespace MonitorService
{
    [Migration(20241210005)]
    public class MonitorMigrator2 : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_menu where menu_id=130");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 130,
                menu_name = "假期设置",
                parent_id = 107,
                order_num = 12,
                path = "org/vacationConfig",
                component = "system/org/vacationConfig",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "1",
                status = "0",
                perms = "/AuthService/Holiday/List",
                icon = "chukujilu",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            if (Schema.Table("mz_job").Column("holiday_name").Exists())
            {
                Delete.Column("holiday_name").FromTable("mz_job");
            }

            Execute.Sql("DROP TABLE IF EXISTS mz_holiday_org");
            Create.Table("mz_holiday_org").WithDescription("企业的节日表")
.WithColumn("Id").AsString(64).PrimaryKey().WithColumnDescription("编号")
.WithColumn("OrgId").AsInt64().Indexed("HolidayOrgOrgId").WithColumnDescription("所属组织ID")
.WithColumn("Holiday").AsDateTime().WithColumnDescription("节日日期")
.WithColumn("TimeWay").AsInt32().WithColumnDescription("0表示整天，1表示起点，2表示结束")
.WithColumn("DayStr").AsString().WithColumnDescription("日期表示的字符串")
.WithColumn("HolidayTypeId").AsString().Indexed().WithColumnDescription("关联的节日类型Id");

            Create.Index("IDXHolidayOrg").OnTable("mz_holiday_org").OnColumn("OrgId").Ascending().OnColumn("Holiday").Ascending();
            Create.Index("IDXHolidayDayStrOrg").OnTable("mz_holiday_org").OnColumn("OrgId").Ascending().OnColumn("DayStr").Ascending();

            Execute.Sql("DROP TABLE IF EXISTS mz_holiday_type");
            Create.Table("mz_holiday_type").WithDescription("节日类型表(每日根据类型定时生成节日)")
.WithColumn("Id").AsString().PrimaryKey().WithColumnDescription("节日类型ID")
.WithColumn("OrgId").AsInt64().Indexed("HolidayTypeOrgId").WithColumnDescription("所属组织ID")
.WithColumn("HolidayName").AsString(50).WithColumnDescription("节日类型名称")
.WithColumn("CalendarType").AsInt32().WithColumnDescription("日历类型：0为公历、1为农历、2为星期")
.WithColumn("HolidayStart").AsDateTime().WithColumnDescription("开始放假")
.WithColumn("HolidayEnd").AsDateTime().WithColumnDescription("结束放假")
.WithColumn("NextTime").AsDateTime().Indexed().WithColumnDescription("下次生成节日时间")
.WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间");
        }
        public override void Down()
        {
        }
    }
}
