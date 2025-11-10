using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Models
{
    /// <summary>
    /// 数据库执行请求参数
    /// </summary>
    public class In_Database
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
        /// 数据库名
        /// </summary>
        public string baseName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 执行sql结构
        /// </summary>
        public string executeSql { get; set; }
        /// <summary>
        /// 缓存时间（单位秒）:为0不缓存
        /// </summary>
        public int cacheTime { get; set; } = 0;
    }
}
