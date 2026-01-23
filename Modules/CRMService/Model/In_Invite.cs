using Common.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRMService.Model
{
    public class In_Invite
    {
        /// <summary>
        /// 绑定的客户
        /// </summary>
        public string BindCustomerId { get; set; }
        /// <summary>
        /// 代理级别（手动代理需要传）
        /// </summary>
        public string GradeId { get; set; }
        /// <summary>
        /// 代理的工厂（下级代理发出邀请时，需要传）
        /// </summary>
        public long? FactoryId { get; set; }
        /// <summary>
        /// 代理的区域代码，多个用,分隔
        /// </summary>
        public string Regions { get; set; }


        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }


        /// <summary>
        /// 组织名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 组织Logo
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string Logo { get; set; }
        /// <summary>
        /// 行业类型
        /// </summary>
        public int? Industry { get; set; }
        /// <summary>
        /// 员工规模
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double? Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Lat { get; set; }
        /// <summary>
        /// 省市区代码
        /// </summary>
        public string AddressCode { get; set; }
        /// <summary>
        /// 地址名称
        /// </summary>
        public string AddressName { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string AddressDetail { get; set; }
    }
}
