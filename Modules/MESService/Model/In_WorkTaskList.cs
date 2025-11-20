using AuthService.Fields;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    public class In_WorkTaskList : BaseQueryParam
    {
        /// <summary>
        /// 0为进行中，1为已完成
        /// </summary>
        public int? Status { get; set; }
    }
}
