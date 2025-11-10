using AuthService;
using Common.Attr;
using MyAccess.DB.Attr;


namespace ProducerService.Model
{
    public class Out_Agent : MZ_Agent
    {
        /// <summary>
        /// 上级企业名称
        /// </summary>
        public string ParentOrgName { get; set; }
        /// <summary>
        /// 当前企业名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 级别名称
        /// </summary>
        public string GradeName { get; set; }
        /// <summary>
        /// 生产商名称
        /// </summary>
        public string FactoryName { get; set; }
        /// <summary>
        /// 代理区域名称
        /// </summary>
        public string RegionsName { get; set; }
        /// <summary>
        /// 组织Logo
        /// </summary>
        public string Logo { get; set; }
    }
}
