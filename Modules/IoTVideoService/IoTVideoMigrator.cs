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
.WithColumn("VideoType").AsByte().WithColumnDescription("摄像头类型:0为固定地址,1为GB28181设备，2为GB28181通道")
.WithColumn("VideoKey").AsString(128).Unique().WithColumnDescription("ZLMediaKit的视频Key")
.WithColumn("PullAddr").AsString(255).WithColumnDescription("拉流地址")
.WithColumn("ChannelId").AsString(50).WithColumnDescription("通道Id")
.WithColumn("UserName").AsString(50).Indexed().WithColumnDescription("用户名")
.WithColumn("UserPwd").AsString(50).WithColumnDescription("密码")
.WithColumn("AITasks").AsString(20000).WithColumnDescription("AI检测任务")
.WithColumn("PullNode").AsString(128).Indexed().WithColumnDescription("节点GUID")
.WithColumn("NodeId").AsString(50).Indexed().WithColumnDescription("服务器节点Id");


            Create.Table("mz_iot_record").WithDescription("录像计划")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id")
                .WithColumn("VideoKey").AsString(128).Indexed().WithColumnDescription("ZLMediaKit的视频Key")
                .WithColumn("SaveCycle").AsInt32().WithDefaultValue(7).WithColumnDescription("录像保存周期（天），默认7天")
                .WithColumn("RecordTimeType").AsString(16).WithColumnDescription("时段类型：week（按周）、custom（自定义时段）、holiday（假期）、work（工作日）")
                .WithColumn("RecordTimeDesc").AsString(512).WithColumnDescription("录像时段描述（如：周一 08:00-18:00）")
                .WithColumn("WeekConfig").AsString(20000).Nullable().WithColumnDescription("按周配置（JSON格式）：[{\"week\":1,\"startTime\":\"08:00:00\",\"endTime\":\"18:00:00\"},...]")
                .WithColumn("CustomStartTime").AsString(8).Nullable().WithColumnDescription("自定义时段开始时间（HH:mm:ss）")
                .WithColumn("CustomEndTime").AsString(8).Nullable().WithColumnDescription("自定义时段结束时间（HH:mm:ss）")
                .WithColumn("Status").AsByte().NotNullable().WithDefaultValue(1).WithColumnDescription("状态：0-禁用，1-启用")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Create.Table("mz_iot_recordlog").WithDescription("录像计划的执行日志")
        .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id")
        .WithColumn("PlanId").AsInt64().WithColumnDescription("关联录像计划ID")
        .WithColumn("VideoId").AsString(128).WithColumnDescription("视频源Id")
        .WithColumn("VideoKey").AsString(128).WithColumnDescription("ZLMediaKit的视频Key")
        .WithColumn("LogType").AsString(16).WithColumnDescription("日志类型：start（计划启动）、stop（计划停止）、key（关键帧）、success（录像成功）、fail（录像失败）、clean（文件清理）")
        .WithColumn("Content").AsString(1024).WithColumnDescription("日志内容（如：录像失败原因、文件清理数量等）")
        .WithColumn("FilePath").AsString(255).WithColumnDescription("关键帧图片路径")
        .WithColumn("ExecTime").AsDateTime().WithColumnDescription("执行时间");

            Create.Index()
    .OnTable("mz_iot_recordlog")
    .OnColumn("PlanId").Ascending()
    .OnColumn("VideoKey").Ascending()
    .OnColumn("LogType").Ascending()
    .WithOptions().NonClustered();

            Create.Index()
.OnTable("mz_iot_recordlog")
.OnColumn("VideoId").Ascending()
.OnColumn("VideoKey").Ascending()
.OnColumn("LogType").Ascending()
.WithOptions().NonClustered();


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
