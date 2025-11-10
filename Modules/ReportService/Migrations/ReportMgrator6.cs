using FluentMigrator;
using FluentMigrator.Runner.Generators.Base;
using System;

namespace ReportService.Migrations
{
    [Migration(20250317001)]
    public class ReportMgrator6 : Migration
    {
        public override void Up()
        {
            if (Schema.Table("mz_report_group").Exists())
            {
                Delete.Table("mz_report_group");
            }
            Create.Table("mz_report_group").WithDescription("报表分组")
    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("分组Id")
    .WithColumn("OrgId").AsInt64().Indexed("ReportGroupOrgId").WithColumnDescription("所属组织ID")
    .WithColumn("Name").AsString(30).WithColumnDescription("分组名")
    .WithColumn("Sort").AsInt32().WithColumnDescription("排序用，值越小越前面")
       .WithColumn("ParentId").AsString(128).WithColumnDescription("父分组Id")
       .WithColumn("Path").AsString(800).Indexed().WithColumnDescription("分组层级")
       .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
       .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
       .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
       .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


        }
        public override void Down()
        {
        }
    }
}
