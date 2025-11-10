using FluentMigrator;

namespace WeiXinService
{
    [Migration(20250722005)]
    public class WeiXinMigrator3 : Migration
    {
        public override void Up()
        {
            this.Alter.Table("mz_corp_sync").WithDescription("修改企业微信同步记录表")
                  .AddColumn("UserName").AsString(32).WithColumnDescription("账号绑定的企业微信扩展字段");
        }
        public override void Down()
        {
        }
    }
}
