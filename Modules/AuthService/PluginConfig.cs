using System;
using TemplateAction.Core;
using TemplateAction.NetCore;
using Microsoft.Extensions.Configuration;
using Common;
using AuthService.Business;
using AuthService.DAL;
using Common.EventBus;
using Common.DataAc;
using System.Collections.Generic;
using AuthService.Fields;

namespace AuthService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => Array.Empty<string>();
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            if (string.IsNullOrEmpty(Constants.General.token_url))
            {
                services.AddTransient<OperatorHelper>();
                services.AddBLL<LoginLogBLL>();
                services.AddBLL<MenuBLL>();
                services.AddBLL<PermissionBLL>();
                services.AddBLL<AuthBLL>();
                services.AddBLL<UserBLL>();
                services.AddBLL<DeptBLL>();
                services.AddBLL<OrgBLL>();
                services.AddBLL<UpgradeBLL>();
                services.AddBLL<DataChangeBLL>();
                services.AddScope<ConfigBLL>();
                services.AddBLL<StyleBLL>();
                services.AddBLL<GroupViewBLL>();

                services.AddDAL<UserDAL>();
                services.AddDAL<LoginLogDAL>();
                services.AddDAL<PermissionDAL>();
                services.AddDAL<MenuDAL>();
                services.AddDAL<OrgDAL>();
                services.AddSingleton<DeptDAL>();
                services.AddSingleton<ConfigDAL>();
                services.AddTransient<CodeBLL>();
                services.AddSingleton<CodeDAL>();
                services.AddSingleton<AuthMiddleware>();
                services.AddDAL<AdminExtDAL>();
                services.AddDAL<OrgExtDAL>();
                services.AddDAL<UpgradeDAL>();
                services.AddDAL<StyleDAL>();
                services.AddDAL<OrgStyleDAL>();
                services.AddDAL<GroupViewDAL>();

                services.AddSingleton<ConfigCache>();
            }

        }
        private DA_Table tb1;
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            //使用身份认证
            ((TASiteApplication)app).UseMiddlewareFirst<AuthMiddleware>();

            #region 可变动数据
            var redis = app.ServiceProvider.GetService<GeneralRedisHelper>();
            tb1 = new DA_Table()
            {
                name = "用户信息",
                code = "mz_admin"
            };
            tb1.fields = new List<DA_Field>
                {
                    new DA_Field()
                    {
                        name = "工作签名",
                        code = "Signature",
                        type = "Text",
                        used = 0,
                        formlist = new List<DA_Value>
                        {
                           new DA_Value()
                           {
                               name="原签名",
                               val="$waitsign"
                           }
                        }
                    },
                    new DA_Field()
                    {
                        name = "用户",
                        code = "Id",
                        type = "User",
                        used = 1,
                        formlist = new List<DA_Value>
                        {
                           new DA_Value()
                           {
                               name="执行人",
                               val="$executor"
                           },
                            new DA_Value()
                           {
                               name="发起人",
                               val="$initiator"
                           }
                        }
                    }
                };

            redis.HashSet("BusChange-Event", "AuthService", new List<DA_Table> { tb1 });
            #endregion


            List<FieldBase> fields = new List<FieldBase>();
            fields.Add(new TextField()
            {
                mapid = "RealName",
                name = "真实姓名",
                type = "文本"
            });
            fields.Add(new TextField()
            {
                mapid = "Signature",
                name = "工作签名",
                type = "文本"
            });
            fields.Add(new TextField()
            {
                mapid = "Mobile",
                name = "手机号码",
                type = "文本"
            });
            redis.HashSet("FixedFields", "用户", fields);


            //监听数据变动
            plg.RegisterCall("ChangeData", async (evt) =>
            {
                var paramdata = evt.To<ActionChangeData>();
                if (tb1.IsThisTable(paramdata))
                {
                    paramdata.TargetName = "用户信息";
                    var res = await app.ServiceProvider.GetService<DataChangeBLL>().DoActionEvent(paramdata);
                    return CallResponse.CreateFrom(res);
                }
                return CallResponse.Next();
            });

            //清除本地缓存配置
            plg.RegisterBus("ClearConfig", async (evt) =>
            {
                app.ServiceProvider.GetService<ConfigCache>().Clear();
            });

        }


    }
}
