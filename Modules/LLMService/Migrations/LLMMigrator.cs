using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Migrations
{
    [Migration(20260611001)]
    public class LLMMigrator : Migration
    {
        public override void Up()
        {
            // 创建知识库表
            if (!Schema.Table("llm_knowledge").Exists())
            {
                Create.Table("llm_knowledge")
                    .WithColumn("Id").AsString(128).PrimaryKey()
                    .WithColumn("Cover").AsString(500).Nullable().WithColumnDescription("封面URL")
                    .WithColumn("Name").AsString(100).NotNullable().WithColumnDescription("文库名称")
                    .WithColumn("Description").AsString(500).Nullable().WithColumnDescription("文库描述")
                    .WithColumn("OrgId").AsInt64().Indexed().NotNullable().WithColumnDescription("组织ID")
                    .WithColumn("IsPublic").AsBoolean().Indexed().WithColumnDescription("是否公开")
                    .WithColumn("Status").AsInt32().Indexed().NotNullable().WithDefaultValue(1).WithColumnDescription("状态：3-审核失败，2-审核中，1-已发布，0-草稿")
                    .WithColumn("DocCount").AsInt32().NotNullable().WithDefaultValue(0).WithColumnDescription("文档数量")
                    .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                    .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                    .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                    .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            }

            // 创建栏目表
            if (!Schema.Table("llm_kb_column").Exists())
            {
                Create.Table("llm_kb_column")
                    .WithColumn("Id").AsString(128).PrimaryKey()
                    .WithColumn("KbId").AsString(128).Indexed().NotNullable().WithColumnDescription("知识库ID")
                    .WithColumn("Name").AsString(100).NotNullable().WithColumnDescription("栏目名称")
                    .WithColumn("ParentId").AsString().Indexed().NotNullable().WithDefaultValue(0).WithColumnDescription("父栏目ID，空表示顶级栏目")
                    .WithColumn("SortOrder").AsInt32().NotNullable().WithDefaultValue(1).WithColumnDescription("排序号")
                    .WithColumn("Path").AsString(800).Indexed().WithColumnDescription("分类层级")
                    .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                    .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                    .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                    .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");



                Create.ForeignKey("fk_llm_col_kb")
                    .FromTable("llm_kb_column").ForeignColumn("KbId")
                    .ToTable("llm_knowledge").PrimaryColumn("Id")
                    .OnDelete(System.Data.Rule.Cascade);
            }

            // 创建文章表
            if (!Schema.Table("llm_article").Exists())
            {
                Create.Table("llm_article")
                    .WithColumn("Id").AsString(128).PrimaryKey()
                    .WithColumn("KbId").AsString(128).Indexed().NotNullable().WithColumnDescription("知识库ID")
                    .WithColumn("ColumnId").AsString(128).Indexed().Nullable().WithColumnDescription("栏目ID")
                    .WithColumn("Title").AsString(200).NotNullable().WithColumnDescription("文章标题")
                    .WithColumn("Content").AsString(100000).Nullable().WithColumnDescription("文章内容")
                    .WithColumn("KeyWords").AsString(2000).WithDefaultValue("").WithColumnDescription("文章关键词")
                    .WithColumn("ViewCount").AsInt32().NotNullable().WithDefaultValue(0).WithColumnDescription("阅读次数")
                    .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
                    .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
                    .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
                    .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


                Create.ForeignKey("fk_llm_art_kb")
                    .FromTable("llm_article").ForeignColumn("KbId")
                    .ToTable("llm_knowledge").PrimaryColumn("Id")
                    .OnDelete(System.Data.Rule.Cascade);

            }

            this.Execute.Sql("delete FROM mz_menu where menu_id=55");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 55,
                menu_name = "AI助手",
                parent_id = 4,
                order_num = 4,
                path = "ai/assistant",
                component = "llm/ai/assistant",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "1",
                status = "0",
                perms = "/LLMService/AI/Assistant",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });



            // 添加知识库管理菜单
            this.Execute.Sql("delete FROM mz_menu where menu_id=56");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 56,
                menu_name = "知识库",
                parent_id = 4,
                order_num = 5,
                path = "klg/index",
                component = "llm/klg/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/LLMService/Knowledge/List",
                icon = "haocaiguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            //创建ai助手的数据库查询用户
            this.Execute.Sql("CREATE USER 'ai_user'@'%' IDENTIFIED BY 'Ai@123456';\r\nGRANT SELECT ON mz_iot_device TO 'ai_user'@'%';\r\nGRANT SELECT ON mz_leave_stock TO 'ai_user'@'%';\r\nGRANT SELECT ON mz_leave_detail TO 'ai_user'@'%';\r\nGRANT SELECT ON mz_stock_pile TO 'ai_user'@'%';\r\nGRANT SELECT ON mz_stock_record TO 'ai_user'@'%';\r\nGRANT SELECT ON mz_store_house TO 'ai_user'@'%';\r\nGRANT SELECT ON mz_enter_stock TO 'ai_user'@'%';\r\nGRANT SELECT ON mz_enter_detail TO 'ai_user'@'%';\r\nGRANT SELECT ON mz_leave_apply TO 'ai_user'@'%';\r\nGRANT SELECT ON mz_leave_apply_detail TO 'ai_user'@'%';\r\nGRANT SELECT ON mz_admin TO 'ai_user'@'%';\r\nGRANT SELECT ON mz_dept TO 'ai_user'@'%';\r\nFLUSH PRIVILEGES;");
        }
        public override void Down()
        {
            if (Schema.Table("llm_kb_column").Exists())
            {
                Delete.Table("llm_kb_column");
            }
            if (Schema.Table("llm_article").Exists())
            {
                Delete.Table("llm_article");
            }
            if (Schema.Table("llm_knowledge").Exists())
            {
                Delete.Table("llm_knowledge");
            }
        }
    }
}
