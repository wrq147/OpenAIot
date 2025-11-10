using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdPartyService
{
    [Migration(20250507001)]
    public class ThirdMigrator : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_config where config_id=5000");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 5000,
                config_name = "天气接口源",
                config_key = "weather.from",
                config_value = "高德",
                config_type = "Y",
                remark = "天气用接口",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            this.Execute.Sql("delete FROM mz_config where config_id=5001");
            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 5001,
                config_name = "天气接口配置信息",
                config_key = "weather.option",
                config_value = "{\"key\":\"e731dd6b802e2d7ca63d85691d4e50e6\"}",
                config_type = "Y",
                remark = "天气用接口",
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
