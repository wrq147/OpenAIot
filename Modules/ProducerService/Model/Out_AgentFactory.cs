using Common.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    public class Out_AgentFactory : MZ_Agent
    {
        /// <summary>
        /// 生产商名称
        /// </summary>
        public string FactoryName { get; set; }
        /// <summary>
        /// 生产商Logo
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string Logo { get; set; }
        /// <summary>
        /// 上级企业名称
        /// </summary>
        public string ParentOrgName { get; set; }
        /// <summary>
        /// 代理级别名称
        /// </summary>
        public string GradeName { get; set; }
        /// <summary>
        /// 代理区域名称（多个,号分隔）
        /// </summary>
        public string RegionsName { get; set; }
        /// <summary>
        /// 授权证书打印模板Id
        /// </summary>
        public string CertTemplateId { get; set; }
    }
}
