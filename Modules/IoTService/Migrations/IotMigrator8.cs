using FluentMigrator;
using InfluxDB.Client.Configurations;
using System;


namespace IoTService.Migrations
{
    [Migration(20250930001)]
    public class IotMigrator8 : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_iot_his_source");
            Create.Table("mz_iot_his_source").WithDescription("历史数据存储源")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("HisSourceOrgId").WithColumnDescription("所属组织ID")
.WithColumn("Name").AsString(50).WithColumnDescription("数据源名称")
.WithColumn("StorageType").AsFixedLengthString(10).WithColumnDescription("数据源类型：influx（默认）")
.WithColumn("StorageConfig").AsString(3000).WithColumnDescription("历史数据存储配置Json")
.WithColumn("Remark").AsString(500).WithColumnDescription("备注")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            this.Execute.Sql("delete FROM mz_menu where menu_id=4431");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 4431,
                menu_name = "存储数据源",
                parent_id = 4000,
                order_num = 12,
                path = "HistorySource/List",
                component = "iot/HistorySource/List",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTService/IotSource/ListPage",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });


            Insert.IntoTable("mz_iot_his_source").Row(new
            {
                Id = 1,
                OrgId = 0,
                Name = "云端Influx数据库",
                StorageType = "influx",
                StorageConfig = "{\"org\":\"hd.mz\",\"url\":\"http://172.18.0.3:8086/\",\"token\":\"hmL3fiChMcW-WxvQWYCE3uRsH3hKiqMg0vX1eMuuSVn2fQtQapFRE0rKsZbP3JSYlhYNvxjuiURJA0lU_sYFDA==\",\"bucket\":\"MZIoT\"}",
                Remark = "默认云端存储物联的历史数据",
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
