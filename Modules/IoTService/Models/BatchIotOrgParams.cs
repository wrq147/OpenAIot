using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Models
{
    public class BatchIotOrgParams
    {
        public List<string> Ids { get; set; }
        public long? UseOrgId { get; set; }
        public long? OwnerOrgId { get; set; }
        public long? ClearOwnerOrgId { get; set; }
    }
}
