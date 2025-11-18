using Common.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    /// <summary>
    /// 协议名称
    /// </summary>
    public class Out_ProductName
    {
        public string Id { get; set; }
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        public string Name { get; set; }
    }
}
