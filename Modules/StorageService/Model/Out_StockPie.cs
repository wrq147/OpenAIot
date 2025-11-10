using Common.Attr;
using FluentMigrator.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class Out_StockPie : MZ_StockPile
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 规格编码
        /// </summary>
        public string SkuNumber { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string StoreName { get; set; }
    }
}
