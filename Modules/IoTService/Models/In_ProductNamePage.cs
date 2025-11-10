using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class In_ProductNamePage : BaseQueryParam
    {
        /// <summary>
        /// 过滤分类
        /// </summary>
        public string ClassId { get; set; }
    }
}
