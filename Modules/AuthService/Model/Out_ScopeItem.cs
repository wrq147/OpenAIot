using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Model
{
    public class Out_ScopeItem
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string menu_name { get; set; }
        /// <summary>
        /// 菜单Id
        /// </summary>
        public long menu_id { get; set; }
        /// <summary>
        /// 数据范围
        /// </summary>
        public string DataScope { get; set; }
    }
}
