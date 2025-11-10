using ChannelUtility.Buffers;
using ChannelUtility.GraphScript;
using ChannelUtility.Js;
using ChannelUtility.Message;
using ChannelUtility.Tsl;
using EasyNetQ;
using Jint.Runtime;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChannelUtility
{
    public static class ClientBusExtends
    {
        public static async Task<List<ModbusMatch>> rawDataTo(this ClientBusProxy bus, string productId, string deviceId, byte[] payload, string codeprefix = "", bool sendconn = true)
        {
            List<ModbusMatch> newmmlist = null;
            //获取物模型，防止在线设备未添加
            var ret = await bus.GetTsl(productId, deviceId, sendconn);
            if (ret == null)
            {
                await bus.Print(deviceId, "设备上报消息", "尝试初始化失败，请绑定设备编码后重启您的设备");
                return newmmlist;
            }
            if (ret.Status == "0")
            {
                if (payload.Length > 4096)
                {
                    await bus.Print(deviceId, "设备上报消息", "因数据超过4096字节，无法在控制台显示");
                }
                else
                {
                    await bus.Print(deviceId, "设备上报消息", FastBufferHelper.ByteToHexStr(payload));
                }
            }
            if (ret.Model == null)
            {
                await bus.Print(deviceId, "设备上报消息", "物模型不存在");
                return newmmlist;
            }
            var tsl = ret.Model;
            productId = ret.ProductId;
            bool isCute = false;
            FastReader lastReader = bus.BytesToReader(payload, deviceId);
            while (!lastReader.EndOfBuffer)
            {
                Dictionary<string, object> propsDict = null;
                if (lastReader.Length > 5 && tsl.modbus != null)
                {
                    int mmIdx = -1;
                    bool needcrc = true;
                    bool iscc = true;
                    if (tsl.modbus.Mode == "TCP")
                    {
                        needcrc = false;
                        mmIdx = lastReader.ReadUInt16BE();
                        lastReader.ReadUInt16BE();
                        int mmlen = lastReader.ReadUInt16BE();
                        if (lastReader.Length < (mmlen + 4))
                        {
                            lastReader.MergeRead();
                            iscc = false;
                        }
                    }

                    if (iscc == true)
                    {
                        byte slaveAddress = lastReader.ReadByte();
                        byte funcByte = lastReader.ReadByte();
                        if (funcByte == 5 || funcByte == 6 || funcByte == 15 || funcByte == 16)
                        {
                            #region 解释modbus写回复
                            if (lastReader.Length >= 8)
                            {
                                ushort startAddress = lastReader.ReadUInt16BE();
                                lastReader.ReadBytes(2);
                                bool crcrs = true;
                                if (needcrc)
                                {
                                    ushort u = lastReader.ReadUInt16LE();
                                    ushort crc;
                                    if (tsl.modbus.Mode == "RTU")
                                    {
                                        crc = FastBufferHelper.CalcCRC16(lastReader.ToArray(), 0, 6);
                                    }
                                    else
                                    {
                                        crc = FastBufferHelper.CalcLRC(lastReader.ToArray(), 0, 6);
                                    }
                                    if (u != crc)
                                    {
                                        crcrs = false;
                                    }
                                }
                                if (crcrs)
                                {
                                    string callkey = "Func#" + slaveAddress + "#" + funcByte + "#" + startAddress + "#" + codeprefix;
                                    await bus.PushReply(deviceId, "ok", callkey);
                                    if (newmmlist == null)
                                    {
                                        newmmlist = new List<ModbusMatch>();
                                    }
                                    newmmlist.Add(new ModbusMatch()
                                    {
                                        SlaveId = slaveAddress,
                                        Name = "Func",
                                        FuncCode = funcByte,
                                        StartAddress = startAddress
                                    });
                                }
                                else
                                {
                                    lastReader.Reset();
                                }
                            }
                            else
                            {
                                lastReader.MergeRead();
                            }
                            #endregion
                        }
                        else if (funcByte > 0 && funcByte < 7)
                        {
                            byte bdlen = lastReader.ReadByte();

                            //校验长度
                            if ((lastReader.Length - 5) >= bdlen)
                            {
                                var body = new FastReader(lastReader.ReadBytes(bdlen));
                                int tcount = lastReader.Position + 1;

                                bool crcrs = true;
                                if (needcrc)
                                {
                                    ushort u = lastReader.ReadUInt16LE();
                                    ushort crc;
                                    if (tsl.modbus.Mode == "RTU")
                                    {
                                        crc = FastBufferHelper.CalcCRC16(lastReader.ToArray(), 0, tcount);
                                    }
                                    else
                                    {
                                        crc = FastBufferHelper.CalcLRC(lastReader.ToArray(), 0, tcount);
                                    }
                                    //校验码验证
                                    if (u != crc)
                                    {
                                        crcrs = false;
                                    }
                                }

                                if (crcrs)
                                {
                                    propsDict = new Dictionary<string, object>();

                                    #region 开始解释modbus读回复
                                    if (mmIdx >= 0 && mmIdx < tsl.modbus.Matches.Count)
                                    {
                                        if (newmmlist == null)
                                        {
                                            newmmlist = new List<ModbusMatch>();
                                        }
                                        newmmlist.Add(tsl.modbus.Matches[mmIdx]);
                                    }
                                    else
                                    {
                                        newmmlist = tsl.modbus.Matches.Where(x => x.SlaveId == slaveAddress && x.FuncCode == funcByte && x.GetByteLength() == bdlen).ToList();
                                    }

                                    if (newmmlist == null || newmmlist.Count == 0)
                                    {
                                        lastReader.Reset();
                                        await bus.Print(deviceId, "设备上报消息", $"modbus无匹配规则:{FastBufferHelper.ByteToHexStr(lastReader.ReadToEnd())}");
                                        return newmmlist;
                                    }

                                    foreach (var matchItem in newmmlist)
                                    {
                                        body.Reset();
                                        foreach (var prop in matchItem.Items)
                                        {
                                            if (prop.NumRegister == "b")
                                            {
                                                int tmpbit = body.ReadBit();
                                                if (!string.IsNullOrEmpty(prop.PropertyCode))
                                                {
                                                    if (propsDict.ContainsKey(prop.PropertyCode))
                                                    {
                                                        propsDict[prop.PropertyCode] = tmpbit;
                                                    }
                                                    else
                                                    {
                                                        propsDict.Add(prop.PropertyCode, tmpbit);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (prop.ByteOrder == "C")
                                                {
                                                    byte[] tmpb = body.ReadBytes(prop.GetRegisterLen());
                                                    if (!string.IsNullOrEmpty(prop.PropertyCode))
                                                    {
                                                        if (propsDict.ContainsKey(prop.PropertyCode))
                                                        {
                                                            propsDict[prop.PropertyCode] = ASCIIEncoding.ASCII.GetString(tmpb);
                                                        }
                                                        else
                                                        {
                                                            propsDict.Add(prop.PropertyCode, ASCIIEncoding.ASCII.GetString(tmpb));
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    int datalen = prop.GetRegisterLen();
                                                    if (datalen == 1)
                                                    {
                                                        byte tmpb = body.ReadByte();
                                                        if (!string.IsNullOrEmpty(prop.PropertyCode))
                                                        {
                                                            if (propsDict.ContainsKey(prop.PropertyCode))
                                                            {
                                                                propsDict[prop.PropertyCode] = tmpb;
                                                            }
                                                            else
                                                            {
                                                                propsDict.Add(prop.PropertyCode, tmpb);
                                                            }

                                                        }
                                                    }
                                                    else if (datalen == 2)
                                                    {
                                                        short tmps;
                                                        if (prop.ByteOrder == "H")
                                                        {
                                                            tmps = body.ReadInt16BE();
                                                        }
                                                        else
                                                        {
                                                            tmps = body.ReadInt16LE();
                                                        }
                                                        if (!string.IsNullOrEmpty(prop.PropertyCode))
                                                        {
                                                            if (propsDict.ContainsKey(prop.PropertyCode))
                                                            {
                                                                propsDict[prop.PropertyCode] = "#" + tmps;
                                                            }
                                                            else
                                                            {
                                                                propsDict.Add(prop.PropertyCode, "#" + tmps);
                                                            }

                                                        }
                                                    }
                                                    else if (datalen == 4)
                                                    {
                                                        int tmpi;
                                                        switch (prop.ByteOrder)
                                                        {
                                                            case "L":
                                                                tmpi = body.ReadInt32LE();
                                                                break;
                                                            case "CDAB":
                                                                tmpi = body.ReadInt32CDAB();
                                                                break;
                                                            case "BADC":
                                                                tmpi = body.ReadInt32BADC();
                                                                break;
                                                            default:
                                                                tmpi = body.ReadInt32BE();
                                                                break;
                                                        }

                                                        if (!string.IsNullOrEmpty(prop.PropertyCode))
                                                        {
                                                            if (!string.IsNullOrEmpty(codeprefix))
                                                            {
                                                                var prpitem = tsl.properties.Where(x => x.code == prop.PropertyCode).FirstOrDefault();
                                                                if (prpitem == null)
                                                                {
                                                                    continue;
                                                                }
                                                                if (!string.IsNullOrEmpty(prpitem.prefixcode) && prpitem.prefixcode != codeprefix)
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                            if (propsDict.ContainsKey(prop.PropertyCode))
                                                            {
                                                                propsDict[prop.PropertyCode] = tmpi;
                                                            }
                                                            else
                                                            {
                                                                propsDict.Add(prop.PropertyCode, tmpi);
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                         

                                        }
                                    }

                                    #endregion


                                }
                                else
                                {
                                    lastReader.Reset();
                                }
                            }
                            else
                            {
                                lastReader.MergeRead();
                            }
                        }
                        else
                        {
                            lastReader.Reset();
                        }
                    }

                }

                //自定义解释
                await bus.PushCustom(productId, deviceId, propsDict, lastReader, ret.script, tsl, codeprefix);
                if (lastReader.Position > -1)
                {
                    //有剩余数据包，则下个循环处理
                    if (!lastReader.EndOfBuffer)
                    {
                        lastReader = lastReader.CopyTo(lastReader.Position + 1);
                        isCute = true;
                    }
                }
                else
                {
                    if (lastReader.IsMergeRead || isCute)
                    {
                        //保存下次使用
                        bus.SaveFastReader(deviceId, lastReader);
                    }
                    return newmmlist;
                }

            }
            return newmmlist;
        }
        public static async Task<RequestMessage> toRawData(this ClientBusProxy bus, RequestMessage msg, TslReturn ret)
        {

            if (msg is ModbusMessage modbusMessage)
            {
                var tsl = ret.Model;
                if (tsl.modbus == null)
                {
                    throw new Exception("modbus未配置");
                }
                int searchIdx = 0;
                ModbusMatch matchItem = null;
                foreach (var tmpitem in tsl.modbus.Matches)
                {
                    if (tmpitem.Name == modbusMessage.MatchName)
                    {
                        matchItem = tmpitem;
                        break;
                    }
                    ++searchIdx;
                }
                if (matchItem == null)
                {
                    throw new Exception("modbus无匹配规则");
                }

                byte[] data = FastBufferHelper.ModbusMatch2Bytes(tsl.modbus.Mode, matchItem, searchIdx);
                RawDataMessage rawdata = new RawDataMessage();
                rawdata.DeviceId = msg.DeviceId;
                rawdata.MessageId = msg.MessageId;
                rawdata.ProductId = msg.ProductId;
                var items = matchItem.Items.Where(x => !string.IsNullOrEmpty(x.PropertyCode)).ToList();
                if (items.Count > 0)
                {
                    var tmpfff = tsl.properties.Where(x => x.code == items[0].PropertyCode).FirstOrDefault();
                    if (tmpfff != null)
                    {
                        rawdata.prefix = tmpfff.prefixcode;
                    }
                }

                rawdata.Data = data;
                return rawdata;
            }
            else if (msg is ReadPropertyMessage proMsg)
            {
                if (proMsg.Properties == null)
                {
                    return null;
                }
                var hs = proMsg.Properties.ToHashSet();
                List<ModbusMatch> retmmList = new List<ModbusMatch>();
                List<int> retIdx = new List<int>();
                if (ret.Model.modbus != null)
                {
                    var mm = ret.Model.modbus.Matches;
                    for (int i = 0; i < mm.Count; i++)
                    {
                        var mitem = mm[i];
                        foreach (var mmi in mitem.Items)
                        {
                            if (hs.Contains(mmi.PropertyCode))
                            {
                                retmmList.Add(mitem);
                                retIdx.Add(i);
                                break;
                            }
                        }
                    }
                }

                if (retmmList.Count == 0)
                {
                    RawDataMessage rawdata = new RawDataMessage();
                    rawdata.DeviceId = msg.DeviceId;
                    rawdata.MessageId = msg.MessageId;
                    rawdata.ProductId = msg.ProductId;
                    rawdata.Data = await bus.ParseCustom(msg, ret.Model, ret.script);
                    if (rawdata.Data == null)
                    {
                        return null;
                    }
                    return rawdata;
                }
                else
                {
                    //存在匹配的modbus指令，则下发modbus指令
                    for (int x = 0; x < retmmList.Count; x++)
                    {
                        RawDataMessage rawdata = new RawDataMessage();
                        rawdata.DeviceId = msg.DeviceId;
                        rawdata.MessageId = msg.MessageId + "_" + x;
                        rawdata.ProductId = msg.ProductId;
                        rawdata.Data = FastBufferHelper.ModbusMatch2Bytes(ret.Model.modbus.Mode, retmmList[x], retIdx[x]);
                        var firstmm = retmmList[x].Items.Where(y => !string.IsNullOrEmpty(y.PropertyCode)).FirstOrDefault();
                        if (firstmm != null)
                        {
                            var tmpfff = ret.Model.properties.Where(y => y.code == firstmm.PropertyCode).FirstOrDefault();
                            if (tmpfff != null)
                            {
                                rawdata.prefix = tmpfff.prefixcode;
                            }
                        }
                        await bus.PublicMessage(rawdata, ret);
                    }

                    return null;
                }
            }
            else if (msg is FunctionInvokeMessage funcMessage)
            {
                RawDataMessage rawdata = new RawDataMessage();
                rawdata.DeviceId = msg.DeviceId;
                rawdata.MessageId = msg.MessageId;
                rawdata.ProductId = msg.ProductId;
                if (ret.Status == "0")
                {
                    await bus.Print(msg.DeviceId, "解释下发消息", $"开始执行功能{funcMessage.FunctionId}");
                }
                var funModel = ret.Model.functions.Where(x => x.code == funcMessage.FunctionId).FirstOrDefault();
                if (funModel == null)
                {
                    throw new Exception($"物模型未配置功能：{funcMessage.FunctionId}");
                }
                rawdata.prefix = funModel.prefixcode;
                if (funModel.downway == 1)
                {
                    IDictionary<string, object> inputs = funModel.CreateInputs();
                    foreach (var kvp in funcMessage.Inputs)
                    {
                        if (inputs.ContainsKey(kvp.Key))
                        {
                            inputs[kvp.Key] = kvp.Value;
                        }
                        else
                        {
                            inputs.Add(kvp);
                        }
                    }
                    try
                    {
                        rawdata.Data = funModel.GetModbusBytes(ret.Model.modbus.Mode, inputs);
                    }
                    catch (Exception funEx)
                    {
                        await bus.Print(funcMessage.DeviceId, "modbus异常", funEx.Message);
                        await bus.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, false, null, funEx.Message, funcMessage.MessageId);
                        return null;
                    }
                    if (rawdata.Data != null && !string.IsNullOrEmpty(msg.MessageId))
                    {
                        var tmmm = funModel.GetModbusMatch();
                        string callkey = "Func#" + tmmm.SlaveId + "#" + tmmm.FuncCode + "#" + tmmm.StartAddress + "#" + funModel.prefixcode;
                        if (funModel.waitreturn == true)
                        {
                            string rs = await bus.PublicWait(funcMessage.DeviceId, callkey, async () =>
                            {
                                //未发布打印
                                if (ret.Status == "0")
                                {
                                    await bus.Print(funcMessage.DeviceId, "解释下发消息", FastBufferHelper.ByteToHexStr(rawdata.Data));
                                }

                                await bus.PublicMessage(rawdata, ret);
                            });
                        }
                        else
                        {
                            await bus.PublicMessage(rawdata, ret);
                        }

                        await bus.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, true, null, string.Empty, funcMessage.MessageId);
                        return null;
                    }
                    else
                    {
                        return rawdata;
                    }
                }
                else if (funModel.downway == 2)
                {
                    var mmidx = ret.Model.modbus.Matches.FindIndex(x => x.Name == funModel.downdata);
                    if (mmidx == -1)
                    {
                        return null;
                    }
                    var mm = ret.Model.modbus.Matches[mmidx];
                    try
                    {
                        rawdata.Data = FastBufferHelper.ModbusMatch2Bytes(ret.Model.modbus.Mode, mm, mmidx);
                    }
                    catch (Exception funEx)
                    {
                        await bus.Print(funcMessage.DeviceId, "modbus异常", funEx.Message);
                        await bus.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, false, null, funEx.Message, funcMessage.MessageId);
                        return null;
                    }
                    await bus.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, true, null, string.Empty, funcMessage.MessageId);
                    return rawdata;
                }
                else if (funModel.downway == 3)
                {
                    var props = System.Text.Json.JsonSerializer.Deserialize<List<string>>(funModel.downdata);
                    var propslist = props.ToList();
                    propslist.Sort();
                    ReadPropertyMessage newmsg = new ReadPropertyMessage();
                    newmsg.DeviceId = msg.DeviceId;
                    newmsg.ProductId = msg.ProductId;
                    newmsg.Properties = propslist;
                    newmsg.MessageId = $"Rd{msg.DeviceId}-{propslist.Count}-{UtilityTool.MD5(string.Join('#', props))}";
                    var rt = await bus.PublicWaitReadProperty(newmsg);
                    if (rt == null)
                    {
                        await bus.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, false, null, "直接读属性执行失败", funcMessage.MessageId);
                        return null;
                    }
                    else
                    {
                        await bus.ConfirmFuncReply(funcMessage.ProductId, funcMessage.DeviceId, true, rt.Properties, string.Empty, funcMessage.MessageId);
                        return null;
                    }
                }
                else if (funModel.downway == 4)
                {
                    var context = new FuncMessageContext(msg, bus, ret.Model, rawdata.prefix);
                    LiteGraphParser.Instance.Run(context, funModel.downdata);
                    return null;
                }
                else
                {
                    await bus.ParseFunc(funModel.downdata, funcMessage, ret.Model, rawdata.prefix);
                    return null;
                }
            }
            else
            {
                byte[] data = await bus.ParseCustom(msg, ret.Model, ret.script);
                if (data != null)
                {
                    RawDataMessage rawdata = new RawDataMessage();
                    rawdata.DeviceId = msg.DeviceId;
                    rawdata.MessageId = msg.MessageId;
                    rawdata.ProductId = msg.ProductId;
                    rawdata.Data = data;
                    return rawdata;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
