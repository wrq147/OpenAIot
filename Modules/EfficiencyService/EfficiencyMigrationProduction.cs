using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using FluentMigrator;

namespace EfficiencyService
{

    [Migration(20250903765)]
    public class EfficiencyMigrationProduction : Migration
    {
        public override void Up()
        {
            //生产数据采集表
            Execute.Sql("DROP TABLE IF EXISTS t_prod_production");
            Create.Table("t_prod_production").WithDescription("生产数据采集表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                 .WithColumn("FacilityId").AsString(128).WithColumnDescription("设施编码")
                .WithColumn("ProductId").AsString(128).WithColumnDescription("产品编码")
                .WithColumn("DDate").AsDate().WithColumnDescription("统计日")
                .WithColumn("Price").AsDouble().WithColumnDescription("单价")
                .WithColumn("Unit").AsString(128).WithColumnDescription("单位")
                .WithColumn("OutPut").AsDouble().WithColumnDescription("产量")
                .WithColumn("OutValue").AsDouble().WithColumnDescription("产值")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //设备能耗采集表
            Execute.Sql("DROP TABLE IF EXISTS t_prod_energy");
            Create.Table("t_prod_energy").WithDescription("设备能耗采集表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("EquipmentId").AsString(128).WithColumnDescription("设备编码")
                .WithColumn("FactorId").AsString().WithColumnDescription("排放因子编码")
                .WithColumn("FactorName").AsString().WithColumnDescription("排放因子编码")
                .WithColumn("PolicyId").AsString(128).WithColumnDescription("计费政策编码")
                .WithColumn("PolicyName").AsString(128).WithColumnDescription("计费政策名称")
                .WithColumn("DDate").AsDate().WithColumnDescription("统计日")
                .WithColumn("InitVale").AsDouble().WithColumnDescription("表码期初值")
                .WithColumn("EndVale").AsDouble().WithColumnDescription("表码期末值")
                .WithColumn("Memo").AsString(128).WithColumnDescription("备注")
                .WithColumn("DataSource").AsString(128).WithColumnDescription("数据来源")
                .WithColumn("Unit").AsString(128).WithColumnDescription("一级单位")
                .WithColumn("LageUnit").AsString(128).WithColumnDescription("二级单位")
                .WithColumn("UseVale").AsString(128).WithColumnDescription("能源消耗量")
                .WithColumn("CostVale").AsString(128).WithColumnDescription("成本")
                .WithColumn("CarbonEmission").AsDouble().WithColumnDescription("碳排放量")
                .WithColumn("ConvertCoal").AsDouble().WithColumnDescription("折标准煤")
                .WithColumn("TimePeriod").AsString(128).WithColumnDescription("时段类型（1代表尖 2代表峰 3代表平 4代表谷 5代表全天）")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //自动采集配置表
            Execute.Sql("DROP TABLE IF EXISTS t_prod_config");
            Create.Table("t_prod_config").WithDescription("设备能耗采集表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("BeginTime").AsString(128).WithColumnDescription("上次开始时间")
                .WithColumn("EndTime").AsString().WithColumnDescription("上次结束时间")
                .WithColumn("TotalUse").AsString().WithColumnDescription("累计阶梯值")
                .WithColumn("TotalTime").AsString(128).WithColumnDescription("累计开始时间");

        }
        public override void Down()
        {
        }
    }
}
