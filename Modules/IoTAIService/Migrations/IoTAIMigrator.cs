using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.Migrations
{
    [Migration(20251113001)]
    public class IoTAIMigrator : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_ai_house");
            Create.Table("mz_ai_house").WithDescription("人员建模库表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属企业Id")
.WithColumn("HouseName").AsString(50).WithColumnDescription("建模库名称")
.WithColumn("Remark").AsString(500).WithColumnDescription("建模库备注")
.WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0禁用、1启用）")
.WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间");



            Execute.Sql("DROP TABLE IF EXISTS mz_ai_mem");
            Create.Table("mz_ai_mem").WithDescription("人员建模信息表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed().WithColumnDescription("所属企业Id")
.WithColumn("HouseId").AsString(128).Indexed().WithColumnDescription("所属库")
.WithColumn("MemId").AsInt64().Indexed().WithColumnDescription("人员Id")
.WithColumn("FStatus").AsByte().WithColumnDescription("建模状态：0未建模，1为建模成功，2为建模失败")
.WithColumn("FaceImg").AsString(255).WithColumnDescription("人脸建模头像")
.WithColumn("MilvusId").AsInt64().WithDefaultValue(0).WithColumnDescription("建模Id")
.WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间");


            this.Execute.Sql("delete FROM mz_menu where menu_id=4501");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 4501,
                menu_name = "人脸建模",
                parent_id = 4000,
                order_num = 13,
                path = "ai/facelist",
                component = "iot/ai/facelist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/IoTAIService/Face/ListPage",
                icon = "haocaiguanli",
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
