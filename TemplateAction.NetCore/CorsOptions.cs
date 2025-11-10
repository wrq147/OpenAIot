
namespace TemplateAction.NetCore
{
    /// <summary>
    /// 跨域选项
    /// </summary>
    public class CorsOptions
    {
        public static CorsOptions Default = new CorsOptions()
        {
            AllowOrigin = "*",
            AllowMethods = "*",
            AllowCredentials = "true",
            AllowHeaders = ""
        };
        public string AllowOrigin { get; set; }
        public string AllowCredentials { get; set; }
        public string AllowMethods { get; set; }
        public string AllowHeaders { get; set; }
    }
}
