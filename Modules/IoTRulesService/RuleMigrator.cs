using FluentMigrator;
using System;
namespace IoTRulesService
{
    [Migration(20221223001)]
    public class RuleMigrator : Migration
    {
        public override void Up()
        {
            Create.Table("mz_rule_template").WithDescription("规则模板")
.WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("规则模板ID")
.WithColumn("OrgId").AsInt64().Indexed("RuleTemplateOrgId").WithColumnDescription("规则模板关联组织ID")
.WithColumn("GroupId").AsString(128).WithColumnDescription("分组Id")
.WithColumn("Name").AsString(30).WithColumnDescription("规则名称")
.WithColumn("CreatedFrom").AsString(10).WithColumnDescription("创建源：pc、mobile")
.WithColumn("TriggerWay").AsByte().Indexed("IDXTriggerWay").WithColumnDescription("触发方式：0为订阅，1为Http")
.WithColumn("TimerCron").AsString(100).WithColumnDescription("Cron表达式（定时触发用）")
.WithColumn("TimerJobId").AsInt64().WithColumnDescription("定时器关联的Job")
.WithColumn("HttpParams").AsString(20000).Nullable().WithColumnDescription("http方式的输入参数json")
.WithColumn("Sort").AsInt32().WithColumnDescription("规则优先级")
.WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0启用 1停用）")
.WithColumn("RuleJson").AsString(20000).WithColumnDescription("流程内容")
.WithColumn("Remark").AsString(255).WithColumnDescription("备注")
       .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
       .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
       .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
       .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");



            Create.Table("mz_rule_trigger").WithDescription("规则模板的触发方式")
    .WithColumn("Id").AsInt64().PrimaryKey().Identity().WithColumnDescription("编号")
    .WithColumn("RuleId").AsInt64().Indexed("TrRuleId").WithColumnDescription("规则模板Id")
    .WithColumn("TopicDevice").AsString(255).WithColumnDescription("订阅的设备：/Product/为指定产品，/Device/为指定设备")
    .WithColumn("TopicMsg").AsString(80).WithColumnDescription("订阅的消息类型：参考JsonMessageConverter里的");

            Create.Index("IDXTopicDevice").OnTable("mz_rule_trigger").OnColumn("TopicDevice").Ascending().OnColumn("TopicMsg").Ascending();



            Create.Table("mz_rule_event").WithDescription("规则执行事件")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("规则执行实例Id")
                .WithColumn("RuleId").AsInt64().WithColumnDescription("规则模板Id")
                .WithColumn("InstanceJson").AsString(20000).WithColumnDescription("实例Json")
                .WithColumn("StackJson").AsString(20000).WithColumnDescription("规则栈Json")
                .WithColumn("SourceJson").AsString(20000).WithColumnDescription("源数据Json")
                .WithColumn("Index").AsInt32().WithColumnDescription("触发节点索引");





            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 4201,
                menu_name = "规则引擎",
                parent_id = 4000,
                order_num = 1,
                path = "rulesEngine/index",
                component = "iot/rulesEngine/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTRulesService/Ruleflow/ListPage",
                icon = "a-guizeliebiao",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 4202,
                menu_name = "规则设计",
                parent_id = 4000,
                order_num = 1,
                path = "rulesEngine/add",
                component = "iot/rulesEngine/add",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "1",
                status = "0",
                perms = "/IoTRulesService/Ruleflow/Design",
                icon = "",
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
