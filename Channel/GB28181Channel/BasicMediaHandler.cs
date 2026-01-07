using GB28181Channel.GB28181.DTO;
using GB28181Channel.GB28181.Interface;
using SIPSorcery.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel
{
    public class BasicMediaHandler : IMediaHandler
    {
        private readonly Dictionary<string, int> _rtpSessions = new Dictionary<string, int>();

        public string StartRtpReceiver(PlaybackParams @params)
        {
            var sessionId = Guid.NewGuid().ToString();

            lock (_rtpSessions)
            {
                _rtpSessions.Add(sessionId, @params.LocalRtpPort);
            }

            Console.WriteLine($"[RTP接收] 启动Session {sessionId}，端口：{@params.LocalRtpPort}");
            return sessionId;
        }

        public bool StopRtpReceiver(string sessionId)
        {
            lock (_rtpSessions)
            {
                if (_rtpSessions.ContainsKey(sessionId))
                {
                    _rtpSessions.Remove(sessionId);
                    Console.WriteLine($"[RTP接收] 停止Session {sessionId}");
                    return true;
                }
            }

            return false;
        }

        public string GenerateSDP(PlaybackParams @params)
        {
            var sdp = new SDP
            {
                Username = "gb28181",
                SessionId = "0",
                AnnouncementVersion = 0,
                NetworkType = "IN",
                AddressType = "IP4",
                AddressOrHost = @params.RemoteIp,
                SessionName = "GB28181 Stream",
                Connection = new SDPConnectionInformation(IPAddress.Parse(@params.RemoteIp)),
                Media = new List<SDPMediaAnnouncement>()
            };
            var medialist = new List<SDPAudioVideoMediaFormat>();
            medialist.Add(new SDPAudioVideoMediaFormat(SDPMediaTypesEnum.video, 96, "PS/90000"));
            medialist.Add(new SDPAudioVideoMediaFormat(SDPMediaTypesEnum.video, 97, "MPEG4/90000"));
            medialist.Add(new SDPAudioVideoMediaFormat(SDPMediaTypesEnum.video, 98, "H264/90000"));
            var videoMedia = new SDPMediaAnnouncement(SDPMediaTypesEnum.video, @params.LocalRtpPort, medialist);
            sdp.Media.Add(videoMedia);
            return sdp.ToString();
        }
    }
}
