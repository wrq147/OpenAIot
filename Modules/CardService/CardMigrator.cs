using FluentMigrator;
using System;

namespace CardService
{
    [Migration(20220628002)]
    public class CardMigrator : Migration
    {
        public override void Up()
        {
            if (Schema.Table("mz_card").Exists())
            {
                return;
            }



            Create.Table("mz_card_org").WithDescription("组织扩展信息")
           .WithColumn("OrgId").AsInt64().PrimaryKey().WithColumnDescription("对应的组织Id")
           .WithColumn("BindId").AsString(128).Unique("xxBindOrgID").Nullable().WithColumnDescription("绑定的组织Id,由第三方的开发者Id和第三方企业Id组成")
           .WithColumn("ProConfig").AsString(1000).WithColumnDescription("产品模块配置")
           .WithColumn("CaseConfig").AsString(1000).WithColumnDescription("案例模块配置");

            Execute.Sql("CREATE VIEW mz_card_org_v as select o.*,oc.* from mz_org o left join mz_card_org oc on o.Id=oc.OrgId");

            Create.Table("mz_card").WithDescription("名片表")
    .WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("Id主键")
    .WithColumn("RealName").AsString(20).WithColumnDescription("真实姓名")
    .WithColumn("Mobile").AsString(11).WithColumnDescription("手机")
    .WithColumn("Email").AsString(50).WithColumnDescription("邮箱")
    .WithColumn("WxNumber").AsString(20).WithColumnDescription("微信号")
    .WithColumn("Website").AsString(255).WithColumnDescription("官网")
    .WithColumn("Avatar").AsString(255).WithColumnDescription("名片头像")
    .WithColumn("TemplateId").AsInt32().WithColumnDescription("名片风格")
    .WithColumn("TemplateBk").AsInt32().WithColumnDescription("名片风格背景图")
    .WithColumn("UserId").AsInt64().WithColumnDescription("所属用户")
    .WithColumn("OrgId").AsInt64().WithColumnDescription("所属组织,为0则无所属企业")
    .WithColumn("DeptName").AsString(30).WithColumnDescription("所属部门")
    .WithColumn("PostName").AsString(50).WithColumnDescription("职位,多个职位用逗号分隔")
    .WithColumn("ShareTitle").AsString(80).WithColumnDescription("分享标题")
    .WithColumn("Intro").AsString(3000).WithColumnDescription("个人简介")
    .WithColumn("Lng").AsDouble().WithColumnDescription("经度")
    .WithColumn("Lat").AsDouble().WithColumnDescription("纬度")
    .WithColumn("AddressCode").AsString(6).WithColumnDescription("省市区代码")
    .WithColumn("AddressName").AsString(255).WithColumnDescription("地址名称")
    .WithColumn("AddressDetail").AsString(255).WithColumnDescription("详细地址")
    .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
    .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
    .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
    .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
    .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Execute.Sql("CREATE VIEW mz_card_v as select c.*,o.OrgName from mz_card c left join mz_org o on c.OrgId=o.Id");




            Create.Table("mz_card_holder").WithDescription("通讯录")
.WithColumn("UserId").AsInt64().PrimaryKey().WithColumnDescription("关联账号")
.WithColumn("CardId").AsInt64().PrimaryKey().WithColumnDescription("目标名片")
.WithColumn("CreatedOn").AsDateTime().WithColumnDescription("添加时间");


            Create.Table("mz_card_msg").WithDescription("访客消息表")
.WithColumn("Id").AsInt64().PrimaryKey().Identity().WithColumnDescription("编号")
.WithColumn("UserId").AsInt64().WithColumnDescription("访问者")
.WithColumn("VisitCardId").AsInt64().WithColumnDescription("访问者名片")
.WithColumn("VisitSource").AsInt32().WithColumnDescription("0为其它")
.WithColumn("VisitType").AsInt32().WithColumnDescription("访问模块,0为名片,1为产品,2为案例")
.WithColumn("VisitNumber").AsInt32().WithColumnDescription("记录第几次访问")
.WithColumn("TargetId").AsInt64().Indexed("TargetIdTAA").WithColumnDescription("目标Id")
.WithColumn("ReceiveUserId").AsInt64().Indexed("ReceiveUserIdTAR").WithColumnDescription("消息接收人")
.WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0未读 1已读）")
.WithColumn("CreatedOn").AsDateTime().WithColumnDescription("访问时间")
.WithColumn("EndOn").AsDateTime().Nullable().WithColumnDescription("结束时间")
.WithColumn("ReadedOn").AsDateTime().Nullable().WithColumnDescription("读取时间");


            Create.Table("mz_card_exchange").WithDescription("名片交换请求表")
.WithColumn("Id").AsInt64().PrimaryKey().Identity().WithColumnDescription("编号")
    .WithColumn("SendCardId").AsInt64().WithColumnDescription("请求名片Id")
    .WithColumn("ReceiveCardId").AsInt64().WithColumnDescription("接收人名片Id")
    .WithColumn("ReceiveUserId").AsInt64().Indexed("ReceiveUserIdEXR").WithColumnDescription("接收人")
    .WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0待处理 1同意 2为忽略）")
    .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
    .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
    .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
    .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Create.Table("mz_card_pro").WithDescription("产品表")
.WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("编号")
.WithColumn("OrgId").AsInt64().WithColumnDescription("所属组织Id")
.WithColumn("DeptId").AsInt64().WithColumnDescription("所属部门Id")
.WithColumn("ImageUrl").AsString(255).WithColumnDescription("产品主图")
.WithColumn("ProName").AsString(30).WithColumnDescription("产品名称")
.WithColumn("CategoryId").AsInt64().WithColumnDescription("产品类别Id")
.WithColumn("Price").AsDecimal(14, 2).WithColumnDescription("单价")
.WithColumn("WholePrice").AsDecimal(14, 2).WithColumnDescription("批发价")
.WithColumn("Detail").AsString(3000).WithColumnDescription("产品详情")
    .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
    .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
    .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
    .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
    .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Create.Table("mz_card_procat").WithDescription("产品分类表")
.WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("编号")
.WithColumn("OrgId").AsInt64().WithColumnDescription("所属组织Id")
.WithColumn("CategoryName").AsString(30).WithColumnDescription("分类名称")
.WithColumn("Sort").AsInt32().WithColumnDescription("分类排序")
.WithColumn("ParentId").AsInt64().WithColumnDescription("父分类Id")
.WithColumn("Path").AsString(128).Indexed().WithColumnDescription("分类层级");


            Create.Table("mz_card_case").WithDescription("案例表")
.WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("编号")
.WithColumn("OrgId").AsInt64().WithColumnDescription("所属组织Id")
.WithColumn("Title").AsString(30).WithColumnDescription("案例标题")
.WithColumn("ImageUrl").AsString(255).WithColumnDescription("案例封面")
.WithColumn("Detail").AsString(3000).WithColumnDescription("案例详情")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");



            Insert.IntoTable("mz_dict_type").Row(new
            {
                dict_name = "访问来源",
                dict_type = "visit_source",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "访客记录的来源"
            });

            Insert.IntoTable("mz_dict_data").Row(new
            {
                dict_sort = 0,
                dict_label = "其它",
                dict_value = "0",
                dict_type = "visit_source",
                css_class = string.Empty,
                list_class = "default",
                is_default = "Y",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = string.Empty
            });

        }
        public override void Down()
        {
        }

    }
}
