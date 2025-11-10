using Common.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FlowService.FlowNode
{
    public class ObjData
    {
        public long id { get; set; }
        /// <summary>
        /// dept表示部门，user表示用户
        /// </summary>
        public string type { get; set; }
        public string name { get; set; }
        [JsonConverter(typeof(ImageUrl), true)]
        public string avatar { get; set; }
    }
    public class DeviceData
    {
        public string id { get; set; }
        /// <summary>
        /// device表示设备
        /// </summary>
        public string type { get; set; }
        public string name { get; set; }
        [JsonConverter(typeof(ImageUrl))]
        public string photoUrl { get; set; }
        /// <summary>
        /// 设备第三方编号
        /// </summary>
        public string deviceNumber { get; set; }
    }
}
