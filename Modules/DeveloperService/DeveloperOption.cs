using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperService
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public class DeveloperOption
    {
        /// <summary>
        /// AccessToken存活时间（单位分钟）
        /// </summary>
        public int token_life_time { get; set; }
        /// <summary>
        /// 公钥
        /// </summary>
        public string public_key { get; set; }
        /// <summary>
        /// 私钥
        /// </summary>
        public string private_key { get; set; }
    }
}
