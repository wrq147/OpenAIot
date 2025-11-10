using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Models
{
    public class TagSave_In
    {
        /// <summary>
        /// 设备第三方编码
        /// </summary>
        public string number { get; set; }
        /// <summary>
        /// 标签值列表
        /// </summary>
        public List<TagSaveItem> list { get; set; }
    }
    public class TagSaveItem
    {
        /// <summary>
        /// 标签标识
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标签值
        /// </summary>
        public object Value { get; set; }
    }
}
