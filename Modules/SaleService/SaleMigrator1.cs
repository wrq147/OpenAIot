using Common;
using FluentMigrator;
using System;
namespace SaleService
{
    [Migration(20250401001)]
    public class SaleMigrator1 : Migration
    {
        public override void Up()
        {
            if (Schema.Table("mz_sales_order").Exists())
            {
                Delete.Table("mz_sales_order");
            }
            Create.Table("mz_sales_order").WithDescription("销售订单")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织")
.WithColumn("PayType").AsString(50).WithColumnDescription("支付方式，枚举值：QQ,WEIXIN,ALIPAY,LIANLIANPAY")
.WithColumn("PayNo").AsString(128).WithColumnDescription("支付单号")
.WithColumn("PayAmount").AsDecimal(10, 2).WithColumnDescription("支付金额（元）支付金额=商品金额-折扣金额+邮费+服务费")
.WithColumn("Postage").AsDouble().WithColumnDescription("邮费")
.WithColumn("ReceiverName").AsString(6).WithColumnDescription("收件人姓名")
.WithColumn("ReceiverPhone").AsString(50).WithColumnDescription("收件人电话")
.WithColumn("ExpressNumber").AsString(255).WithColumnDescription("物流单号")
.WithColumn("ExpressCompany").AsString(50).WithColumnDescription("物流公司")
.WithColumn("AddressCode").AsString(6).WithColumnDescription("收件人省市区代码")
.WithColumn("AddressName").AsString(255).WithColumnDescription("收件人地址名称")
.WithColumn("AddressDetail").AsString(255).WithColumnDescription("收件人详细地址")
.WithColumn("Status").AsInt32().WithColumnDescription("状态 0：待支付 1：待发货 2：待收货 3：已签收")
.WithColumn("RefundStatus").AsInt32().WithColumnDescription("退款状态 1：无售后或售后关闭，2：售后处理中，3：退款中，4： 退款成功")
.WithColumn("AfterStatus").AsInt32().WithColumnDescription("0：无售后  2：买家申请退款，待商家处理 3：退货退款，待商家处理 4：商家同意退款，退款中 5：平台同意退款，退款中 6：驳回退款，待买家处理 7：已同意退货退款,待用户发货 8：平台处理中 9：平台拒绝退款，退款关闭 10：退款成功 11：买家撤销 12：买家逾期未处理，退款失败 13：买家逾期，超过有效期 14：换货补寄待商家处理 15：换货补寄待用户处理 16：换货补寄成功 17：换货补寄失败 18：换货补寄待用户确认完成 21：待商家同意维修 22：待用户确认发货 24：维修关闭 25：维修成功 27：待用户确认收货 31：已同意拒收退款，待用户拒收 32：补寄待商家发货 33：同意召回后退款，待商家召回")
.WithColumn("IsCheck").AsBoolean().WithColumnDescription("订单是否在审核中")
.WithColumn("IsReturnFreightPayer").AsBoolean().WithColumnDescription("订单是否退货包运费")
.WithColumn("IsDeliveryOneDay").AsBoolean().WithColumnDescription("是否当日发货")
.WithColumn("Remark").AsString(500).WithColumnDescription("商家订单备注")
.WithColumn("FromWay").AsString(50).WithColumnDescription("来源方式  为空表示无来源，PDD为拼多多，JD为京东，TB为淘宝，DY为抖音")
.WithColumn("FromSN").AsString(128).WithColumnDescription("来源订单号")
.WithColumn("PayOn").AsDateTime().Nullable().WithColumnDescription("支付时间")
.WithColumn("ShippingOn").AsDateTime().Nullable().WithColumnDescription("发货时间")
.WithColumn("ReceiveOn").AsDateTime().Nullable().WithColumnDescription("签收时间")
.WithColumn("UpdatedAt").AsDateTime().WithColumnDescription("最近一次更新时间");

            Create.Index("IDXSalesOrderFrom").OnTable("mz_sales_order").OnColumn("FromWay").Ascending().OnColumn("FromSN").Ascending();

            if (Schema.Table("mz_sales_item").Exists())
            {
                Delete.Table("mz_sales_item");
            }
            Create.Table("mz_sales_item").WithDescription("订单中商品")
                .WithColumn("OrderId").AsString(128).PrimaryKey().WithColumnDescription("关联订单Id")
                .WithColumn("GoodsId").AsString(128).PrimaryKey().WithColumnDescription("关联商品Id")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织")
                .WithColumn("GoodsCount").AsInt32().WithColumnDescription("商品数量")
                .WithColumn("GoodsName").AsString(50).WithColumnDescription("商品名称")
                .WithColumn("SkuOriPrice").AsDecimal(10, 2).WithColumnDescription("原价")
                .WithColumn("SkuSellPrice").AsDecimal(10, 2).WithColumnDescription("销售价")
                .WithColumn("SkuPath").AsString(255).WithColumnDescription("显示用规格:规格值1,规格值2")
                .WithColumn("SkuId").AsString(128).WithColumnDescription("商品规格编码")
                .WithColumn("SkuImg").AsString(255).WithColumnDescription("商品规格对应的图片");

            this.Execute.Sql("delete FROM mz_menu where menu_id=6250");
            this.Execute.Sql("delete FROM mz_menu where menu_id=6260");
            this.Execute.Sql("delete FROM mz_menu where menu_id=6270");
            this.Execute.Sql("delete FROM mz_menu where menu_id=6280");

            this.Execute.Sql("delete FROM mz_menu where menu_id=9");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 9,
                menu_name = "销售平台",
                parent_id = 0,
                order_num = 9,
                path = "sale",
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "M",
                visible = "0",
                status = "0",
                perms = "/Sales/",
                icon = "jinxiaocun",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6250,
                menu_name = "经营分析",
                parent_id = 9,
                order_num = 1,
                path = "analysis/index",
                component = "sale/analysis/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/SaleService/Analysis/Index",
                icon = "kucunchaxun",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6260,
                menu_name = "销售订单",
                parent_id = 9,
                order_num = 2,
                path = "order/list",
                component = "sale/order/list",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/SaleService/Order/List",
                icon = "kucunchaxun",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6270,
                menu_name = "商品列表",
                parent_id = 9,
                order_num = 3,
                path = "goods/list",
                component = "sale/goods/list",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/SaleService/Goods/List",
                icon = "cangkuguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6280,
                menu_name = "商品分类",
                parent_id = 9,
                order_num = 4,
                path = "goods/categorylist",
                component = "sale/goods/categorylist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/SaleService/Category/List",
                icon = "cangkuguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            if (Schema.Table("mz_goods").Exists())
            {
                Delete.Table("mz_goods");
            }
            Create.Table("mz_goods").WithDescription("销售的商品")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织")
                .WithColumn("GoodsName").AsString(50).WithColumnDescription("商品名称")
                .WithColumn("KeyWords").AsString(2000).WithColumnDescription("搜索关键字")
                .WithColumn("CategoryId").AsString(128).WithColumnDescription("商品所属分类")
                .WithColumn("CategoryPath").AsString(500).WithColumnDescription("分类路径")
                .WithColumn("SoldNum").AsInt32().WithColumnDescription("累计销量")
                .WithColumn("GoodsStatus").AsInt32().WithDefaultValue(1).Indexed().WithColumnDescription("状态：默认值为1表示正常，0下架，")
                .WithColumn("GoodsDetail").AsString(5000).WithDefaultValue(string.Empty).WithColumnDescription("商品详情")
                .WithColumn("GoodsPrice").AsDecimal(14, 2).WithColumnDescription("商品列表显示用价格")
                .WithColumn("CreatedOn").AsDateTime().WithColumnDescription("商品创建时间")
                .WithColumn("UpdatedOn").AsDateTime().WithColumnDescription("商品最后更新时间");

            Execute.Sql("ALTER TABLE mz_goods ADD FULLTEXT INDEX GoodsKeywords (KeyWords);");

            if (Schema.Table("mz_goods_category").Exists())
            {
                Delete.Table("mz_goods_category");
            }
            Create.Table("mz_goods_category").WithDescription("商品的分类")
               .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
               .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织")
               .WithColumn("Name").AsString(50).WithColumnDescription("分类名称")
               .WithColumn("ParentId").AsString(128).WithColumnDescription("父分类Id")
               .WithColumn("ImgUrl").AsString(255).WithColumnDescription("分类图片")
               .WithColumn("Path").AsString(500).WithColumnDescription("分类层级")
               .WithColumn("Sort").AsInt32().WithColumnDescription("排序值：越小越前面");


            if (Schema.Table("mz_goods_pic").Exists())
            {
                Delete.Table("mz_goods_pic");
            }
            Create.Table("mz_goods_pic").WithDescription("商品的图片")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织")
                .WithColumn("GoodsId").AsString(128).Indexed().WithColumnDescription("关联的商品Id")
                .WithColumn("PathUrl").AsString(255).WithColumnDescription("图片路径")
                .WithColumn("PathType").AsFixedLengthAnsiString(1).WithColumnDescription("V代表视频，P代表图片")
                .WithColumn("IsMain").AsBoolean().WithColumnDescription("是否主图")
                .WithColumn("Sort").AsInt32().WithColumnDescription("排序值：越小越前面");



            if (Schema.Table("mz_goods_sku_template").Exists())
            {
                Delete.Table("mz_goods_sku_template");
            }
            Create.Table("mz_goods_sku_template").WithDescription("商品的SKU模板")
    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
    .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织")
    .WithColumn("GoodsId").AsString(128).Indexed().WithColumnDescription("关联的商品Id")
    .WithColumn("SkuName").AsString(50).WithColumnDescription("规格名称")
    .WithColumn("SkuValues").AsString(255).WithColumnDescription("规格可选值");


            if (Schema.Table("mz_goods_sku").Exists())
            {
                Delete.Table("mz_goods_sku");
            }

            Create.Table("mz_goods_sku").WithDescription("商品的SKU")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
                .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织")
                .WithColumn("GoodsId").AsString(128).Indexed().WithColumnDescription("关联的商品Id")
                .WithColumn("SkuImgUrl").AsString(255).WithColumnDescription("sku图片路径")
                .WithColumn("SkuNumber").AsString(128).WithColumnDescription("sku编码（对应产品的编码）")
                .WithColumn("SkuPath").AsString(255).WithColumnDescription("显示用规格:规格值1,规格值2")
                .WithColumn("SkuOriPrice").AsDecimal(14, 2).WithColumnDescription("原价")
                .WithColumn("SkuSellPrice").AsDecimal(14, 2).WithColumnDescription("销售价")
                .WithColumn("Stocks").AsInt32().WithColumnDescription("库存");

        }
        public override void Down()
        {
        }
    }
}
