using FluentMigrator;
using System;
namespace FlowService
{
    [Migration(20220615001)]
    public class FlowMigrator : Migration
    {
        public override void Up()
        {
            Create.Table("mz_flow").WithDescription("工作流实例表")
       .WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("工作流实例ID")
       .WithColumn("FlowNumber").AsString(50).Indexed().WithColumnDescription("流程第三方编号或关联工单")
       .WithColumn("OrgId").AsInt64().Indexed("FlowOrgId").WithColumnDescription("关联组织ID")
       .WithColumn("GroupId").AsInt64().WithColumnDescription("分组Id")
       .WithColumn("FlowName").AsString(50).WithColumnDescription("流程实例名称")
       .WithColumn("Description").AsString().WithColumnDescription("流程描述")
       .WithColumn("PersistenceData").AsString(20000).WithColumnDescription("流程实例持久化")
       .WithColumn("FormFields").AsString(20000).WithColumnDescription("流程表单持久化")
       .WithColumn("Assign").AsString(20000).WithColumnDescription("保存自选审核人节点数据")
       .WithColumn("FinishTime").AsDateTime().Nullable().WithColumnDescription("完成或终止时间")
       .WithColumn("Status").AsInt32().WithColumnDescription("0为运行、1为保存中、2为完成、3取消")
       .WithColumn("notify").AsString(500).WithColumnDescription("消息通知方式(json内容)")
       .WithColumn("TemplateId").AsInt64().Indexed().WithColumnDescription("对应的流程模板Id")
       .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
       .WithColumn("IsEmbed").AsBoolean().WithColumnDescription("是否为嵌入式流程")
       .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
       .WithColumn("create_time").AsDateTime().Nullable().WithColumnDescription("创建时间")
       .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
       .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Create.Table("mz_flow_query").WithDescription("流程表单请求参数")
.WithColumn("FlowId").AsInt64().PrimaryKey()
.WithColumn("Name").AsString().PrimaryKey().WithColumnDescription("请求参数名")
.WithColumn("Value").AsString(500).WithColumnDescription("请求参数值")
.WithColumn("TemplateId").AsInt64().WithColumnDescription("对应的流程模板Id");

            Create.Index("IDXFlowQueryNV").OnTable("mz_flow_query").OnColumn("Name").Ascending().OnColumn("Value").Ascending();

            Create.Table("mz_flow_value").WithDescription("流程表单数据")
.WithColumn("FlowId").AsInt64().PrimaryKey()
.WithColumn("FieldId").AsString().PrimaryKey()
.WithColumn("LongValue").AsString(50000).Nullable().WithColumnDescription("保存长文本数据")
.WithColumn("Value").AsString(500).Indexed("FLOW_VALUE_IDX").Nullable().WithColumnDescription("json类型（该表单项的值）")
.WithColumn("NumberValue").AsDouble().Indexed("FLOW_NUMBER_IDX").Nullable().WithColumnDescription("数值类型（该表单项的值）");
            Create.Index("IDXFlowIdxxNV").OnTable("mz_flow_value").OnColumn("FlowId").Ascending();

            Create.Table("mz_flow_group").WithDescription("流程模板的分组信息")
    .WithColumn("Id").AsInt64().PrimaryKey().Identity()
    .WithColumn("OrgId").AsInt64().Indexed("FlowGroupOrgId").WithColumnDescription("关联组织ID")
    .WithColumn("Name").AsString(30).WithColumnDescription("分组名")
    .WithColumn("Sort").AsInt32().WithColumnDescription("排序用，值越小越前面");


            Create.Table("mz_flow_node").WithDescription("表示操作节点")
.WithColumn("Id").AsInt64().PrimaryKey()
.WithColumn("FlowId").AsInt64().WithColumnDescription("流程实例Id")
.WithColumn("StepId").AsString(128)
.WithColumn("StepName").AsString(50)
.WithColumn("Active").AsBoolean()
.WithColumn("StartTime").AsDateTime().Nullable()
.WithColumn("EndTime").AsDateTime().Nullable()
.WithColumn("EventKey").AsString().Nullable()
.WithColumn("EventPublished").AsBoolean().Nullable()
.WithColumn("ParentId").AsInt64().Nullable()
.WithColumn("Outcome").AsString().Nullable()
.WithColumn("Status").AsInt32();


            Create.Table("mz_flow_extension_attr").WithDescription("节点扩展信息")
.WithColumn("Id").AsInt64().PrimaryKey().Identity().WithColumnDescription("节点扩展ID")
.WithColumn("ExecutionNodeId").AsInt64().Indexed("ExecutionIdx").WithColumnDescription("对应的节点Id")
.WithColumn("AttributeKey").AsString().Indexed("AttrKeyIdx")
.WithColumn("AttributeValue").AsString(20000);


            Create.Table("mz_flow_template").WithDescription("流程模板")
.WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("模板ID")
.WithColumn("OrgId").AsInt64().Indexed("FlowTemplateOrgId").WithColumnDescription("关联组织ID")
.WithColumn("GroupId").AsInt64().WithColumnDescription("分组Id")
.WithColumn("Name").AsString(30).WithColumnDescription("模板名称")
.WithColumn("Sort").AsInt32().WithColumnDescription("排序用，值越小越前面")
.WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0正常 1停用）")
.WithColumn("Icon").AsString(20).WithColumnDescription("图标")
.WithColumn("Background").AsString(25).WithColumnDescription("背景色")
.WithColumn("FormId").AsInt64().WithColumnDescription("对应表单类型")
.WithColumn("FlowJson").AsString(20000).WithColumnDescription("流程内容")
.WithColumn("notify").AsString(500).WithColumnDescription("消息通知方式(json内容)")
.WithColumn("sign").AsBoolean().WithColumnDescription("审批同意时是否需要签字")
.WithColumn("sublimit").AsInt32().WithColumnDescription("限制提交次数")
.WithColumn("startlimit").AsBoolean().WithColumnDescription("是否禁止直接发起")
.WithColumn("remark").AsString(255).WithColumnDescription("备注")
.WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
       .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
       .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
       .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
       .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Create.Table("mz_flow_template_link").WithDescription("可提交流程的对象")
.WithColumn("TemplateId").AsInt64().Indexed("TemplateLinkId")
.WithColumn("LinkType").AsFixedLengthAnsiString(1).WithColumnDescription("U为用户，D为部门")
.WithColumn("LinkId").AsInt64().WithColumnDescription("对象Id");

            Create.Table("mz_form").WithDescription("流程对应的表单类型")
.WithColumn("Id").AsInt64().PrimaryKey()
.WithColumn("FormName").AsString(50).WithColumnDescription("表单名称")
.WithColumn("FormFields").AsString(20000).WithColumnDescription("表单的字段，json格式存储");




            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 2020,
                menu_name = "流程管理",
                parent_id = 0,
                order_num = 6,
                path = "flowable",
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "M",
                visible = "0",
                status = "0",
                perms = "/FlowService/Flow",
                icon = "cascader",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 2022,
                menu_name = "流程定义",
                parent_id = 2020,
                order_num = 1,
                path = "FormsPanel",
                component = "flowable/definition/FormsPanel",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/FlowService/Flow/List",
                icon = "liucheng",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 2031,
                menu_name = "流程设计",
                name = "design",
                parent_id = 2020,
                order_num = 2,
                path = "/definition/design",
                component = "flowable/definition/FormProcessDesign",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "1",
                status = "0",
                perms = "/FlowService/Flow/Design",
                icon = string.Empty,
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 2051,
                menu_name = "流程记录",
                name = "applicationrecord",
                parent_id = 2020,
                order_num = 2,
                path = "/definition/applicationrecord",
                component = "flowable/definition/ApplicationRecord",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "1",
                status = "0",
                perms = "/FlowService/Flow/Record",
                icon = string.Empty,
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 2023,
                menu_name = "待办任务",
                parent_id = 2020,
                order_num = 3,
                path = "todo",
                component = "flowable/task/todo/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/FlowService/Task/WaitList",
                icon = "daibanrenwu",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 2025,
                menu_name = "已办任务",
                parent_id = 2020,
                order_num = 4,
                path = "finished",
                component = "flowable/task/finished/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/FlowService/Task/List",
                icon = "time-range",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 2032,
                menu_name = "抄送我的",
                parent_id = 2020,
                order_num = 5,
                path = "cclist",
                component = "flowable/task/cclist/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/FlowService/Task/CCList",
                icon = "documentation",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 2026,
                menu_name = "我的流程",
                parent_id = 2020,
                order_num = 6,
                path = "process",
                component = "flowable/task/process/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/FlowService/Task/FlowList",
                icon = "guide",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 2027,
                menu_name = "发起流程",
                parent_id = 2020,
                order_num = 7,
                path = "record",
                component = "flowable/task/record/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "1",
                status = "0",
                perms = "/FlowService/Task/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

        }

        public override void Down()
        {

        }

    }
}
