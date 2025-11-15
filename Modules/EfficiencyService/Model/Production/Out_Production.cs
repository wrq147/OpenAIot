using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    public class Out_Production : T_Prod_Production
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品类型
        /// </summary>
        public string ProductType { get; set; }


        /// <summary>
        /// 设施名称
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductModel { get; set; }

    }

    public class Out_ProductionDay
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 统计日
        /// </summary>
        public string DDate { get; set; }

        /// <summary>
        /// 产量
        /// </summary>
        public double OutPut { get; set; }

        /// <summary>
        /// 产值
        /// </summary>
        public double OutValue { get; set; }


    }
}
