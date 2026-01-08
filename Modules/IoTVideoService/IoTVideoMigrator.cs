using FluentMigrator;
using System;

namespace IoTVideoService
{
    [Migration(20251229004)]
    public class IoTVideoMigrator : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_menu where menu_id=4502");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 4502,
                menu_name = "视频源",
                parent_id = 4000,
                order_num = 14,
                path = "video/list",
                component = "iot/video/list",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTVideoService/Source/ListPage",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });


            Execute.Sql("DROP TABLE IF EXISTS mz_iot_video_source");
            Create.Table("mz_iot_video_source").WithDescription("视频源表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("视频源Id")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
.WithColumn("Position").AsString(50).WithColumnDescription("视频源位置")
.WithColumn("VideoType").AsByte().WithColumnDescription("摄像头类型:0为固定地址,1为GB28181")
.WithColumn("VideoKey").AsString(128).Unique().WithColumnDescription("ZLMediaKit的视频Key")
.WithColumn("PullAddr").AsString(255).WithColumnDescription("拉流地址")
.WithColumn("UserName").AsString(50).WithColumnDescription("用户名")
.WithColumn("UserPwd").AsString(50).WithColumnDescription("密码")
.WithColumn("GBPublicAddr").AsString(64).WithColumnDescription("GB28181服务的公网主机")
.WithColumn("GBPublicPort").AsInt32().WithColumnDescription("GB28181服务的公网主机端口")
.WithColumn("AITasks").AsString(20000).WithColumnDescription("AI检测任务")
.WithColumn("PullNode").AsString(128).Indexed().WithColumnDescription("节点GUID")
.WithColumn("NodeId").AsString(50).Indexed().WithColumnDescription("服务器节点Id");




            this.Execute.Sql("delete FROM mz_iot_code_group where Id=731");
            Insert.IntoTable("mz_iot_code_group").Row(new
            {
                Id = 731,
                Name = "智能摄像头",
                ParentId = 1,
                Sort = 11
            });

        }
        public override void Down()
        {
        }
    }
}
