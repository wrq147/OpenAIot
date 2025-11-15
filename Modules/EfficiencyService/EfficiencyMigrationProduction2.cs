using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using FluentMigrator;

namespace EfficiencyService
{
    [Migration(20250918133)]
    public class EfficiencyMigrationProduction2 : Migration
    {
        public override void Up()
        {
            //产品物料清单表
            Execute.Sql("DROP TABLE IF EXISTS t_prod_bom");
            Create.Table("t_prod_bom").WithDescription("产品物料清单表")
                .WithColumn("ModelId").AsString(128).WithColumnDescription("模型编码")
                .WithColumn("LinkId").AsString(128).WithColumnDescription("工序编码")
                .WithColumn("MaterialCarbonId").AsString(128).WithColumnDescription("产品碳足迹编码")
                .WithColumn("Dosage").AsInt32().WithColumnDescription("用量")
                .WithColumn("BomType").AsString(128).WithColumnDescription("清单类型");
        }

        public override void Down()
        {
        }
    }
}
