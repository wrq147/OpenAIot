using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Migrations
{
    [Migration(20241202001)]
    public class ReportMgrator5 : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_report_warn");
            Create.Table("mz_report_warn").WithDescription("报表预警")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("预警Id")
.WithColumn("OrgId").AsInt64().Indexed("IDXReportWarnOrgId").WithColumnDescription("所属组织ID")
.WithColumn("ReportId").AsString(128).Indexed().WithColumnDescription("所属报表Id")
.WithColumn("Name").AsString(50).WithColumnDescription("预警名称")
.WithColumn("ConditionJson").AsString(10000).WithColumnDescription("条件json")
.WithColumn("TimerCron").AsString(100).WithColumnDescription("Cron表达式")
.WithColumn("TimerJobId").AsInt64().WithColumnDescription("定时器关联的Job")
.WithColumn("SilenceTime").AsInt32().WithColumnDescription("沉默周期，单位秒（默认表示86400S，最小60秒）")
.WithColumn("NoticeUserType").AsInt32().WithDefaultValue(0).WithColumnDescription("通知对象类型：0为人员，1为角色")
.WithColumn("NoticeUsers").AsString(500).WithColumnDescription("通知人员")
.WithColumn("NoticeWay").AsString(500).WithColumnDescription("通知方式：APP站内通知,EMAIL邮件通知,SMS短信通知,WX微信通知，多个逗号隔开")
.WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0启用 1停用）")
.WithColumn("ShareId").AsString(128).WithColumnDescription("报表分享Id")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().Nullable().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Execute.Sql("DROP TABLE IF EXISTS mz_report_share");
            Create.Table("mz_report_share").WithDescription("报表分享")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("ShareOrgId").WithColumnDescription("所属组织ID")
.WithColumn("ReportId").AsString(128).WithColumnDescription("报表Id")
.WithColumn("UsingPassword").AsString(50).WithDefaultValue(string.Empty).WithColumnDescription("报表的使用密码")
.WithColumn("TokenStr").AsString(500).WithColumnDescription("分享的登录令牌")
.WithColumn("EffectiveTime").AsInt32().WithColumnDescription("有效时间(单位：天)")
.WithColumn("ExpirationTime").AsDateTime().WithColumnDescription("过期时间")
.WithColumn("TimerCron").AsString(100).WithColumnDescription("Cron表达式")
.WithColumn("TimerJobId").AsInt64().WithColumnDescription("定时器关联的Job")
.WithColumn("NoticeUserType").AsInt32().WithColumnDescription("通知对象类型：0为人员，1为角色")
.WithColumn("NoticeUsers").AsString(500).WithColumnDescription("通知人员")
.WithColumn("NoticeWay").AsString(500).WithColumnDescription("通知方式：APP站内通知,EMAIL邮件通知,SMS短信通知,WX微信通知，多个逗号隔开")
.WithColumn("TimerStatus").AsFixedLengthAnsiString(1).WithColumnDescription("是否启用定时推送（0启用 1停用）")
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
