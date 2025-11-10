using AuthService;
using Common.Share;
using MyAccess.DB.Attr;
using System;



namespace ProducerService.Model
{
    /// <summary>
    /// 生产商配置
    /// </summary>
    [TableName("mz_factory")]
    public class MZ_Factory : BaseEntity
    {
        /// <summary>
        /// 生产商Id
        /// </summary>
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 代理方式：auto表示逐级自动分配代理级别，manual表示符合代理条件后手动升级
        /// </summary>
        public string GradeWay { get; set; }
        /// <summary>
        /// 授权证书打印模板Id
        /// </summary>
        public string CertTemplateId { get; set; }
        /// <summary>
        /// 自定义批次前缀
        /// </summary>
        public string PHNumPrefix { get; set; }

    }
}
