using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService
{
    [Migration(20250327009)]
    public class FlowMigrator3 : Migration
    {
        public override void Up()
        {
            if (Schema.Table("mz_flow_trilog").Exists())
            {
                Delete.Table("mz_flow_trilog");
            }
            Create.Table("mz_flow_trilog").WithDescription("子流程发起记录")
.WithColumn("FlowNumber").AsString(50).PrimaryKey()
.WithColumn("TemplateIdSet").AsString(500);
        }

        public override void Down()
        {

        }

    }
}
