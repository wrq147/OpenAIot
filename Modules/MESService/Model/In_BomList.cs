using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    public class In_BomList : BaseQueryParam
    {
        /// <summary>
        /// 查询关键字
        /// </summary>
        public string Key { get; set; }
    }
}
