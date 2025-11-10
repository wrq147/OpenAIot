using Common;
using FluentMigrator;
using System.IO;

namespace AuthService.Migrators
{
    [Migration(20240201001)]
    public class AuthMigrator4 : Migration
    {
        public override void Up()
        {
            if (Schema.Table("mz_industry").Exists())
            {
                Delete.Table("mz_industry");
            }
            Create.Table("mz_industry").WithDescription("行业表")
               .WithColumn("Id").AsInt32().PrimaryKey().WithColumnDescription("行业编码").NotNullable()
               .WithColumn("Name").AsString(50).WithColumnDescription("行业名称").Nullable()
               .WithColumn("ParentId").AsInt32().WithColumnDescription("父Id").Nullable()
               .WithColumn("Sort").AsInt32().WithColumnDescription("排序").Nullable();

            string exesqldirpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "exesql" + Path.DirectorySeparatorChar;
            Execute.Script(exesqldirpath + "mz_industry.sql");

            if (Schema.Table("mz_area").Exists())
            {
                Delete.Table("mz_area");
            }
            Create.Table("mz_area").WithDescription("区域代码表")
               .WithColumn("Id").AsString(45).PrimaryKey().WithColumnDescription("区域代码").NotNullable()
               .WithColumn("ParentId").AsString(45).WithColumnDescription("父区域").Nullable()
               .WithColumn("LevelType").AsString(45).WithColumnDescription("级别").Nullable()
               .WithColumn("Name").AsString(45).WithColumnDescription("名称").Nullable()
               .WithColumn("ParentPath").AsString(45).WithColumnDescription("层级路径").Nullable()
               .WithColumn("Province").AsString(45).WithColumnDescription("所属省").Nullable()
               .WithColumn("City").AsString(45).WithColumnDescription("所属市").Nullable()
               .WithColumn("District").AsString(45).WithColumnDescription("所属区").Nullable()
               .WithColumn("Street").AsString(45).WithColumnDescription("所属镇").Nullable()
               .WithColumn("Pinyin").AsString(45).WithColumnDescription("拼音").Nullable()
               .WithColumn("Jianpin").AsString(45).WithColumnDescription("首字母拼音").Nullable()
               .WithColumn("FirstChar").AsString(45).WithColumnDescription("首字母").Nullable()
               .WithColumn("CityCode").AsString(45).WithColumnDescription("城市区号").Nullable()
               .WithColumn("ZipCode").AsString(45).WithColumnDescription("邮政编码").Nullable()
               .WithColumn("Lng").AsString(45).WithColumnDescription("经度").Nullable()
               .WithColumn("Lat").AsString(45).WithColumnDescription("纬度").Nullable();


            Create.Index("MZ_Area_Parent_Key").OnTable("mz_area").OnColumn("ParentId").Ascending();
            Create.Index("MZ_Area_ParentPath_Key").OnTable("mz_area").OnColumn("ParentPath").Ascending();
            Create.Index("MZ_Pinyin_Key").OnTable("mz_area").OnColumn("Pinyin").Ascending();
            Create.Index("MZ_Jianpin_Key").OnTable("mz_area").OnColumn("Jianpin").Ascending();

            //生成区域代码
            Execute.Script(exesqldirpath + "mz_area_0.sql");
            Execute.Script(exesqldirpath + "mz_area_1.sql");
            Execute.Script(exesqldirpath + "mz_area_2.sql");
            Execute.Script(exesqldirpath + "mz_area_3.sql");
            Execute.Script(exesqldirpath + "mz_area_4.sql");
            Execute.Script(exesqldirpath + "mz_area_5.sql");
            Execute.Script(exesqldirpath + "mz_area_6.sql");
            Execute.Script(exesqldirpath + "mz_area_7.sql");

            if (Constants.General.sqltype != "Sqlite")
            {
                if (Schema.Table("mz_area_geo").Exists())
                {
                    Delete.Table("mz_area_geo");
                }
                Create.Table("mz_area_geo").WithDescription("区域地图")
       .WithColumn("Id").AsString(45).PrimaryKey().NotNullable()
       .WithColumn("geohash").AsString(45).Nullable()
       .WithColumn("polygon").AsCustom("geometry").NotNullable();
                Create.Index("AreaGEOHASH").OnTable("mz_area_geo").OnColumn("geohash").Ascending();

                string geodirpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "geosql" + Path.DirectorySeparatorChar;
                for (int i = 0; i < 76; i++)
                {
                    Execute.Script(geodirpath + $"mz_area_geo_{i + 1}.sql");
                }
                Execute.Sql("CREATE SPATIAL INDEX tmpareeageo ON mz_area_geo(polygon);");
            }
        }
        public override void Down()
        {
        }
    }
}
