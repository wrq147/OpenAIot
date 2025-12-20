using FluentMigrator;
using System;

namespace IoTVideoService
{
    [Migration(20251219003)]
    public class IoTVideoMigrator : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_iot_video_source");
            Create.Table("mz_iot_video_source").WithDescription("视频源表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("视频源Id")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
.WithColumn("VideoType").AsByte().WithColumnDescription("摄像头类型:0为固定地址,1为GB/T28181")
.WithColumn("VideoKey").AsString(128).Unique().WithColumnDescription("ZLMediaKit的视频Key")
.WithColumn("PullAddr").AsString(255).WithColumnDescription("拉流地址")
.WithColumn("UserName").AsString(50).WithColumnDescription("用户名")
.WithColumn("UserPwd").AsString(50).WithColumnDescription("密码")
.WithColumn("BitType").AsByte().WithColumnDescription("码流类型：0为主码流，1为子码流")
.WithColumn("FrameInterval").AsInt32().WithDefaultValue(25).WithColumnDescription("AI检测帧间隔,默认25帧")
.WithColumn("AIParams").AsString(20000).WithColumnDescription("AI检测参数")
.WithColumn("PullNode").AsString(50).Indexed().WithColumnDescription("当前拉流的节点名称，无为空");




            this.Execute.Sql("delete FROM mz_iot_code_group where Id=731");
            Insert.IntoTable("mz_iot_code_group").Row(new
            {
                Id = 731,
                Name = "智能摄像头",
                ParentId = 3,
                Sort = 11
            });

        }
        public override void Down()
        {
        }
    }
}
