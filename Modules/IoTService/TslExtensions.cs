using ChannelUtility;
using ChannelUtility.Buffers;
using ChannelUtility.Tsl;
using Jint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService
{
    /// <summary>
    /// 物模型扩展方法
    /// </summary>
    public static class TslExtensions
    {
        private static string ByteTrueTo(object input, params string[] strs)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                long linput = Convert.ToInt64(input);
                if (strs.Length == 0)
                {
                    List<long> result = new List<long>(31);
                    for (int i = 0; i < 31; i++)
                    {
                        if ((linput & (1L << i)) != 0) //检查第i位是否为1
                        {
                            result.Add(1 << i);
                        }
                    }

                    return string.Join(',', result);
                }
                else
                {
                    int jdge = 1;
                    foreach (var s in strs)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            if ((linput & jdge) != 0)
                            {
                                sb.Append(s).Append(",");
                            }
                        }
                        jdge = jdge << 1;
                    }
                    return sb.ToString().Trim(',');
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        private static string ByteAllTo(object input, params string[] strs)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                long linput = Convert.ToInt64(input);
                int jdge = 1;
                for (int i = 0; i < strs.Length; i += 2)
                {
                    if (!string.IsNullOrEmpty(strs[i]))
                    {
                        if ((linput & jdge) != 0)
                        {
                            sb.Append(strs[i]).Append(",");
                        }
                        else
                        {
                            if ((i + 1) >= strs.Length)
                            {
                                continue;
                            }
                            sb.Append(strs[i + 1]).Append(",");
                        }
                    }

                    jdge = jdge << 1;
                }
                return sb.ToString().Trim(',');
            }
            catch
            {
                return string.Empty;
            }
        }
        private static float ObjToFloat(object input)
        {
            try
            {
                var linput = Convert.ToInt32(input);
                byte[] bytes = BitConverter.GetBytes(linput);
                return BitConverter.ToSingle(bytes);
            }
            catch
            {
                return 0;
            }
        }
        private static uint ObjToUnsigned(object input)
        {
            try
            {
                if (input is short sval)
                {
                    return (ushort)sval;
                }
                else
                {
                    return (uint)input;
                }
            }
            catch
            {
                return 0;
            }
        }
        delegate string ByteTrueToDelegate(object input, params string[] strs);
        /// <summary>
        /// 原数据转换成显示数据
        /// </summary>
        /// <param name="input"></param>
        /// <param name="getProp"></param>
        /// <returns></returns>
        public static object RawTo(this BaseValueOption bv, object input, Func<string, object> getProp)
        {
            if (string.IsNullOrEmpty(bv.express))
            {
                return bv.InnerRawTo(input);
            }
            else
            {
                try
                {
                    var eng = new Engine()
                    .SetValue("toFloat", new Func<object, float>(ObjToFloat))
                    .SetValue("toUnsigned", new Func<object, uint>(ObjToUnsigned))
                    .SetValue("data", input)
                    .SetValue("byteTo", new ByteTrueToDelegate(ByteTrueTo))
                    .SetValue("byteAllTo", new ByteTrueToDelegate(ByteAllTo))
                    .SetValue("prop", getProp);
                    var res = eng.Evaluate(bv.express);
                    return bv.InnerRawTo(res.ToObject());
                }
                catch
                {
                    switch (bv.type)
                    {
                        case "geo":
                        case "enum":
                        case "string":
                            return string.Empty;
                        default:
                            return 0;
                    }
                }

            }
        }





        /// <summary>
        /// 原数据转换成显示数据
        /// </summary>
        /// <param name="input"></param>
        /// <param name="getProp"></param>
        /// <returns></returns>
        public static Dictionary<string, object> RawToProp(this TslModel model, IDictionary<string, object> input, Func<string, object> getProp)
        {
            Dictionary<string, object> newout = new Dictionary<string, object>();
            foreach (var bp in model.properties)
            {
                object val;
                if (input.TryGetValue(bp.code, out val))
                {
                    newout.Add(bp.code, bp.option.RawTo(val, getProp));
                }
            }
            return newout;
        }




        /// <summary>
        /// 获取显示用设备属性
        /// </summary>
        /// <param name="input"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        public static async Task<List<DeviceProperty>> PropertyList(this TslModel model, Dictionary<string, DevicePropertyValue> input, Func<BaseProperty, Task<bool>>? validate = null)
        {
            List<DeviceProperty> list = new List<DeviceProperty>();
            foreach (BaseProperty bp in model.properties)
            {
                DevicePropertyValue dpv;
                if (input.TryGetValue(bp.code, out dpv))
                {
                    if (validate != null)
                    {
                        if (!await validate.Invoke(bp))
                        {
                            continue;
                        }
                    }

                    DeviceProperty property = new DeviceProperty();
                    property.Code = bp.code;
                    property.Name = bp.name;
                    property.Value = dpv.val;
                    property.Unit = string.Empty;
                    property.Description = bp.description;
                    switch (bp.option.type)
                    {
                        case "int":
                            property.Unit = ((IntOption)bp.option).unit;
                            break;
                        case "float":
                            property.Unit = ((FloatOption)bp.option).unit;
                            break;
                        case "boolean":
                            {
                                bool tmpb = Convert.ToBoolean(dpv.val);
                                if (tmpb)
                                {
                                    property.Value = ((BooleanOption)bp.option).trueText;
                                }
                                else
                                {
                                    property.Value = ((BooleanOption)bp.option).falseText;
                                }
                            }
                            break;
                        case "date":
                            {
                                long tmpl = Convert.ToInt64(dpv.val);
                                if (tmpl == 0)
                                {
                                    property.Value = string.Empty;
                                }
                                else
                                {
                                    var dto = DateTimeOffset.FromUnixTimeMilliseconds(tmpl);
                                    property.Value = dto.LocalDateTime.ToString(((DateOption)bp.option).format);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    property.UpdatedOn = dpv.date;
                    property.OptionType = bp.option.type;

                    list.Add(property);
                }
            }
            return list;
        }



        public static byte[] ObjToBytes(this BaseFunc func, ModbusMatchItem mi, object input)
        {
            var iptset = func.inputs.Where(x => x.code == mi.PropertyCode).FirstOrDefault();
            int reglen = mi.GetRegisterLen();
            byte[] initBytes = new byte[reglen];
            if (iptset == null)
            {
                return initBytes;
            }
            byte[] bytes = Array.Empty<byte>();
            if (mi.ByteOrder == "C")
            {
                bytes = ASCIIEncoding.ASCII.GetBytes((input ?? "").ToString());
            }
            else
            {
                switch (iptset.type)
                {
                    case "float":
                        bytes = BitConverter.GetBytes(Convert.ToSingle(input));
                        break;
                    case "int":
                        var tmpval = Convert.ToInt32(input);
                        if (reglen == 4)
                        {
                            bytes = BitConverter.GetBytes(tmpval);
                        }
                        else if (reglen == 2)
                        {
                            if (short.MinValue <= tmpval && short.MaxValue >= tmpval)
                            {
                                bytes = BitConverter.GetBytes(Convert.ToInt16(input));
                            }
                            else
                            {
                                bytes = BitConverter.GetBytes(Convert.ToUInt16(input));
                            }
                        }
                        else if (reglen == 1)
                        {
                            bytes = new byte[1];
                            bytes[0] = Convert.ToByte(input);
                        }
                        break;
                    case "boolean":
                        bytes = BitConverter.GetBytes(Convert.ToBoolean(input));
                        break;
                    case "date":
                        bytes = BitConverter.GetBytes(new DateTimeOffset(Convert.ToDateTime(input)).ToUnixTimeMilliseconds());
                        break;
                    case "enum":
                        {
                            string valstr = (input ?? "").ToString();
                            var tmpvalkey = iptset.elements.Values.Where(x => x == valstr).FirstOrDefault();
                            if (tmpvalkey != null)
                            {
                                if (int.TryParse(tmpvalkey, out int newint))
                                {
                                    if (reglen == 4)
                                    {
                                        bytes = BitConverter.GetBytes(newint);
                                    }
                                    else if (reglen == 2)
                                    {
                                        if (short.MinValue <= newint && short.MaxValue >= newint)
                                        {
                                            bytes = BitConverter.GetBytes(Convert.ToInt16(newint));
                                        }
                                        else
                                        {
                                            bytes = BitConverter.GetBytes(Convert.ToUInt16(newint));
                                        }
                                    }
                                    else if (reglen == 1)
                                    {
                                        bytes = new byte[1];
                                        bytes[0] = Convert.ToByte(newint);
                                    }
                                }
                            }

                        }

                        break;
                }

                if (BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        if (i < initBytes.Length)
                        {
                            initBytes[i] = bytes[i];
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = bytes.Length - 1; i >= 0; i--)
                    {
                        if (i < initBytes.Length)
                        {
                            initBytes[i] = bytes[i];
                        }
                        else
                        {
                            break;
                        }
                    }


                }
            }



            switch (mi.ByteOrder)
            {
                case "H":
                    {
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(initBytes);
                    }
                    break;
                case "L":
                    {
                        if (!BitConverter.IsLittleEndian)
                            Array.Reverse(initBytes);
                    }
                    break;
            }
            if (mi.GetRegisterLen() > 3)
            {
                switch (mi.ByteOrder)
                {
                    case "CDAB":
                        {
                            if (BitConverter.IsLittleEndian)
                            {
                                byte tmpb = initBytes[0];
                                initBytes[0] = initBytes[1];
                                initBytes[1] = tmpb;
                                tmpb = initBytes[3];
                                initBytes[3] = initBytes[2];
                                initBytes[2] = tmpb;
                            }
                            else
                            {
                                byte tmpb = initBytes[0];
                                initBytes[0] = initBytes[2];
                                initBytes[2] = tmpb;
                                tmpb = initBytes[3];
                                initBytes[3] = initBytes[1];
                                initBytes[1] = tmpb;
                            }
                        }
                        break;
                    case "BADC":
                        {
                            {
                                if (BitConverter.IsLittleEndian)
                                {
                                    byte tmpb = initBytes[0];
                                    initBytes[0] = initBytes[2];
                                    initBytes[2] = tmpb;
                                    tmpb = initBytes[3];
                                    initBytes[3] = initBytes[1];
                                    initBytes[1] = tmpb;
                                }
                                else
                                {
                                    byte tmpb = initBytes[0];
                                    initBytes[0] = initBytes[1];
                                    initBytes[1] = tmpb;
                                    tmpb = initBytes[3];
                                    initBytes[3] = initBytes[2];
                                    initBytes[2] = tmpb;
                                }
                            }
                            break;
                        }
                }

            }


            return initBytes;
        }
        /// <summary>
        /// 功能转modbus字节
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="inputData"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static byte[] GetModbusBytes(this BaseFunc func, string mode, IDictionary<string, object> inputData)
        {
            var mm = func.GetModbusMatch();
            FastWriter writeBytes = new FastWriter();
            writeBytes.WriteByte(mm.SlaveId);
            writeBytes.WriteByte(mm.FuncCode);
            writeBytes.WriteUInt16BE(mm.StartAddress);
            if (mm.Items.Count == 0 || func.inputs == null || func.inputs.Count == 0)
            {
                throw new Exception($"功能{func.name}缺少输入数据");
            }

            //写入的
            switch (mm.FuncCode)
            {
                case 5:
                    {
                        var item = mm.Items[0];
                        object rsobj;
                        if (!inputData.TryGetValue(item.PropertyCode, out rsobj))
                        {
                            throw new Exception($"功能{func.name}缺少输入参数{item.PropertyCode}");
                        }
                        bool rs = Convert.ToBoolean(rsobj);
                        if (rs == true)
                        {
                            writeBytes.WriteUInt16BE(0xFF00);
                        }
                        else
                        {
                            writeBytes.WriteUInt16BE(0x0000);
                        }
                    }
                    break;
                case 6:
                    {
                        var item = mm.Items[0];
                        object rsobj;
                        if (!inputData.TryGetValue(item.PropertyCode, out rsobj))
                        {
                            throw new Exception($"功能{func.name}缺少输入参数{item.PropertyCode}");
                        }
                        if (rsobj == null)
                        {
                            throw new Exception($"功能{func.name}的参数{item.PropertyCode}不能为null");
                        }
                        writeBytes.WriteBytes(func.ObjToBytes(item, rsobj));
                    }
                    break;
                case 15:
                    {
                        //线圈数
                        writeBytes.WriteUInt16BE(Convert.ToUInt16(mm.Items.Count));
                        byte bynum = Convert.ToByte(mm.Items.Count / 8 + 1);
                        //字节数
                        writeBytes.WriteByte(bynum);
                        byte[] outbytes = new byte[bynum];
                        for (int i = 0; i < bynum; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                int idx = i * 8 + j;
                                if (idx < mm.Items.Count)
                                {
                                    object rsobj;
                                    if (!inputData.TryGetValue(mm.Items[idx].PropertyCode, out rsobj))
                                    {
                                        throw new Exception($"功能{func.name}缺少输入参数{mm.Items[idx].PropertyCode}");
                                    }
                                    bool rs = Convert.ToBoolean(rsobj);
                                    if (rs == true)
                                    {
                                        if (i == 0)
                                        {
                                            outbytes[i] = 1;
                                        }
                                        else
                                        {
                                            outbytes[i] |= Convert.ToByte(1 << j);
                                        }
                                    }

                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        writeBytes.WriteBytes(outbytes);
                    }
                    break;
                case 16:
                    {
                        int bynum = mm.GetByteLength();
                        //寄存器数
                        writeBytes.WriteUInt16BE(Convert.ToUInt16(bynum / 2));
                        //字节数
                        writeBytes.WriteByte(Convert.ToByte(bynum));
                        foreach (var mitem in mm.Items)
                        {
                            object rsobj;
                            if (!inputData.TryGetValue(mitem.PropertyCode, out rsobj))
                            {
                                throw new Exception($"功能{func.name}缺少输入参数{mitem.PropertyCode}");
                            }
                            if (rsobj == null)
                            {
                                throw new Exception($"功能{func.name}的参数{mitem.PropertyCode}不能为null");
                            }
                            writeBytes.WriteBytes(func.ObjToBytes(mitem, rsobj));
                        }
                    }
                    break;
                case 1:
                case 2:
                    writeBytes.WriteUInt16BE(Convert.ToUInt16(mm.GetBitLength()));
                    break;
                default:
                    writeBytes.WriteUInt16BE(Convert.ToUInt16(mm.GetByteLength() / 2));
                    break;
            }
            if (mode == "TCP")
            {
                FastWriter wrapBytes = new FastWriter();
                wrapBytes.WriteUInt16BE(0x1001);
                wrapBytes.WriteUInt16BE(0);
                wrapBytes.WriteUInt16BE(Convert.ToUInt16(writeBytes.Length));
                wrapBytes.WriteBytes(writeBytes);
                return wrapBytes.ToArray();
            }
            else
            {
                if (mode == "RTU")
                {
                    ushort crc = FastBufferHelper.CalcCRC16(writeBytes.ToArray(), 0, writeBytes.Length);
                    writeBytes.WriteUInt16LE(crc);
                }
                else
                {
                    ushort crc = FastBufferHelper.CalcLRC(writeBytes.ToArray(), 0, writeBytes.Length);
                    writeBytes.WriteUInt16LE(crc);
                }
                return writeBytes.ToArray();
            }
        }

        public static bool Check(this EnableCondition condition, IDictionary<string, object> data)
        {
            object compare1;
            if (!data.TryGetValue(condition.code, out compare1))
            {
                return false;
            }
            bool curcondrs;
            if (condition.valtype == "Double")
            {
                double cm1 = Convert.ToDouble(compare1);
                double cm2 = Convert.ToDouble(condition.val);

                switch (condition.compare)
                {
                    case "=":
                        curcondrs = cm1 == cm2;
                        break;
                    case "!=":
                        curcondrs = cm1 != cm2;
                        break;
                    case ">":
                        curcondrs = cm1 > cm2;
                        break;
                    case "<":
                        curcondrs = cm1 < cm2;
                        break;
                    case ">=":
                        curcondrs = cm1 >= cm2;
                        break;
                    case "<=":
                        curcondrs = cm1 <= cm2;
                        break;
                    default:
                        curcondrs = false;
                        break;
                }
            }
            else if (condition.valtype == "Long" || condition.valtype == "Date")
            {
                long cm1 = Convert.ToInt64(compare1);
                long cm2 = Convert.ToInt64(condition.val);


                switch (condition.compare)
                {
                    case "=":
                        curcondrs = cm1 == cm2;
                        break;
                    case "!=":
                        curcondrs = cm1 != cm2;
                        break;
                    case ">":
                        curcondrs = cm1 > cm2;
                        break;
                    case "<":
                        curcondrs = cm1 < cm2;
                        break;
                    case ">=":
                        curcondrs = cm1 >= cm2;
                        break;
                    case "<=":
                        curcondrs = cm1 <= cm2;
                        break;
                    default:
                        curcondrs = false;
                        break;
                }
            }
            else
            {
                if (condition.compare == "=")
                {
                    string compareval = condition.val;
                    curcondrs = Convert.ToString(compare1) == compareval;
                }
                else if (condition.compare == "!=")
                {
                    string compareval = condition.val;
                    curcondrs = Convert.ToString(compare1) != compareval;
                }
                else
                {
                    curcondrs = false;
                }
            }

            return curcondrs;
        }
    }
}
