using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Migrations
{
    [Migration(20250428001)]
    public class ReportMgrator7 : Migration
    {
        public override void Up()
        {

            this.Execute.Sql("delete FROM mz_api_source where Id='$System1'");
            Insert.IntoTable("mz_api_source").Row(new
            {
                Id = "$System1",
                OrgId = 0,
                InterfaceName = "证书模板接口",
                ApiType = "1",
                Url = "/ProducerService/Agent/CertInfo",
                Method = "GET",
                ParamJson = "{}",
                ParamType = "JSON",
                HeaderJson = "[{\"name\":\"Authorization\",\"value\":\"$Authorization\"}]",
                createId = 0,
                create_time = DateTime.Now,
                updateId = 0,
                update_time = DateTime.Now
            });
            this.Execute.Sql("delete FROM mz_print_data where Id='$PrintData1'");
            Insert.IntoTable("mz_print_data").Row(new
            {
                Id = "$PrintData1",
                Name = "证书数据源",
                DataMap = @"[{
		""id"": ""$cer0001"",
		""title"": ""代理商名称"",
		""field"": ""OrgName"",
		""typeName"": ""String""
	},
	{
		""id"": ""$cer0002"",
		""title"": ""上级代理商"",
		""field"": ""ParentOrgName"",
		""typeName"": ""String""
	},
	{
		""id"": ""$cer0003"",
		""title"": ""代理级别"",
		""field"": ""GradeName"",
		""typeName"": ""String""
	},
	{
		""id"": ""$cer0004"",
		""title"": ""生产商名称"",
		""field"": ""FactoryName"",
		""typeName"": ""String""
	},
	{
		""id"": ""$cer0005"",
		""title"": ""代理区域名称"",
		""field"": ""RegionsName"",
		""typeName"": ""String""
	},
	{
		""id"": ""$cer0006"",
		""title"": ""生产商Logo"",
		""field"": ""Logo"",
		""typeName"": ""String""
	},
	{
		""id"": ""$cer0007"",
		""title"": ""生产商省市区地址"",
		""field"": ""AddressName"",
		""typeName"": ""String""
	},
	{
		""id"": ""$cer0008"",
		""title"": ""生产商详细地址"",
		""field"": ""AddressDetail"",
		""typeName"": ""String""
	},
	{
		""id"": ""$cer0009"",
		""title"": ""生产商简介"",
		""field"": ""Intro"",
		""typeName"": ""String""
	}
]",
                ApiId = "$System1"
            });



            this.Execute.Sql("delete FROM mz_api_source where Id='$System2'");
            Insert.IntoTable("mz_api_source").Row(new
            {
                Id = "$System2",
                OrgId = 0,
                InterfaceName = "设备模板接口",
                ApiType = "1",
                Url = "/AfterService/Dev/DevInfo",
                Method = "GET",
                ParamJson = "{}",
                ParamType = "JSON",
                HeaderJson = "[{\"name\":\"Authorization\",\"value\":\"$Authorization\"}]",
                createId = 0,
                create_time = DateTime.Now,
                updateId = 0,
                update_time = DateTime.Now
            });




            this.Execute.Sql("delete FROM mz_print_data where Id='$PrintData2'");
            Insert.IntoTable("mz_print_data").Row(new
            {
                Id = "$PrintData2",
                Name = "设备数据源",
                DataMap = @"[{
		""id"": ""$dev0001"",
		""title"": ""设备名称"",
		""field"": ""Name"",
		""typeName"": ""String""
	},
	{
		""id"": ""$dev0002"",
		""title"": ""第三方编码"",
		""field"": ""DeviceNumber"",
		""typeName"": ""String""
	},
	{
		""id"": ""$dev0003"",
		""title"": ""规格编码"",
		""field"": ""SkuNumber"",
		""typeName"": ""String""
	},
	{
		""id"": ""$dev0004"",
		""title"": ""拥有者组织ID"",
		""field"": ""OwnerOrgId"",
		""typeName"": ""String""
	},
	{
		""id"": ""$dev0005"",
		""title"": ""设备来源组织ID"",
		""field"": ""OrgId"",
		""typeName"": ""String""
	},
	{
		""id"": ""$dev0006"",
		""title"": ""通讯编码"",
		""field"": ""DeviceId"",
		""typeName"": ""String""
	},
	{
		""id"": ""$dev0007"",
		""title"": ""设备ID"",
		""field"": ""Id"",
		""typeName"": ""String""
	},
	{
		""id"": ""$dev0008"",
		""title"": ""设备房间"",
		""field"": ""DeviceRoom"",
		""typeName"": ""String""
	}
]",
                ApiId = "$System2"
            });

        }
        public override void Down()
        {

        }

    }
}
