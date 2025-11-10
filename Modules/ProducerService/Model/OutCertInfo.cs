using Common.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.Model
{
    /// <summary>
    /// 代理证书数据源
    /// </summary>
    public class OutCertInfo: Out_Agent
    {

        /// <summary>
        /// 生产商Logo
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string Logo { get; set; }
        /// <summary>
        /// 生产商省市区地址
        /// </summary>
        public string AddressName { get; set; }
        /// <summary>
        /// 生产商详细地址
        /// </summary>
        public string AddressDetail { get; set; }
        /// <summary>
        /// 生产商简介
        /// </summary>
        public string Intro { get; set; }
        /// <summary>
        /// 返回示例数据
        /// </summary>
        /// <returns></returns>
        public static OutCertInfo DemoData()
        {
            OutCertInfo info = new OutCertInfo();
            info.OrgName = "示例代理商";
            info.ParentOrgName = "示例上级";
            info.GradeName = "一级代理";
            info.FactoryName = "示例生产商";
            info.RegionsName = "福建省";
            info.Logo = string.Empty;
            info.AddressName = "福建省泉州市";
            info.AddressDetail = "台商区111街";
            info.Intro = "公司以“专注网站，用心服务”为核心价值，一切以用户需求为中心，希望通过专业水平和不懈努力，重塑企业网络形象，为企业产品推广文化发展提供服务指导";
            return info;
        }
    }
}
