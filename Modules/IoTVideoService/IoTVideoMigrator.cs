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
            this.Execute.Sql("delete FROM mz_menu where menu_id=4503");
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
            }).Row(new
            {
                menu_id = 4503,
                menu_name = "录像计划",
                parent_id = 4000,
                order_num = 15,
                path = "video/recordlist",
                component = "iot/video/recordlist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTVideoService/Record/ListPage",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 4504,
                menu_name = "视频策略",
                parent_id = 4000,
                order_num = 16,
                path = "video/conflist",
                component = "iot/video/conflist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTVideoService/Conf/ListPage",
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
.WithColumn("VideoType").AsByte().WithColumnDescription("摄像头类型:0为固定地址,1为GB28181设备，2为通道，3为Onvif设备")
.WithColumn("VideoKey").AsString(128).Unique().WithColumnDescription("ZLMediaKit的视频Key")
.WithColumn("PullAddr").AsString(255).WithColumnDescription("拉流地址")
.WithColumn("UserName").AsString(50).Indexed().WithColumnDescription("用户名")
.WithColumn("UserPwd").AsString(50).WithColumnDescription("密码")
.WithColumn("ConfigId").AsString(128).Indexed().Nullable().WithColumnDescription("视频策略Id")
.WithColumn("NodeId").AsString(50).Indexed().WithColumnDescription("服务器节点Id");


            Execute.Sql("DROP TABLE IF EXISTS mz_iot_video_config");
            Create.Table("mz_iot_video_config").WithDescription("视频策略表")
            .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("策略Id")
            .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
            .WithColumn("Name").AsString(50).WithColumnDescription("策略名称")
            .WithColumn("AITasks").AsString(20000).WithColumnDescription("AI检测任务")
            .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
            .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
            .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
            .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间"); ;


            Execute.Sql("DROP TABLE IF EXISTS mz_iot_record");
            Create.Table("mz_iot_record").WithDescription("录像计划")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
                .WithColumn("PlanName").AsString(50).WithColumnDescription("计划名称")
                .WithColumn("VideoId").AsString(128).Unique().WithColumnDescription("视频源Id")
                .WithColumn("Position").AsString(50).WithColumnDescription("视频源位置")
                .WithColumn("StorageWay").AsByte().WithColumnDescription("0为文件存储，1为云存储")
                .WithColumn("SaveCycle").AsInt32().WithDefaultValue(7).WithColumnDescription("录像保存周期（天），默认7天")
                .WithColumn("RecordTimeType").AsString(16).WithColumnDescription("时段类型：week（按周）、time（按时段）")
                .WithColumn("RecordTimeDesc").AsString(512).WithColumnDescription("录像时段描述（如：周一 08:00-18:00）")
                .WithColumn("WeekConfig").AsString(20000).Nullable().WithColumnDescription("按周配置（JSON格式）：[{\"week\":1,\"Time\":8,\"Op\":\"Start\"},...]")
                .WithColumn("TimeConfig").AsString(20000).Nullable().WithColumnDescription("按时段配置（JSON格式）：[{\"Time\":8,\"Op\":\"Start\"},{\"Time\":18,\"Op\":\"End\"},...]")
                .WithColumn("Status").AsByte().NotNullable().WithDefaultValue(1).WithColumnDescription("状态：0-禁用，1-启用")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Execute.Sql("DROP TABLE IF EXISTS mz_iot_recordlog");
            Create.Table("mz_iot_recordlog").WithDescription("录像计划的执行日志")
        .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id")
        .WithColumn("PlanId").AsString(128).WithColumnDescription("关联录像计划ID")
        .WithColumn("VideoId").AsString(128).WithColumnDescription("视频源Id")
        .WithColumn("Position").AsString(50).WithColumnDescription("视频源位置")
        .WithColumn("LogType").AsString(16).WithColumnDescription("日志类型：start（录像启动）、stop（录像停止）、fail（录像失败）、clean（文件清理）")
        .WithColumn("Content").AsString(1024).WithColumnDescription("日志内容（如：录像失败原因、文件清理数量等）")
        .WithColumn("ExecTime").AsDateTime().WithColumnDescription("执行时间");

            Create.Index()
    .OnTable("mz_iot_recordlog")
    .OnColumn("PlanId").Ascending()
    .OnColumn("LogType").Ascending()
    .WithOptions().NonClustered();

            Create.Index()
.OnTable("mz_iot_recordlog")
.OnColumn("VideoId").Ascending()
.OnColumn("LogType").Ascending()
.WithOptions().NonClustered();

            Execute.Sql("DROP TABLE IF EXISTS mz_iot_record_file");
            Create.Table("mz_iot_record_file").WithDescription("录像播放文件信息")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id")
                .WithColumn("FileDate").AsDateTime().WithColumnDescription("记录的日期")
                .WithColumn("FileName").AsString(128).WithColumnDescription("文件名")
                .WithColumn("FileSize").AsFloat().WithColumnDescription("文件大小，单位MB")
                .WithColumn("PlanId").AsString(128).WithColumnDescription("关联录像计划ID")
                .WithColumn("VideoId").AsString(128).Indexed().WithColumnDescription("视频源Id")
                .WithColumn("VideoKey").AsString(128).Indexed().WithColumnDescription("ZLMediaKit的视频Key")
                .WithColumn("NodeId").AsString(128).Indexed().WithColumnDescription("节点Id")
                .WithColumn("SaveType").AsByte().WithColumnDescription("0为mp4、1为hls")
                .WithColumn("StorageWay").AsByte().WithColumnDescription("0为文件存储，1为云存储")
                .WithColumn("StartTime").AsDateTime().WithColumnDescription("开始时间")
                .WithColumn("EndTime").AsDateTime().Nullable().WithColumnDescription("结束时间");

            Create.Index()
.OnTable("mz_iot_record_file")
.OnColumn("PlanId").Ascending()
.OnColumn("VideoKey").Ascending()
.WithOptions().NonClustered();


            Execute.Sql("DROP TABLE IF EXISTS mz_iot_record_key");
            Create.Table("mz_iot_record_key").WithDescription("录像播放文件的关键帧")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id")
                .WithColumn("VideoKey").AsString(128).Indexed().WithColumnDescription("ZLMediaKit的视频Key")
                .WithColumn("KeyDate").AsDateTime().WithColumnDescription("记录的日期")
                .WithColumn("EvtDes").AsString(500).WithColumnDescription("关键帧事件描述")
                .WithColumn("FilePath").AsString(255).WithColumnDescription("关键帧图片路径");



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
