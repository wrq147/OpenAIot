using AuthService.Controller;
using TemplateAction.Core;
using Common;
using TemplateAction.Route;
using ThirdPartyService.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using AuthService;
using System.Collections.Generic;

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
                var weather_option = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(weatherOption);
                weainfo = new WeatherInfo_V();
                switch (weatherFrom)
                {
                    case "高德":
                        {
                            string tmpkey = weather_option["key"];
                            var rt = await HttpHelper.Instance.GetAsync("https://restapi.amap.com/v3/weather/weatherInfo?city=" + code + "&key=" + tmpkey);
                            var cardrs = JsonConvert.DeserializeObject<dynamic>(rt);
                            if (cardrs.status != 1)
                            {
                                return this.Error<string>(12, (string)cardrs.info);
                            }
                            JArray tmparr = cardrs.lives;
                            if (tmparr.Count == 0)
                            {
                                return this.Error<string>(13, "无天气信息");
                            }
                            weainfo.province = tmparr[0]["province"].ToString();
                            weainfo.city = tmparr[0]["city"].ToString();
                            weainfo.weather = tmparr[0]["weather"].ToString();
                            weainfo.temperature = tmparr[0]["temperature"].ToString();
                            weainfo.winddirection = tmparr[0]["winddirection"].ToString();
                            weainfo.windpower = tmparr[0]["windpower"].ToString();
                            weainfo.humidity = tmparr[0]["humidity"].ToString();
                            weainfo.reporttime = tmparr[0]["reporttime"].ToString();
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
