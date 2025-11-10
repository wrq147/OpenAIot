using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSService
{
    public interface ISmsHelper
    {
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="templateCode">模板编码</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        Task<bool> SendSMSCode(string phone, string templateCode, JObject parameters);
    }
}
