using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using FluentMigrator;

namespace EfficiencyService
{
    [Migration(20250915333)]
    public class EfficiencyMigrationProduction1 : Migration
    {
        public override void Up()
        {
            //供应商表
            Execute.Sql("DROP TABLE IF EXISTS t_prod_provider");
            Create.Table("t_prod_provider").WithDescription("供应商表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("ProviderCode").AsString(128).WithColumnDescription("供应商代码")
                .WithColumn("ProviderName").AsString(128).WithColumnDescription("供应商名称")
                .WithColumn("Manager").AsString(128).WithColumnDescription("联系人")
                .WithColumn("Contact").AsString(128).WithColumnDescription("联系电话")
                .WithColumn("Area").AsString(128).WithColumnDescription("所属区域")
                .WithColumn("ProviderAddress").AsString(1280).WithColumnDescription("供应商地址")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //物料表
            Execute.Sql("DROP TABLE IF EXISTS t_prod_material");
            Create.Table("t_prod_material").WithDescription("物料表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("MaterialCode").AsString(128).WithColumnDescription("物料代码")
                .WithColumn("MaterialName").AsString(128).WithColumnDescription("物料名称")
                .WithColumn("MaterialType").AsString(128).WithColumnDescription("物料类型")
                .WithColumn("MaterialUnit").AsString(128).WithColumnDescription("物料单位")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //供应商物料表
            Execute.Sql("DROP TABLE IF EXISTS t_prod_providermaterial");
            Create.Table("t_prod_providermaterial").WithDescription("供应商物料表")
                .WithColumn("MaterialId").AsString(128).WithColumnDescription("物料编码")
                .WithColumn("ProviderId").AsString(128).WithColumnDescription("供应商编码")
                .WithColumn("ProductModel").AsString(128).WithColumnDescription("规格型号");

            //物料碳足迹表
            Execute.Sql("DROP TABLE IF EXISTS t_prod_materialcarbon");
            Create.Table("t_prod_materialcarbon").WithDescription("物料碳足迹表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("MaterialId").AsString(128).WithColumnDescription("物料编码")
                .WithColumn("ProviderId").AsString(128).WithColumnDescription("供应商编码")
                .WithColumn("ProductBorder").AsString(128).WithColumnDescription("产品生命周期边界")
                .WithColumn("CarbonEmission").AsDouble().WithColumnDescription("碳排放量")
                .WithColumn("CarbonUnit").AsString(128).WithColumnDescription("碳排放量单位")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //碳排计划表
            Execute.Sql("DROP TABLE IF EXISTS t_prod_carbonplan");
            Create.Table("t_prod_carbonplan").WithDescription("碳排计划表")
                .WithColumn("Year").AsString(128).WithColumnDescription("年份")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("CarbonType").AsString(128).WithColumnDescription("碳排数据")
                .WithColumn("Unit").AsString(128).WithColumnDescription("单位")
                .WithColumn("Granularity").AsString(128).WithColumnDescription("数据粒度")
                .WithColumn("Month1").AsDouble().WithColumnDescription("1月")
                .WithColumn("Month2").AsDouble().WithColumnDescription("2月")
                .WithColumn("Month3").AsDouble().WithColumnDescription("3月")
                .WithColumn("Month4").AsDouble().WithColumnDescription("4月")
                .WithColumn("Month5").AsDouble().WithColumnDescription("5月")
                .WithColumn("Month6").AsDouble().WithColumnDescription("6月")
                .WithColumn("Month7").AsDouble().WithColumnDescription("7月")
                .WithColumn("Month8").AsDouble().WithColumnDescription("8月")
                .WithColumn("Month9").AsDouble().WithColumnDescription("9月")
                .WithColumn("Month10").AsDouble().WithColumnDescription("10月")
                .WithColumn("Month11").AsDouble().WithColumnDescription("11月")
                .WithColumn("Month12").AsDouble().WithColumnDescription("12月")
                .WithColumn("YearTotal").AsDouble().WithColumnDescription("年度合计");

        }
        public override void Down()
        {
        }
    }
}
