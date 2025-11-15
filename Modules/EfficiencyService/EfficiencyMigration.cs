using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using FluentMigrator;

namespace EfficiencyService
{
    [Migration(20250814065)]
    public class EfficiencyMigration : Migration
    {
        public override void Up()
        {
            //排放因子表
            Execute.Sql("DROP TABLE IF EXISTS t_eng_factor");
            Create.Table("t_eng_factor").WithDescription("排放因子表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("Year").AsString().WithColumnDescription("年份")
                .WithColumn("Version").AsString().WithColumnDescription("版本")
                .WithColumn("TypeId").AsString(128).WithColumnDescription("排放因子分类编码")
                .WithColumn("FactorName").AsString(150).WithColumnDescription("因子名称")
                .WithColumn("EmissionFactor").AsDouble().WithColumnDescription("因子数值")
                .WithColumn("AvgCalorific").AsString(128).WithColumnDescription("平均低位发热量")
                .WithColumn("EqCoal").AsString(128).WithColumnDescription("折标准煤系数")
                .WithColumn("Memo").AsString(500).WithColumnDescription("来源说明")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");
            //排放因子分类表
            Execute.Sql("DROP TABLE IF EXISTS t_eng_factortype");
            Create.Table("t_eng_factortype").WithDescription("排放因子分类表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("TypeName").AsString(150).WithColumnDescription("分类名称")
                .WithColumn("FactorType").AsBoolean().WithColumnDescription("排放分类（False代表目录 Ture代表分类）")
                .WithColumn("FactorDigits").AsInt64().WithColumnDescription("小数位数")
                .WithColumn("NameTitle").AsString(150).WithColumnDescription("因子名称")
                .WithColumn("FactorTitle").AsString(150).WithColumnDescription("数值名称")
                .WithColumn("FactorUnit").AsString(150).WithColumnDescription("因子单位")
                .WithColumn("ActivityUnit").AsString(150).WithColumnDescription("活动水平单位")
                .WithColumn("ActivityConversion").AsDouble().WithColumnDescription("单位换算")
                .WithColumn("ParentId").AsString(128).WithColumnDescription("父节点ID")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");
            //发布年份表
            Execute.Sql("DROP TABLE IF EXISTS t_eng_factoryear");
            Create.Table("t_eng_factoryear").WithDescription("排放因子年份表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("Year").AsString(150).WithColumnDescription("分类名称")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");
            //发布年份版本表
            Execute.Sql("DROP TABLE IF EXISTS t_eng_factoryearversion");
            Create.Table("t_eng_factoryearversion").WithDescription("排放因子年份版本表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("YearId").AsString(128).WithColumnDescription("年份编码")
                .WithColumn("Version").AsString(150).WithColumnDescription("版本号")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");
            //企业排放因子表
            Execute.Sql("DROP TABLE IF EXISTS t_eng_factororg");
            Create.Table("t_eng_factororg").WithDescription("企业排放因子表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("FactorId").AsString().WithColumnDescription("排放因子编码")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Insert.IntoTable("t_eng_factoryear").Row(new
            {
                Id = "1",
                Year = "2024",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factoryearversion").Row(new
            {
                Id = "1",
                YearId = "1",
                Version = "2021",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factoryearversion").Row(new
            {
                Id = "2",
                YearId = "1",
                Version = "2022",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factoryearversion").Row(new
            {
                Id = "3",
                YearId = "1",
                Version = "2024",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factortype").Row(new
            {
                Id = "1001",
                TypeName = "化石燃料",
                FactorType = true,
                FactorDigits = 1,
                NameTitle = "燃料品种",
                FactorTitle = "因子(tCO₂/TJ)",
                FactorUnit = "tCO₂/TJ",
                ActivityUnit = "TJ",
                ActivityConversion = 1,
                ParentId = "-",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factortype").Row(new
            {
                Id = "1021",
                TypeName = "天然气",
                FactorType = true,
                FactorDigits = 1,
                NameTitle = "燃料品种",
                FactorTitle = "因子(tCO₂/TJ)",
                FactorUnit = "tCO₂/TJ",
                ActivityUnit = "TJ",
                ActivityConversion = 1,
                ParentId = "1001",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            /*
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "1001",
                Year = "2024",
                Version = "",
                TypeId = "1001",
                FactorName = "电煤",
                EmissionFactor = 96.9,
                AvgCalorific = 20934,
                EqCoal = 0.7143,
                Memo = "本土化数据，数据来源于国家应对气候变化战略研究和国际合作中心",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "1002",
                Year = "2024",
                Version = "",
                TypeId = "1001",
                FactorName = "无烟煤",
                EmissionFactor = 98.3,
                AvgCalorific = 20934,
                EqCoal = 0.7143,
                Memo = "数据来源于《2006年IPCC清单指南》",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "1003",
                Year = "2024",
                Version = "",
                TypeId = "1001",
                FactorName = "炼焦烟煤",
                EmissionFactor = 94.6,
                AvgCalorific = 20934,
                EqCoal = 0.7143,
                Memo = "数据来源于《2006年IPCC清单指南》",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "1004",
                Year = "2024",
                Version = "",
                TypeId = "1001",
                FactorName = "一般烟煤",
                EmissionFactor = 94.6,
                AvgCalorific = 20934,
                EqCoal = 0.7143,
                Memo = "数据来源于《2006年IPCC清单指南》",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "1005",
                Year = "2024",
                Version = "",
                TypeId = "1001",
                FactorName = "褐煤",
                EmissionFactor = 101.2,
                AvgCalorific = 20934,
                EqCoal = 0.7143,
                Memo = "数据来源于《2006年IPCC清单指南》",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "1006",
                Year = "2024",
                Version = "",
                TypeId = "1001",
                FactorName = "煤制品",
                EmissionFactor = 97.5,
                AvgCalorific = 20934,
                EqCoal = 0.7143,
                Memo = "数据来源于《2006年IPCC清单指南》",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "1007",
                Year = "2024",
                Version = "",
                TypeId = "1001",
                FactorName = "焦炭",
                EmissionFactor = 107.1,
                AvgCalorific = 20934,
                EqCoal = 0.7143,
                Memo = "数据来源于《2006年IPCC清单指南》",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "1008",
                Year = "2024",
                Version = "",
                TypeId = "1001",
                FactorName = "焦炉煤气",
                EmissionFactor = 44.4,
                AvgCalorific = 20934,
                EqCoal = 0.7143,
                Memo = "数据来源于《2006年IPCC清单指南》",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "1009",
                Year = "2024",
                Version = "",
                TypeId = "1001",
                FactorName = "高炉煤气",
                EmissionFactor = 259.6,
                AvgCalorific = 20934,
                EqCoal = 0.7143,
                Memo = "数据来源于《2006年IPCC清单指南》",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "1010",
                Year = "2024",
                Version = "",
                TypeId = "1001",
                FactorName = "转炉煤气",
                EmissionFactor = 181.9,
                AvgCalorific = 20934,
                EqCoal = 0.7143,
                Memo = "数据来源于《2006年IPCC清单指南》",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "1011",
                Year = "2024",
                Version = "",
                TypeId = "1001",
                FactorName = "其他煤气",
                EmissionFactor = 44.4,
                AvgCalorific = 20934,
                EqCoal = 0.7143,
                Memo = "数据来源于《2006年IPCC清单指南》",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "2001",
                Year = "2024",
                Version = "",
                TypeId = "1001",
                FactorName = "石脑油",
                EmissionFactor = 73.3,
                AvgCalorific = 41868,
                EqCoal = 1.4286,
                Memo = "数据来源于《2006年IPCC清单指南》",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            */
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "3001",
                Year = "2024",
                Version = "",
                TypeId = "1021",
                FactorName = "天然气",
                EmissionFactor = 56.1,
                AvgCalorific = 32238,
                EqCoal = 1.1,
                Memo = "数据来源于《2006年IPCC清单指南》",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factortype").Row(new
            {
                Id = "1006",
                TypeName = "电力与蒸汽",
                FactorType = true,
                FactorDigits = 4,
                NameTitle = "因子名称",
                FactorTitle = "因子(kgCO₂/kWh)",
                FactorUnit = "kgCO₂/kWh",
                ActivityUnit = "kgCO₂",
                ActivityConversion = 0.001,
                ParentId = "-",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factortype").Row(new
            {
                Id = "4026",
                TypeName = "电力",
                FactorType = true,
                FactorDigits = 4,
                NameTitle = "因子名称",
                FactorTitle = "因子(kgCO₂/kWh)",
                FactorUnit = "kgCO₂/kWh",
                ActivityUnit = "kgCO₂",
                ActivityConversion = 0.001,
                ParentId = "1006",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "3002",
                Year = "2024",
                Version = "2022",
                TypeId = "4026",
                FactorName = "全国",
                EmissionFactor = 0.5366,
                AvgCalorific = 1,
                EqCoal = 0.1229,
                Memo = "中国本土化数据，数据来源于生态环境部、国家统计局《关于发布2022年电力二氧化碳排放因子的公告》(公告2024年第33号)",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "3003",
                Year = "2024",
                Version = "2022",
                TypeId = "4026",
                FactorName = "华东",
                EmissionFactor = 0.5366,
                AvgCalorific = 1,
                EqCoal = 0.1229,
                Memo = "中国本土化数据，数据来源于生态环境部、国家统计局《关于发布2022年电力二氧化碳排放因子的公告》(公告2024年第33号)",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });
            Insert.IntoTable("t_eng_factor").Row(new
            {
                Id = "3004",
                Year = "2024",
                Version = "2022",
                TypeId = "4026",
                FactorName = "华南",
                EmissionFactor = 0.5366,
                AvgCalorific = 1,
                EqCoal = 0.1229,
                Memo = "中国本土化数据，数据来源于生态环境部、国家统计局《关于发布2022年电力二氧化碳排放因子的公告》(公告2024年第33号)",
                del_flag = "0",
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
