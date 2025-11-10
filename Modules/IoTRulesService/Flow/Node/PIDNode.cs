using System;

namespace IoTRulesService.Flow.Node
{
    public class PIDNode : RuleBaseNode
    {
        public PIDProps props { get; set; }
    }

    public class PIDProps
    {
        /// <summary>
        /// 参考的设备id或选择当前设备
        /// </summary>
        public string refdevid { get; set; }
        /// <summary>
        /// 0为位置式、1为增量式
        /// </summary>
        public int pidtype { get; set; }
        /// <summary>
        /// 参考的设备属性（浮点）
        /// </summary>
        public string refprop { get; set; }
        /// <summary>
        /// 目标值参数（浮点）
        /// </summary>
        public string targetval { get; set; }
        /// <summary>
        /// 比例参数（浮点，一般在 10～50 之间）
        /// </summary>
        public string pname { get; set; }
        /// <summary>
        /// 积分时间（浮点，通常在 30～100 秒之间）
        /// </summary>
        public string iname { get; set; }
        /// <summary>
        /// 微分时间（浮点，一般在 5～20 秒之间）
        /// </summary>
        public string dname { get; set; }
        /// <summary>
        /// 控制的设备列表
        /// </summary>
        public DevControllerItem[] items { get; set; }
    }

    public class DevControllerItem
    {
        /// <summary>
        /// 设备Id或选择当前设备
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 最大控制量参数（浮点型）
        /// </summary>
        public string maxval { get; set; }
        /// <summary>
        /// 最小控制量参数（浮点型）
        /// </summary>
        public string minval { get; set; }
        /// <summary>
        /// 执行的功能
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 功能的控制量参数
        /// </summary>
        public string codeval { get; set; }
    }
}
