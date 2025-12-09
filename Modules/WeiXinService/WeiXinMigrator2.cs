using FluentMigrator;
using System;

namespace WeiXinService
{
    [Migration(20250513011)]
    public class WeiXinMigrator2 : Migration
    {
        public override void Up()
        {

            this.Execute.Sql("delete FROM mz_config where config_id=65");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 65,
                config_name = "微信配置参数",
                config_key = "system.wx",
                config_value = "{\r\n    \"default_appid\": \"wx77be7d9dcd9e9dd2\",\r\n    \"push_appid\": \"\",\r\n    \"push_template\": {\r\n      \"设备告警\": {\"templateid\":\"xxxx\",\"url\":\"hhh\",\"miniprogram_appid\":\"yyyy\",\"miniprogram_pagepath\":\"zzzz\",\"firstData\":\"$label\",\"keyword1\":\"$content\",\"keyword2\":\"$now\",\"keyword3\":\"\",\"keyword4\":\"\",\"remark\":\"请尽快确认\"}\r\n    }\r\n  }",
                config_type = "Y",
                remark = "配置微信默认appid和推送模板",
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
