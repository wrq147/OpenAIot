using Common;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Migrators
{
    [Migration(20230129001)]
    public class AuthMigrator3 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 107,
                menu_name = "企业管理",
                parent_id = 0,
                order_num = 5,
                path = "org",
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "M",
                visible = "0",
                status = "0",
                perms = "/AuthService/Org/",
                icon = "a-qiyeguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 700,
                menu_name = "企业信息",
                parent_id = 107,
                order_num = 5,
                path = "org",
                component = "system/org/orgInfo",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/AuthService/Org/InfoPage",
                icon = "a-qiyeguanli",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 701,
                menu_name = "编辑企业",
                parent_id = 700,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Org/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 702,
                menu_name = "解散企业",
                parent_id = 700,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Org/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 703,
                menu_name = "移交企业",
                parent_id = 700,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Org/ChangeCreator",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 101,
                menu_name = "角色管理",
                parent_id = 107,
                order_num = 2,
                path = "role",
                component = "system/role/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/AuthService/Role/",
                icon = "peoples",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1008,
                menu_name = "角色查询",
                parent_id = 101,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Role/List",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1009,
                menu_name = "角色新增",
                parent_id = 101,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Role/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1010,
                menu_name = "角色修改",
                parent_id = 101,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Role/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1011,
                menu_name = "角色删除",
                parent_id = 101,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Role/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1012,
                menu_name = "角色导出",
                parent_id = 101,
                order_num = 5,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Role/Export",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 103,
                menu_name = "部门管理",
                parent_id = 107,
                order_num = 4,
                path = "dept",
                component = "system/dept/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/AuthService/Dept/",
                icon = "tree",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1017,
                menu_name = "部门查询",
                parent_id = 103,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Dept/List",
                icon = "#",
                scope = 1,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1018,
                menu_name = "部门新增",
                parent_id = 103,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Dept/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1019,
                menu_name = "部门修改",
                parent_id = 103,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Dept/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1020,
                menu_name = "部门删除",
                parent_id = 103,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Dept/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 600,
                menu_name = "员工管理",
                parent_id = 107,
                order_num = 3,
                path = "Employee",
                component = "system/Employee/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/AuthService/Member/List",
                icon = "user",
                scope = 1,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 601,
                menu_name = "邀请员工",
                parent_id = 600,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Member/YaoQing",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 602,
                menu_name = "移除员工",
                parent_id = 600,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Member/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 603,
                menu_name = "编辑员工",
                parent_id = 600,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Member/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });





            Insert.IntoTable("mz_role").Row(new
            {
                RoleID = 2,
                RoleName = "企业管理员",
                RoleSort = 0,
                RoleDesc = "企业的管理人员权限",
                IsSystem = "1",
                Status = "0",
                OrgId = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Insert.IntoTable("mz_role_permission").Row(new
            {
                RoleID = 2,
                MenuId = 702
            }).Row(new
            {
                RoleID = 2,
                MenuId = 600
            }).Row(new
            {
                RoleID = 2,
                MenuId = 601
            }).Row(new
            {
                RoleID = 2,
                MenuId = 602
            }).Row(new
            {
                RoleID = 2,
                MenuId = 603
            }).Row(new
            {
                RoleID = 2,
                MenuId = 107
            }).Row(new
            {
                RoleID = 2,
                MenuId = 700
            }).Row(new
            {
                RoleID = 2,
                MenuId = 701
            }).Row(new
            {
                RoleID = 2,
                MenuId = 703
            }).Row(new
            {
                RoleID = 2,
                MenuId = 103
            }).Row(new
            {
                RoleID = 2,
                MenuId = 1017
            }).Row(new
            {
                RoleID = 2,
                MenuId = 1018
            }).Row(new
            {
                RoleID = 2,
                MenuId = 1019
            }).Row(new
            {
                RoleID = 2,
                MenuId = 1020
            }).Row(new
            {
                RoleID = 2,
                MenuId = 101
            }).Row(new
            {
                RoleID = 2,
                MenuId = 1008
            }).Row(new
            {
                RoleID = 2,
                MenuId = 1009
            }).Row(new
            {
                RoleID = 2,
                MenuId = 1010
            }).Row(new
            {
                RoleID = 2,
                MenuId = 1011
            }).Row(new
            {
                RoleID = 2,
                MenuId = 1012
            });

            Insert.IntoTable("mz_user_role").Row(new
            {
                UserId = 1,
                RoleID = 2,
                OrgId = 1
            });


            Create.Table("mz_admin_ext").WithDescription("用户扩展信息")
.WithColumn("UserId").AsInt64().PrimaryKey().WithColumnDescription("用户Id")
.WithColumn("ExtField").AsString(50).PrimaryKey().WithColumnDescription("扩展字段")
.WithColumn("ExtValue").AsString(255).WithColumnDescription("扩展值");


            Create.Table("mz_org_ext").WithDescription("企业扩展设置")
.WithColumn("OrgId").AsInt64().PrimaryKey().WithColumnDescription("企业Id")
.WithColumn("ExtField").AsString(50).PrimaryKey().WithColumnDescription("扩展字段")
.WithColumn("ExtValue").AsString(50000).WithColumnDescription("扩展值");


            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 8,
                config_name = "腾讯地图Key",
                config_key = "map.key",
                config_value = "",
                config_type = "Y",
                remark = "系统使用的腾讯地图Key,申请地址：https://lbs.qq.com/",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 25,
                config_name = "高德地图key",
                config_key = "map.gdkey",
                config_value = "",
                config_type = "Y",
                remark = "系统使用的高德地图Key,申请地址：https://lbs.amap.com/",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 26,
                config_name = "高德地图安全密钥",
                config_key = "map.gdsecret",
                config_value = "",
                config_type = "Y",
                remark = "系统使用的高德地图安全密钥,申请地址：https://lbs.amap.com/",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 27,
                config_name = "高德地图js的key",
                config_key = "map.gdsecretkey",
                config_value = "",
                config_type = "Y",
                remark = "系统使用的高德地图JS版的Key,申请地址：https://lbs.amap.com/",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 10,
                config_name = "注册方式",
                config_key = "reg.way",
                config_value = "phone,email",
                config_type = "Y",
                remark = "格式：注册方式1,注册方式2（phone表示手机注册、email表示邮箱注册）",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });


            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 119,
                menu_name = "App升级中心",
                parent_id = 1,
                order_num = 10,
                path = "upgrade",
                component = "system/upgrade/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/AuthService/UpgradeMan/",
                icon = "chukujilu",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });


            Create.Table("mz_upgrade").WithDescription("App升级中心")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().WithColumnDescription("编号")
                .WithColumn("OrgId").AsInt64().WithDefaultValue(0).Indexed().WithColumnDescription("所属企业Id，0为默认")
                .WithColumn("StyleId").AsString(50).WithDefaultValue("").Indexed().WithColumnDescription("所属主题Id,空为默认")
                .WithColumn("PackageType").AsInt32().WithColumnDescription("包类型：0为原生App安装包，1为Wgt资源包")
                .WithColumn("Title").AsString(255).WithColumnDescription("更新标题")
                .WithColumn("UpContent").AsString(2000).WithColumnDescription("更新内容")
                .WithColumn("Platform").AsString(30).WithColumnDescription("平台：android、ios（多个,号分隔）")
                .WithColumn("UpVersion").AsString(20).WithColumnDescription("当前包版本号，必须大于当前线上发行版本号")
                .WithColumn("MinAppVersion").AsString(20).WithColumnDescription("Wgt资源包时，原生App最低版本")
                .WithColumn("UpUrl").AsString(255).WithColumnDescription("下载链接")
                .WithColumn("IsSilently").AsBoolean().WithColumnDescription("WGT是否静默更新")
                .WithColumn("IsMandatory").AsBoolean().WithColumnDescription("App安装包是否强制更新")
                .WithColumn("IsPublish").AsBoolean().WithColumnDescription("是否上线发行")
                .WithColumn("CreatedOn").AsDateTime().WithColumnDescription("创建时间");




        }
        public override void Down()
        {
        }

    }
}
