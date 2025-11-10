using System;

namespace ThirdPartyService.Model
{
    public class WeatherInfo_V
    {
        /// <summary>
        /// 省名称
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 市名称
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 天气现象
        /// </summary>
        public string weather { get; set; }
        /// <summary>
        /// 实时气温，单位：摄氏度
        /// </summary>
        public string temperature { get; set; }
        /// <summary>
        /// 风向描述
        /// </summary>
        public string winddirection { get; set; }
        /// <summary>
        /// 风力级别，单位：级
        /// </summary>
        public string windpower { get; set; }
        /// <summary>
        /// 空气湿度
        /// </summary>
        public string humidity { get; set; }
        /// <summary>
        /// 数据发布的时间
        /// </summary>
        public string reporttime { get; set; }
    }
}
