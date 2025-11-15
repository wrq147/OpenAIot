using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfficiencyService.Controller;
using FluentMigrator;

namespace EfficiencyService
{
    [Migration(20250817777)]
    public class EfficiencyMigrationCommon: Migration
    {
        public override void Up()
        {
            //产品表
            Execute.Sql("DROP TABLE IF EXISTS t_com_product");
            Create.Table("t_com_product").WithDescription("产品表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("ProductModel").AsString(128).WithColumnDescription("产品型号")
                .WithColumn("ProductName").AsString(128).WithColumnDescription("产品名称")
                .WithColumn("ShortName").AsString(128).WithColumnDescription("产品简称或缩写")
                .WithColumn("BrandName").AsString(128).WithColumnDescription("品牌名称")
                .WithColumn("ProductType").AsString(128).WithColumnDescription("产品形态 1代表成品 2代表配套件")
                .WithColumn("ProductPrice").AsDouble().WithColumnDescription("零售单价（元）")
                .WithColumn("Unit").AsString(128).WithColumnDescription("单位")
                .WithColumn("OnMarket").AsString(128).WithColumnDescription("是否上市 是/否")
                .WithColumn("MarketTime").AsDateTime().WithColumnDescription("上市时间")
                .WithColumn("Memo").AsString(500).WithColumnDescription("产品说明")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //设备表
            Execute.Sql("DROP TABLE IF EXISTS t_com_equipment");
            Create.Table("t_com_equipment").WithDescription("设备表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("EquipmentCode").AsString(128).WithColumnDescription("设备代码")
                .WithColumn("EquipmentName").AsString(128).WithColumnDescription("设备名称")
                .WithColumn("DataState").AsString(128).WithColumnDescription("数据状态（1代表纳入计算 2代表不纳入计算）")
                .WithColumn("PolicyId").AsString(128).WithColumnDescription("计费政策编码")
                .WithColumn("ThirdId").AsString(128).WithColumnDescription("第三方编码")
                .WithColumn("FacilityId").AsString(128).WithColumnDescription("设施编码")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //设施表
            Execute.Sql("DROP TABLE IF EXISTS t_com_facility");
            Create.Table("t_com_facility").WithDescription("设施表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("FacilityName").AsString(128).WithColumnDescription("设施名称")
                .WithColumn("FacilityCode").AsString(128).WithColumnDescription("设施代码")
                .WithColumn("Manager").AsString(128).WithColumnDescription("负责人")
                .WithColumn("Contact").AsString(128).WithColumnDescription("联系方式")
                .WithColumn("ParentId").AsString(128).WithColumnDescription("父节点ID")
                .WithColumn("FacilityType").AsBoolean().WithColumnDescription("设施分类（False代表目录级 Ture代表绑定设备级）")
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
