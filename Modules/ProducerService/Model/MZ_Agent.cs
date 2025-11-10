using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    /// <summary>
    /// 企业代理信息表
    /// </summary>
    [TableName("mz_agent")]
    public class MZ_Agent : BaseEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 上级企业Id
        /// </summary>
        public long? ParentOrgId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 代理级别
        /// </summary>
        public string GradeId { get; set; }
        /// <summary>
        /// 代理商层级信息，每一层,号分隔
        /// </summary>
        public string LevelPath { get; set; }
        /// <summary>
        /// 生产商Id
        /// </summary>
        public long? FactoryId { get; set; }
        /// <summary>
        /// 代理区域代码（多个,号分隔）
        /// </summary>
        public string Regions { get; set; }
    }
}
