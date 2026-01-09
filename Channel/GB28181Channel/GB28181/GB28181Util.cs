using ChannelUtility.Message;
using GB28181Channel.GB28181.DTO;
using GB28181Channel.GB28181.Enum;
using Org.BouncyCastle.Tls;
using SIPSorcery.Net;
using SIPSorcery.SIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

        /// <summary>
        /// 构造GB28181标准SDP
        /// </summary>
        public static string BuildGB28181SDP(string serverId, string serverIp, int rtpPort)
        {

            var sdp = new SDP
            {
                Username = serverId,
                SessionId = "0",
                AnnouncementVersion = 0,
                NetworkType = "IN",
                AddressType = "IP4",
                AddressOrHost = serverIp,
                SessionName = "Play",
                Connection = new SDPConnectionInformation(IPAddress.Parse(serverIp)),
                Media = new List<SDPMediaAnnouncement>()
            };
            sdp.AddExtra($"y={GB28181Util.GetPlaySsrc(serverId)}");
            var videoFormats = new List<SDPAudioVideoMediaFormat>();
            videoFormats.Add(new SDPAudioVideoMediaFormat(SDPMediaTypesEnum.data, 96, "PS/90000"));
            videoFormats.Add(new SDPAudioVideoMediaFormat(SDPMediaTypesEnum.data, 97, "MPEG4/90000"));
            videoFormats.Add(new SDPAudioVideoMediaFormat(SDPMediaTypesEnum.data, 98, "H264/90000"));
            var videoMedia = new SDPMediaAnnouncement(SDPMediaTypesEnum.video, rtpPort, videoFormats);
            videoMedia.MediaStreamStatus = MediaStreamStatusEnum.RecvOnly;
            sdp.Media.Add(videoMedia);
            return sdp.ToString();
        }

        private static readonly List<string> _usedNumbers = new List<string>();
        private static readonly Queue<string> _unusedNumbers = new Queue<string>();

        // 线程锁保证多线程安全
        private static readonly object _lockObj = new object();
        // 序号最大值（1-9999）
        private const int MaxSerialNumber = 9999;

        /// <summary>
        /// 获取视频预览的SSRC值（第一位固定为0）
        /// </summary>
        /// <param name="serverId">服务器ID（唯一值，用于生成前缀）</param>
        /// <returns>完整的SSRC字符串</returns>
        public static string GetPlaySsrc(string serverId)
        {
            if (string.IsNullOrEmpty(serverId))
            {
                throw new ArgumentNullException(nameof(serverId), "serverId不能为空");
            }
            return "0" + GetSsrcPrefix(serverId) + GetSerialNumber();
        }

        /// <summary>
        /// 释放已使用的SSRC，将序号归还到未使用池
        /// </summary>
        /// <param name="ssrc">需要释放的SSRC字符串</param>
        public static void ReleaseSsrc(string ssrc)
        {
            if (string.IsNullOrEmpty(ssrc) || ssrc.Length < 7)
            {
                throw new ArgumentException("无效的SSRC格式", nameof(ssrc));
            }

            // 截取后4位序号（SSRC格式：[0/1] + 5位前缀 + 4位序号）
            string serialNumber = ssrc.Substring(6);

            lock (_lockObj)
            {
                // 从已使用池移除，归还到未使用池
                if (_usedNumbers.Remove(serialNumber))
                {
                    _unusedNumbers.Enqueue(serialNumber);
                }
            }
        }

        /// <summary>
        /// 懒加载获取SSRC前缀（仅初始化一次）
        /// </summary>
        /// <param name="serverId">服务器ID（唯一值）</param>
        /// <returns>5位的SSRC前缀字符串</returns>
        private static string GetSsrcPrefix(string serverId)
        {
            return serverId.Substring(3, 5);
        }

        /// <summary>
        /// 获取4位序号（按需生成，优先复用归还的序号）
        /// </summary>
        /// <returns>4位补零的序号字符串</returns>
        /// <exception cref="InvalidOperationException">序号池耗尽时抛出</exception>
        private static string GetSerialNumber()
        {
            lock (_lockObj)
            {
                // 优先使用归还的序号（复用）
                if (_unusedNumbers.Count > 0)
                {
                    string sn = _unusedNumbers.Dequeue();
                    _usedNumbers.Add(sn);
                    return sn;
                }

                // 无归还序号时，生成新序号（递增生成，不一次性初始化）
                int nextNumber = _usedNumbers.Count + 1;
                if (nextNumber > MaxSerialNumber)
                {
                    throw new InvalidOperationException($"SSRC序号池已耗尽（最大支持{MaxSerialNumber}个）");
                }

                // 生成4位补零的新序号
                string newSn = nextNumber.ToString().PadLeft(4, '0');
                _usedNumbers.Add(newSn);
                return newSn;
            }
        }

        // 【可选】重置序号池（用于特殊场景，如重启序号分配）
        public static void ResetPool()
        {
            lock (_lockObj)
            {
                _usedNumbers.Clear();
                _unusedNumbers.Clear();
            }
        }

    }
}
