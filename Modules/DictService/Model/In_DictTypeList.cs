using Common.Share;

namespace DictService.Model
{
    public class In_DictTypeList : BaseQueryParam
    {
        public string dictName { get; set; }
        public string status { get; set; }
        public string dictType { get; set; }

    }
}