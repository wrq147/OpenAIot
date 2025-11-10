using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class Out_FactoryItem : MZ_Factory
    {
        /// <summary>
        /// 组织名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 地址名称
        /// </summary>
        public string AddressName { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string AddressDetail { get; set; }
        /// <summary>
        /// 行业类型代码
        /// </summary>
        public int Industry { get; set; }
        /// <summary>
        /// 行业类型名称
        /// </summary>
        public string IndustryName { get; set; }
        /// <summary>
        /// 员工规模代码
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 员工规模名称
        /// </summary>
        public string SizeName { get; set; }
    }
}
