using FluentMigrator;


namespace FlowService
{
    [Migration(20240830111)]
    public class FlowMigrator2 : Migration
    {
        public override void Up()
        {
            Alter.Table("mz_flow_extension_attr").AddColumn("FlowId").AsInt64().Indexed("ExFlowIdIdx").WithColumnDescription("流程实例Id");
            Execute.Sql("update mz_flow_extension_attr set FlowId=(select FlowId from mz_flow_node where Id=mz_flow_extension_attr.ExecutionNodeId limit 1)");

        }

        public override void Down()
        {

        }

    }
}
