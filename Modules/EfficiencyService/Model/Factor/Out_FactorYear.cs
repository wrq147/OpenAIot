using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyAccess.DB.Attr;

namespace EfficiencyService.Model
{
    public class Out_FactorYear
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 发布年份
        /// </summary>
        public string Year { get; set; }

        public List<Out_FactorYearVersion> Versions { get; set; }

    }

    public class Out_FactorYearVersion
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 年份ID
        /// </summary>
        public string YearId { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

    }
}
