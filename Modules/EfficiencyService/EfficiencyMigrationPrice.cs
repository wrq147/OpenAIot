using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace EfficiencyService
{
    [Migration(20250815089)]
    public class EfficiencyMigrationPrice : Migration
    {
        public override void Up()
        {
            //电价政策表
            Execute.Sql("DROP TABLE IF EXISTS t_price_policy");
            Create.Table("t_price_policy").WithDescription("计费政策表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("PolicyName").AsString(128).WithColumnDescription("政策名称")
                .WithColumn("EnergyType").AsString(128).WithColumnDescription("能源类型")
                .WithColumn("FactorId").AsString().WithColumnDescription("排放因子编码")
                .WithColumn("Unit").AsString(128).WithColumnDescription("一级计量单位")
                .WithColumn("LageUnit").AsString(128).WithColumnDescription("二级计量单位")
                .WithColumn("Memo").AsString(500).WithColumnDescription("来源说明")
                .WithColumn("Version").AsString(128).WithColumnDescription("版本号")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //电价政策明细表
            Execute.Sql("DROP TABLE IF EXISTS t_price_policydetil");
            Create.Table("t_price_policydetil").WithDescription("计费政策明细表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("PolicyId").AsString(128).WithColumnDescription("政策编码")
                .WithColumn("PolicyMonth").AsString(128).WithColumnDescription("生效月份")
                .WithColumn("PolicyType").AsFixedLengthAnsiString(1).WithColumnDescription("计费方式（1代表分时 2代表不分时 3代表阶梯）")
                .WithColumn("Version").AsString(128).WithColumnDescription("版本号")
                .WithColumn("CycleType").AsFixedLengthAnsiString(1).WithColumnDescription("计费周期（1年 2季度 3月）")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //尖峰平谷价格表
            Execute.Sql("DROP TABLE IF EXISTS t_price_period");
            Create.Table("t_price_period").WithDescription("尖峰平谷价格表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("DetilId").AsString(128).WithColumnDescription("政策明细编码")
                .WithColumn("TimePeriod").AsFixedLengthAnsiString(1).WithColumnDescription("时段类型（1代表尖 2代表峰 3代表平 4代表谷 5代表全天）")
                .WithColumn("Price").AsDouble().WithColumnDescription("电价")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //尖峰平谷时段表
            Execute.Sql("DROP TABLE IF EXISTS t_price_time");
            Create.Table("t_price_time").WithDescription("尖峰平谷时段表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("DetilId").AsString(128).WithColumnDescription("政策明细编码")
                .WithColumn("TimePeriod").AsFixedLengthAnsiString(1).WithColumnDescription("时段类型（1代表尖 2代表峰 3代表平 4代表谷 5代表全天）")
                .WithColumn("StartTime").AsString(128).WithColumnDescription("开始时间")
                .WithColumn("EndTime").AsString(128).WithColumnDescription("结束时间")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //阶梯电价表
            Execute.Sql("DROP TABLE IF EXISTS t_price_tier");
            Create.Table("t_price_tier").WithDescription("阶梯电价表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("DetilId").AsString(128).WithColumnDescription("政策明细编码")
                .WithColumn("TierLevel").AsFixedLengthAnsiString(1).WithColumnDescription("阶梯级别（1代表第一档 2代表第二档 3代表第三档）")
                .WithColumn("MinKwh").AsDouble().WithColumnDescription("最小用电量")
                .WithColumn("MaxKwh").AsDouble().WithColumnDescription("最大用电量")
                .WithColumn("Price").AsDouble().WithColumnDescription("电价")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


        }
        public override void Down()
        {
        }
    }
}
