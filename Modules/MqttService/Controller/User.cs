using AuthService.Controller;
using Common;
using Microsoft.Extensions.Options;
using MqttService.Model;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace MqttService.Controller
{
    public class User : AbstractLoginedController
    {
        public User()
        {
        }
        /// <summary>
        /// 获取当前用户连接mqtt的clientid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> ClientId()
        {
            var cliendid = MyAccess.Core.StringTool.GetGUID();
            long uid = GetUser().UserId;
            await this.ServiceProvider.GetService<GeneralRedisHelper>().StringSetAsync("Client:" + cliendid, uid.ToString(), TimeSpan.FromMinutes(2));
            return this.Success(cliendid);
        }
        /// <summary>
        /// Emqx验证连接用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<EmqxResult> AuthConn(EmqxAuthParam param)
        {
            EmqxResult emqrs = new EmqxResult();
            emqrs.is_superuser = false;
            var option = this.ServiceProvider.GetService<IOptions<MqttOption>>();
            if (param.username == option.Value.sys_username && param.password == option.Value.sys_password)
            {
                emqrs.result = "allow";
                return emqrs;
            }
            string rt = await this.ServiceProvider.GetService<GeneralRedisHelper>().StringGetAsync<string>("Client:" + param.clientid);
            if (rt == null)
            {
                emqrs.result = "deny";
                return emqrs;
            }
            else if (param.username != rt)
            {
                emqrs.result = "deny";
                return emqrs;
            }
            else
            {
                emqrs.result = "allow";
                return emqrs;
            }
        }
        /// <summary>
        /// Emqx验证发布和订阅
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<EmqxResult> AuthAcl(EmqxAclParam param)
        {
            EmqxResult emqrs = new EmqxResult();
            emqrs.is_superuser = false;
            var option = this.ServiceProvider.GetService<IOptions<MqttOption>>();
            if (param.username == option.Value.sys_username)
            {
                emqrs.result = "allow";
                return emqrs;
            }
            if (param.action == "subscribe")
            {
                if (param.topic.StartsWith($"user/{param.username}") || param.topic.StartsWith("console/") || param.topic.StartsWith("newprop/") || param.topic.StartsWith("newfun/"))
                {
                    emqrs.result = "allow";
                    return emqrs;
                }
                else
                {
                    emqrs.result = "deny";
                    return emqrs;
                }
            }
            else if (param.action == "publish")
            {
                //暂不限制发布
                emqrs.result = "allow";
                return emqrs;
            }
            else
            {
                emqrs.result = "deny";
                return emqrs;
            }

        }
    }
}
