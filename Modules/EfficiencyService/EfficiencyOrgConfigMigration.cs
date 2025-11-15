using Mysqlx.Prepare;
using FluentMigrator;

namespace EfficiencyService
{
    [Migration(20250822006)]
    public class EfficiencyOrgConfigMigration : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS t_effic_org_conf");
            Create.Table("t_effic_org_conf").WithDescription("企业能碳配置")
                .WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("企业ID")
                .WithColumn("ElecCode").AsString().Nullable().WithDefaultValue("").WithColumnDescription("电总有功电能标识符")
                .WithColumn("ElecPowerCode").AsString().Nullable().WithDefaultValue("").WithColumnDescription("电总有功功率标识符")
                .WithColumn("ContectName").AsString().WithColumnDescription("联系人")
                .WithColumn("ContectTel").AsString(20).WithColumnDescription("联系电话")
                .WithColumn("PricePolicyId").AsString(128).Nullable().WithDefaultValue("").WithColumnDescription("默认电价政策Id")
                .WithColumn("EmissionSourceJson").AsString(500).WithDefaultValue("").WithColumnDescription("存放排放源与因子json");

        }
        public override void Down()
        {
        }
    }
}
