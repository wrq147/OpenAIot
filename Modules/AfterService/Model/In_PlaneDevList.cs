using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class In_PlaneDevList : BaseQueryParam
    {
        /// <summary>
        /// 计划类型Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 通过关键词搜索设备名称、编号、通讯Id
        /// </summary>
        public string Key { get; set; }
    }
}
