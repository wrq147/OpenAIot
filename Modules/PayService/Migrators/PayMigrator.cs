using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayService.Migrators
{
    [Migration(20250524002)]
    public class PayMigrator : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_pay_detail");
            Create.Table("mz_pay_detail").WithDescription("统一交易记录表：合并充值记录、直接支付记录和零钱记录")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().WithColumnDescription("所属企业Id")
.WithColumn("PayUser").AsInt64().WithColumnDescription("支付人")
.WithColumn("PayChannelId").AsString(128).WithColumnDescription("支付渠道Id")
.WithColumn("OrderType").AsInt32().WithColumnDescription("订单类型")
.WithColumn("OrderId").AsString(128).WithColumnDescription("订单Id")
.WithColumn("Status").AsInt32().WithColumnDescription("0未支付，1支付成功，2支付失败，3退款中，4退款失败，5退款成功")
.WithColumn("PayAmount").AsDecimal(10, 2).WithColumnDescription("支付金额")
.WithColumn("Fee").AsDecimal(5, 4).WithColumnDescription("交易手续费")
.WithColumn("ErrorMsg").AsString(500).WithColumnDescription("错误信息")
.WithColumn("Remark").AsString(500).WithColumnDescription("备注");


            Execute.Sql("DROP TABLE IF EXISTS mz_pay_channels");
            Create.Table("mz_pay_channels").WithDescription("支付渠道表")
                 .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("支付渠道ID")
                 .WithColumn("OrgId").AsInt64().WithColumnDescription("所属企业Id,为0表示系统渠道")
                 .WithColumn("ChannelName").AsString(50).WithColumnDescription("支付渠道名称")
                 .WithColumn("ChannelLabel‌").AsString(20).WithColumnDescription("渠道标识符：支付宝、微信")
                 .WithColumn("AppId").AsString(32).WithColumnDescription("商户的appid")
                 .WithColumn("PublicKey").AsString(2000).WithColumnDescription("支付公钥")
                 .WithColumn("MerchantPrivateKey").AsString(2000).WithColumnDescription("商户私钥")
                 .WithColumn("OtherConfig").AsString(1000).WithColumnDescription("其他配置")
                 .WithColumn("EncryptKey").AsString(255).WithColumnDescription("存储加密的密钥")
                 .WithColumn("FeeRate").AsDecimal(5, 4).WithColumnDescription("手续费率，按比例收取");



            Execute.Sql("DROP TABLE IF EXISTS mz_wallet");
            Create.Table("mz_wallet").WithDescription("钱包表")
                .WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("用户ID")
                .WithColumn("TotalBalance").AsDecimal(10, 2).WithDefaultValue(0.0).WithColumnDescription("钱包当前余额")
                .WithColumn("CurrencyCode").AsString(10).WithDefaultValue("CNY").WithColumnDescription("钱包货币类型，ISO 4217标准代码")
                .WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间")
                .WithColumn("UpdatedOn").AsDateTime().WithColumnDescription("更新时间");


        }
        public override void Down()
        {
        }

    }
}
