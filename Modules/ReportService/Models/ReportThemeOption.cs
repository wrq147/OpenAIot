using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    public class ReportThemeOption
    {
        public List<GlobalDataItem> globalData { get; set; }
    }
    public class GlobalDataItem
    {
        public string name { get; set; }
        public string dataSourceType { get; set; }
        public GlobalDatabase database { get; set; }
    }
    public class GlobalDatabase
    {
        public string type { get; set; }
        public string ipAdress { get; set; }
        public int port { get; set; }
		public string baseName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string sourseItem { get; set; }
        public string executeSql { get; set; }
        public string sqlType { get; set; }
    }
}
