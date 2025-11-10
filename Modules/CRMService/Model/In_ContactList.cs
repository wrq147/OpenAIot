using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    public class In_ContactList : BaseQueryParam
    {
        /// <summary>
        /// 过滤客户Id
        /// </summary>
        public string CustomerId { get; set; }
    }
}
