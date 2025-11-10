using AirJointUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Api
{
    public static class CheckApi
    {
        /// <summary>
        /// 检查Api接口是否初始化完成
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckApiReady()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(300);
                    var result = await httpClient.GetAsync(Constants.ContactUrl("/AuthService/Test/IsOk"));
                    return result.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

        }
    }
}
