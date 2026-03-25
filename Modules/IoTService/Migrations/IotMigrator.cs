using Common;
using FluentMigrator;
using System;

namespace IotService.Migrations
{
    [Migration(20221129001)]
    public class IotMigrator : Migration
    {
        public override void Up()
        {
            Create.Table("mz_iot_product").WithDescription("Iot协议")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed("IotProductOrgId").WithColumnDescription("所属组织ID")
                .WithColumn("Name").AsString(50).WithColumnDescription("协议名称")
                .WithColumn("PhotoUrl").AsString(255).WithColumnDescription("图片地址")
                .WithColumn("Remark").AsString(500).WithColumnDescription("备注说明")
                .WithColumn("ClassifiedId").AsString(128).Indexed("IDXClassifiedId").WithColumnDescription("所属品类ID")
                .WithColumn("NetworkWay").AsString(50).WithColumnDescription("设备接入方式：mqmodus、mq、http、tcp，为空则无物联")
                .WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0未发布、1已发布）")
                .WithColumn("PhysicsWay").AsString(20).WithColumnDescription("设备通信方式：WiFi、以太网、2G网络、3G网络、4G网络、5G网络、NB-IoT")
                .WithColumn("NoticeWay").AsString(20000).WithColumnDescription("告警通知方式")
                .WithColumn("InterScripts").AsString(20000).WithColumnDescription("数据解释脚本")
                .WithColumn("StorageConfig").AsString(3000).WithColumnDescription("历史数据存储配置Json")
                .WithColumn("ModelTSL").AsString(20000).WithColumnDescription("物模型Json")
                .WithColumn("Version").AsInt32().WithDefaultValue(0).WithColumnDescription("当前版本号")
                .WithColumn("PublicTime").AsDateTime().Nullable().WithColumnDescription("最后一次发布时间")
                .WithColumn("MonitorReportToken").AsString(2000).Nullable().WithColumnDescription("画面监控报表")
                .WithColumn("TSLUpdated").AsDateTime().Nullable().WithColumnDescription("物模型变更时间")
       .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
       .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
       .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
       .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Create.Table("mz_iot_class").WithDescription("Iot协议分类")
    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
    .WithColumn("OrgId").AsInt64().Indexed("IotClassOrgId").WithColumnDescription("所属组织ID")
    .WithColumn("Name").AsString(50).WithColumnDescription("分类名称")
    .WithColumn("PhotoUrl").AsString(255).WithColumnDescription("图片地址")
    .WithColumn("Sort").AsInt32().WithColumnDescription("排序值：越小越前面")
    .WithColumn("Remark").AsString(255).WithColumnDescription("备注说明")
    .WithColumn("ParentId").AsString(128).WithColumnDescription("父分类Id")
    .WithColumn("Path").AsString(800).Indexed().WithColumnDescription("分类层级");


            Execute.Sql("DROP VIEW IF EXISTS mz_iot_product_v;");
            Execute.Sql("CREATE VIEW mz_iot_product_v as select p.*,c.Path from mz_iot_product p left join mz_iot_class c on p.ClassifiedId=c.Id");

            Insert.IntoTable("mz_iot_class").Row(new
            {
                Id = "1",
                OrgId = 0,
                Name = "其它类别",
                PhotoUrl = string.Empty,
                Sort = 0,
                Remark = string.Empty,
                ParentId = string.Empty,
                Path = "1,"
            });
            Insert.IntoTable("mz_iot_product").Row(new
            {
                Id = "1",
                OrgId = 0,
                Name = "其它协议",
                PhotoUrl = string.Empty,
                Remark = string.Empty,
                ClassifiedId = "1",
                NetworkWay = string.Empty,
                Status = "1",
                PhysicsWay = string.Empty,
                NoticeWay = string.Empty,
                InterScripts = string.Empty,
                StorageConfig = "{\"enable\":\"0\"}",
                ModelTSL = "{\"tags\":[],\"properties\":[],\"functions\":[],\"events\":[]}",
                Version = 0,
                PublicTime = DateTime.Now,
                MonitorReportToken = string.Empty,
                TSLUpdated = DateTime.Now,
                createId = 2,
                create_time = DateTime.Now,
                updateId = 2,
                update_time = DateTime.Now
            });


            Create.Table("mz_iot_device").WithDescription("Iot设备")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("IotDeviceOrgId").WithColumnDescription("来源组织ID")
.WithColumn("OwnerOrgId").AsInt64().Indexed("IotDeviceOwnerOrgId").WithColumnDescription("拥有者组织ID")
.WithColumn("UseOrgId").AsInt64().Indexed("IotDeviceUseOrgId").WithColumnDescription("使用者组织ID")
.WithColumn("UseUserId").AsInt64().Indexed("IotDeviceUseUserId").WithColumnDescription("当前使用者")
.WithColumn("PhotoUrl").AsString(255).WithColumnDescription("图片地址")
.WithColumn("DeviceNumber").AsString(50).Unique().WithColumnDescription("设备唯一编号")
.WithColumn("ProductId").AsString(128).Indexed().WithColumnDescription("所属协议Id")
.WithColumn("MesProductId").AsString(128).Indexed().WithColumnDescription("产品Id：为1表示物联产品")
.WithColumn("Online").AsByte().WithColumnDescription("联网状态：0为离线，1为在线，2为未初始化")
.WithColumn("DState").AsString(50).Indexed().WithDefaultValue("").WithColumnDescription("运行状态")
.WithColumn("Name").AsString(50).WithColumnDescription("设备名称")
.WithColumn("DeviceId").AsString(128).Nullable().Indexed("UNDeviceID").WithColumnDescription("设备通讯用的Id")
.WithColumn("Lng").AsDouble().Nullable().WithColumnDescription("经度")
.WithColumn("Lat").AsDouble().Nullable().WithColumnDescription("纬度")
.WithColumn("AreaCode").AsString(45).Nullable().WithColumnDescription("设备所在区域代码")
.WithColumn("GeoHash").AsString(20).Nullable().WithColumnDescription("经纬度对应geohash码值")
.WithColumn("CreateOn").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("LastOnline").AsDateTime().Nullable().WithColumnDescription("最后在线时间")
.WithColumn("ProductVer").AsInt32().WithDefaultValue(0).WithColumnDescription("协议版本")
                .WithColumn("Remark").AsString(5000).WithColumnDescription("备注说明")
                .WithColumn("OwnerOrgPath").AsString(500).WithColumnDescription("设备经过的组织路径")
                .WithColumn("DeviceUpIdx").AsInt32().Indexed().WithDefaultValue(0).WithColumnDescription("设备所属处理节点索引")
                .WithColumn("KeyWords").AsString(2000).WithDefaultValue("").WithColumnDescription("设备关键词")
                .WithColumn("NeedUpdateKey").AsBoolean().Indexed().WithDefaultValue(false).WithColumnDescription("设备是否需要更新关键词")
                .WithColumn("dBm").AsFloat().Nullable().WithDefaultValue(0).WithColumnDescription("信号强度");


            Create.Index().OnTable("mz_iot_device").OnColumn("ProductId").Ascending().OnColumn("OrgId").Ascending();

            if (Constants.General.sqltype != "Sqlite")
            {
                Execute.Sql("ALTER TABLE mz_iot_device ADD FULLTEXT INDEX DeviceKeywords (KeyWords);");
                Execute.Sql("ALTER TABLE mz_iot_device ADD FULLTEXT INDEX DEVICEOwnerPath (OwnerOrgPath);");
            }

            Create.Table("mz_iot_device_tag").WithDescription("Iot设备标签")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("设备Id")
.WithColumn("Code").AsString(255).PrimaryKey().WithColumnDescription("标签标识")
.WithColumn("Name").AsString(50).WithColumnDescription("标签名称")
.WithColumn("Value").AsString(500).Nullable().Indexed("TAG_VALUE_IDX").WithColumnDescription("扩展信息值（字符串存储）")
.WithColumn("NumValue").AsDouble().Nullable().Indexed("TAG_NUMVALUE_IDX").WithColumnDescription("扩展信息值（数值存储）");


            Execute.Sql("DROP TABLE IF EXISTS mz_iot_warn_config");
            Create.Table("mz_iot_warn_config").WithDescription("告警工单配置")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
.WithColumn("ProductId").AsString(128).Unique().WithColumnDescription("所属物联协议Id")
.WithColumn("WarnFlowId").AsInt64().WithColumnDescription("告警工单执行流程")
.WithColumn("WarnFlowInitJson").AsString(50000).WithColumnDescription("表单初始化映射");

            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 4000,
                menu_name = "物联网开发",
                parent_id = 0,
                order_num = 7,
                path = "iot",
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "M",
                visible = "0",
                status = "0",
                perms = "/IoTService/",
                icon = "a-guizeyinqing",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 4204,
                menu_name = "开发协议",
                parent_id = 4000,
                order_num = 1,
                path = "physicalModel/productList",
                component = "iot/physicalModel/productList",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTService/IotProduct/ListPage",
                icon = "a-kaifachanpin",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 4205,
                menu_name = "协议分类",
                parent_id = 4000,
                order_num = 2,
                path = "physicalModel/productClass",
                component = "iot/physicalModel/productClass",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTService/IotClass/ListTree",
                icon = "a-chanpinfenlei",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 4206,
                menu_name = "开发设备",
                parent_id = 4000,
                order_num = 3,
                path = "deviceManage/index",
                component = "iot/deviceManage/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTService/IotDevice/ListPage",
                icon = "a-shebeiguanli",
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
