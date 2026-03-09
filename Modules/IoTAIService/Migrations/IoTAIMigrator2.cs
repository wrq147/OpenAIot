using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.Migrations
{
    [Migration(20260309001)]
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

            this.Execute.Sql("delete FROM mz_iot_code where Id=10002");
            Insert.IntoTable("mz_iot_code").Row(new
            {
                Id = 10002,
                Name = "陌生人闯入",
                Code = "UnkIn",
                CodeGroup = 731,
                OptionData = "{\"SilenceTime\":0,\"description\":\"\",\"outputs\":[{\"name\":\"抓拍图\",\"code\":\"face_img\",\"type\":\"file\",\"bodyType\":\"base64\"}]}",
                CodeType = 2,
                Sort = 1
            });

            this.Execute.Sql("delete FROM mz_iot_code where Id=10003");
            Insert.IntoTable("mz_iot_code").Row(new
            {
                Id = 10003,
                Name = "熟人闯入",
                Code = "KnwIn",
                CodeGroup = 731,
                OptionData = "{\"SilenceTime\":0,\"description\":\"\",\"outputs\":[{\"name\":\"抓拍图\",\"code\":\"face_img\",\"type\":\"file\",\"bodyType\":\"base64\"},{\"name\":\"闯入者\",\"code\":\"face_name\",\"type\":\"string\"},{\"name\":\"闯入者Id\",\"code\":\"name_id\",\"type\":\"string\"}]}",
                CodeType = 2,
                Sort = 1
            });

            this.Execute.Sql("delete FROM mz_iot_code where Id=10004");
            Insert.IntoTable("mz_iot_code").Row(new
            {
                Id = 10004,
                Name = "物品闯入",
                Code = "ItemIn",
                CodeGroup = 731,
                OptionData = "{\"SilenceTime\":0,\"description\":\"\",\"outputs\":[{\"name\":\"抓拍图\",\"code\":\"item_img\",\"type\":\"file\",\"bodyType\":\"base64\"}]}",
                CodeType = 2,
                Sort = 1
            });
        }

        public override void Down()
        {
        }

    }
}
