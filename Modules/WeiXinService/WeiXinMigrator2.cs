using FluentMigrator;
using System;

namespace WeiXinService
{
    [Migration(20250513011)]
    public class WeiXinMigrator2 : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_config where config_id=21");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 21,
                config_name = "短信跳转小程序",
                config_key = "smsjmp",
                config_value = "<html>\r\n<head>\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n</head>\r\n<body>\r\n    <style>\r\n        html {\r\n            font-size: 14px;\r\n            background-color: #fff;\r\n        }\r\n\r\n        body {\r\n            background-color: #fff;\r\n        }\r\n    </style>\r\n    <div style=\"padding:150px 16px 0 16px;\">\r\n        <div style=\"padding-bottom: 20px; color: #999;\">提示:无法自动跳转则点击按钮跳转</div>\r\n        <div style=\"display: flex; justify-content: center;\">\r\n            <a id=\"xhre\" href=\"\" style=\"display:flex; width:180px;height:50px;justify-content:center;align-items:center;border:solid 1px #0094ff;\">立即打开</a>\r\n        </div>\r\n    </div>\r\n\r\n    <script type=\"text/javascript\">\r\n        function getUrlParam(name) {\r\n            let reg = new RegExp(\"(^|&)\" + name + \"=([^&]*)(&|$)\");\r\n            let r = window.location.search.substr(1).match(reg);\r\n            if (r != null) return unescape(r[2]); return null;\r\n        }\r\n        var tick = getUrlParam(\"t\");\r\n        var linkurl = 'weixin://dl/business/?t=' + tick;\r\n        location.href = linkurl;\r\n        document.getElementById(\"xhre\").href = linkurl;\r\n    </script>\r\n\r\n</body>\r\n</html>\r\n\r\n",
                config_type = "Y",
                remark = "短信跳转小程序的中转页面",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

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
