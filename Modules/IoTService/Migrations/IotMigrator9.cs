using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Migrations
{
    [Migration(20251017003)]
    public class IotMigrator9 : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_iot_code");
            Create.Table("mz_iot_code").WithDescription("物联标识模板")
.WithColumn("Id").AsInt32().PrimaryKey().WithColumnDescription("编码")
.WithColumn("Name").AsString(50).WithColumnDescription("标识符名称")
.WithColumn("Code").AsString(50).WithColumnDescription("标识符")
.WithColumn("CodeGroup").AsInt32().WithColumnDescription("标识符分组")
.WithColumn("OptionData").AsString(500).WithColumnDescription("默认属性选项内容")
.WithColumn("CodeType").AsInt32().WithColumnDescription("0为属性、1为功能")
.WithColumn("Sort").AsInt32().WithColumnDescription("排序值：越小越前面");


            Execute.Sql("DROP TABLE IF EXISTS mz_iot_code_group");
            Create.Table("mz_iot_code_group").WithDescription("物联标识符分组")
                .WithColumn("Id").AsInt32().PrimaryKey().WithColumnDescription("编码")
                .WithColumn("Name").AsString(50).WithColumnDescription("分组名称")
                .WithColumn("ParentId").AsInt32().WithColumnDescription("父Id")
                .WithColumn("Sort").AsInt32().WithColumnDescription("排序值：越小越前面");


            Insert.IntoTable("mz_iot_code_group").Row(new
            {
                Id = 1,
                Name = "传感器",
                ParentId = 0,
                Sort = 1
            }).Row(new
            {
                Id = 2,
                Name = "执行器",
                ParentId = 0,
                Sort = 2
            }).Row(new
            {
                Id = 3,
                Name = "嵌入式",
                ParentId = 0,
                Sort = 3
            }).Row(new
            {
                Id = 4,
                Name = "控制器",
                ParentId = 0,
                Sort = 4
            });

            //传感器类别
            Insert.IntoTable("mz_iot_code_group").Row(new
            {
                Id = 11,
                Name = "电表",
                ParentId = 1,
                Sort = 1
            }).Row(new
            {
                Id = 12,
                Name = "流量计",
                ParentId = 1,
                Sort = 2
            });

            //执行器类别
            Insert.IntoTable("mz_iot_code_group").Row(new
            {
                Id = 21,
                Name = "智能插座",
                ParentId = 2,
                Sort = 1
            });

            //嵌入式类别
            Insert.IntoTable("mz_iot_code_group").Row(new
            {
                Id = 31,
                Name = "智能电视",
                ParentId = 3,
                Sort = 1
            });

            //控制器类别
            Insert.IntoTable("mz_iot_code_group").Row(new
            {
                Id = 41,
                Name = "空压机",
                ParentId = 4,
                Sort = 1
            });


            Insert.IntoTable("mz_iot_code").Row(new
            {
                Id = 1,
                Name = "总有功功率",
                Code = "TotalCos",
                CodeGroup = 11,
                OptionData = "{\"description\":\"\",\"option\":{\"type\":\"float\",\"decimals\":3,\"max\":999999999,\"min\":0,\"unit\":\"kw\",\"spacing\":0,\"multiple\":0.015}}",
                CodeType = 0,
                Sort = 1
            }).Row(new
            {
                Id = 2,
                Name = "总有功电能",
                Code = "Totalkwh",
                CodeGroup = 11,
                OptionData = "{\"description\":\"\",\"option\":{\"type\":\"float\",\"decimals\":2,\"max\":999999999,\"min\":0,\"unit\":\"kwh\",\"spacing\":0,\"multiple\":0.01}}",
                CodeType = 0,
                Sort = 2
            }).Row(new
            {
                Id = 10,
                Name = "瞬时流量",
                Code = "SFlow",
                CodeGroup = 12,
                OptionData = "{\"description\":\"\",\"option\":{\"type\":\"float\",\"express\":\"toFloat(data)\",\"decimals\":1,\"max\":999999999,\"min\":0,\"unit\":\"Nm³/min\",\"spacing\":0,\"multiple\":0}}",
                CodeType = 0,
                Sort = 1
            }).Row(new
            {
                Id = 11,
                Name = "流量计压力",
                Code = "ProtePressure",
                CodeGroup = 12,
                OptionData = "{\"description\":\"\",\"option\":{\"type\":\"float\",\"express\":\"toFloat(data)\",\"decimals\":2,\"max\":999999999,\"min\":0,\"unit\":\"Mpa\",\"spacing\":0,\"multiple\":0}}",
                CodeType = 0,
                Sort = 2
            }).Row(new
            {
                Id = 12,
                Name = "流量计温度",
                Code = "T2",
                CodeGroup = 12,
                OptionData = "{\"description\":\"\",\"option\":{\"type\":\"float\",\"express\":\"toFloat(data)\",\"decimals\":2,\"max\":999999999,\"min\":0,\"unit\":\"°C\",\"spacing\":0,\"multiple\":0}}",
                CodeType = 0,
                Sort = 3
            }).Row(new
            {
                Id = 13,
                Name = "累计流量",
                Code = "AFlow",
                CodeGroup = 12,
                OptionData = "{\"description\":\"\",\"option\":{\"type\":\"float\",\"express\":\"toFloat(data)\",\"decimals\":2,\"max\":999999999,\"min\":0,\"unit\":\"Nm³\",\"spacing\":0,\"multiple\":0}}",
                CodeType = 0,
                Sort = 4
            });


            Execute.Sql("DROP TABLE IF EXISTS mz_iot_win_rule");
            Create.Table("mz_iot_win_rule").WithDescription("物联协议的属性规则")
            .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("Id")
            .WithColumn("ProductId").AsString(128).WithColumnDescription("所属协议Id")
            .WithColumn("PropCode").AsString(50).WithColumnDescription("关联属性")
            .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
            .WithColumn("WindowWay").AsByte().WithColumnDescription("统计时间：0每时、1每日、2每月")
            .WithColumn("MergeWay").AsString(50).WithColumnDescription("统计方式：最大值:max，最小值：min，平均值：mean，合计：sum，期初值：first，期末值：last")
            .WithColumn("MergeCode").AsString(50).WithColumnDescription("统计属性")
            .WithColumn("Priority").AsInt32().WithDefaultValue(0).Indexed().WithColumnDescription("0占位，暂无用处");


            Create.Index().OnTable("mz_iot_win_rule").WithOptions().Unique().OnColumn("ProductId").Ascending().OnColumn("PropCode").Ascending();
        }
        public override void Down()
        {
        }
    }
}
