using FluentMigrator;
using System;

namespace ShortLinkService
{
    [Migration(20251209004)]
    public class ShortLinkMigrator : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_menu where menu_id=121");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 121,
                menu_name = "短链接管理",
                parent_id = 1,
                order_num = 13,
                path = "link",
                component = "system/link/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/ShortLinkService/LinkMan/List",
                icon = "baobiaoguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });


            this.Execute.Sql("delete FROM mz_config where config_id=21");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 21,
                config_name = "短信跳转小程序",
                config_key = "wxjmp",
                config_value = "<html>\r\n<head>\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n</head>\r\n<body>\r\n    <style>\r\n        html {\r\n            font-size: 14px;\r\n            background-color: #fff;\r\n        }\r\n\r\n        body {\r\n            background-color: #fff;\r\n        }\r\n    </style>\r\n    <div style=\"padding:150px 16px 0 16px;\">\r\n        <div style=\"padding-bottom: 20px; color: #999;\">提示:无法自动跳转则点击按钮跳转</div>\r\n        <div style=\"display: flex; justify-content: center;\">\r\n            <a id=\"xhre\" href=\"\" style=\"display:flex; width:180px;height:50px;justify-content:center;align-items:center;border:solid 1px #0094ff;\">立即打开</a>\r\n        </div>\r\n    </div>\r\n\r\n    <script type=\"text/javascript\">\r\n        function getUrlParam(name) {\r\n            let reg = new RegExp(\"(^|&)\" + name + \"=([^&]*)(&|$)\");\r\n            let r = window.location.search.substr(1).match(reg);\r\n            if (r != null) return unescape(r[2]); return null;\r\n        }\r\n        var tick = getUrlParam(\"t\");\r\n        var linkurl = 'weixin://dl/business/?t=' + tick;\r\n        location.href = linkurl;\r\n        document.getElementById(\"xhre\").href = linkurl;\r\n    </script>\r\n\r\n</body>\r\n</html>\r\n\r\n",
                config_type = "Y",
                remark = "短信跳转小程序的中转页面",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Execute.Sql("DROP TABLE IF EXISTS mz_short_link");
            Create.Table("mz_short_link").WithDescription("短链接映射表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("映射Id")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
.WithColumn("Url").AsString(500).WithColumnDescription("映射的url地址")
.WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间");
        }
        public override void Down()
        {
        }
    }
}
