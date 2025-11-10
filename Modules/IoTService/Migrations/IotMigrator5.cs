using FluentMigrator;
using System;
namespace IoTService.Migrations
{
    [Migration(20240409009)]
    public class IotMigrator5 : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_iot_card");
            Create.Table("mz_iot_card").WithDescription("物联网卡表")
    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("卡Id")
    .WithColumn("OrgId").AsInt64().Indexed("IotCardOrgId").WithColumnDescription("所属组织ID")
    .WithColumn("ICCID").AsString().Unique().WithColumnDescription("ICCID号")
    .WithColumn("IMSI").AsString().WithColumnDescription("IMSI号")
    .WithColumn("MSISDN").AsString().WithColumnDescription("对应的手机号码")
    .WithColumn("CardFrom").AsString().WithColumnDescription("卡来源：YiDong、SimBoss、Sohan、Unicom、Unknow")
    .WithColumn("SpeedLimit").AsDouble().WithColumnDescription("网络限速值，单位：Kbps，4G上限为150Mbps(153600Kbps)")
    .WithColumn("Carrier").AsString().WithColumnDescription("运营商")
    .WithColumn("CardType").AsString().WithColumnDescription("SINGLE：单卡，POOL：流量池卡")
    .WithColumn("CardPoolId").AsString().WithColumnDescription("流量池id，CardType为POOL的才有此字段")
    .WithColumn("RatePlanName").AsString().WithColumnDescription("卡当前套餐名称")
    .WithColumn("RatePlanId").AsString().WithColumnDescription("卡当前套餐ID")
    .WithColumn("TotalDataVolume").AsDouble().WithColumnDescription("卡套餐大小, 单位M")
    .WithColumn("UsedDataVolume").AsDouble().WithColumnDescription("卡套餐用量")
    .WithColumn("UseCountAsVolume").AsBoolean().WithColumnDescription("卡套餐单位，false表示按流量（MB），true表示按次数")
    .WithColumn("StartDate").AsDateTime().Nullable().WithColumnDescription("激活时间")
    .WithColumn("ExpirationDate").AsDateTime().Nullable().Indexed("IotCardOverDate").WithColumnDescription("到期时间")
    .WithColumn("LastSyncDate").AsDateTime().Nullable().WithColumnDescription("最后一次同步时间")
    .WithColumn("UsingDevice").AsString().Indexed().WithColumnDescription("使用中的设备Id")
    .WithColumn("Status").AsString().WithColumnDescription("卡在运营商的状态, 可测试: TEST_READY_NAME, 库存：INVENTORY_NAME，可激活：ACTIVATION_READY_NAME， 已激活：ACTIVATED_NAME， 已停卡：DEACTIVATED_NAME，已销卡：RETIRED_NAME, 已清除：PURGED_NAME")
    .WithColumn("UsingOn").AsDateTime().Nullable().WithColumnDescription("设备绑定时间")
    .WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间");

            Execute.Sql("DROP TABLE IF EXISTS mz_iot_config");
            Create.Table("mz_iot_config").WithDescription("企业物联信息配置表")
                    .WithColumn("OrgId").AsInt64().PrimaryKey().WithColumnDescription("企业Id")
                    .WithColumn("EnableAutoAdd").AsBoolean().WithColumnDescription("是否自动增加物联卡")
                    .WithColumn("YiDongOption").AsString(500).Nullable().WithColumnDescription("移动物联卡接口配置")
                    .WithColumn("SimBossOption").AsString(500).Nullable().WithColumnDescription("SimBoss物联卡接口配置")
                    .WithColumn("SohanOption").AsString(500).Nullable().WithColumnDescription("Sohan物联卡接口配置")
                    .WithColumn("UnicomOption").AsString(500).Nullable().WithColumnDescription("Unicom联通卡接口配置");


            this.Execute.Sql("delete FROM mz_menu where menu_id=4301");
            this.Execute.Sql("delete FROM mz_menu where menu_id=4302");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 4301,
                menu_name = "第三方接入",
                parent_id = 4000,
                order_num = 1,
                path = "config/index",
                component = "iot/config/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTService/IotConfig/Update",
                icon = "link",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 4302,
                menu_name = "物联卡管理",
                parent_id = 4000,
                order_num = 9,
                path = "card/index",
                component = "iot/card/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTService/IotCard/ListPage",
                icon = "a-caidanguanli",
                scope = 0,
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
