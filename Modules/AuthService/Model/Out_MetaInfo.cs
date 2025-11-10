using Common;
using Newtonsoft.Json;
using System;

namespace AuthService
{
    public class Out_MetaInfo
    {
        /// <summary>
        /// 设置该路由在侧边栏和面包屑中展示的名字
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 设置该路由的图标，对应路径src/assets/icons/svg
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 设置为true，则不会被 <keep-alive>缓存
        /// </summary>
        public bool noCache { get; set; }
        /// <summary>
        /// 内链地址（http(s)://开头）
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string link { get; set; }
        public Out_MetaInfo(string title, string icon)
        {
            this.title = title;
            this.icon = icon;
        }
        public Out_MetaInfo(string title, string icon, bool noCache)
        {
            this.title = title;
            this.icon = icon;
            this.noCache = noCache;
        }

        public Out_MetaInfo(String title, string icon, string link)
        {
            this.title = title;
            this.icon = icon;
            this.link = link;
        }

        public Out_MetaInfo(String title, String icon, bool noCache, string link)
        {
            this.title = title;
            this.icon = icon;
            this.noCache = noCache;

            if (link.IsHttp())
            {
                this.link = link;
            }
        }
    }
}
