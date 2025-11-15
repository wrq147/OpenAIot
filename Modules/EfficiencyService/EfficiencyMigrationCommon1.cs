using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace EfficiencyService
{
    [Migration(20250826788)]
    public class EfficiencyMigrationCommon1: Migration
    {
        public override void Up()
        {
            //排放类别表
            Execute.Sql("DROP TABLE IF EXISTS t_com_class");
            Create.Table("t_com_class").WithDescription("排放类别表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("ClassNo").AsString(128).WithColumnDescription("排放类别序号")
                .WithColumn("ClassName").AsString(128).WithColumnDescription("排放类别名称")
                .WithColumn("RangeId").AsString(128).WithColumnDescription("排放范围（1代表范围1 2代表范围2 3代表范围3）")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
                .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            //排放子类别表
            Execute.Sql("DROP TABLE IF EXISTS t_com_subclass");
            Create.Table("t_com_subclass").WithDescription("排放类别表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("ClassId").AsString(128).WithColumnDescription("排放类别编码")
                .WithColumn("SubClassName").AsString(128).WithColumnDescription("子排放类别名称")
                .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）");

            //企业排放类别表
            Execute.Sql("DROP TABLE IF EXISTS t_com_orgclass");
            Create.Table("t_com_orgclass").WithDescription("企业排放类别表")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
                .WithColumn("ClassId").AsString(128).WithColumnDescription("类别编码")
                .WithColumn("SubClassId").AsString(128).WithColumnDescription("子类别编码")
                .WithColumn("FacilityId").AsString(128).WithColumnDescription("设施编码")
                .WithColumn("FactorId").AsString(128).WithColumnDescription("排放因子编码")
                .WithColumn("FactorType").AsString(128).WithColumnDescription("排放类型编码")
                .WithColumn("DataSource").AsString(128).WithColumnDescription("活动数据来源（1代表计量设备 2代表手工录入 3供应链数据）")
                .WithColumn("EquipmentIds").AsString(3200).WithColumnDescription("关联设备编码")
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
