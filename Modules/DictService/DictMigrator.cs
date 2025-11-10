using FluentMigrator;
using System;

namespace DictService
{
    [Migration(20220000001)]
    public class DictMigrator : Migration
    {
        public override void Up()
        {
            if (Schema.Table("mz_dict_data").Exists())
            {
                return;
            }
            Create.Table("mz_dict_type").WithDescription("字典类型表")
    .WithColumn("dict_id").AsInt64().PrimaryKey().WithColumnDescription("字典主键")
    .WithColumn("dict_name").AsString(100).WithColumnDescription("字典名称")
    .WithColumn("dict_type").AsString(100).Unique("dict_type").WithColumnDescription("字典类型")
    .WithColumn("status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0正常 1停用）")
    .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
    .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
    .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
    .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间")
    .WithColumn("remark").AsString(500).WithColumnDescription("备注");


            Insert.IntoTable("mz_dict_type").Row(new
            {
                dict_id = 1,
                dict_name = "用户性别",
                dict_type = "sys_user_sex",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "用户性别列表"
            }).Row(new
            {
                dict_id = 2,
                dict_name = "菜单状态",
                dict_type = "sys_show_hide",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "菜单状态列表"
            }).Row(new
            {
                dict_id = 3,
                dict_name = "系统开关",
                dict_type = "sys_normal_disable",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "系统开关列表"
            }).Row(new
            {
                dict_id = 4,
                dict_name = "任务状态",
                dict_type = "sys_job_status",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "任务状态列表"
            }).Row(new
            {
                dict_id = 5,
                dict_name = "任务分组",
                dict_type = "sys_job_group",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "任务分组列表"
            }).Row(new
            {
                dict_id = 6,
                dict_name = "系统是否",
                dict_type = "sys_yes_no",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "系统是否列表"
            }).Row(new
            {
                dict_id = 7,
                dict_name = "通知类型",
                dict_type = "sys_notice_type",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "通知类型列表"
            }).Row(new
            {
                dict_id = 8,
                dict_name = "通知状态",
                dict_type = "sys_notice_status",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "通知状态列表"
            }).Row(new
            {
                dict_id = 9,
                dict_name = "系统状态",
                dict_type = "sys_common_status",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "登录状态列表"
            });

            Alter.Table("mz_dict_type").AlterColumn("dict_id").AsInt64().Identity();


            Create.Table("mz_dict_data").WithDescription("字典数据表")
.WithColumn("dict_code").AsInt64().PrimaryKey().WithColumnDescription("字典编码")
.WithColumn("dict_sort").AsInt32().WithColumnDescription("字典排序")
.WithColumn("dict_label").AsString(100).WithColumnDescription("字典标签")
.WithColumn("dict_value").AsString(100).WithColumnDescription("字典键值")
.WithColumn("dict_type").AsString(100).WithColumnDescription("字典类型")
.WithColumn("css_class").AsString(100).WithColumnDescription("样式属性（其他样式扩展）")
.WithColumn("list_class").AsString(100).WithColumnDescription("表格回显样式")
.WithColumn("is_default").AsFixedLengthAnsiString(1).WithColumnDescription("是否默认（Y是 N否）")
.WithColumn("status").AsFixedLengthAnsiString(1).WithColumnDescription("状态（0正常 1停用）")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间")
.WithColumn("remark").AsString(500).WithColumnDescription("备注");

            Insert.IntoTable("mz_dict_data").Row(new
            {
                dict_code = 1,
                dict_sort = 1,
                dict_label = "男",
                dict_value = "0",
                dict_type = "sys_user_sex",
                css_class = string.Empty,
                list_class = string.Empty,
                is_default = "Y",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "性别男"
            }).Row(new
            {
                dict_code = 2,
                dict_sort = 2,
                dict_label = "女",
                dict_value = "1",
                dict_type = "sys_user_sex",
                css_class = string.Empty,
                list_class = string.Empty,
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "性别女"
            }).Row(new
            {
                dict_code = 3,
                dict_sort = 3,
                dict_label = "未知",
                dict_value = "2",
                dict_type = "sys_user_sex",
                css_class = string.Empty,
                list_class = string.Empty,
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "性别未知"
            }).Row(new
            {
                dict_code = 4,
                dict_sort = 1,
                dict_label = "显示",
                dict_value = "0",
                dict_type = "sys_show_hide",
                css_class = string.Empty,
                list_class = "primary",
                is_default = "Y",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "显示菜单"
            }).Row(new
            {
                dict_code = 5,
                dict_sort = 2,
                dict_label = "隐藏",
                dict_value = "1",
                dict_type = "sys_show_hide",
                css_class = string.Empty,
                list_class = "danger",
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "隐藏菜单"
            }).Row(new
            {
                dict_code = 6,
                dict_sort = 1,
                dict_label = "正常",
                dict_value = "0",
                dict_type = "sys_normal_disable",
                css_class = string.Empty,
                list_class = "primary",
                is_default = "Y",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "正常状态"
            }).Row(new
            {
                dict_code = 7,
                dict_sort = 2,
                dict_label = "停用",
                dict_value = "1",
                dict_type = "sys_normal_disable",
                css_class = string.Empty,
                list_class = "danger",
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "停用状态"
            }).Row(new
            {
                dict_code = 8,
                dict_sort = 1,
                dict_label = "正常",
                dict_value = "0",
                dict_type = "sys_job_status",
                css_class = string.Empty,
                list_class = "primary",
                is_default = "Y",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "正常状态"
            }).Row(new
            {
                dict_code = 9,
                dict_sort = 2,
                dict_label = "暂停",
                dict_value = "1",
                dict_type = "sys_job_status",
                css_class = string.Empty,
                list_class = "danger",
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "停用状态"
            }).Row(new
            {
                dict_code = 10,
                dict_sort = 1,
                dict_label = "默认",
                dict_value = "DEFAULT",
                dict_type = "sys_job_group",
                css_class = string.Empty,
                list_class = string.Empty,
                is_default = "Y",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "默认分组"
            }).Row(new
            {
                dict_code = 11,
                dict_sort = 2,
                dict_label = "系统",
                dict_value = "SYSTEM",
                dict_type = "sys_job_group",
                css_class = string.Empty,
                list_class = string.Empty,
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "系统分组"
            }).Row(new
            {
                dict_code = 12,
                dict_sort = 1,
                dict_label = "是",
                dict_value = "Y",
                dict_type = "sys_yes_no",
                css_class = string.Empty,
                list_class = "primary",
                is_default = "Y",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "系统默认是"
            }).Row(new
            {
                dict_code = 13,
                dict_sort = 2,
                dict_label = "否",
                dict_value = "N",
                dict_type = "sys_yes_no",
                css_class = string.Empty,
                list_class = "danger",
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "系统默认否"
            }).Row(new
            {
                dict_code = 14,
                dict_sort = 1,
                dict_label = "通知",
                dict_value = "1",
                dict_type = "sys_notice_type",
                css_class = string.Empty,
                list_class = "warning",
                is_default = "Y",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "通知"
            }).Row(new
            {
                dict_code = 15,
                dict_sort = 2,
                dict_label = "公告",
                dict_value = "2",
                dict_type = "sys_notice_type",
                css_class = string.Empty,
                list_class = "success",
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "公告"
            }).Row(new
            {
                dict_code = 16,
                dict_sort = 1,
                dict_label = "正常",
                dict_value = "0",
                dict_type = "sys_notice_status",
                css_class = string.Empty,
                list_class = "primary",
                is_default = "Y",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "正常状态"
            }).Row(new
            {
                dict_code = 17,
                dict_sort = 2,
                dict_label = "关闭",
                dict_value = "1",
                dict_type = "sys_notice_status",
                css_class = string.Empty,
                list_class = "danger",
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "关闭状态"
            }).Row(new
            {
                dict_code = 27,
                dict_sort = 1,
                dict_label = "成功",
                dict_value = "0",
                dict_type = "sys_common_status",
                css_class = string.Empty,
                list_class = "primary",
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "正常状态"
            }).Row(new
            {
                dict_code = 28,
                dict_sort = 2,
                dict_label = "失败",
                dict_value = "1",
                dict_type = "sys_common_status",
                css_class = string.Empty,
                list_class = "danger",
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "停用状态"
            });

            Alter.Table("mz_dict_data").AlterColumn("dict_code").AsInt64().Identity();

            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 105,
                menu_name = "字典管理",
                parent_id = 1,
                order_num = 1,
                path = "dict",
                component = "system/dict/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/DictService/DictType/",
                icon = "dict",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1026,
                menu_name = "字典查询",
                parent_id = 105,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/DictService/DictType/List",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1027,
                menu_name = "字典新增",
                parent_id = 105,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/DictService/DictType/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1028,
                menu_name = "字典修改",
                parent_id = 105,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/DictService/DictType/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1029,
                menu_name = "字典删除",
                parent_id = 105,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/DictService/DictType/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1030,
                menu_name = "字典导出",
                parent_id = 105,
                order_num = 5,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/DictService/DictType/Export",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });



            Insert.IntoTable("mz_dict_type").Row(new
            {
                dict_name = "组织规模",
                dict_type = "org_size",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "组织规模列表"
            });

            Insert.IntoTable("mz_dict_data").Row(new
            {
                dict_sort = 0,
                dict_label = "1-50人",
                dict_value = "10001",
                dict_type = "org_size",
                css_class = string.Empty,
                list_class = "default",
                is_default = "Y",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = string.Empty
            }).Row(new
            {
                dict_sort = 1,
                dict_label = "51-100人",
                dict_value = "10002",
                dict_type = "org_size",
                css_class = string.Empty,
                list_class = "default",
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = string.Empty
            }).Row(new
            {
                dict_sort = 2,
                dict_label = "101-200人",
                dict_value = "10003",
                dict_type = "org_size",
                css_class = string.Empty,
                list_class = "default",
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = string.Empty
            }).Row(new
            {
                dict_sort = 3,
                dict_label = "201-500人",
                dict_value = "10004",
                dict_type = "org_size",
                css_class = string.Empty,
                list_class = "default",
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = string.Empty
            }).Row(new
            {
                dict_sort = 4,
                dict_label = "501-1000人",
                dict_value = "10005",
                dict_type = "org_size",
                css_class = string.Empty,
                list_class = "default",
                is_default = "N",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = string.Empty
            }).Row(new
            {
                dict_sort = 5,
                dict_label = "1001人及以上",
                dict_value = "10006",
                dict_type = "org_size",
                css_class = string.Empty,
                list_class = "default",
                is_default = "N",
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
