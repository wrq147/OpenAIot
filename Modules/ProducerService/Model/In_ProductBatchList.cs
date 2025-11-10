using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class In_ProductBatchList : BaseQueryParam
    {
        /// <summary>
        /// 批次搜索关键词：唯一编号、产品编号、产品名称
        /// </summary>
        public string Key { get; set; }
    }
}
