using FluentMigrator;
using System;

namespace IoTVideoService
{
    [Migration(20251211002)]
    public class IoTVideoMigrator : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_iot_video_source");
            Create.Table("mz_iot_video_source").WithDescription("视频源表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("视频源Id")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
.WithColumn("VideoType").AsByte().WithColumnDescription("摄像头类型:0为固定地址,1为GB/T28181")
.WithColumn("VideoUrl").AsString(255).WithColumnDescription("视频预览地址")
.WithColumn("PushAddr").AsString(255).WithColumnDescription("推流地址")
.WithColumn("PullAddr").AsString(255).WithColumnDescription("拉流地址")
.WithColumn("VideoIp").AsString(50).WithColumnDescription("摄像头Ip")
.WithColumn("VideoPort").AsInt32().WithColumnDescription("摄像头端口")
.WithColumn("UserName").AsString(50).WithColumnDescription("用户名")
.WithColumn("UserPwd").AsString(50).WithColumnDescription("密码")
.WithColumn("BitType").AsByte().WithColumnDescription("码流类型：0为主码流，1为子码流")
.WithColumn("FrameInterval").AsInt32().WithDefaultValue(25).WithColumnDescription("AI检测帧间隔,默认25帧")
.WithColumn("PullNode").AsString(50).Indexed().WithColumnDescription("当前拉流的节点名称，无为空");



            Execute.Sql("DROP TABLE IF EXISTS mz_video_task");
            Create.Table("mz_video_task").WithDescription("视频设备的任务表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
.WithColumn("NodeName").AsString(50).WithColumnDescription("所属节点")
.WithColumn("DeviceId").AsString(128).WithColumnDescription("设备Id")
.WithColumn("TaskType").AsByte().WithColumnDescription("任务类型：0为实时AI检测，1为抓拍计划，2为录像计划")
.WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间");


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
