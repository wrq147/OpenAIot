using Common.Share;

namespace AuthService
{
    public class In_ConfigList : BaseQueryParam
    {
        public string configName { get; set; }
        public string configKey { get; set; }
        public string configType { get; set; }
    }
}
