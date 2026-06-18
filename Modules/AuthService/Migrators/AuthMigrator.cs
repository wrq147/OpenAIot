using Common;
using FluentMigrator;
using System;
using System.IO;
using System.Threading;

namespace AuthService
{
    [Migration(20220000000)]
    public class AuthMigrator : Migration
    {

        public override void Up()
        {
            Create.Table("mz_admin").WithDescription("用户表")
      .WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("ID")
      .WithColumn("OrgId").AsInt64().WithColumnDescription("最近一次使用的企业编号")
      .WithColumn("UserName").AsString(32).Indexed("AdmUserName").WithColumnDescription("账号,不存在则为空")
      .WithColumn("RealName").AsString(50).WithColumnDescription("真实姓名")
      .WithColumn("Password").AsString(64).WithColumnDescription("密码(内部用)")
      .WithColumn("Salt").AsString(16).WithColumnDescription("密码盐(内部用)")
      .WithColumn("Mobile").AsString(20).Indexed("AdmMobile").WithDefaultValue(string.Empty).WithColumnDescription("手机号码")
      .WithColumn("Email").AsString(50).Indexed("AdmEmail").WithDefaultValue(string.Empty).WithColumnDescription("用户邮箱")
      .WithColumn("EmailActive").AsBoolean().WithDefaultValue(false).WithColumnDescription("邮箱是否激活")
      .WithColumn("Avatar").AsString(255).WithColumnDescription("用户头像")
      .WithColumn("Sex").AsFixedLengthAnsiString(1).WithColumnDescription("用户性别（0男 1女 2未知）")
      .WithColumn("Introduction").AsString(500).WithColumnDescription("备注")
      .WithColumn("status").AsFixedLengthAnsiString(1).WithColumnDescription("帐号状态（0正常 1停用）")
      .WithColumn("Signature").AsString(50).WithDefaultValue("").WithColumnDescription("工作签名")
      .WithColumn("WaitSignature").AsString(50).WithDefaultValue("").WithColumnDescription("原工作签名")
      .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
      .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
      .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
      .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
      .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Insert.IntoTable("mz_admin").Row(new
            {
                Id = 1,
                OrgId = 1,
                UserName = "admin",
                RealName = "管理员",
                Password = "6dbad6a6ca882e9aabda48cd82c56b3e",
                Salt = "tPsSkMc04yjonyyT",
                Mobile = string.Empty,
                Email = string.Empty,
                Avatar = string.Empty,
                Sex = "0",
                Introduction = string.Empty,
                status = "0",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                //此账号用来表示系统，无任何权限
                Id = 2,
                OrgId = 1,
                UserName = "system",
                RealName = "系统",
                Password = "333874df393be07d46b1cfdc7aa88037",
                Salt = "222",
                Mobile = string.Empty,
                Email = string.Empty,
                Avatar = string.Empty,
                Sex = "0",
                Introduction = string.Empty,
                status = "0",
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Create.Table("mz_config").WithDescription("参数表")
      .WithColumn("config_id").AsInt64().PrimaryKey().WithColumnDescription("参数主键")
      .WithColumn("config_name").AsString(100).WithColumnDescription("参数名称")
      .WithColumn("config_key").AsString(100).WithColumnDescription("参数键名")
      .WithColumn("config_value").AsString(3000).WithColumnDescription("参数键值")
      .WithColumn("config_type").AsFixedLengthAnsiString(1).WithColumnDescription("系统内置（Y是 N否）")
      .WithColumn("remark").AsString(500).WithColumnDescription("备注")
      .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
      .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
      .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
      .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");


            Insert.IntoTable("mz_config").Row(new
            {
                config_id = 1,
                config_name = "主框架页-默认皮肤样式名称",
                config_key = "sys.index.skinName",
                config_value = "skin-blue",
                config_type = "Y",
                remark = "蓝色 skin-blue、绿色 skin-green、紫色 skin-purple、红色 skin-red、黄色 skin-yellow",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                config_id = 2,
                config_name = "用户管理-账号初始密码",
                config_key = "sys.user.initPassword",
                config_value = "123456",
                config_type = "Y",
                remark = "初始化密码 123456",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                config_id = 3,
                config_name = "主框架页-侧边栏主题",
                config_key = "sys.index.sideTheme",
                config_value = "theme-dark",
                config_type = "Y",
                remark = "深色主题theme-dark，浅色主题theme-light",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                config_id = 4,
                config_name = "账号自助-验证码开关",
                config_key = "sys.account.captchaOnOff",
                config_value = "true",
                config_type = "Y",
                remark = "是否开启验证码功能（true开启，false关闭）",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                config_id = 5,
                config_name = "账号自助-是否开启用户注册功能",
                config_key = "sys.account.registerUser",
                config_value = "false",
                config_type = "Y",
                remark = "是否开启注册用户功能（true开启，false关闭）",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                config_id = 6,
                config_name = "绑定邮箱-邮件发送的模板",
                config_key = "sys.email.sendTemplate",
                config_value = @"<table align=""center"" cellpadding=""0"" cellspacing =""0"" border =""0""
    style=""width:700px;padding:0 10px;margin:0 auto;font-family:arial,&#39;微软雅黑&#39;;color:#333333;font-size:14px;"">
    <tr style=""background -color:#1B84B3;text-align:center;height:65px;color:#fff;font-size: 20px; font-weight: bold;"">
        <td>悟空云</td>
    </tr><tr><td><table align=""center"" cellpadding =""0"" cellspacing =""0"" border =""0"" style =""width:500px;"">
                <tr>
                    <td style=""padding -top:35px;font-size:18px;text-align:center;"">您可能被要求输入验证码：</td>
                </tr>
                <tr>
                    <td style=""padding:5px 0 30px;font-size:24px;text-align:center;border-bottom:1px solid #969FA8;font-weight:bold;"">@{code}</td>
                </tr>
                <tr>
                    <td style=""padding -top:40px;line-height:24px;"">亲爱的用户:</td>
                </tr>
                <tr>
                    <td style=""line -height:24px;"">点击确认按钮，即可快速绑定备用邮箱。</td>
                </tr>
                <tr>
                    <td style=""padding -top:10px;text-align:left;text-decoration:none;""><a href=""@{link}"" target=""_blank"" style=""border -radius:3px;width:100%;line-height:40px;background-color:#29ABE1;color:#fff;cursor:pointer;display:block;text-align:center;text-decoration:none;"" >确认</a></td>
                </tr>
                <tr>
                    <td style=""padding -top:20px;line-height:24px;"">绑定备用邮箱可避免您无法登录的情况发生。大部分会员使用工作与个人两个邮箱。</td>
                </tr>
                <tr>
                    <td style=""padding -top:20px;line-height:24px;"">链接无法打开？ 请将如下地址复制并粘贴到您的浏览器地址栏中:</td>
                </tr>
                <tr>
                    <td style=""line -height:24px;word-break:break-all;""><a style=""color:#333;"" href =""@{link}"" >@{link}</a></td>
                </tr>
                <tr>
                    <td style=""padding -top:20px;line-height:24px;"">祝您使用愉快！</td>
                </tr>
                <tr>
                    <td style=""padding -top:20px;line-height:20px;font-size:11px;color:#acacac;text-align:center;"">此邮件由系统发出，请不要直接回复</td>
                </tr>
                <tr>
                    <td style=""line -height:20px;font-size:11px;color:#acacac;text-align:center;"">如果您有任何建议和问题，欢迎与我们联系：<a href=""876513851@qq.com"" style =""font -weight:bold;color:#acacac;"" >876513851@qq.com</a></td>
                </tr>
                <tr>
                    <td style=""padding -bottom:20px;line-height:20px;font-size:11px;color:#acacac;text-align:center;"">Copyright @ 2022 悟空云 | 保留所有权利</td>
                </tr>
            </table></td></tr></table>",
                config_type = "Y",
                remark = "@{code}变量表示绑定时的验证码，@{link}表示通过链接绑定的地址",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                config_id = 12,
                config_name = "邮箱验证码-邮件发送的模板",
                config_key = "sys.email.validTemplate",
                config_value = @"<table align=""center"" cellpadding=""0"" cellspacing =""0"" border =""0""
    style=""width:700px;padding:0 10px;margin:0 auto;font-family:arial,&#39;微软雅黑&#39;;color:#333333;font-size:14px;"">
    <tr style=""background -color:#1B84B3;text-align:center;height:65px;color:#fff;font-size: 20px; font-weight: bold;"">
        <td>悟空云</td>
    </tr><tr><td><table align=""center"" cellpadding =""0"" cellspacing =""0"" border =""0"" style =""width:500px;"">
                <tr>
                    <td style=""padding -top:35px;font-size:18px;text-align:center;"">您可能被要求输入验证码：</td>
                </tr>
                <tr>
                    <td style=""padding:5px 0 30px;font-size:24px;text-align:center;border-bottom:1px solid #969FA8;font-weight:bold;"">@{code}</td>
                </tr>
                <tr>
                    <td style=""padding -top:20px;line-height:20px;font-size:11px;color:#acacac;text-align:center;"">此邮件由系统发出，请不要直接回复</td>
                </tr>
                <tr>
                    <td style=""line -height:20px;font-size:11px;color:#acacac;text-align:center;"">如果您有任何建议和问题，欢迎与我们联系：<a href=""876513851@qq.com"" style =""font -weight:bold;color:#acacac;"" >876513851@qq.com</a></td>
                </tr>
                <tr>
                    <td style=""padding -bottom:20px;line-height:20px;font-size:11px;color:#acacac;text-align:center;"">Copyright @ 2022 悟空云 | 保留所有权利</td>
                </tr>
            </table></td></tr></table>",
                config_type = "Y",
                remark = "@{code}变量表示验证码",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });



            Alter.Table("mz_config").AlterColumn("config_id").AsInt64().Identity();

            Create.Table("mz_org").WithDescription("组织单位表")
.WithColumn("Id").AsInt64().PrimaryKey().WithColumnDescription("组织编号")
.WithColumn("OrgName").AsString(50).WithColumnDescription("组织名称")
.WithColumn("KeyWords").AsString(2000).WithColumnDescription("组织关键字")
.WithColumn("Logo").AsString(255).WithDefaultValue(string.Empty).WithColumnDescription("组织Logo")
.WithColumn("Industry").AsInt32().WithColumnDescription("行业类型")
.WithColumn("Size").AsInt32().WithColumnDescription("员工规模")
.WithColumn("Lng").AsDouble().WithColumnDescription("经度")
.WithColumn("Lat").AsDouble().WithColumnDescription("纬度")
.WithColumn("Geo").AsString(30).Nullable().WithColumnDescription("经纬度的geo编码")
.WithColumn("AddressCode").AsString(6).WithColumnDescription("省市区代码")
.WithColumn("AddressName").AsString(255).WithColumnDescription("地址名称")
.WithColumn("AddressDetail").AsString(255).WithColumnDescription("详细地址")
.WithColumn("Intro").AsString(3000).WithColumnDescription("企业简介")
.WithColumn("status").AsFixedLengthAnsiString(1).WithColumnDescription("组织认证状态（0未认证 1为认证中 2为已认证）")
.WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间")
.WithColumn("createId").AsInt64().WithColumnDescription("归属人Id")
.WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id");

            if (Constants.General.sqltype != "Sqlite")
            {
                Execute.Sql("ALTER TABLE mz_org ADD FULLTEXT INDEX OrgKeywords (KeyWords);");
            }


            Insert.IntoTable("mz_org").Row(new
            {
                Id = 1,
                OrgName = "系统组织",
                KeyWords = "系统 系统组织",
                Logo = string.Empty,
                Industry = 0,
                Size = 0,
                Lng = 0,
                Lat = 0,
                Geo = string.Empty,
                AddressCode = string.Empty,
                AddressName = string.Empty,
                AddressDetail = string.Empty,
                Intro = string.Empty,
                status = 0,
                del_flag = "0",
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 1,
                updateId = 1
            });


            Create.Table("mz_dept").WithDescription("部门表")
        .WithColumn("dept_id").AsInt64().PrimaryKey().WithColumnDescription("部门id")
        .WithColumn("parent_id").AsInt64().WithColumnDescription("父部门id")
        .WithColumn("ancestors").AsString(500).WithColumnDescription("祖级列表")
        .WithColumn("dept_name").AsString(50).WithColumnDescription("部门名称")
        .WithColumn("order_num").AsInt32().WithColumnDescription("显示顺序")
        .WithColumn("phone").AsString(11).WithColumnDescription("联系电话")
        .WithColumn("email").AsString(50).WithColumnDescription("邮箱")
        .WithColumn("status").AsFixedLengthAnsiString(1).WithColumnDescription("部门状态（0正常 1停用）")
        .WithColumn("del_flag").AsFixedLengthAnsiString(1).WithColumnDescription("删除标志（0代表存在 2代表删除）")
        .WithColumn("OrgId").AsInt64().Indexed("FDeptOrgId").WithColumnDescription("关联组织ID")
        .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
        .WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
        .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id")
        .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间");

            Insert.IntoTable("mz_dept").Row(new
            {
                dept_id = 1,
                parent_id = 0,
                ancestors = "1,",
                dept_name = "系统默认组织",
                order_num = 0,
                phone = string.Empty,
                email = string.Empty,
                status = "0",
                del_flag = "0",
                OrgId = 1,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });


            Create.Table("mz_user_org").WithDescription("用户组织关联表")
.WithColumn("UserId").AsInt64().PrimaryKey().WithColumnDescription("关联用户ID")
.WithColumn("OrgId").AsInt64().PrimaryKey().WithColumnDescription("关联组织ID")
.WithColumn("dept_id").AsInt64().PrimaryKey().Indexed().WithColumnDescription("所属部门")
.WithColumn("post_name").AsString(50).WithColumnDescription("当前职位")
.WithColumn("IsLeader").AsBoolean().WithDefaultValue(false).WithColumnDescription("是否为部门领导")
.WithColumn("IsPrimary").AsBoolean().WithDefaultValue(true).WithColumnDescription("是否为主要部门");

            Insert.IntoTable("mz_user_org").Row(new
            {
                UserId = 1,
                OrgId = 1,
                dept_id = 1,
                post_name = string.Empty,
                IsLeader = false,
                IsPrimary = true
            }).Row(new
            {
                UserId = 2,
                OrgId = 1,
                dept_id = 1,
                post_name = string.Empty,
                IsLeader = false,
                IsPrimary = true
            });

            Execute.Sql("CREATE VIEW mz_admin_v as select u.*,uo.dept_id,uo.post_name from mz_admin u left join mz_user_org uo on u.Id=uo.UserId and u.OrgId=uo.OrgId and uo.IsPrimary=1");
            Execute.Sql("CREATE VIEW mz_admin_ov as select u.Id,u.UserName,u.RealName,u.Password,u.Salt,u.Mobile,u.Email,u.EmailActive,u.Avatar,u.Sex,u.Introduction,u.status,u.Signature,u.WaitSignature,u.del_flag,u.createId,u.create_time,u.updateId,u.update_time,uo.dept_id,uo.post_name,uo.OrgId,uo.IsPrimary from mz_admin u left join mz_user_org uo on u.Id=uo.UserId");


            Create.Table("mz_login_log").WithDescription("登录日志表")
.WithColumn("SysLogID").AsInt64().PrimaryKey().Identity().WithColumnDescription("id编号")
.WithColumn("UserId").AsInt64().Indexed("syslogloginname").WithColumnDescription("登录用户ID")
.WithColumn("Status").AsByte().WithColumnDescription("登录状态（0成功 1失败）")
.WithColumn("IPAddress").AsString(50).WithColumnDescription("ip地址")
.WithColumn("IPLocation").AsString(100).WithColumnDescription("登录地点")
.WithColumn("Browser").AsString(50).WithColumnDescription("浏览器类型")
.WithColumn("OS").AsString(50).WithColumnDescription("操作系统")
.WithColumn("Terminal").AsString(50).WithDefaultValue(string.Empty).WithColumnDescription("终端设备")
.WithColumn("Info").AsString(255).WithColumnDescription("提示消息")
.WithColumn("CreateDate").AsDateTime().Indexed("loginLogdateDes").WithColumnDescription("记录时间");

            Create.Index("IDXUserLogCreate").OnTable("mz_login_log").OnColumn("UserId").Ascending().OnColumn("Terminal").Ascending().OnColumn("CreateDate").Descending();


            Create.Table("mz_menu").WithDescription("菜单权限表")
.WithColumn("menu_id").AsInt64().PrimaryKey().WithColumnDescription("菜单ID")
.WithColumn("menu_name").AsString(50).WithColumnDescription("菜单名称")
.WithColumn("name").AsString(50).Nullable().WithColumnDescription("路由名称")
.WithColumn("parent_id").AsInt64().WithColumnDescription("父菜单ID")
.WithColumn("order_num").AsInt32().WithColumnDescription("显示顺序")
.WithColumn("path").AsString(200).WithColumnDescription("路由地址")
.WithColumn("component").AsString(255).WithColumnDescription("组件路径")
.WithColumn("query").AsString(255).WithColumnDescription("路由参数")
.WithColumn("is_frame").AsByte().WithColumnDescription("是否为外链（0否 1是）")
.WithColumn("is_cache").AsByte().WithColumnDescription("是否缓存（0缓存 1不缓存）")
.WithColumn("menu_type").AsFixedLengthAnsiString(1).WithColumnDescription("菜单类型（M目录 C菜单 F按钮）")
.WithColumn("visible").AsFixedLengthAnsiString(1).WithColumnDescription("显示状态（0显示 1隐藏）")
.WithColumn("status").AsFixedLengthAnsiString(1).WithColumnDescription("菜单状态（0正常 1停用）")
.WithColumn("perms").AsString(255).WithColumnDescription("权限标识")
.WithColumn("icon").AsString(100).WithColumnDescription("菜单图标")
.WithColumn("scope").AsByte().WithDefaultValue(0).WithColumnDescription("是否过滤数据")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
.WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间")
        .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
        .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id");




            Insert.IntoTable("mz_menu").Row(new
            {
                menu_id = 1,
                menu_name = "系统管理",
                parent_id = 0,
                order_num = 1,
                path = "system",
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "M",
                visible = "0",
                status = "0",
                perms = "Workbench",
                icon = "system",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 100,
                menu_name = "账号管理",
                parent_id = 1,
                order_num = 1,
                path = "user",
                component = "system/user/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/AuthService/User/",
                icon = "user",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 102,
                menu_name = "菜单管理",
                parent_id = 1,
                order_num = 3,
                path = "menu",
                component = "system/menu/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/AuthService/Menu/",
                icon = "tree-table",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 106,
                menu_name = "参数设置",
                parent_id = 1,
                order_num = 7,
                path = "config",
                component = "system/config/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 0,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/AuthService/Config/",
                icon = "edit",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 116,
                menu_name = "系统接口",
                parent_id = 1,
                order_num = 4,
                path = "swagger",
                component = "system/swagger/index",
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "C",
                visible = "0",
                status = "0",
                perms = "/SysTool/Swagger/List",
                icon = "swagger",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1001,
                menu_name = "用户查询",
                parent_id = 100,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/User/List",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1002,
                menu_name = "用户新增",
                parent_id = 100,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/User/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1003,
                menu_name = "用户修改",
                parent_id = 100,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/User/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1004,
                menu_name = "用户删除",
                parent_id = 100,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/User/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1005,
                menu_name = "用户导出",
                parent_id = 100,
                order_num = 5,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/User/Export",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1006,
                menu_name = "用户导入",
                parent_id = 100,
                order_num = 6,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/User/Import",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1007,
                menu_name = "重置密码",
                parent_id = 100,
                order_num = 7,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/User/ResetPwd",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1013,
                menu_name = "菜单查询",
                parent_id = 102,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Menu/List",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1014,
                menu_name = "菜单新增",
                parent_id = 102,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Menu/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1015,
                menu_name = "菜单修改",
                parent_id = 102,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Menu/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1016,
                menu_name = "菜单删除",
                parent_id = 102,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Menu/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1031,
                menu_name = "参数查询",
                parent_id = 106,
                order_num = 1,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Config/List",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1032,
                menu_name = "参数新增",
                parent_id = 106,
                order_num = 2,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Config/Add",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1033,
                menu_name = "参数修改",
                parent_id = 106,
                order_num = 3,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Config/Edit",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1034,
                menu_name = "参数删除",
                parent_id = 106,
                order_num = 4,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Config/Remove",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            }).Row(new
            {
                menu_id = 1035,
                menu_name = "参数导出",
                parent_id = 106,
                order_num = 5,
                path = string.Empty,
                component = string.Empty,
                query = string.Empty,
                is_frame = 0,
                is_cache = 1,
                menu_type = "F",
                visible = "0",
                status = "0",
                perms = "/AuthService/Config/Export",
                icon = "#",
                scope = 0,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });


            Create.Table("mz_role").WithDescription("角色表")
.WithColumn("RoleID").AsInt64().PrimaryKey().Identity().WithColumnDescription("角色ID")
.WithColumn("RoleName").AsString(255).WithColumnDescription("角色名称")
.WithColumn("RoleSort").AsInt32().WithColumnDescription("显示顺序")
.WithColumn("RoleDesc").AsString(500).WithColumnDescription("角色描述")
.WithColumn("IsSystem").AsFixedLengthAnsiString(1).WithColumnDescription("是否为系统角色，不可删除，不可修改")
.WithColumn("NoAlloca").AsFixedLengthAnsiString(1).WithDefaultValue("0").WithColumnDescription("是否禁止分配:1为禁止，0为可分配")
.WithColumn("Status").AsFixedLengthAnsiString(1).WithColumnDescription("角色状态（0正常 1停用）")
.WithColumn("OrgId").AsInt64().WithColumnDescription("关联组织ID")
.WithColumn("create_time").AsDateTime().WithColumnDescription("创建时间")
        .WithColumn("update_time").AsDateTime().WithColumnDescription("更新时间")
        .WithColumn("createId").AsInt64().WithColumnDescription("创建者Id")
        .WithColumn("updateId").AsInt64().WithColumnDescription("更新者Id");

            Insert.IntoTable("mz_role").Row(new
            {
                RoleID = 1,
                RoleName = "超级管理员",
                RoleSort = 0,
                RoleDesc = "初始角色,开发人员专用",
                IsSystem = "1",
                Status = "0",
                OrgId = 1,
                create_time = DateTime.Now,
                update_time = DateTime.Now,
                createId = 0,
                updateId = 0
            });

            Create.Table("mz_role_scope").WithDescription("角色的数据权限")
.WithColumn("RoleID").AsInt64().PrimaryKey().WithColumnDescription("角色Id")
.WithColumn("MenuId").AsInt64().PrimaryKey().WithColumnDescription("菜单Id")
.WithColumn("DataScope").AsFixedLengthAnsiString(1).WithColumnDescription("数据范围（1：全部数据权限 2：自定数据权限 3：本部门数据权限 4：本部门及以下数据权限 5:仅本人数据权限）")
.WithColumn("CustomScope").AsString(10000).Nullable().WithColumnDescription("自定义数据权限Json");


            Create.Table("mz_role_permission").WithDescription("角色权限表")
.WithColumn("RoleID").AsInt64().PrimaryKey().WithColumnDescription("角色ID")
.WithColumn("MenuId").AsInt64().PrimaryKey().WithColumnDescription("权限ID");

            Insert.IntoTable("mz_role_permission").Row(new
            {
                RoleID = 1,
                MenuId = 1
            }).Row(new
            {
                RoleID = 1,
                MenuId = 102
            }).Row(new
            {
                RoleID = 1,
                MenuId = 1013
            }).Row(new
            {
                RoleID = 1,
                MenuId = 1014
            }).Row(new
            {
                RoleID = 1,
                MenuId = 1015
            }).Row(new
            {
                RoleID = 1,
                MenuId = 1016
            }).Row(new
            {
                RoleID = 1,
                MenuId = 3
            }).Row(new
            {
                RoleID = 1,
                MenuId = 116
            });

            Create.Table("mz_user_role").WithDescription("用户角色表")
.WithColumn("UserId").AsInt64().PrimaryKey().WithColumnDescription("用户ID")
.WithColumn("RoleID").AsInt64().PrimaryKey().WithColumnDescription("角色ID")
.WithColumn("OrgId").AsInt64().PrimaryKey().WithDefaultValue(0).WithColumnDescription("用户在指定企业才拥有此角色，为0表示系统授权的角色");

            //超管赋权
            Insert.IntoTable("mz_user_role").Row(new
            {
                UserId = 1,
                RoleID = 1,
                OrgId = 1
            });


        }
        public override void Down()
        {
        }

    }
}
