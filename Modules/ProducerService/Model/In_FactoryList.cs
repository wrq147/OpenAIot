using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class In_FactoryList : BaseQueryParam
    {
        /// <summary>
        /// 过滤生产商名称
        /// </summary>
        public string FactoryName { get; set; }
    }
}
