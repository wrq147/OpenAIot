using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class Out_PlaneStatis
    {
        /// <summary>
        /// 进行中数量
        /// </summary>
        public int doing { get; set; }
        /// <summary>
        /// 待执行数量
        /// </summary>
        public int wait_work { get; set; }
        /// <summary>
        /// 执行中数量
        /// </summary>
        public int working { get; set; }
        /// <summary>
        /// 已完成数量
        /// </summary>
        public int finish { get; set; }
        /// <summary>
        /// 已过期数量
        /// </summary>
        public int expired { get; set; }
        /// <summary>
        /// 已验收数量
        /// </summary>
        public int accepted { get; set; }
        /// <summary>
        /// 验收失败数量
        /// </summary>
        public int noaccept { get; set; }
        /// <summary>
        /// 已作废数量
        /// </summary>
        public int invalid { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public int total { get; set; }
    }

    public class Out_PlaneStatisItem: Out_PlaneStatis
    {
        public string Name { get; set; }
    }
}
