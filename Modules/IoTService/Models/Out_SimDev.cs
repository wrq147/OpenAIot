using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class Out_SimDev
    {
        /// <summary>
        /// 编码Id
        /// </summary>
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public int? DeviceUpIdx { get; set; }
    }
}
