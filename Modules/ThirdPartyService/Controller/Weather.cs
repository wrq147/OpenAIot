using AuthService.Controller;
using TemplateAction.Core;
using Common;
using TemplateAction.Route;
using ThirdPartyService.Model;
using System.Threading.Tasks;
using System;
using AuthService;
using System.Collections.Generic;
using Common.Json;
using System.Dynamic;
using System.Linq;

namespace ThirdPartyService.Controller
{
    /// <summary>
    /// 第三方天气接口
    /// </summary>
    public class Weather : AbstractLoginedController
    {
        /// <summary>
        /// 根据城市代码获取天气信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(string code)
        {
            string tkey = $"weather::{code}";
            GeneralRedisHelper tmpredis = this.ServiceProvider.GetService<GeneralRedisHelper>();
            WeatherInfo_V weainfo = await tmpredis.StringGetAsync<WeatherInfo_V>(tkey);
            if (weainfo == null)
            {
                var configBLL = this.ServiceProvider.GetService<ConfigBLL>();
                var weatherFrom = await configBLL.SelectConfigByKey("weather.from");
                var weatherOption = await configBLL.SelectConfigByKey("weather.option");
                if (string.IsNullOrEmpty(weatherFrom))
                {
                    return this.Error<WeatherInfo_V>(15, "没有配置天气接口源");
                }
                if (string.IsNullOrEmpty(weatherOption))
                {
                    return this.Error<WeatherInfo_V>(16, $"天气接口源{weatherFrom}未配置");
                }
                var weather_option = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(weatherOption, MyDefaultTextJsonConfig.DefaultOptions);
                weainfo = new WeatherInfo_V();
                switch (weatherFrom)
                {
                    case "高德":
                        {
                            string tmpkey = weather_option["key"];
                            var rt = await HttpHelper.Instance.GetAsync("https://restapi.amap.com/v3/weather/weatherInfo?city=" + code + "&key=" + tmpkey);
                            dynamic cardrs = System.Text.Json.JsonSerializer.Deserialize<object>(rt, MyDefaultTextJsonConfig.DefaultOptions) as ExpandoObject;
                            if (cardrs.status != 1)
                            {
                                return this.Error<string>(12, (string)cardrs.info);
                            }
                            IList<object> tmparr = cardrs.lives;
                            if (tmparr.Count == 0)
                            {
                                return this.Error<string>(13, "无天气信息");
                            }
                            var titem = tmparr.FirstOrDefault() as IDictionary<string, object>;
                            weainfo.province = titem["province"].ToString();
                            weainfo.city = titem["city"].ToString();
                            weainfo.weather = titem["weather"].ToString();
                            weainfo.temperature = titem["temperature"].ToString();
                            weainfo.winddirection = titem["winddirection"].ToString();
                            weainfo.windpower = titem["windpower"].ToString();
                            weainfo.humidity = titem["humidity"].ToString();
                            weainfo.reporttime = titem["reporttime"].ToString();
                        }
                        break;
                }
                //缓存15分钟
                await tmpredis.StringSetAsync<WeatherInfo_V>(tkey, weainfo, TimeSpan.FromMinutes(15));
            }
            return this.Success(weainfo);

        }
    }
}
