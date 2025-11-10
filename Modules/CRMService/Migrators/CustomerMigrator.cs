using Common;
using FluentMigrator;
using System;


namespace CRMService.Migrators
{
    [Migration(20230523001)]
    public class CustomerMigrator : Migration
    {
        public override void Up()
        {

            //生产商添加进销存管理
            Insert.IntoTable("mz_role_permission").Row(new
            {
                RoleID = 3,
                MenuId = 8
            }).Row(new
            {
                RoleID = 3,
                MenuId = 6120
            }).Row(new
            {
                RoleID = 3,
                MenuId = 6121
            }).Row(new
            {
                RoleID = 3,
                MenuId = 6122
            }).Row(new
            {
                RoleID = 3,
                MenuId = 6123
            }).Row(new
            {
                RoleID = 3,
                MenuId = 6130
            }).Row(new
            {
                RoleID = 3,
                MenuId = 6210
            }).Row(new
            {
                RoleID = 3,
                MenuId = 6220
            }).Row(new
            {
                RoleID = 3,
                MenuId = 6230
            }).Row(new
            {
                RoleID = 3,
                MenuId = 6240
            }).Row(new
            {
                RoleID = 3,
                MenuId = 6250
            }).Row(new
            {
                RoleID = 3,
                MenuId = 6260
            });

            //企业的代理授权
            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 6,
                menu_name = "客户关系",
                parent_id = 0,
                order_num = 9,
                path = "crm",
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "M",
                visible = "0",
                status = "0",
                perms = "/CRMMan/",
                icon = "kehu",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6110,
                menu_name = "公海池",
                parent_id = 6,
                order_num = 2,
                path = "customer/publist",
                component = "crm/customer/publist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/CRMService/Customer/PubList",
                icon = "gonghaichi",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6111,
                menu_name = "领取客户",
                parent_id = 6110,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Customer/Draw",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6112,
                menu_name = "添加客户",
                parent_id = 6110,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Customer/PubAdd",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6113,
                menu_name = "修改客户",
                parent_id = 6110,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Customer/PubEdit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6114,
                menu_name = "删除客户",
                parent_id = 6110,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Customer/PubRemove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6200,
                menu_name = "客户",
                parent_id = 6,
                order_num = 2,
                path = "customer/prilist",
                component = "crm/customer/prilist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/CRMService/Customer/List",
                icon = "kehu",
                scope = 1,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6201,
                menu_name = "添加客户",
                parent_id = 6200,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Customer/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6202,
                menu_name = "修改客户",
                parent_id = 6200,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Customer/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6203,
                menu_name = "删除客户",
                parent_id = 6200,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Customer/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6204,
                menu_name = "退回客户",
                parent_id = 6200,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Customer/Return",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6207,
                menu_name = "邀请客户",
                parent_id = 6200,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Agent/AddInvite",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6300,
                menu_name = "线索池",
                parent_id = 6,
                order_num = 3,
                path = "clue/publist",
                component = "crm/clue/publist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/CRMService/Clue/PubList",
                icon = "xiansuochi",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6301,
                menu_name = "添加线索",
                parent_id = 6300,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Clue/PubAdd",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6302,
                menu_name = "修改线索",
                parent_id = 6300,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Clue/PubEdit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6303,
                menu_name = "删除线索",
                parent_id = 6300,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Clue/PubRemove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6304,
                menu_name = "领取线索",
                parent_id = 6300,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Clue/Draw",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6400,
                menu_name = "线索",
                parent_id = 6,
                order_num = 4,
                path = "clue/prilist",
                component = "crm/clue/prilist",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/CRMService/Clue/PriList",
                icon = "xiansuo",
                scope = 1,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6401,
                menu_name = "添加线索",
                parent_id = 6400,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Clue/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6402,
                menu_name = "修改线索",
                parent_id = 6400,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Clue/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6403,
                menu_name = "删除线索",
                parent_id = 6400,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Clue/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6405,
                menu_name = "转换线索",
                parent_id = 6300,
                order_num = 5,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Clue/Transform",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6406,
                menu_name = "退回线索",
                parent_id = 6300,
                order_num = 6,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Clue/Return",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6500,
                menu_name = "联系人",
                parent_id = 6,
                order_num = 5,
                path = "contact/list",
                component = "crm/contact/list",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/CRMService/Contact/List",
                icon = "lianxiren",
                scope = 1,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6501,
                menu_name = "添加联系人",
                parent_id = 6500,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Contact/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6502,
                menu_name = "修改联系人",
                parent_id = 6500,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Contact/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6503,
                menu_name = "删除联系人",
                parent_id = 6500,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Contact/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6600,
                menu_name = "商机",
                parent_id = 6,
                order_num = 6,
                path = "opport/list",
                component = "crm/opport/list",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/CRMService/Opportunity/List",
                icon = "shangji",
                scope = 1,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6601,
                menu_name = "添加商机",
                parent_id = 6600,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Opportunity/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6602,
                menu_name = "修改商机",
                parent_id = 6600,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Opportunity/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6603,
                menu_name = "删除商机",
                parent_id = 6600,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Opportunity/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6700,
                menu_name = "跟进计划",
                parent_id = 6,
                order_num = 7,
                path = "plan/list",
                component = "crm/plan/list",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/CRMService/Plan/List",
                icon = "genjinjihua",
                scope = 1,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6701,
                menu_name = "添加计划",
                parent_id = 6700,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Plan/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6702,
                menu_name = "修改计划",
                parent_id = 6700,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Plan/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6703,
                menu_name = "删除计划",
                parent_id = 6700,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Plan/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6704,
                menu_name = "完成计划",
                parent_id = 6700,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Plan/Finish",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6800,
                menu_name = "跟进记录",
                parent_id = 6,
                order_num = 8,
                path = "follow/list",
                component = "crm/follow/list",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/CRMService/Follow/List",
                icon = "genjinjilu",
                scope = 1,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6801,
                menu_name = "添加跟进",
                parent_id = 6800,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Follow/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6802,
                menu_name = "修改跟进",
                parent_id = 6800,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Follow/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 6803,
                menu_name = "删除跟进",
                parent_id = 6800,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/CRMService/Follow/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

         

            Insert.IntoTable("mz_role_permission").Row(new
            {
                RoleID = 4,
                MenuId = 6
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6100
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6110
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6111
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6112
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6113
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6114
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6200
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6201
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6202
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6203
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6204
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6207
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6300
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6301
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6302
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6303
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6304
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6400
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6401
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6402
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6403
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6405
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6406
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6500
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6501
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6502
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6503
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6600
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6601
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6602
            }).Row(new
            {
                RoleID = 4,
                MenuId = 6603
            });

      


            Create.Table("mz_customer").WithDescription("客户表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("CustomerOrgId").WithColumnDescription("所属企业Id")
.WithColumn("DeptId").AsInt64().WithColumnDescription("所属部门")
.WithColumn("CustomerNumber").AsString(50).Unique().WithColumnDescription("客户唯一编号")
.WithColumn("CustomerName").AsString(50).WithColumnDescription("客户名称")
.WithColumn("CustomerType").AsInt32().WithColumnDescription("客户类型：0为代理，1为直销")
.WithColumn("LeaderId").AsInt64().WithColumnDescription("负责人（为0则为公海）")
.WithColumn("FromType").AsString(10).WithColumnDescription("线索来源：微信线索weixin,流程表单form,其它other")
.WithColumn("FromId").AsString(128).WithColumnDescription("来源表单Id")
.WithColumn("BindOrgId").AsInt64().Indexed("BindOrgId").WithColumnDescription("客户对应的企业Id,未邀请则为0")
.WithColumn("Lng").AsDouble().WithColumnDescription("经度")
.WithColumn("Lat").AsDouble().WithColumnDescription("纬度")
.WithColumn("Geo").AsString(30).WithColumnDescription("经纬度的geo编码")
.WithColumn("AddressCode").AsString(6).WithColumnDescription("省市区代码")
.WithColumn("AddressName").AsString(255).WithColumnDescription("地址名称")
.WithColumn("AddressDetail").AsString(255).WithColumnDescription("详细地址")
.WithColumn("Industry").AsInt32().WithColumnDescription("所属行业")
.WithColumn("CompanyTel").AsString(255).WithColumnDescription("公司电话")
.WithColumn("CompanyUrl").AsString(255).WithColumnDescription("公司网址")
.WithColumn("Helper").AsString(500).WithColumnDescription("协作者（多个,号分隔）")
.WithColumn("Remark").AsString(5000).WithColumnDescription("备注")
.WithColumn("LastFollowId").AsString(128).WithColumnDescription("最后一条跟进")
.WithColumn("LastFollowDate").AsDateTime().Nullable().WithColumnDescription("最后一条跟进时间")
      .WithColumn("StartFollowDate").AsDateTime().Nullable().WithColumnDescription("领取时间")
      .WithColumn("ReturnReason").AsString(500).WithColumnDescription("退回原因")
      .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
      .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
      .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
      .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
      .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");
            if (Constants.General.sqltype != "Sqlite")
            {
                Execute.Sql("ALTER TABLE mz_customer ADD FULLTEXT INDEX CustomerHelper (Helper);");
            }


            Create.Table("mz_clue").WithDescription("线索表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("ClueOrgId").WithColumnDescription("所属企业Id")
.WithColumn("DeptId").AsInt64().WithColumnDescription("所属部门")
.WithColumn("RealName").AsString(50).WithColumnDescription("姓名")
.WithColumn("Mobile").AsString(11).WithColumnDescription("手机号码")
.WithColumn("CompanyName").AsString(50).WithColumnDescription("客户名称")
.WithColumn("FromType").AsString(10).WithColumnDescription("线索来源：微信线索weixin,流程表单form,其它other")
.WithColumn("FromId").AsString(128).WithColumnDescription("来源表单Id")
.WithColumn("LeaderId").AsInt64().WithColumnDescription("负责人（为0则为公海）")
.WithColumn("PostName").AsString(50).WithColumnDescription("职务")
.WithColumn("DeptName").AsString(50).WithColumnDescription("部门")
.WithColumn("Helper").AsString(500).WithColumnDescription("协作者（多个,号分隔）")
.WithColumn("Remark").AsString(5000).WithColumnDescription("备注")
.WithColumn("LastFollowId").AsString(128).WithColumnDescription("最后一条跟进")
.WithColumn("LastFollowDate").AsDateTime().Nullable().WithColumnDescription("最后一条跟进时间")
.WithColumn("StartFollowDate").AsDateTime().Nullable().WithColumnDescription("领取时间")
.WithColumn("ChangeDate").AsDateTime().Nullable().WithColumnDescription("转换时间")
.WithColumn("ReturnReason").AsString(500).WithColumnDescription("退回原因")
.WithColumn("ChangeId").AsString(128).WithColumnDescription("转换的目标客户Id")
.WithColumn("SyncFollow").AsBoolean().WithColumnDescription("是否在客户中同步显示跟进")
.WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在、1代表被转换、 2代表删除）")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");
            if (Constants.General.sqltype != "Sqlite")
            {
                Execute.Sql("ALTER TABLE mz_clue ADD FULLTEXT INDEX ClueHelper (Helper);");
            }

            Create.Table("mz_contact").WithDescription("联系人")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("ContactOrgId").WithColumnDescription("所属企业Id")
.WithColumn("RealName").AsString(50).WithColumnDescription("姓名")
.WithColumn("Mobile").AsString(11).WithColumnDescription("手机号码")
.WithColumn("PostName").AsString(50).WithColumnDescription("职务")
.WithColumn("DeptName").AsString(50).WithColumnDescription("部门")
.WithColumn("Sex").AsFixedLengthAnsiString(1).WithColumnDescription("性别（0男 1女 2未知）")
.WithColumn("Email").AsString(50).WithColumnDescription("邮箱")
.WithColumn("WxNumber").AsString(50).WithColumnDescription("微信号")
.WithColumn("Remark").AsString(5000).WithColumnDescription("备注")
.WithColumn("LeaderId").AsInt64().WithColumnDescription("负责人")
.WithColumn("DeptId").AsInt64().WithColumnDescription("所属部门")
.WithColumn("Helper").AsString(500).WithColumnDescription("协作者（多个,号分隔）")
.WithColumn("CustomerId").AsString(128).WithColumnDescription("客户Id")
.WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");
            if (Constants.General.sqltype != "Sqlite")
            {
                Execute.Sql("ALTER TABLE mz_contact ADD FULLTEXT INDEX ContactHelper (Helper);");
            }


            Create.Table("mz_opportunity").WithDescription("商机表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OpportNumber").AsString(50).WithColumnDescription("商机唯一编号")
.WithColumn("OrgId").AsInt64().Indexed("OpportOrgId").WithColumnDescription("所属企业Id")
.WithColumn("DeptId").AsInt64().WithColumnDescription("所属部门")
.WithColumn("LeaderId").AsInt64().WithColumnDescription("负责人")
.WithColumn("OpportName").AsString(50).WithColumnDescription("商机名称")
.WithColumn("CustomerId").AsString(128).WithColumnDescription("客户Id")
.WithColumn("ContactId").AsString(128).WithColumnDescription("联系人Id")
.WithColumn("Probability").AsFloat().WithColumnDescription("预计成交几率，单位%")
.WithColumn("Period").AsString(128).WithColumnDescription("销售阶段")
.WithColumn("PeriodType").AsString(10).WithColumnDescription("阶段类型")
.WithColumn("Helper").AsString(500).WithColumnDescription("协作者（多个,号分隔）")
.WithColumn("Remark").AsString(5000).WithColumnDescription("备注")
.WithColumn("LoseRemark").AsString(2000).WithColumnDescription("输单原因")
.WithColumn("LastFollowId").AsString(128).WithColumnDescription("最后一条跟进")
.WithColumn("LastFollowDate").AsDateTime().Nullable().WithColumnDescription("最后一条跟进时间")
.WithColumn("StartFollowDate").AsDateTime().Nullable().WithColumnDescription("领取时间")
.WithColumn("ReturnReason").AsString(500).WithColumnDescription("退回原因")
.WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");
            if (Constants.General.sqltype != "Sqlite")
            {
                Execute.Sql("ALTER TABLE mz_opportunity ADD FULLTEXT INDEX OpportunityHelper (Helper);");
            }


            Create.Table("mz_opport_detail").WithDescription("商机明细表")
.WithColumn("OpportId").AsString().PrimaryKey().WithColumnDescription("商机Id")
.WithColumn("ProductId").AsString().PrimaryKey().WithColumnDescription("产品Id")
.WithColumn("Quantity").AsInt32().WithColumnDescription("数量")
.WithColumn("Price").AsDecimal(10, 2).WithColumnDescription("价格")
.WithColumn("OrgId").AsInt64().Indexed("OpportDetailOrgId").WithColumnDescription("所属企业Id")
.WithColumn("Sort").AsInt32().WithColumnDescription("排序")
.WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）");


            Create.Table("mz_period").WithDescription("销售阶段表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("PeriodOrgId").WithColumnDescription("所属企业Id")
.WithColumn("PeriodName").AsString(50).WithColumnDescription("阶段名称")
.WithColumn("PeriodType").AsString(10).WithColumnDescription("阶段类型：进行中ing,赢单win,输单lose,无效invalid")
.WithColumn("Probability").AsFloat().WithColumnDescription("赢率，单位%")
.WithColumn("Sort").AsInt32().WithColumnDescription("排序");


            Create.Table("mz_follow_plan").WithDescription("跟进计划表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("FollowPlanOrgId").WithColumnDescription("所属企业Id")
.WithColumn("DeptIds").AsString(500).WithColumnDescription("所属部门（多个,号分隔）")
.WithColumn("CustomerId").AsString(128).WithColumnDescription("跟进的目标客户")
.WithColumn("Executor").AsString(500).WithColumnDescription("计划执行人（多个,号分隔）")
.WithColumn("PlanTime").AsDateTime().WithColumnDescription("计划时间")
.WithColumn("Remark").AsString(5000).WithColumnDescription("计划内容")
.WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("状态：A待完成、F已完成")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");
            if (Constants.General.sqltype != "Sqlite")
            {
                Execute.Sql("ALTER TABLE mz_follow_plan ADD FULLTEXT INDEX FollowPlanDeptIds (DeptIds);");
                Execute.Sql("ALTER TABLE mz_follow_plan ADD FULLTEXT INDEX FollowPlanExecutor (Executor);");
            }


            Create.Table("mz_follow").WithDescription("跟进记录表")
.WithColumn("Id").AsString(128).PrimaryKey().WithColumnDescription("编码")
.WithColumn("OrgId").AsInt64().Indexed("FollowOrgId").WithColumnDescription("所属企业Id")
.WithColumn("DeptId").AsInt64().WithColumnDescription("所属部门")
.WithColumn("TargetType").AsInt32().WithColumnDescription("跟进目标类型：0为客户、1为线索")
.WithColumn("TargetId").AsString().WithColumnDescription("跟进目标Id")
.WithColumn("OpportId").AsString().WithColumnDescription("商机Id")
.WithColumn("Remark").AsString(5000).WithColumnDescription("跟进内容")
.WithColumn("FollowTime").AsDateTime().WithColumnDescription("跟进时间")
.WithColumn("FollowUser").AsInt64().WithColumnDescription("跟进人")
.WithColumn("ContactId").AsString(128).WithColumnDescription("联系人Id")
.WithColumn("FollowWay").AsString(100).WithColumnDescription("跟进方式")
.WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
.WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");





            Insert.IntoTable("mz_dict_type").Row(new
            {
                dict_name = "跟进方式",
                dict_type = "follow_way",
                status = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0,
                remark = "用在CRM的跟进方式"
            });

            Insert.IntoTable("mz_dict_data").Row(new
            {
                dict_sort = 0,
                dict_label = "现场拜访",
                dict_value = "10001",
                dict_type = "follow_way",
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
                dict_label = "电话拜访",
                dict_value = "10002",
                dict_type = "follow_way",
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




            Insert.IntoTable("mz_dict_data").Row(new
            {
                dict_sort = 1,
                dict_label = "线索评论",
                dict_value = "线索",
                dict_type = "comment_type",
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
                dict_label = "客户评论",
                dict_value = "客户",
                dict_type = "comment_type",
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
                dict_label = "商机评论",
                dict_value = "商机",
                dict_type = "comment_type",
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
