using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class In_HouseList : BaseQueryParam
    {
        /// <summary>
        /// 过滤名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 状态：0为停用，1为正常
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 过滤系统仓库
        /// </summary>
        public bool? IsSystem { get; set; }
    }
}
