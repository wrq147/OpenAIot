using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Fields
{
    public class SearchPageParam
    {
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 对象类型：用户、部门
        /// </summary>
        public string objtype { get; set; }
        /// <summary>
        /// 分页号
        /// </summary>
        public int pageNum { get; set; }
        /// <summary>
        /// 分页大小
        /// </summary>
        public int pageSize { get; set; }
    }
}
