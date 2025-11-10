using ChannelUtility.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChannelUtility.Tsl
{
    public class BaseFunc : BaseAll
    {
        /// <summary>
        /// 前缀标识符
        /// </summary>
        public string prefixcode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 显示方式：null表示全部显示，空为都不显示，org为来源组织显示，own拥有者组织显示，use使用者组织显示，person个人使用者显示，多个用逗号分隔
        /// </summary>
        public string showway { get; set; }
        /// <summary>
        /// 启用条件
        /// </summary>
        public EnableCondition[] conditions { get; set; }
        /// <summary>
        /// 条件组合:(A & B) | C
        /// </summary>
        public string GroupTxt { get; set; }
        /// <summary>
        /// 屏蔽方式：0为隐藏，1为禁用
        /// </summary>
        public int actionway { get; set; }
        /// <summary>
        /// 下发方式：0为数据解释下发，1为modbus下发，2为modbus读属性下发（不等待），3为直接读属性下发（等待），4为图形编程
        /// </summary>
        public int downway { get; set; }
        /// <summary>
        /// 下发方式为1时设置是否等待返回
        /// </summary>
        public bool waitreturn { get; set; }
        /// <summary>
        /// 下发的数据，modbus时为json格式
        /// </summary>
        public string downdata { get; set; }
        /// <summary>
        /// 输入参数
        /// </summary>
        public List<BaseInputValue> inputs { get; set; }
        public IDictionary<string, object> CreateInputs()
        {
            IDictionary<string, object> dict = new Dictionary<string, object>();
            if (inputs != null)
            {
                foreach (var input in inputs)
                {
                    if (input.defval != null)
                    {
                        dict.Add(input.code, input.defval);
                    }
                }
            }
            return dict;
        }
        private ModbusMatch _mm;
        public ModbusMatch GetModbusMatch()
        {
            if (_mm != null)
            {
                return _mm;
            }
            try
            {
                _mm = System.Text.Json.JsonSerializer.Deserialize<ModbusMatch>(this.downdata);
            }
            catch { }
            if (_mm == null)
            {
                throw new Exception($"功能{name}的modbus格式错误");
            }
            return _mm;
        }
        private byte[] ObjToBytes(ModbusMatchItem mi, object input)
        {
            var iptset = inputs.Where(x => x.code == mi.PropertyCode).FirstOrDefault();
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
        public byte[] GetModbusBytes(string mode, IDictionary<string, object> inputData)
        {
            var mm = GetModbusMatch();
            FastWriter writeBytes = new FastWriter();
            writeBytes.WriteByte(mm.SlaveId);
            writeBytes.WriteByte(mm.FuncCode);
            writeBytes.WriteUInt16BE(mm.StartAddress);
            if (mm.Items.Count == 0 || this.inputs == null || this.inputs.Count == 0)
            {
                throw new Exception($"功能{name}缺少输入数据");
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
                            throw new Exception($"功能{name}缺少输入参数{item.PropertyCode}");
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
                            throw new Exception($"功能{name}缺少输入参数{item.PropertyCode}");
                        }
                        if (rsobj == null)
                        {
                            throw new Exception($"功能{name}的参数{item.PropertyCode}不能为null");
                        }
                        writeBytes.WriteBytes(ObjToBytes(item, rsobj));
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
                                        throw new Exception($"功能{name}缺少输入参数{mm.Items[idx].PropertyCode}");
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
                                throw new Exception($"功能{name}缺少输入参数{mitem.PropertyCode}");
                            }
                            if (rsobj == null)
                            {
                                throw new Exception($"功能{name}的参数{mitem.PropertyCode}不能为null");
                            }
                            writeBytes.WriteBytes(ObjToBytes(mitem, rsobj));
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
    }
    public class EnableCondition
    {
        /// <summary>
        ///  属性代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 条件值类型:Long、Double、String、Bool、Date
        /// </summary>
        public string valtype { get; set; }
        /// <summary>
        /// 比较
        /// </summary>
        public string compare { get; set; }
        /// <summary>
        /// 条件值
        /// </summary>
        public string val { get; set; }
        public bool Check(IDictionary<string, object> data)
        {
            object compare1;
            if (!data.TryGetValue(this.code, out compare1))
            {
                return false;
            }
            bool curcondrs;
            if (this.valtype == "Double")
            {
                double cm1 = Convert.ToDouble(compare1);
                double cm2 = Convert.ToDouble(this.val);

                switch (this.compare)
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
            else if (this.valtype == "Long" || this.valtype == "Date")
            {
                long cm1 = Convert.ToInt64(compare1);
                long cm2 = Convert.ToInt64(this.val);


                switch (this.compare)
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
                if (this.compare == "=")
                {
                    string compareval = this.val;
                    curcondrs = Convert.ToString(compare1) == compareval;
                }
                else if (this.compare == "!=")
                {
                    string compareval = this.val;
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
