using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Migrations
{
    [Migration(20240713019)]
    public class IotMigrator7 : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_prop_except");
            Create.Table("mz_prop_except").WithDescription("属性异常表")
.WithColumn("Id").AsInt64().Identity().PrimaryKey().WithColumnDescription("Id")
.WithColumn("DtuId").AsString(128).Indexed().WithColumnDescription("通讯Id")
.WithColumn("PropCode").AsString(50).Indexed().WithColumnDescription("属性的标识符")
.WithColumn("ExceptType").AsString(50).WithColumnDescription("异常类型：峰值spike、更改change")
.WithColumn("ExceptValue").AsString(2000).WithColumnDescription("异常值")
.WithColumn("CreatedOn").AsInt64().WithColumnDescription("发生时间");


            Create.Index("IDXDeviceProp").OnTable("mz_prop_except").OnColumn("DtuId").Ascending().OnColumn("PropCode").Ascending();

        }
        public override void Down()
        {
        }
    }
}
