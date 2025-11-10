using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class In_PartsList : BaseQueryParam
    {
        /// <summary>
        /// 搜索耗材名称或耗材编号
        /// </summary>
        public string Key { get; set; }
    }
}
