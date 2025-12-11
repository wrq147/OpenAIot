using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Migrations
{
    [Migration(20251211001)]
    public class IotMigratorA1 : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_iot_device_video");
            Create.Table("mz_iot_device_video").WithDescription("视频设备的信息")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("设备Id")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
.WithColumn("VideoType").AsByte().WithColumnDescription("摄像头类型:0为固定地址,1为GB/T28181")
.WithColumn("PullAddr").AsString(255).WithColumnDescription("拉流地址")
.WithColumn("VideoIp").AsString(50).WithColumnDescription("摄像头Ip")
.WithColumn("VideoPort").AsInt32().WithColumnDescription("摄像头端口")
.WithColumn("UserName").AsString(50).WithColumnDescription("用户名")
.WithColumn("UserPwd").AsString(50).WithColumnDescription("密码")
.WithColumn("BitType").AsByte().WithColumnDescription("码流类型：0为主码流，1为子码流");


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
