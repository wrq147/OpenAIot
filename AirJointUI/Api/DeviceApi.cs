using AirJointUI.Models;
using AirJointUI.Utils;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System;

namespace AirJointUI.Api
{
    public static class DeviceApi
    {
        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        public static async Task<List<DeviceItem>> GetDeviceList(string groupId = null)
        {
            Dictionary<string, string> reqparams = new Dictionary<string, string>();
            reqparams.Add("showAll", "true");
            reqparams.Add("pageSize", "0");
            if (!string.IsNullOrEmpty(groupId))
            {
                reqparams.Add("GroupId", groupId);
            }
            try
            {
                string rs = await HttpHelper.Instance.GetAsync(Constants.ContactUrl("/IoTService/HttpSync/ListPage"), reqparams);
                var rsobj = JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultListObjectDeviceItem);
                if (rsobj != null && rsobj.code == 0)
                {
                    return rsobj.data.List;
                }
                else
                {
                    return new List<DeviceItem>();
                }
            }
            catch
            {
                return new List<DeviceItem>();
            }

        }
        /// <summary>
        /// 获取协议名称列表
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ProductItem>> GetProductNames()
        {
            Dictionary<string, string> reqparams = new Dictionary<string, string>();
            reqparams.Add("showAll", "true");
            reqparams.Add("pageSize", "0");
            string rs = await HttpHelper.Instance.GetAsync(Constants.ContactUrl("/IoTService/HttpSync/ProductNames"), reqparams);
            var rsobj = JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultListObjectProductItem);
            if (rsobj != null && rsobj.code == 0)
            {
                return rsobj.data.List;
            }
            else
            {
                return new List<ProductItem>();
            }
        }
        /// <summary>
        /// 保存设备协议
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<ApiResult<string>> SaveDeviceProto(DeviceProto_In data)
        {
            try
            {
                string jsonstr = JsonSerializer.Serialize(data, GenericJsonContext.Default.DeviceProto_In);
                string rs = await HttpHelper.Instance.PostJsonAsync(Constants.ContactUrl("/IoTService/HttpSync/EditDevice"), jsonstr, System.Text.Encoding.UTF8);
                return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultString);
            }
            catch(Exception ex)
            {
                var err = new ApiResult<string>();
                err.code = 999;
                err.message = ex.Message;
                return err;
            }

        }
        /// <summary>
        /// 获取指定设备的标签信息
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static async Task<ApiResult<List<TagItem>>> GetTagList(string number)
        {
            string rs = await HttpHelper.Instance.GetAsync(Constants.ContactUrl($"/IoTService/HttpSync/TagList?number={number}"), System.Text.Encoding.UTF8);
            return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultListTagItem);
        }
        /// <summary>
        /// 获取指定设备的实时属性
        /// </summary>
        /// <param name="dtuId"></param>
        /// <returns></returns>
        public static async Task<ApiResult<List<DevicePropItem>>> GetPropList(string dtuId)
        {
            string rs = await HttpHelper.Instance.GetAsync(Constants.ContactUrl($"/IoTRulesService/HttpRule/Live?id={dtuId}"), System.Text.Encoding.UTF8);
            return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultListDevicePropItem);
        }
        /// <summary>
        /// 修改设备标签
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<ApiResult<string>> SaveTags(TagSave_In data)
        {
            string jsonstr = JsonSerializer.Serialize(data, GenericJsonContext.Default.TagSave_In);
            string rs = await HttpHelper.Instance.PostJsonAsync(Constants.ContactUrl("/IoTService/HttpSync/SaveTags"), jsonstr, System.Text.Encoding.UTF8);
            return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultString);
        }
        /// <summary>
        /// 执行功能
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<ApiResult<IDictionary<string, object>>> ExeFunc(FunExe_In data)
        {
            string jsonstr = JsonSerializer.Serialize(data, GenericJsonContext.Default.FunExe_In);
            string rs = await HttpHelper.Instance.PostJsonAsync(Constants.ContactUrl("/IoTService/HttpSync/ExeFunc"), jsonstr, System.Text.Encoding.UTF8);
            return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultIDictionaryStringObject);
        }
        /// <summary>
        /// 刷新所属所有产品的缓存
        /// </summary>
        /// <returns></returns>
        public static async Task<ApiResult<string>> RefreshAllProductCache()
        {
            string rs = await HttpHelper.Instance.GetAsync(Constants.ContactUrl($"/IoTService/HttpSync/RefreshAllProductCache"), System.Text.Encoding.UTF8);
            return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultString);
        }
    }
}
