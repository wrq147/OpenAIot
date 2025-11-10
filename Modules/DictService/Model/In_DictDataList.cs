using Common.Share;
using System;

namespace DictService.Model
{
    public class In_DictDataList : BaseQueryParam
    {
        public string dictName { get; set; }
        public string dictType { get; set; }
        public string status { get; set; }
    }
}
