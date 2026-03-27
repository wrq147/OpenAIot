using System;
using System.Text;
using System.Collections.Generic;
using ChannelUtility.Buffers;
using ChannelUtility.Message;
using System.Net.Http;
using ChannelUtility.Tsl;
using System.Linq;
using ChannelUtility;

namespace IoTRulesService.DataParser.Js
{
    public class DataContext
    {
        private string _deviceId;
        private string _productId;
        private PackParser _client;
        private FastReader _readObj;
        private ReadPropertyMessageReply _reply;
        private string _codeprefix;
        private TslModel _model;
        private string _nodeGuid;
        public DataContext(ReadPropertyMessageReply reply, FastReader reader, string productId, string deviceId, PackParser client, TslModel model, string codeprefix, string nodeGuid)
        {
            _model = model;
            _reply = reply;
            _productId = productId;
            _deviceId = deviceId;
            _client = client;
            _readObj = reader;
            _codeprefix = codeprefix;
            _nodeGuid = nodeGuid;
        }
        /// <summary>
        /// 上报的扩展信息
        /// </summary>
        /// <returns></returns>
        public string CodePrefix()
        {
            return _codeprefix;
        }
        /// <summary>
        /// 返回当前的回复消息
        /// </summary>
        /// <returns></returns>
        public ReadPropertyMessageReply ReplyMessage()
        {
            return _reply;
        }

        public FastReader Payload()
        {
            return _readObj;
        }
        public string ToUtf8(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        public string ToASCII(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
        public string ToHex(byte[] bytes, bool space = false)
        {
            return FastBufferHelper.ByteToHexStr(bytes, space);
        }

        public object ToObject(string json)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json, JsonMessageSerializerConfig.ObjectOptions);
            }
            catch
            {
                return null;
            }
        }
        public float ToFloat(object input)
        {
            try
            {
                long linput = Convert.ToInt64(input);
                byte[] bytes = BitConverter.GetBytes(linput);
                return BitConverter.ToSingle(bytes);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取指定属性定义
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public BaseProperty GetTslProps(string code)
        {
            return _model.properties.Where(x => x.code == code).FirstOrDefault();
        }

        /// <summary>
        /// 获取当前设备的所有属性信息
        /// </summary>
        /// <returns></returns>
        public object GetProps()
        {
            var res = _client.GetProps(_deviceId);
            return res.Result;
        }
        /// <summary>
        /// 创建读属性回复包
        /// </summary>
        /// <returns></returns>
        public ReadPropertyMessageReply CreatePropertyMessage()
        {
            var msg = new ReadPropertyMessageReply();
            msg.DeviceId = _deviceId;
            msg.ProductId = _productId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.RedirectFromProductId = string.Empty;
            msg.IsTagSync = false;
            msg.NodeId = _nodeGuid;
            return msg;
        }
        /// <summary>
        /// 创建ICCID回复包
        /// </summary>
        /// <returns></returns>
        public QueryICCIDMessageReply CreateICCIDReply()
        {
            QueryICCIDMessageReply msg = new QueryICCIDMessageReply();
            msg.DeviceId = _deviceId;
            msg.ProductId = _productId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.NodeId = _nodeGuid;
            return msg;
        }
        /// <summary>
        /// 创建设备事件包
        /// </summary>
        /// <returns></returns>
        public DeviceEventMessage CreateEventMessage()
        {
            var msg = new DeviceEventMessage();
            msg.DeviceId = _deviceId;
            msg.ProductId = _productId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.NodeId = _nodeGuid;
            return msg;
        }
        /// <summary>
        /// 创建空回复
        /// </summary>
        /// <returns></returns>
        public EmptyMessageReply CreateEmptyMessage()
        {
            var msg = new EmptyMessageReply();
            msg.DeviceId = _deviceId;
            msg.ProductId = _productId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.NodeId = _nodeGuid;
            return msg;
        }

        public void Print(object msg)
        {
            var res = _client.Print(_deviceId, "上报解释", msg);
        }
        /// <summary>
        /// 回复消息Id
        /// </summary>
        /// <param name="msgId">可为null,为null时自动从系统缓存里取</param>
        /// <param name="value"></param>
        public void ReplyMsgId(string msgId, string value)
        {
            var res = _client.PushReply(_deviceId, value, msgId);
            res.Wait();
        }

        /// <summary>
        /// 直接推送数据
        /// </summary>
        /// <param name="bytes"></param>
        public void Public(byte[] bytes)
        {
            RawDataMessage rawdata = new RawDataMessage();
            rawdata.Data = bytes;
            rawdata.DeviceId = _deviceId;
            rawdata.MessageId = string.Empty;
            rawdata.ProductId = _productId;
            var res = _client.PublicMessage(rawdata, null);
            res.Wait();
        }
        /// <summary>
        /// 推送字符串数据
        /// </summary>
        /// <param name="input"></param>
        /// <param name="hex">是否为hex字符串</param>
        public void PublicStr(string input, bool hex = false)
        {
            if (hex)
            {
                Public(FastBufferHelper.StrToToHex(input));
            }
            else
            {
                Public(Encoding.UTF8.GetBytes(input));
            }
        }

        /// <summary>
        /// 向指定站点上传文件并返回文件的url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filename"></param>
        /// <param name="bytes"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public string UploadFile(string url, string filename, byte[] bytes, IDictionary<string, object> headers = null)
        {
            using (var client = new HttpClient())
            {
                //带参数
                if (headers != null)
                {
                    foreach (var hitem in headers)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(hitem.Key, hitem.Value.ToString());
                    }
                }

                // 以MultipartFormData格式上传
                using (var content = new MultipartFormDataContent())
                {

                    content.Add(new ByteArrayContent(bytes), "file", filename);
                    try
                    {
                        // 上传文件,获取返回的字符串内容
                        var result = client.PostAsync(url, content).Result.Content.ReadAsStringAsync();
                        return result.Result;
                    }
                    catch (Exception ex)
                    {
                        var result = _client.Print(_deviceId, "文件上传异常", ex.Message);
                        return null;
                    }
                }
            }
        }
    }
}
