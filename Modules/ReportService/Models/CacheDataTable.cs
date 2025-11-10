using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    public class CacheDataTable
    {
        public DataTable dt { get; set; }
        public DateTime expire { get; set; }
    }
}
