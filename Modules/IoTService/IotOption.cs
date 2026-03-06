using System;
namespace IoTService
{
    public class IotOption
    {
        /// <summary>
        /// 规则执行者数量（默认4）
        /// </summary>
        public int runer_count { get; set; }
        /// <summary>
        /// 物联服务名称（多个物联服务名称不能一样）
        /// </summary>
        public string node_name { get; set; }
    }


}
