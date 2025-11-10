using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    public class WarnJsonConfig
    {
        /// <summary>
        /// 数据集
        /// </summary>
        public string DataSet { get; set; }
        /// <summary>
        /// 数据集类型
        /// </summary>
        public string DataSetType { get; set; }
        /// <summary>
        /// 条件字段
        /// </summary>
        public List<ConditionField> Conditions { get; set; }
        /// <summary>
        /// 条件组合
        /// </summary>
        public List<string> Groups { get; set; }
    }
    public class ConditionField
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 比较字符
        /// </summary>
        public string compare { get; set; }
        /// <summary>
        /// 值类型
        /// </summary>
        public string valtype { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object val { get; set; }
    }
}
