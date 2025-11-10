using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    public class In_DatabaseNames
    {
        /// <summary>
        /// 数据源类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 数据源链接IP地址
        /// </summary>
        public string ipAdress { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int port { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
    }
}
