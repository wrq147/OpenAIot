using Common;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace MqttService.Controller
{
    public class Server : TANetController
    {
        public Server()
        {
        }

        /// <summary>
        /// 获取Mqtt的web连接地址
        /// </summary>
        /// <param name="ssl">是否为ssl</param>
        /// <returns></returns>
        [HttpGet]
        public AjaxResult WebIp(bool ssl = false)
        {
            var option = this.ServiceProvider.GetService<IOptions<MqttOption>>();
            if (string.IsNullOrEmpty(option.Value.mqtt_tcp_server))
            {
                if (option.Value.mqtt_web_ssl_port > 0 && option.Value.mqtt_web_port > 0)
                {
                    return this.Success(this.Context.Request.Url.Host + ":" + (ssl ? option.Value.mqtt_web_ssl_port : option.Value.mqtt_web_port));
                }
                else
                {
                    return this.Success(this.Context.Request.Url.Host + ":" + this.Context.Request.Url.Port);
                }

            }
            else
            {
                return this.Success(option.Value.mqtt_tcp_server + ":" + (ssl ? option.Value.mqtt_web_ssl_port : option.Value.mqtt_web_port));
            }
        }
        /// <summary>
        /// 返回服务端格林威治时间和本地时间之间的时差：
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public AjaxResult GetTimezoneOffset()
        {
            var dt = DateTime.Now;
            var udt = dt.ToUniversalTime();
            return this.Success((udt - dt).TotalMinutes);
        }
    }
}
