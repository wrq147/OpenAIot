using FluentMigrator;
using System;

namespace MonitorService
{
    [Migration(20220000002)]
    public class MonitorMigrator : Migration
    {
        public override void Up()
        {
            Create.Table("mz_job").WithDescription("定时任务调度表")
.WithColumn("job_id").AsInt64().PrimaryKey().Identity().WithColumnDescription("任务ID")
.WithColumn("job_name").AsString(64).WithColumnDescription("任务名称")
.WithColumn("job_group").AsString(64).WithColumnDescription("任务组名")
.WithColumn("invoke_target").AsString(500).WithColumnDescription("调用目标字符串,为url则直接被get调用")
.WithColumn("cron_expression").AsString(255).WithColumnDescription("cron执行表达式")
.WithColumn("misfire_policy").AsString(20).WithColumnDescription("计划执行错误策略（1立即执行 2执行一次 3放弃执行）")
.WithColumn("concurrent").AsFixedLengthAnsiString(1).WithColumnDescription("是否并发执行（0允许 1禁止）")
.WithColumn("status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0正常 1暂停）")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间")
        .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
        .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id");
            Create.Index("MZJOB_IDX").OnTable("mz_job").OnColumn("job_name").Ascending().OnColumn("job_group").Ascending();

            Create.Table("mz_job_log").WithDescription("定时任务调度日志表")
.WithColumn("job_log_id").AsInt64().PrimaryKey().Identity().WithColumnDescription("任务日志ID")
.WithColumn("job_name").AsString(64).WithColumnDescription("任务名称")
.WithColumn("job_group").AsString(64).WithColumnDescription("任务组名")
.WithColumn("invoke_target").AsString(500).WithColumnDescription("调用目标字符串")
.WithColumn("job_message").AsString(500).WithColumnDescription("日志信息")
.WithColumn("status").AsFixedLengthAnsiString(1).WithColumnDescription("执行状态（0正常 1失败）")
.WithColumn("exception_info").AsString(2000).WithColumnDescription("异常信息")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间");

            Create.Table("mz_oper_log").WithDescription("操作日志记录表")
.WithColumn("oper_id").AsInt64().PrimaryKey().Identity().WithColumnDescription("日志主键")
.WithColumn("title").AsString(50).WithColumnDescription("模块标题")
.WithColumn("method").AsString(100).WithColumnDescription("方法名称")
.WithColumn("request_method").AsString(20).WithColumnDescription("请求方式")
.WithColumn("operator_type").AsString(20).WithColumnDescription("操作类别（pc weixin android ios）")
.WithColumn("oper_name").AsString(50).WithColumnDescription("操作人员")
.WithColumn("oper_uid").AsInt64().WithColumnDescription("操作人员ID")
.WithColumn("oper_org").AsInt64().WithColumnDescription("操作人员所属组织")
.WithColumn("oper_url").AsString(255).WithColumnDescription("请求URL")
.WithColumn("oper_ip").AsString(128).WithColumnDescription("主机地址")
.WithColumn("oper_location").AsString(255).WithColumnDescription("操作地点")
.WithColumn("oper_param").AsString(2000).WithColumnDescription("请求参数")
.WithColumn("json_result").AsString(3000).WithColumnDescription("返回参数")
.WithColumn("status").AsInt32().WithColumnDescription("操作状态（0正常 1异常）")
.WithColumn("error_msg").AsString(2000).WithColumnDescription("错误消息")
.WithColumn("oper_time").AsDateTime().WithColumnDescription("操作时间");


            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 2,
                menu_name = "系统监控",
                parent_id = 0,
                order_num = 2,
                path = "monitor",
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "M",
                visible = "0",
                status = "0",
                perms = "/SysMonitor/",
                icon = "monitor",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 109,
                menu_name = "在线用户",
                parent_id = 2,
                order_num = 1,
                path = "online",
                component = "monitor/online/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MonitorService/Online/",
                icon = "online",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 110,
                menu_name = "定时任务",
                parent_id = 2,
                order_num = 2,
                path = "job",
                component = "monitor/job/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MonitorService/Job/",
                icon = "job",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 112,
                menu_name = "服务监控",
                parent_id = 2,
                order_num = 4,
                path = "server",
                component = "monitor/server/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MonitorService/Server/",
                icon = "server",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 113,
                menu_name = "缓存监控",
                parent_id = 2,
                order_num = 5,
                path = "cache",
                component = "monitor/cache/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MonitorService/Cache/",
                icon = "redis",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 500,
                menu_name = "操作日志",
                parent_id = 2,
                order_num = 6,
                path = "operlog",
                component = "monitor/operlog/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/MonitorService/OperLog",
                icon = "form",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 501,
                menu_name = "登录日志",
                parent_id = 2,
                order_num = 7,
                path = "logininfor",
                component = "monitor/logininfor/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/AuthService/LoginiLog",
                icon = "logininfor",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1040,
                menu_name = "操作查询",
                parent_id = 500,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/MonitorService/OperLog/List",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1041,
                menu_name = "操作删除",
                parent_id = 500,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/MonitorService/OperLog/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1042,
                menu_name = "日志导出",
                parent_id = 500,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/MonitorService/OperLog/Export",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1043,
                menu_name = "登录查询",
                parent_id = 501,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/LoginiLog/List",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1044,
                menu_name = "登录删除",
                parent_id = 501,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/LoginiLog/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1045,
                menu_name = "日志导出",
                parent_id = 501,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/LoginiLog/Export",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1046,
                menu_name = "在线查询",
                parent_id = 109,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/MonitorService/Online/List",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1048,
                menu_name = "强退用户",
                parent_id = 109,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/MonitorService/Online/ForceLogout",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1049,
                menu_name = "任务查询",
                parent_id = 110,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/MonitorService/Job/List",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1050,
                menu_name = "任务新增",
                parent_id = 110,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/MonitorService/Job/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1051,
                menu_name = "任务修改",
                parent_id = 110,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/MonitorService/Job/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1052,
                menu_name = "任务删除",
                parent_id = 110,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/MonitorService/Job/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1053,
                menu_name = "状态修改",
                parent_id = 110,
                order_num = 5,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/MonitorService/Job/ChangeStatus",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1054,
                menu_name = "任务导出",
                parent_id = 110,
                order_num = 6,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/MonitorService/Job/Export",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });



            // 创建QRTZ_TRIGGERS表
            if (Schema.Table("QRTZ_TRIGGERS").Exists())
            {
                Delete.Table("QRTZ_TRIGGERS");
            }
            Create.Table("QRTZ_TRIGGERS")
               .WithColumn("SCHED_NAME").AsString(120).PrimaryKey().WithColumnDescription("调度名称").NotNullable()
               .WithColumn("TRIGGER_NAME").AsString(200).PrimaryKey().WithColumnDescription("触发器的名字").NotNullable()
               .WithColumn("TRIGGER_GROUP").AsString(200).PrimaryKey().WithColumnDescription("触发器所属组的名字").NotNullable()
               .WithColumn("JOB_NAME").AsString(200).WithColumnDescription("QRTZ_JOB_DETAILS表job_name的外键").NotNullable()
               .WithColumn("JOB_GROUP").AsString(200).WithColumnDescription("QRTZ_JOB_DETAILS表job_group的外键").NotNullable()
               .WithColumn("DESCRIPTION").AsString(250).WithColumnDescription("相关介绍").Nullable()
               .WithColumn("NEXT_FIRE_TIME").AsInt64().WithColumnDescription("上一次触发时间（毫秒）").Nullable()
               .WithColumn("PREV_FIRE_TIME").AsInt64().WithColumnDescription("下一次触发时间（默认为-1表示不触发）").Nullable()
               .WithColumn("PRIORITY").AsInt32().WithColumnDescription("优先级").Nullable()
               .WithColumn("TRIGGER_STATE").AsString(16).WithColumnDescription("触发器状态").NotNullable()
               .WithColumn("TRIGGER_TYPE").AsString(8).WithColumnDescription("触发器的类型").NotNullable()
               .WithColumn("START_TIME").AsInt64().WithColumnDescription("开始时间").NotNullable()
               .WithColumn("END_TIME").AsInt64().WithColumnDescription("结束时间").Nullable()
               .WithColumn("CALENDAR_NAME").AsString(200).WithColumnDescription("日程表名称").Nullable()
               .WithColumn("MISFIRE_INSTR").AsInt16().WithColumnDescription("补偿执行的策略").Nullable()
               .WithColumn("JOB_DATA").AsBinary().WithColumnDescription("存放持久化job对象").Nullable();

            // 创建QRTZ_BLOB_TRIGGERS表
            if (Schema.Table("QRTZ_BLOB_TRIGGERS").Exists())
            {
                Delete.Table("QRTZ_BLOB_TRIGGERS");
            }
            Create.Table("QRTZ_BLOB_TRIGGERS")
               .WithColumn("SCHED_NAME").AsString(120).PrimaryKey().NotNullable()
               .WithColumn("TRIGGER_NAME").AsString(200).PrimaryKey().NotNullable()
               .WithColumn("TRIGGER_GROUP").AsString(200).PrimaryKey().NotNullable()
               .WithColumn("BLOB_DATA").AsBinary().NotNullable();

            Create.Index("SCHED_NAME").OnTable("QRTZ_BLOB_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("TRIGGER_NAME").Ascending().OnColumn("TRIGGER_GROUP").Ascending();
            Create.ForeignKey("qrtz_blob_triggers_ibfk_1").FromTable("QRTZ_BLOB_TRIGGERS")
                .ForeignColumns("SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP").ToTable("QRTZ_TRIGGERS").PrimaryColumns("SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP");

            // 创建QRTZ_CALENDARS表
            if (Schema.Table("QRTZ_CALENDARS").Exists())
            {
                Delete.Table("QRTZ_CALENDARS");
            }
            Create.Table("QRTZ_CALENDARS")
               .WithColumn("SCHED_NAME").AsString(120).PrimaryKey().NotNullable()
               .WithColumn("CALENDAR_NAME").AsString(200).PrimaryKey().NotNullable()
               .WithColumn("CALENDAR").AsBinary().NotNullable();


            // 创建QRTZ_CRON_TRIGGERS表
            if (Schema.Table("QRTZ_CRON_TRIGGERS").Exists())
            {
                Delete.Table("QRTZ_CRON_TRIGGERS");
            }
            Create.Table("QRTZ_CRON_TRIGGERS")
               .WithColumn("SCHED_NAME").AsString(120).PrimaryKey().NotNullable()
               .WithColumn("TRIGGER_NAME").AsString(200).PrimaryKey().NotNullable()
               .WithColumn("TRIGGER_GROUP").AsString(200).PrimaryKey().NotNullable()
               .WithColumn("CRON_EXPRESSION").AsString(120).NotNullable()
               .WithColumn("TIME_ZONE_ID").AsString(80).Nullable();
            Create.ForeignKey("qrtz_cron_triggers_ibfk_1").FromTable("QRTZ_BLOB_TRIGGERS")
    .ForeignColumns("SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP").ToTable("QRTZ_TRIGGERS").PrimaryColumns("SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP");


            // 创建QRTZ_FIRED_TRIGGERS表
            if (Schema.Table("QRTZ_FIRED_TRIGGERS").Exists())
            {
                Delete.Table("QRTZ_FIRED_TRIGGERS");
            }
            Create.Table("QRTZ_FIRED_TRIGGERS")
               .WithColumn("SCHED_NAME").AsString(120).PrimaryKey().NotNullable()
               .WithColumn("ENTRY_ID").AsString(140).PrimaryKey().NotNullable()
               .WithColumn("TRIGGER_NAME").AsString(200).NotNullable()
               .WithColumn("TRIGGER_GROUP").AsString(200).NotNullable()
               .WithColumn("INSTANCE_NAME").AsString(200).NotNullable()
               .WithColumn("FIRED_TIME").AsInt64().NotNullable()
               .WithColumn("SCHED_TIME").AsInt64().NotNullable()
               .WithColumn("PRIORITY").AsInt32().NotNullable()
               .WithColumn("STATE").AsString(16).NotNullable()
               .WithColumn("JOB_NAME").AsString(200).Nullable()
               .WithColumn("JOB_GROUP").AsString(200).Nullable()
               .WithColumn("IS_NONCONCURRENT").AsBoolean().Nullable()
               .WithColumn("REQUESTS_RECOVERY").AsBoolean().Nullable();
            Create.Index("IDX_QRTZ_FT_TRIG_INST_NAME").OnTable("QRTZ_FIRED_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("INSTANCE_NAME").Ascending();
            Create.Index("IDX_QRTZ_FT_INST_JOB_REQ_RCVRY").OnTable("QRTZ_FIRED_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("INSTANCE_NAME").Ascending().OnColumn("REQUESTS_RECOVERY").Ascending();
            Create.Index("IDX_QRTZ_FT_J_G").OnTable("QRTZ_FIRED_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("JOB_NAME").Ascending().OnColumn("JOB_GROUP").Ascending();
            Create.Index("IDX_QRTZ_FT_JG").OnTable("QRTZ_FIRED_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("JOB_GROUP").Ascending();
            Create.Index("IDX_QRTZ_FT_T_G").OnTable("QRTZ_FIRED_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("TRIGGER_NAME").Ascending().OnColumn("TRIGGER_GROUP").Ascending();
            Create.Index("IDX_QRTZ_FT_TG").OnTable("QRTZ_FIRED_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("TRIGGER_GROUP").Ascending();

            // 创建QRTZ_JOB_DETAILS表
            if (Schema.Table("QRTZ_JOB_DETAILS").Exists())
            {
                Delete.Table("QRTZ_JOB_DETAILS");
            }
            Create.Table("QRTZ_JOB_DETAILS")
               .WithColumn("SCHED_NAME").AsString(120).PrimaryKey().NotNullable().WithColumnDescription("调度名称")
               .WithColumn("JOB_NAME").AsString(200).PrimaryKey().NotNullable().WithColumnDescription("任务名称")
               .WithColumn("JOB_GROUP").AsString(200).PrimaryKey().NotNullable().WithColumnDescription("任务组名")
               .WithColumn("DESCRIPTION").AsString(250).Nullable().WithColumnDescription("相关介绍")
               .WithColumn("JOB_CLASS_NAME").AsString(250).NotNullable().WithColumnDescription("执行任务类名称")
               .WithColumn("IS_DURABLE").AsBoolean().NotNullable().WithColumnDescription("是否持久化")
               .WithColumn("IS_NONCONCURRENT").AsBoolean().NotNullable().WithColumnDescription("是否并发")
               .WithColumn("IS_UPDATE_DATA").AsBoolean().NotNullable().WithColumnDescription("是否更新数据")
               .WithColumn("REQUESTS_RECOVERY").AsBoolean().NotNullable().WithColumnDescription("是否接受恢复执行")
               .WithColumn("JOB_DATA").AsBinary().Nullable().WithColumnDescription("存放持久化job对象");

            Create.Index("IDX_QRTZ_J_REQ_RECOVERY").OnTable("QRTZ_JOB_DETAILS").OnColumn("SCHED_NAME").Ascending().OnColumn("REQUESTS_RECOVERY").Ascending();
            Create.Index("IDX_QRTZ_J_GRP").OnTable("QRTZ_JOB_DETAILS").OnColumn("SCHED_NAME").Ascending().OnColumn("JOB_GROUP").Ascending();

            // 创建QRTZ_LOCKS表
            if (Schema.Table("QRTZ_LOCKS").Exists())
            {
                Delete.Table("QRTZ_LOCKS");
            }
            Create.Table("QRTZ_LOCKS")
               .WithColumn("SCHED_NAME").AsString(120).PrimaryKey().NotNullable()
               .WithColumn("LOCK_NAME").AsString(40).PrimaryKey().NotNullable();

            // 创建QRTZ_PAUSED_TRIGGER_GRPS表
            if (Schema.Table("QRTZ_PAUSED_TRIGGER_GRPS").Exists())
            {
                Delete.Table("QRTZ_PAUSED_TRIGGER_GRPS");
            }
            Create.Table("QRTZ_PAUSED_TRIGGER_GRPS")
               .WithColumn("SCHED_NAME").AsString(120).PrimaryKey().NotNullable()
               .WithColumn("TRIGGER_GROUP").AsString(200).PrimaryKey().NotNullable();


            // 创建QRTZ_SCHEDULER_STATE表
            if (Schema.Table("QRTZ_SCHEDULER_STATE").Exists())
            {
                Delete.Table("QRTZ_SCHEDULER_STATE");
            }
            Create.Table("QRTZ_SCHEDULER_STATE")
               .WithColumn("SCHED_NAME").AsString(120).PrimaryKey().NotNullable()
               .WithColumn("INSTANCE_NAME").AsString(200).PrimaryKey().NotNullable()
               .WithColumn("LAST_CHECKIN_TIME").AsInt64().NotNullable()
               .WithColumn("CHECKIN_INTERVAL").AsInt64().NotNullable();


            // 创建QRTZ_SIMPLE_TRIGGERS表
            if (Schema.Table("QRTZ_SIMPLE_TRIGGERS").Exists())
            {
                Delete.Table("QRTZ_SIMPLE_TRIGGERS");
            }
            Create.Table("QRTZ_SIMPLE_TRIGGERS")
               .WithColumn("SCHED_NAME").AsString(120).PrimaryKey().NotNullable()
               .WithColumn("TRIGGER_NAME").AsString(200).PrimaryKey().NotNullable()
               .WithColumn("TRIGGER_GROUP").AsString(200).PrimaryKey().NotNullable()
               .WithColumn("REPEAT_COUNT").AsInt64().NotNullable()
               .WithColumn("REPEAT_INTERVAL").AsInt64().NotNullable()
               .WithColumn("TIMES_TRIGGERED").AsInt64().NotNullable();

            Create.ForeignKey("qrtz_simple_triggers_ibfk_1").FromTable("QRTZ_SIMPLE_TRIGGERS")
.ForeignColumns("SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP").ToTable("QRTZ_TRIGGERS").PrimaryColumns("SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP");

            // 创建QRTZ_SIMPROP_TRIGGERS表
            if (Schema.Table("QRTZ_SIMPROP_TRIGGERS").Exists())
            {
                Delete.Table("QRTZ_SIMPROP_TRIGGERS");
            }
            Create.Table("QRTZ_SIMPROP_TRIGGERS")
               .WithColumn("SCHED_NAME").AsString(120).PrimaryKey().NotNullable()
               .WithColumn("TRIGGER_NAME").AsString(200).PrimaryKey().NotNullable()
               .WithColumn("TRIGGER_GROUP").AsString(200).PrimaryKey().NotNullable()
               .WithColumn("STR_PROP_1").AsString(512).Nullable()
               .WithColumn("STR_PROP_2").AsString(512).Nullable()
               .WithColumn("STR_PROP_3").AsString(512).Nullable()
               .WithColumn("INT_PROP_1").AsInt32().Nullable()
               .WithColumn("INT_PROP_2").AsInt32().Nullable()
               .WithColumn("LONG_PROP_1").AsInt64().Nullable()
               .WithColumn("LONG_PROP_2").AsInt64().Nullable()
               .WithColumn("DEC_PROP_1").AsDecimal(13, 4).Nullable()
               .WithColumn("DEC_PROP_2").AsDecimal(13, 4).Nullable()
               .WithColumn("BOOL_PROP_1").AsBoolean().Nullable()
               .WithColumn("BOOL_PROP_2").AsBoolean().Nullable()
               .WithColumn("TIME_ZONE_ID").AsString(80).Nullable();


            Create.ForeignKey("qrtz_simprop_triggers_ibfk_1").FromTable("QRTZ_SIMPROP_TRIGGERS")
.ForeignColumns("SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP").ToTable("QRTZ_TRIGGERS").PrimaryColumns("SCHED_NAME", "TRIGGER_NAME", "TRIGGER_GROUP");




            Create.Index("IDX_QRTZ_T_J").OnTable("QRTZ_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("JOB_NAME").Ascending().OnColumn("JOB_GROUP").Ascending();
            Create.Index("IDX_QRTZ_T_JG").OnTable("QRTZ_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("JOB_GROUP").Ascending();
            Create.Index("IDX_QRTZ_T_C").OnTable("QRTZ_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("CALENDAR_NAME").Ascending();
            Create.Index("IDX_QRTZ_T_G").OnTable("QRTZ_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("TRIGGER_GROUP").Ascending();
            Create.Index("IDX_QRTZ_T_STATE").OnTable("QRTZ_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("TRIGGER_STATE").Ascending();
            Create.Index("IDX_QRTZ_T_N_STATE").OnTable("QRTZ_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("TRIGGER_NAME").Ascending().OnColumn("TRIGGER_GROUP").Ascending().OnColumn("TRIGGER_STATE").Ascending();
            Create.Index("IDX_QRTZ_T_N_G_STATE").OnTable("QRTZ_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("TRIGGER_GROUP").Ascending().OnColumn("TRIGGER_STATE").Ascending();
            Create.Index("IDX_QRTZ_T_NEXT_FIRE_TIME").OnTable("QRTZ_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("NEXT_FIRE_TIME").Ascending();
            Create.Index("IDX_QRTZ_T_NFT_ST").OnTable("QRTZ_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("TRIGGER_STATE").Ascending().OnColumn("NEXT_FIRE_TIME").Ascending();
            Create.Index("IDX_QRTZ_T_NFT_MISFIRE").OnTable("QRTZ_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("MISFIRE_INSTR").Ascending().OnColumn("NEXT_FIRE_TIME").Ascending();
            Create.Index("IDX_QRTZ_T_NFT_ST_MISFIRE").OnTable("QRTZ_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("MISFIRE_INSTR").Ascending().OnColumn("NEXT_FIRE_TIME").Ascending().OnColumn("TRIGGER_STATE").Ascending();
            Create.Index("IDX_QRTZ_T_NFT_ST_MISFIRE_GRP").OnTable("QRTZ_TRIGGERS").OnColumn("SCHED_NAME").Ascending().OnColumn("MISFIRE_INSTR").Ascending().OnColumn("NEXT_FIRE_TIME").Ascending().OnColumn("TRIGGER_GROUP").Ascending().OnColumn("TRIGGER_STATE").Ascending();

            Create.ForeignKey("qrtz_triggers_ibfk_1").FromTable("QRTZ_TRIGGERS")
.ForeignColumns("SCHED_NAME", "JOB_NAME", "JOB_GROUP").ToTable("QRTZ_JOB_DETAILS").PrimaryColumns("SCHED_NAME", "JOB_NAME", "JOB_GROUP");

        }
        public override void Down()
        {
        }

    }
}
