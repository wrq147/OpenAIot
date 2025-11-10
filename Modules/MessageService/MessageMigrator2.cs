using FluentMigrator;
using System;


namespace MessageService
{
    [Migration(20250513012)]
    public class MessageMigrator2 : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_push_client");
            Create.Table("mz_push_client").WithDescription("个推用户与客户端的关联表")
    .WithColumn("UserId").AsInt64().PrimaryKey().WithColumnDescription("用户ID")
    .WithColumn("ClientId").AsString(128).WithColumnDescription("用户最新使用的客户端的CID")
    .WithColumn("UpdatedOn").AsDateTime().WithColumnDescription("修改时间");


            this.Execute.Sql("delete FROM mz_config where config_id=67");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 67,
                config_name = "站内消息配置参数",
                config_key = "system.message",
                config_value = "{\r\n    \"active_notice\": true,\r\n    \"push_appid\": \"zLMVUrt44NANgtroqEQbK6\",\r\n    \"push_appkey\": \"DYE6gdvPAl8lHExeUyZly3\",\r\n    \"push_appsecret\": \"dz0I9Xe7MY9VjTRfixGKy8\",\r\n    \"push_mastersecret\": \"Np49DnA6go9ZdxvMS2fmz\"\r\n  }",
                config_type = "Y",
                remark = "站内消息配置参数，其中包含个推、是否主动通知用户配置",
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
