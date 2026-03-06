using System;
using System.Collections.Generic;

namespace ChannelUtility.Tsl
{
    public class BaseFunc : BaseAll
    {
        /// <summary>
        /// 前缀标识符
        /// </summary>
        public string prefixcode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 显示方式：null表示全部显示，空为都不显示，org为来源组织显示，own拥有者组织显示，use使用者组织显示，person个人使用者显示，多个用逗号分隔
        /// </summary>
        public string showway { get; set; }
        /// <summary>
        /// 启用条件
        /// </summary>
        public EnableCondition[] conditions { get; set; }
        /// <summary>
        /// 条件组合:(A & B) | C
        /// </summary>
        public string GroupTxt { get; set; }
        /// <summary>
        /// 屏蔽方式：0为隐藏，1为禁用
        /// </summary>
        public int actionway { get; set; }
        /// <summary>
        /// 下发方式：0为数据解释下发，1为modbus下发，2为modbus读属性下发（不等待），3为直接读属性下发（等待），4为图形编程
        /// </summary>
        public int downway { get; set; }
        /// <summary>
        /// 下发方式为1时设置是否等待返回
        /// </summary>
        public bool waitreturn { get; set; }
        /// <summary>
        /// 下发的数据，modbus时为json格式
        /// </summary>
        public string downdata { get; set; }
        /// <summary>
        /// 输入参数
        /// </summary>
        public List<BaseInputValue> inputs { get; set; }
        public IDictionary<string, object> CreateInputs()
        {
            IDictionary<string, object> dict = new Dictionary<string, object>();
            if (inputs != null)
            {
                foreach (var input in inputs)
                {
                    if (input.defval != null)
                    {
                        dict.Add(input.code, input.defval);
                    }
                }
            }
            return dict;
        }
        private ModbusMatch _mm;
        public ModbusMatch GetModbusMatch()
        {
            if (_mm != null)
            {
                return _mm;
            }
            try
            {
                _mm = System.Text.Json.JsonSerializer.Deserialize<ModbusMatch>(this.downdata);
            }
            catch { }
            if (_mm == null)
            {
                throw new Exception($"功能{name}的modbus格式错误");
            }
            return _mm;
        }
       
    }
    public class EnableCondition
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
      
    }
}
