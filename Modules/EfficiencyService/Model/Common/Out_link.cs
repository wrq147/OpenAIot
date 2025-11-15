using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    public class Out_link
    {
        /// <summary>
        /// 产品生命周期边界
        /// </summary>
        public string ProductBorder { get; set; }

        /// <summary>
        /// 边界标题
        /// </summary>
        public string BorderTitle { get; set; }

        /// <summary>
        /// 边界名称
        /// </summary>
        public string BorderName { get; set; }

        /// <summary>
        /// 环节编码
        /// </summary>
        public string LinkId { get; set; }

        /// <summary>
        /// 环节名称
        /// </summary>
        public string LinkName { get; set; }
    }
}
