using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.Migrations
{
    [Migration(20260307001)]
    public class IoTAIMigrator2 : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_iot_code where Id=10001");
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

            Insert.IntoTable("mz_iot_code").Row(new
            {
                Id = 10002,
                Name = "陌生人闯入",
                Code = "UnkIn",
                CodeGroup = 731,
                OptionData = "{\"SilenceTime\":0,\"description\":\"\",\"outputs\":[{\"name\":\"人脸图\",\"code\":\"face_img\",\"type\":\"file\",\"isimg\":true,\"bodyType\":\"base64\"}]}",
                CodeType = 2,
                Sort = 1
            });
        }

        public override void Down()
        {
        }

    }
}
