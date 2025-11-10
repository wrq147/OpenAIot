using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    public class In_DefectList : BaseQueryParam
    {
        /// <summary>
        /// 搜索不良品的关键词
        /// </summary>
        public string Key { get; set; }
    }
}
