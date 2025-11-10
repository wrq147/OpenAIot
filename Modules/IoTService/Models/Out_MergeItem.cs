using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class Out_MergeItem
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string MergeWay { get; set; }
        public DateTime Time { get; set; }
        public object Val { get; set; }
        public string Unit { get; set; }
    }
}
