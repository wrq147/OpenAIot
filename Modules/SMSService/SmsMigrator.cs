using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSService
{
    [Migration(20250513010)]
    public class SmsMigrator : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_config where config_id=61");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 61,
                config_name = "短信接口",
                config_key = "system.sms",
                config_value = "json",
                config_type = "Y",
                remark = "默认json使用通用json接口，ali表示阿里短信接口，shanyun表示闪云短信接口",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            this.Execute.Sql("delete FROM mz_config where config_id=62");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 62,
                config_name = "json短信接口配置参数",
                config_key = "sms.json",
                config_value = "{\"sms_template\": {\r\n      \"验证码\": {\"action\":\"send\",\"account\":\"123456\",\"password\":\"kkkk\",\"mobile\":\"$mobile\",\"content\":\"【悟空云】验证码：$code，2分钟内有效\",\"extno\":\"10690\"},\r\n      \"邀请短信\": {\"action\":\"send\",\"account\":\"123539\",\"password\":\"4aukzLnizDVhlP\",\"mobile\":\"$mobile\",\"content\":\"【悟空云】悟空云平台邀请您加入我们，点击链接即可登录$url\",\"extno\":\"10690\"},\r\n      \"设备告警\": {\"action\":\"send\",\"account\":\"123539\",\"password\":\"4aukzLnizDVhlP\",\"mobile\":\"$mobile\",\"content\":\"【悟空云】$label，$content，请尽快确认并修复。\",\"extno\":\"10690\"}\r\n    },\r\n    \"url\": \"http://47.99.242.143:7862/smsv2\",\r\n    \"ok\": \"\\\"status\\\":0,\"}",
                config_type = "Y",
                remark = "通用json短信接口的配置参数，包含的短信模板、url、成功回复。",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            this.Execute.Sql("delete FROM mz_config where config_id=63");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 63,
                config_name = "阿里短信接口配置参数",
                config_key = "sms.ali",
                config_value = "{\r\n    \"signName\": \"\",\r\n    \"accessKeyId\": \"\",\r\n    \"accessKeySecret\": \"\",\r\n    \"sms_template\": {\r\n        \"code\": \"\"\r\n    }\r\n}",
                config_type = "Y",
                remark = "阿里短信接口的配置参数，包含的短信模板、签名、密钥。",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });


            this.Execute.Sql("delete FROM mz_config where config_id=64");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 64,
                config_name = "闪云短信接口配置参数",
                config_key = "sms.shanyun",
                config_value = "{\r\n    \"account\": \"aaa\",\r\n    \"secret\": \"bbb\",\r\n    \"sms_template\": {\r\n        \"验证码\": \"验证码：$code，2分钟内有效\",\r\n        \"邀请短信\": \"悟空云平台邀请您加入我们，点击链接即可登录$url\",\r\n        \"设备告警\": \"$label，$content，请尽快确认并修复。\"\r\n    },\r\n    \"signatureStr\": \"悟空云\"\r\n}",
                config_type = "Y",
                remark = "闪云短信接口的配置参数，适用于带有签名的接口。",
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
