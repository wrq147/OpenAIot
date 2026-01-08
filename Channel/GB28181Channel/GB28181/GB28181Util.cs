using GB28181Channel.GB28181.DTO;
using GB28181Channel.GB28181.Enum;
using Org.BouncyCastle.Tls;
using System;
using System.Xml.Linq;

namespace GB28181Channel.GB28181
{
    public static class GB28181Util
    {
        private static int _cseqCounter = 1;
        private static readonly object _cseqLock = new object();
        public static int GenerateCSeq()
        {
            int currentCseq;
            lock (_cseqLock)
            {
                currentCseq = _cseqCounter++;
                // 防止溢出，重置为1
                if (_cseqCounter > int.MaxValue - 1000)
                {
                    _cseqCounter = 1;
                }
            }
            return currentCseq;
        }
        /// <summary>
        /// 生成PTZ控制XML
        /// </summary>
        public static string GeneratePTZControlXml(PTZControlParams @params)
        {
            var xml = new XDocument(
                new XElement("Control",
                    new XElement("CmdType", "DeviceControl"),
                    new XElement("SN", GB28181Util.GenerateCSeq()),
                    new XElement("DeviceID", @params.ChannelId),
                    new XElement("PTZCmd", @params.CommandType.ToString()),
                    new XElement("Speed", @params.Speed),
                    new XElement("PresetID", @params.PresetId)
                )
            );

            return xml.ToString(SaveOptions.DisableFormatting);
        }
        /// <summary>
        /// 生成GB28181标准的目录查询XML
        /// </summary>
        /// <param name="deviceId">目标设备ID</param>
        /// <returns>XML字符串</returns>
        public static string GenerateCatalogQueryXml(string deviceId, GB28181Version protocolVersion)
        {
            // 适配2016/2022版本的XML格式
            var xml = new XDocument(
                new XDeclaration("1.0", "GB2312", "yes"),
                new XElement("Query",
                    new XElement("CmdType", "Catalog"),
                    new XElement("SN", GB28181Util.GenerateCSeq()),
                    new XElement("DeviceID", deviceId),
                    // 2022版本新增范围参数，兼容2016
                    protocolVersion == GB28181Version.V2022 ? new XElement("Scope", "ALL") : null
                )
            );

            return xml.ToString(SaveOptions.DisableFormatting);
        }
    }
}
