using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class In_DevRangeAreaList : In_DevRangeList
    {
        /// <summary>
        /// 组合方式：Province、City、District
        /// </summary>
        public string GroupBy { get; set; }
        /// <summary>
        /// 过滤联网状态：0为离线，1为在线，2为未初始化
        /// </summary>
        public byte? NetStatus { get; set; }
        /// <summary>
        /// 过滤运行状态
        /// </summary>
        public string DState { get; set; }
    }
}
