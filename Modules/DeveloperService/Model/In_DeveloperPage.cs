using Common.Share;
using System;
namespace DeveloperService.Model
{
    public class In_DeveloperPage : BaseQueryParam
    {
        /// <summary>
        /// 搜索开发者名称
        /// </summary>
        public string Key { get; set; }
    }
}
