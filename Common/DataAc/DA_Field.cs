using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Common.DataAc
{
    public class DA_Field
    {
        /// <summary>
        /// 字段中文名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 字段代码名称
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 字段输入方式：Enum（枚举从后台获取枚举数据）、Text（文本输入）、Number（数字型）
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 可选项
        /// </summary>
        public List<DA_Value> options { get; set; }
        /// <summary>
        /// 表单可选项
        /// </summary>
        public List<DA_Value> formlist { get; set; }
        /// <summary>
        /// 0为全部可用，1为仅过滤，2为仅字段
        /// </summary>
        public int used { get; set; }
    }
}
