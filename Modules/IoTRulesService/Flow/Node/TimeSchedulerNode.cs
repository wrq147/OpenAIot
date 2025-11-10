using System;
using System.Collections.Generic;

namespace IoTRulesService.Flow.Node
{
    public class TimeSchedulerNode : RuleBaseNode
    {
        public TimeSchedulerProps props { get; set; }
    }
    public class TimeSchedulerProps
    {
        /// <summary>
        /// 设备Id列表
        /// </summary>
        public string[] DeviceList { get; set; }
        /// <summary>
        /// 调度数量
        /// </summary>
        public int ScheAmount { get; set; }
        /// <summary>
        /// 设备列表配置
        /// </summary>
        public ItemSet[] DeviceSets { get; set; }
    }
    public class ItemSet
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 优先级参数
        /// </summary>
        public string level { get; set; }
        /// <summary>
        /// 调度条件
        /// </summary>
        public ConditionItem[] conditions { get; set; }
        /// <summary>
        /// 条件组合
        /// </summary>
        public string[] groups { get; set; }
        public ActionItem[] actions { get; set; }
    }
    public class ActionItem
    {
        /// <summary>
        /// 目标类型：0为当前设备、1为选择设备
        /// </summary>
        public byte targettype { get; set; } = 0;
        /// <summary>
        /// 目标设备Id
        /// </summary>
        public string targetid { get; set; }
        /// <summary>
        /// 0为功能，1为参数
        /// </summary>
        public int codetype { get; set; }
        /// <summary>
        /// 执行功能（限无参功能）、修改参数
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 功能参数或参数表达式
        /// </summary>
        public string express { get; set; }
        /// <summary>
        /// 是否中断执行失败
        /// </summary>
        public bool errbreak { get; set; }
    }
    public class ConditionItem
    {
        /// <summary>
        ///  条件参数：属性、$参数
        /// </summary>
        public string enablecode { get; set; }
        /// <summary>
        /// 条件值类型:Long、Double、String、Bool、Date
        /// </summary>
        public string valtype { get; set; }
        /// <summary>
        /// 比较
        /// </summary>
        public string compare { get; set; }
        /// <summary>
        /// 条件值
        /// </summary>
        public string val { get; set; }
        /// <summary>
        /// 0为值、1为参数
        /// </summary>
        public int valuefrom { get; set; }
    }
}
