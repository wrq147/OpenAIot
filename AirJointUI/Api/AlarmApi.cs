using AirJointUI.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirJointUI.Utils;

namespace AirJointUI.Api
{
    public static class AlarmApi
    {
        public static async Task<int> GetAlarmCount()
        {
            try
            {
                string rs = await HttpHelper.Instance.GetAsync(Constants.ContactUrl("/IoTService/HttpSync/WarnCount"));
                var rsobj = JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultInt32);
                return rsobj.data;
            }
            catch
            {
                return 0;
            }
        }
        public static async Task<List<AlarmItem>> GetAlarmList(int page)
        {
            Dictionary<string, string> reqparams = new Dictionary<string, string>();
            reqparams.Add("showAll", "false");
            reqparams.Add("pageSize", "50");
            reqparams.Add("pageNum", page.ToString());
            string rs = await HttpHelper.Instance.GetAsync(Constants.ContactUrl("/IoTService/HttpSync/WarnListPage"), reqparams);
            var rsobj = JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultListObjectAlarmItem);
            if (rsobj != null && rsobj.code == 0)
            {
                return rsobj.data.List;
            }
            else
            {
                return new List<AlarmItem>();
            }
        }
        public static async Task<ApiResult<int>> ReadAlarm(string alarmId)
        {
            try
            {
                Dictionary<string, object> reqparams = new Dictionary<string, object>();
                reqparams.Add("id", Convert.ToInt64(alarmId));
                reqparams.Add("remark", string.Empty);

                string jsonstr = JsonSerializer.Serialize(reqparams, GenericJsonContext.Default.DictionaryStringObject);
                string rs = await HttpHelper.Instance.PostJsonAsync(Constants.ContactUrl("/IoTService/HttpSync/ClearWarn"), jsonstr, System.Text.Encoding.UTF8);
                return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultInt32);
            }
            catch (Exception ex)
            {
                return new ApiResult<int>() { code = 999, message = ex.Message };
            }
        }
        public static async Task<ApiResult<int>> ReadAllAlarm()
        {
            try
            {
                Dictionary<string, string> reqparams = new Dictionary<string, string>();
                reqparams.Add("remark", string.Empty);
                string jsonstr = JsonSerializer.Serialize(reqparams, GenericJsonContext.Default.DictionaryStringString);
                string rs = await HttpHelper.Instance.PostJsonAsync(Constants.ContactUrl("/IoTService/HttpSync/ClearAllWarn"), jsonstr, System.Text.Encoding.UTF8);
                return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultInt32);
            }
            catch (Exception ex)
            {
                return new ApiResult<int>() { code = 999, message = ex.Message };
            }

        }
    }
}
