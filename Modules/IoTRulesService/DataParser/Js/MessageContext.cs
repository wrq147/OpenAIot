using ChannelUtility;
using ChannelUtility.Buffers;
using ChannelUtility.Message;
using ChannelUtility.Tsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoTRulesService.DataParser.Js
{
    public class MessageContext
    {
        protected RequestMessage _msg;
        protected PackParser _client;
        protected TslModel _model;
        protected string _prefix;
        public MessageContext(RequestMessage msg, PackParser client, TslModel model, string prefix)
        {
            _msg = msg;
            _client = client;
            _model = model;
            _prefix = prefix;
        }
        public RequestMessage Message()
        {
            return _msg;
        }
        /// <summary>
        /// 对象转JSON字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string ToJson(object obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj, JsonMessageSerializerConfig.SerializeOptions);
        }
        public ReadPropertyMessage ToPropertyMessage()
        {
            return _msg as ReadPropertyMessage;
        }
        public DeviceBindMessage ToBindMessage()
        {
            return _msg as DeviceBindMessage;
        }
        public FastWriter Payload()
        {
            return new FastWriter();
        }
        public void Print(object msg)
        {
            var res = _client.Print(_msg.DeviceId, "下发解释", msg);
        }
    
        public long Now()
        {
            DateTimeOffset dto = new DateTimeOffset(DateTime.Now);
            return dto.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 获取当前设备的所有属性信息
        /// </summary>
        /// <returns></returns>
        public object GetProps()
        {
            var res = _client.GetProps(_msg.DeviceId);
            return res.Result;
        }
        /// <summary>
        /// 创建绑定回复包
        /// </summary>
        /// <returns></returns>
        public DeviceBindMessageReply CreateBindReply()
        {
            var msg = new DeviceBindMessageReply();
            msg.DeviceId = _msg.DeviceId;
            msg.ProductId = _msg.ProductId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.IsSuccess = true;
            msg.Reason = string.Empty;
            return msg;
        }
        /// <summary>
        /// 创建功能回复包
        /// </summary>
        /// <returns></returns>
        public FunctionInvokeMessageReply CreateFuncReply()
        {
            FunctionInvokeMessageReply msg = new FunctionInvokeMessageReply();
            msg.DeviceId = _msg.DeviceId;
            msg.ProductId = _msg.ProductId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            return msg;
        }
        /// <summary>
        /// 创建ICCID回复包
        /// </summary>
        /// <returns></returns>
        public QueryICCIDMessageReply CreateICCIDReply()
        {
            QueryICCIDMessageReply msg = new QueryICCIDMessageReply();
            msg.DeviceId = _msg.DeviceId;
            msg.ProductId = _msg.ProductId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            return msg;
        }
        /// <summary>
        /// 创建设备事件包
        /// </summary>
        /// <returns></returns>
        public DeviceEventMessage CreateEventMessage()
        {
            DeviceEventMessage msg = new DeviceEventMessage();
            msg.DeviceId = _msg.DeviceId;
            msg.ProductId = _msg.ProductId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            return msg;
        }
        /// <summary>
        /// 创建属性回复包
        /// </summary>
        /// <returns></returns>
        public ReadPropertyMessageReply CreatePropertyReply()
        {
            ReadPropertyMessageReply msg = new ReadPropertyMessageReply();
            msg.DeviceId = _msg.DeviceId;
            msg.ProductId = _msg.ProductId;
            msg.Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            msg.RedirectFromProductId = string.Empty;
            msg.IsTagSync = false;
            return msg;
        }
        /// <summary>
        /// 创建ModbusHex数组
        /// </summary>
        /// <param name="space"></param>
        /// <returns></returns>
        public string[] CreateModbusHex(bool space = false)
        {
            if (_model.modbus == null) return new string[0];
            List<string> tlist = new List<string>();
            int i = 0;
            foreach (var mm in _model.modbus.Matches)
            {
                byte[] tmpbytes = FastBufferHelper.ModbusMatch2Bytes(_model.modbus.Mode, mm, i);
                tlist.Add(FastBufferHelper.ByteToHexStr(tmpbytes, space));
                ++i;
            }
            return tlist.Distinct().ToArray();
        }
        /// <summary>
        /// 获取Modbus信息
        /// </summary>
        /// <returns></returns>
        public ModbusInfo GetModbusInfo()
        {
            return _model.modbus;
        }
        /// <summary>
        /// 获取指定属性定义
        /// </summary>
        /// <returns></returns>
        public BaseProperty GetTslProps(string code)
        {
            return _model.properties.Where(x => x.code == code).FirstOrDefault();
        }
        /// <summary>
        /// 获取最新固件路径
        /// </summary>
        /// <returns></returns>
        public string GetFirmwareNewest(string tag)
        {
            var newfirmwares = _model.firmwares.Where(x => x.tag == tag).ToList();
            if (newfirmwares.Count > 0)
            {
                return newfirmwares[0].url;
            }
            return null;
        }
        /// <summary>
        /// 获取指定名称和标签的固件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public string GetFirmware(string name, string tag)
        {
            var newfirmwares = _model.firmwares.Where(x => x.name == name && x.tag == tag).ToList();
            if (newfirmwares.Count > 0)
            {
                return newfirmwares[0].url;
            }
            return null;
        }
        /// <summary>
        /// 等待回复消息（不发送）
        /// </summary>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public string Wait(string msgId)
        {
            var res = _client.WaitOnly(_msg.DeviceId, msgId);
            return res.Result;
        }
        /// <summary>
        /// 直接推送数据并返回复的消息
        /// </summary>
        /// <param name="msgId">消息标识</param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string PublicWait(string msgId, byte[] bytes)
        {
            var res = _client.PublicWait(_msg.DeviceId, msgId, async () =>
            {
                RawDataMessage rawdata = new RawDataMessage();
                rawdata.Data = bytes;
                rawdata.DeviceId = _msg.DeviceId;
                rawdata.MessageId = msgId;
                rawdata.ProductId = _msg.ProductId;
                rawdata.prefix = this._prefix;
                await _client.PublicMessage(rawdata, null);
            });
            return res.Result;
        }
        /// <summary>
        /// 直接推送数据
        /// </summary>
        /// <param name="bytes"></param>
        public void Public(byte[] bytes)
        {
            RawDataMessage rawdata = new RawDataMessage();
            rawdata.Data = bytes;
            rawdata.DeviceId = _msg.DeviceId;
            rawdata.MessageId = string.Empty;
            rawdata.ProductId = _msg.ProductId;
            rawdata.prefix = this._prefix;
            var res = _client.PublicMessage(rawdata, null);
            res.Wait();
        }

        /// <summary>
        /// 推送字符串数据并返回复的消息
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="input"></param>
        /// <param name="hex">是否为hex字符串</param>
        /// <returns></returns>
        public string PublicStrWait(string msgId, string input, bool hex = false)
        {
            if (hex)
            {
                var bytesource = FastBufferHelper.StrToToHex(input);
                if (msgId == null)
                {
                    var tmpfr = new FastReader(bytesource);
                    if (tmpfr.Length >= 8)
                    {
                        byte slaveAddress = tmpfr.ReadByte();
                        byte funcByte = tmpfr.ReadByte();
                        if (funcByte == 5 || funcByte == 6 || funcByte == 15 || funcByte == 16)
                        {
                            ushort startAddress = tmpfr.ReadUInt16BE();
                            msgId = "Func#" + slaveAddress + "#" + funcByte + "#" + startAddress + "#" + this._prefix;
                        }
                    }
                }

                return PublicWait(msgId, bytesource);
            }
            else
            {
                return PublicWait(msgId, Encoding.UTF8.GetBytes(input));
            }
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
        /// 发送确认回复包
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="msg"></param>
        public void ConfirmReply(string msgId, BaseUpDeviceMessage msg)
        {
            var res = _client.ConfirmReply(msgId, msg);
            res.Wait();
        }

    }
}
