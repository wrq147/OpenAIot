using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using FluentMigrator;

namespace EfficiencyService
{
    [Migration(20251018001)]
    public class EfficiencyMigrationProduction4 : Migration
    {
        public override void Up()
        {
            //产品物料清单表
            Execute.Sql("DROP TABLE IF EXISTS t_prod_energy_t");
            Create.Table("t_prod_energy_t").WithDescription("设备能耗时段表")
              .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("EquipmentId").AsString(128).WithColumnDescription("设备编码")
                .WithColumn("FactorId").AsString().WithColumnDescription("排放因子编码")
                .WithColumn("FactorName").AsString().WithColumnDescription("排放因子编码")
                .WithColumn("DDate").AsDate().WithColumnDescription("统计日")
                .WithColumn("TTime").AsString().WithColumnDescription("统计时段")
                .WithColumn("InitVale").AsDouble().WithColumnDescription("表码期初值")
                .WithColumn("EndVale").AsDouble().WithColumnDescription("表码期末值")
                .WithColumn("Unit").AsString(128).WithColumnDescription("一级单位")
                .WithColumn("LageUnit").AsString(128).WithColumnDescription("二级单位")
                .WithColumn("UseVale").AsString(128).WithColumnDescription("能源消耗量")
                .WithColumn("CostVale").AsString(128).WithColumnDescription("成本")
                .WithColumn("CarbonEmission").AsDouble().WithColumnDescription("碳排放量")
                .WithColumn("ConvertCoal").AsDouble().WithColumnDescription("折标准煤")
                .WithColumn("TimePeriod").AsString(128).WithColumnDescription("时段类型（尖/峰/平/谷）");
        }

        public override void Down()
        {
        }
    }
}
