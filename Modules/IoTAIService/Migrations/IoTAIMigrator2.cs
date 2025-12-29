using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.Migrations
{
    [Migration(20251229001)]
    public class IoTAIMigrator2 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("mz_iot_code").Row(new
            {
                Id = 10001,
                Name = "人脸数量",
                Code = "FaceCount",
                CodeGroup = 731,
                OptionData = "{\"description\":\"\",\"option\":{\"type\":\"int\",\"max\":999999999,\"min\":0,\"unit\":\"张\",\"spacing\":0,\"multiple\":1}}",
                CodeType = 0,
                Sort = 1
            });
        }

        public override void Down()
        {
        }

    }
}
