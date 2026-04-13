using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscussService.Migrators
{
    [Migration(20230828001)]
    public class DiscussMigrator : Migration
    {
        public override void Up()
        {
            Create.Table("mz_subject").WithDescription("主题表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("SubjectOrgId").WithColumnDescription("所属企业Id")
.WithColumn("Title").AsString(255).WithColumnDescription("标题")
.WithColumn("IsExt").AsBoolean().WithColumnDescription("是否为扩展用主题")
.WithColumn("TargetId").AsString(128).WithColumnDescription("关联对象Id")
.WithColumn("TargetType").AsString(100).WithColumnDescription("主题类型")
.WithColumn("SubjectContent").AsString(20000).WithColumnDescription("主题内容")
      .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
      .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
      .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
      .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Create.Table("mz_comment").WithDescription("评论表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("DiscussOrgId").WithColumnDescription("所属企业Id")
.WithColumn("UserId").AsInt64().WithColumnDescription("评论人")
.WithColumn("SubjectId").AsString(128).WithColumnDescription("主题Id")
.WithColumn("ParentCommentId").AsString(128).WithColumnDescription("父评论Id")
.WithColumn("ParentCommentUserId").AsInt64().WithColumnDescription("父评论用户Id")
.WithColumn("Content").AsString(5000).WithColumnDescription("回复内容")
.WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
.WithColumn("PraiseNum").AsInt32().WithColumnDescription("点赞数")
.WithColumn("CreateOn").AsDateTime().WithColumnDescription("创建时间");


            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 3020,
                menu_name = "评论管理",
                parent_id = 4,
                order_num = 6,
                path = "comment/list",
                component = "discuss/comment/list",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/DiscussService/Comment/List",
                icon = "a-qiyeguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 3021,
                menu_name = "添加评论",
                parent_id = 3020,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/DiscussService/Comment/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 3022,
                menu_name = "删除评论",
                parent_id = 3020,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/DiscussService/Comment/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });




            Insert.IntoTable("mz_dict_type").Row(new
            {
                dict_name = "评论类型",
                dict_type = "comment_type",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "评论表的评论类型上"
            });

            Insert.IntoTable("mz_dict_data").Row(new
            {
                dict_sort = 0,
                dict_label = "文章评论",
                dict_value = "文章",
                dict_type = "comment_type",
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
