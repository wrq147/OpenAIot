using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Migrations
{
    [Migration(20251105002)]
    public class IotMigratorA1 : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_iot_warn_config");
            Create.Table("mz_iot_warn_config").WithDescription("告警工单配置")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
.WithColumn("ProductId").AsString(128).Unique().WithColumnDescription("所属物联协议Id")
.WithColumn("WarnFlowId").AsInt64().WithColumnDescription("告警工单执行流程")
.WithColumn("WarnFlowInitJson").AsString(50000).WithColumnDescription("表单初始化映射");


        }
        public override void Down()
        {
        }
    }
}
