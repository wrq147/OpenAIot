using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace EfficiencyService
{
    [Migration(20250908789)]
    public class EfficiencyMigrationCommon2 : Migration
    {
        public override void Up()
        {
            //产品生命周期模型表
            Execute.Sql("DROP TABLE IF EXISTS t_com_model");
            Create.Table("t_com_model").WithDescription("产品生命周期模型表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("ProductId").AsString(128).WithColumnDescription("产品编码")
                .WithColumn("ProductBorder").AsString(128).WithColumnDescription("产品生命周期边界")
                .WithColumn("ModelName").AsString(128).WithColumnDescription("模型名称")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //生产环节表
            Execute.Sql("DROP TABLE IF EXISTS t_com_link");
            Create.Table("t_com_link").WithDescription("生产环节表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("ProductBorder").AsString(128).WithColumnDescription("产品生命周期边界")
                .WithColumn("BorderTitle").AsString(128).WithColumnDescription("边界标题")
                .WithColumn("BorderName").AsString(128).WithColumnDescription("边界名称")
                .WithColumn("LinkId").AsString(128).WithColumnDescription("环节编码")
                .WithColumn("LinkName").AsString(128).WithColumnDescription("环节名称");
            Insert.IntoTable("t_com_link").Row(new
            {
                Id = "1",
                ProductBorder = "1",
                BorderTitle = "从摇篮到大门",
                BorderName = "从资源开采到产品出厂",
                LinkId = "1",
                LinkName = "原材料获取"
            });
            Insert.IntoTable("t_com_link").Row(new
            {
                Id = "2",
                ProductBorder = "1",
                BorderTitle = "从摇篮到大门",
                BorderName = "从资源开采到产品出厂",
                LinkId = "2",
                LinkName = "生产"
            });
            Insert.IntoTable("t_com_link").Row(new
            {
                Id = "3",
                ProductBorder = "2",
                BorderTitle = "从大门到大门",
                BorderName = "从产品制造到产品出厂",
                LinkId = "2",
                LinkName = "生产"
            });
            Insert.IntoTable("t_com_link").Row(new
            {
                Id = "4",
                ProductBorder = "3",
                BorderTitle = "从摇篮到坟墓",
                BorderName = "从资源开采到产品废弃",
                LinkId = "1",
                LinkName = "原材料获取"
            });
            Insert.IntoTable("t_com_link").Row(new
            {
                Id = "5",
                ProductBorder = "3",
                BorderTitle = "从摇篮到坟墓",
                BorderName = "从资源开采到产品废弃",
                LinkId = "2",
                LinkName = "生产"
            });
            Insert.IntoTable("t_com_link").Row(new
            {
                Id = "6",
                ProductBorder = "3",
                BorderTitle = "从摇篮到坟墓",
                BorderName = "从资源开采到产品废弃",
                LinkId = "3",
                LinkName = "物流/仓储/运输"
            });
            Insert.IntoTable("t_com_link").Row(new
            {
                Id = "7",
                ProductBorder = "3",
                BorderTitle = "从摇篮到坟墓",
                BorderName = "从资源开采到产品废弃",
                LinkId = "4",
                LinkName = "使用"
            });
            Insert.IntoTable("t_com_link").Row(new
            {
                Id = "8",
                ProductBorder = "3",
                BorderTitle = "从摇篮到坟墓",
                BorderName = "从资源开采到产品废弃",
                LinkId = "5",
                LinkName = "废弃（使用结束）"
            });

            //产品工序表
            Execute.Sql("DROP TABLE IF EXISTS t_com_process");
            Create.Table("t_com_process").WithDescription("产品工序表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("ModelId").AsString(128).WithColumnDescription("模型编码")
                .WithColumn("LinkId").AsString(128).WithColumnDescription("环节编码")
                .WithColumn("ProcessName").AsString(128).WithColumnDescription("工序名称")
                .WithColumn("ProcessNo").AsString(128).WithColumnDescription("工序序号");

            //产品工序设施表
            Execute.Sql("DROP TABLE IF EXISTS t_com_processitem");
            Create.Table("t_com_processitem").WithDescription("产品工序设施表")
                .WithColumn("ModelId").AsString(128).WithColumnDescription("模型编码")
                .WithColumn("ProcessId").AsString(128).WithColumnDescription("工序编码")
                .WithColumn("ProductId").AsString(128).WithColumnDescription("产品编码")
                .WithColumn("FacilityId").AsString(1300).WithColumnDescription("设施编码")
                .WithColumn("FacilityIds").AsString(1300).WithColumnDescription("包含子设施编码");

            //产品生命周期表
            Execute.Sql("DROP TABLE IF EXISTS t_com_productmodel");
            Create.Table("t_com_productmodel").WithDescription("产品生命周期表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("ModelId").AsString(128).WithColumnDescription("模型编码")
                .WithColumn("BeginDate").AsDateTime().WithColumnDescription("统计开始时间")
                .WithColumn("EndDate").AsDateTime().WithColumnDescription("统计结束时间")
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
