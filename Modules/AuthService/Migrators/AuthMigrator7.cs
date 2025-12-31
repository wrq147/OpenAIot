using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Migrators
{
    [Migration(20250527003)]
    public class AuthMigrator7 : Migration
    {
        public override void Up()
        {
            this.Execute.Sql("delete FROM mz_menu where menu_id=720");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 720,
                menu_name = "自定义字段",
                parent_id = 107,
                order_num = 20,
                path = "field",
                component = "system/field/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/AuthService/Field/",
                icon = "baobiaoguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Execute.Sql("DROP TABLE IF EXISTS mz_field_val");
            Create.Table("mz_field_val").WithDescription("通用自定义字段存储值")
                .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("原表Id")
                .WithColumn("FieldId").AsString(50).PrimaryKey().WithColumnDescription("字段Id")
                .WithColumn("TableName").AsString(50).PrimaryKey().WithColumnDescription("原表名")
                .WithColumn("LongValue").AsString(50000).Nullable().WithColumnDescription("保存长文本数据")
                .WithColumn("Value").AsString(500).Indexed().Nullable().WithColumnDescription("文本值")
                .WithColumn("NumberValue").AsDouble().Indexed().Nullable().WithColumnDescription("数值");


            Execute.Sql("DROP TABLE IF EXISTS mz_group_view");
            Create.Table("mz_group_view").WithDescription("通用分组表")
    .WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
    .WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属组织ID")
    .WithColumn("Name").AsString(50).WithColumnDescription("分组名称")
    .WithColumn("TableName").AsString(20).Indexed().WithColumnDescription("分组的表名称")
    .WithColumn("LevelCode").AsString(20).WithColumnDescription("二级分组字段")
    .WithColumn("PhotoUrl").AsString(255).WithColumnDescription("图片地址")
    .WithColumn("Sort").AsInt32().WithColumnDescription("排序值：越小越前面")
    .WithColumn("ConditionJson").AsString(10000).WithColumnDescription("过滤条件的json")
    .WithColumn("ListFieldsJson").AsString(10000).WithColumnDescription("列表字段的json");


        }
        public override void Down()
        {
        }
    }
}
