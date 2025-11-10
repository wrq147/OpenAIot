using FluentMigrator;
using System;
namespace MessageService
{
    [Migration(20221115001)]
    public class MessageMigrator : Migration
    {
        public override void Up()
        {
            Create.Table("mz_message").WithDescription("站内消息表")
       .WithColumn("id").AsInt64().PrimaryKey()
       .WithColumn("OrgId").AsInt64().Indexed("MessageOrgId").WithColumnDescription("关联组织ID")
       .WithColumn("sender_id").AsInt64().WithColumnDescription("发送者id")
       .WithColumn("receiver_id").AsInt64().WithColumnDescription("接收者id")
       .WithColumn("label").AsString(255).WithColumnDescription("消息标签")
       .WithColumn("click_type").AsString(20).Nullable().WithColumnDescription("消息点击类型")
       .WithColumn("click_url").AsString(255).Nullable().WithColumnDescription("消息点击跳转目标")
       .WithColumn("content").AsString(800).Nullable().WithColumnDescription("正文")
       .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间");


            Create.Table("mz_message_log").WithDescription("用户站内消息处理表")
.WithColumn("id").AsInt64().PrimaryKey().Identity()
.WithColumn("receiver_id").AsInt64().WithColumnDescription("接收者id")
.WithColumn("messsage_id").AsInt64().Indexed("MESSAGE_LOG_ID_IDX").WithColumnDescription("消息id")
.WithColumn("status").AsInt32().WithColumnDescription("0 未读，1 已读，2 删除")
.WithColumn("read_time").AsDateTime().Nullable().WithColumnDescription("读取时间");

            Create.Index("IDXMessLogUni").OnTable("mz_message_log").WithOptions().Unique().OnColumn("receiver_id").Ascending().OnColumn("messsage_id").Ascending();

            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 3010,
                menu_name = "站内消息",
                parent_id = 4,
                order_num = 5,
                path = "message/index",
                component = "message/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MsgSrv/Message/List",
                icon = "message",
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
