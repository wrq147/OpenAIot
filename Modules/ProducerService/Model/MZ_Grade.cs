using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    /// <summary>
    /// 代理级别
    /// </summary>
    [TableName("mz_grade")]
    public class MZ_Grade
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 代理级别
        /// </summary>
        public string GradeName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 0为普通，1为系统
        /// </summary>
        public int? IsSystem { get; set; }
        /// <summary>
        /// 生产商Id
        /// </summary>
        public long? FactoryId { get; set; }

    }
}
