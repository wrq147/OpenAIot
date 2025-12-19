using System;
using System.Collections.Generic;

namespace ChannelUtility.Tsl
{
    public class BaseEvent : BaseAll
    {
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 事件输出
        /// </summary>
        public List<BaseOutputValue> outputs { get; set; }
        /// <summary>
        /// 条件类型：1为在线触发，2为离线触发，3为属性触发。0则只能规则触发，否则根据条件触发
        /// </summary>
        public int CondType { get; set; } = 0;
        /// <summary>
        /// 属性条件
        /// </summary>
        public EventPropCondition[] PropConditions { get; set; }
        /// <summary>
        /// 属性条件组合:(A & B) | C
        /// </summary>
        public string GroupTxt { get; set; }
        /// <summary>
        /// 报警级别：-1忽略、0普通、1告警、2紧急
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 告警目标
        /// </summary>
        public string[] Targets { get; set; }
        /// <summary>
        /// 事件沉默周期，单位秒（默认表示86400S，最小60秒）
        /// </summary>
        public int SilenceTime { get; set; }
    }

    

    public class EventPropCondition
    {
        /// <summary>
        ///  属性代码
        /// </summary>
        public string code { get; set; }
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
        public bool Check(IDictionary<string, DevicePropertyValue> data)
        {
            DevicePropertyValue compare1;
            if (!data.TryGetValue(this.code, out compare1))
            {
                return false;
            }
            bool curcondrs;
            if (this.valtype == "Double")
            {
                double cm1 = Convert.ToDouble(compare1.val);
                double cm2 = Convert.ToDouble(this.val);

                switch (this.compare)
                {
                    case "=":
                        curcondrs = cm1 == cm2;
                        break;
                    case "!=":
                        curcondrs = cm1 != cm2;
                        break;
                    case ">":
                        curcondrs = cm1 > cm2;
                        break;
                    case "<":
                        curcondrs = cm1 < cm2;
                        break;
                    case ">=":
                        curcondrs = cm1 >= cm2;
                        break;
                    case "<=":
                        curcondrs = cm1 <= cm2;
                        break;
                    default:
                        curcondrs = false;
                        break;
                }
            }
            else if (this.valtype == "Long" || this.valtype == "Date")
            {
                long cm1 = Convert.ToInt64(compare1.val);
                long cm2 = Convert.ToInt64(this.val);


                switch (this.compare)
                {
                    case "=":
                        curcondrs = cm1 == cm2;
                        break;
                    case "!=":
                        curcondrs = cm1 != cm2;
                        break;
                    case ">":
                        curcondrs = cm1 > cm2;
                        break;
                    case "<":
                        curcondrs = cm1 < cm2;
                        break;
                    case ">=":
                        curcondrs = cm1 >= cm2;
                        break;
                    case "<=":
                        curcondrs = cm1 <= cm2;
                        break;
                    default:
                        curcondrs = false;
                        break;
                }
            }
            else
            {
                if (this.compare == "=")
                {
                    string compareval = this.val;
                    curcondrs = Convert.ToString(compare1.val) == compareval;
                }
                else if (this.compare == "!=")
                {
                    string compareval = this.val;
                    curcondrs = Convert.ToString(compare1.val) != compareval;
                }
                else
                {
                    curcondrs = false;
                }
            }


            return curcondrs;
        }
    }
}
