using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTRulesService
{
    [Migration(20240425002)]
    public class RuleMigrator2 : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_rule_group");

            Create.Table("mz_rule_group").WithDescription("规则分组表")
       .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
       .WithColumn("OrgId").AsInt64().Indexed("IotRuleGroupOrgId").WithColumnDescription("所属组织ID")
       .WithColumn("GroupName").AsString(50).WithColumnDescription("分组名称")
       .WithColumn("Sort").AsInt32().WithColumnDescription("排序值：越小越前面")
       .WithColumn("Remark").AsString(255).WithColumnDescription("备注说明")
       .WithColumn("ParentId").AsString(128).WithColumnDescription("父分组Id")
       .WithColumn("Path").AsString(800).Indexed().WithColumnDescription("分组层级")
       .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
       .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
       .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
       .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 111,
                menu_name = "规则节点",
                parent_id = 2,
                order_num = 3,
                path = "rulenode",
                component = "monitor/rulenode/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTRulesService/RuleFlow/NodeList",
                icon = "server",
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
