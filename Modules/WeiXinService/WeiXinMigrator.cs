using FluentMigrator;
using System;

namespace WeiXinService
{
    [Migration(20240924003)]
    public class WeiXinMigrator : Migration
    {
        public override void Up()
        {
            Execute.Sql("DROP TABLE IF EXISTS mz_weixin");
            Create.Table("mz_weixin").WithDescription("微信OpenId与UnionId关联信息表")
                .WithColumn("UnionId").AsString(64).PrimaryKey().WithColumnDescription("微信全局Id")
                .WithColumn("AppId").AsString(64).PrimaryKey().WithColumnDescription("对应的AppId")
                .WithColumn("OpenId").AsString(64).WithColumnDescription("微信OpenId");

            Execute.Sql("DROP TABLE IF EXISTS mz_admin_third");
            Execute.Sql("DROP TABLE IF EXISTS mz_admin_wx");
            Create.Table("mz_admin_wx").WithDescription("账号与微信的关联表")
.WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("ID")
.WithColumn("AppId").AsString(64).WithColumnDescription("微信的AppId")
.WithColumn("UnionId").AsString(64).Indexed("AdminUnionBind").WithColumnDescription("绑定的微信unionId")
.WithColumn("UpdatedOn").AsDateTime().WithColumnDescription("更新时间");

            Execute.Sql("DROP TABLE IF EXISTS mz_admin_crop");
            Create.Table("mz_admin_crop").WithDescription("账号与企业微信应用关联")
.WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("ID")
.WithColumn("AppId").AsString(64).PrimaryKey().WithColumnDescription("企业微信的AppId")
.WithColumn("CropUserId").AsString(64).Indexed("CropUserId").WithColumnDescription("企业微信的UserId")
.WithColumn("UpdatedOn").AsDateTime().WithColumnDescription("更新时间");
            Create.Index("IdxCropAppId").OnTable("mz_admin_crop").OnColumn("AppId").Ascending().OnColumn("CropUserId").Ascending();

            Execute.Sql("DROP TABLE IF EXISTS mz_corp_sync");
            Create.Table("mz_corp_sync").WithDescription("企业微信同步记录表")
.WithColumn("AppId").AsString(64).PrimaryKey().WithColumnDescription("企业微信的AppId")
.WithColumn("DeptDict").AsString(80000).WithColumnDescription("部门同步关联字典")
.WithColumn("MemDict").AsString(80000).WithColumnDescription("人员同步关联字典")
.WithColumn("JobId").AsInt64().WithColumnDescription("定时任务Id")
.WithColumn("Speed").AsInt32().WithColumnDescription("速度：1为快速，2为慢速")
.WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0启用 1停用）")
.WithColumn("UpdatedOn").AsDateTime().WithColumnDescription("更新时间");

            Execute.Sql("DROP TABLE IF EXISTS mz_corp_task");
            Create.Table("mz_corp_task").WithDescription("企业微信同步任务表")
.WithColumn("TaskId").AsString(64).PrimaryKey().WithColumnDescription("任务Id")
.WithColumn("AppId").AsString(64).WithColumnDescription("企业微信的AppId")
.WithColumn("IsUpdateDept").AsBoolean().WithDefaultValue(false).WithColumnDescription("是否完成更新部门")
.WithColumn("IsAddDept").AsBoolean().WithDefaultValue(false).WithColumnDescription("是否完成新增部门")
.WithColumn("IsMoveDept").AsBoolean().WithDefaultValue(false).WithColumnDescription("是否完成移动部门")
.WithColumn("IsDelDept").AsBoolean().WithDefaultValue(false).WithColumnDescription("是否完成删除部门")
.WithColumn("IsUpdateMem").AsBoolean().WithDefaultValue(false).WithColumnDescription("是否完成更新人员")
.WithColumn("IsAddMem").AsBoolean().WithDefaultValue(false).WithColumnDescription("是否完成新增人员")
.WithColumn("IsDelMem").AsBoolean().WithDefaultValue(false).WithColumnDescription("是否完成删除人员")
.WithColumn("UpdateDeptErr").AsString(500).WithColumnDescription("更新部门失败信息")
.WithColumn("AddDeptErr").AsString(500).WithColumnDescription("新增部门失败信息")
.WithColumn("MoveDeptErr").AsString(500).WithColumnDescription("移动部门失败信息")
.WithColumn("DelDeptErr").AsString(500).WithColumnDescription("删除部门失败信息")
.WithColumn("UpdateMemErr").AsString(500).WithColumnDescription("更新人员失败信息")
.WithColumn("AddMemErr").AsString(500).WithColumnDescription("新增人员失败信息")
.WithColumn("DelMemErr").AsString(500).WithColumnDescription("删除人员失败信息")
.WithColumn("Status").AsInt32().WithColumnDescription("0为同步中，1为已完成")
.WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间");


            this.Execute.Sql("delete FROM mz_menu where menu_id=118");
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 118,
                menu_name = "微信应用",
                parent_id = 1,
                order_num = 7,
                path = "wx",
                component = "system/wx/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/WeiXinService/Account/List",
                icon = "wechat",
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
