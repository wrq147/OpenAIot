using Common.Share;

namespace AuthService
{
    public class In_DictTypeList : BaseQueryParam
    {
        public string dictName { get; set; }
        public string status { get; set; }
        public string dictType { get; set; }

    }
}