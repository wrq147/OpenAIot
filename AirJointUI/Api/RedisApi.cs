using AirJointUI.Models;
using AirJointUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AirJointUI.Api
{
    public static class RedisApi
    {
        public static async Task<ApiResult<ChannelInfo>> GetOnlyChannelInfo()
        {
            Dictionary<string, string> reqparams = new Dictionary<string, string>();
            reqparams.Add("code", "modbus_only");
            string rs = await HttpHelper.Instance.GetAsync(Constants.ContactUrl("/IoTService/HttpSync/Channel"), reqparams, System.Text.Encoding.UTF8);
            return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultChannelInfo);
        }
        public static ApiResult<string> ChgOnlyChannel(ChannelInfo info)
        {
            try
            {
                ChangeChannelReq req = new ChangeChannelReq();
                req.code = "modbus_only";
                req.data = info;

                string jsonstr = JsonSerializer.Serialize(req, GenericJsonContext.Default.ChangeChannelReq);
                string rs = HttpHelper.Instance.PostJson(Constants.ContactUrl("/IoTService/HttpSync/ChgChannel"), jsonstr, System.Text.Encoding.UTF8);
                return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultString);
            }
            catch (Exception ex)
            {
                return new ApiResult<string>() { code = 322, message = ex.Message };
            }

        }
    }
}
