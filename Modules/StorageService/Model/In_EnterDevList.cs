using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    /// <summary>
    /// 入库列表的参数
    /// </summary>
    public class In_EnterDevList : BaseQueryParam
    {
        /// <summary>
        /// 过滤设备名称、设备绑定的通讯Id、设备的唯一编号
        /// </summary>
        public string Key { get; set; }
    }
}
