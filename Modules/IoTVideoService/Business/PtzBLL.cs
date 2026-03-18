using ChannelUtility.Message;
using Common.EventBus;
using Common.Share;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using Microsoft.Extensions.Options;
using TemplateAction.Core;

namespace IoTVideoService.Business
{
    public class PtzBLL
    {
        private ITAServiceProvider _provider;
        public PtzBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public async Task<BusResponse<string>> ControlPTZ(In_PtzControlParam data)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            var videoSource = (await videoSourceDAL.SelectList(x => x.Id == data.SourceId && x.VideoType == 1)).FirstOrDefault();
            if (videoSource == null || string.IsNullOrEmpty(videoSource.NodeId))
            {
                return BusResponse<string>.Error(111, "视频源未注册");
            }

            MediaPTZMessage msg = new MediaPTZMessage();
            msg.DeviceId = videoSource.Id;
            msg.ProductId = string.Empty;
            msg.MessageId = Guid.NewGuid().ToString("N");
            msg.UserName = videoSource.UserName;
            msg.VideoKey = videoSource.VideoKey;
            if (data.Cmd < 0)
            {
                msg.CommandType = (PTZCommandType)Math.Abs(data.Cmd);
                msg.Speed = -data.Speed;
            }
            else
            {
                msg.CommandType = (PTZCommandType)data.Cmd;
                msg.Speed = data.Speed;
            }

            msg.PresetId = data.PresetId;

            var replyMsg = await _provider.GetService<NatsScope>().PublicWait<MediaPTZMessageReply>(videoSource.NodeId, msg);
            if (replyMsg == null)
            {
                return BusResponse<string>.Error(112, "控制命令无回复");
            }
            if (replyMsg.IsSuccess)
            {
                return BusResponse<string>.Success();
            }
            else
            {
                return BusResponse<string>.Error(113, replyMsg.Reason);
            }
        }
        public async Task<List<Out_VideoChannel>> GetChannelList(string sid)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            var videoSource = (await videoSourceDAL.SelectList(x => x.Id == sid && x.VideoType == 1)).FirstOrDefault();
            if (videoSource == null || string.IsNullOrEmpty(videoSource.NodeId))
            {
                return new List<Out_VideoChannel>();
            }
            var channelSourceList = await videoSourceDAL.SelectList(x => x.UserName == videoSource.UserName && x.VideoType == 2);
            List<Out_VideoChannel> rtlist = new List<Out_VideoChannel>();
            foreach (var channel in channelSourceList)
            {
                rtlist.Add(new Out_VideoChannel()
                {
                    VideoKey = channel.VideoKey,
                    ChannelName = channel.Position
                });
            }

            return rtlist;
        }
        public async Task<Dictionary<string, string>> GetPlayUrlDict(string sId, string type)
        {
            Dictionary<string, string> tdict = new Dictionary<string, string>();
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            var videoSource = (await videoSourceDAL.SelectList(x => x.Id == sId)).FirstOrDefault();
            if (videoSource.VideoType == 0)
            {
                var option = _provider.GetService<IOptions<VideoOption>>();
                if (option.Value.VideoServers.Count == 0)
                {
                    return tdict;
                }
                ServerInfo serverInfo;
                if (string.IsNullOrEmpty(videoSource.NodeId))
                {
                    int pos = Math.Abs(videoSource.Id.GetHashCode() % option.Value.VideoServers.Count);
                    serverInfo = option.Value.VideoServers[pos];
                }
                else
                {
                    serverInfo = option.Value.VideoServers.Where(x => x.NodeId == videoSource.NodeId).FirstOrDefault();
                }
                switch (type)
                {
                    case "rtmp":
                        tdict.Add(videoSource.VideoKey, $"rtmp://{serverInfo.Ip}:{serverInfo.RtmpPort}/live/{videoSource.VideoKey}");
                        break;
                    case "flv":
                        tdict.Add(videoSource.VideoKey, $"http://{serverInfo.Ip}:{serverInfo.HttpPort}/live/{videoSource.VideoKey}.live.flv");
                        break;
                    case "hls":
                        tdict.Add(videoSource.VideoKey, $"http://{serverInfo.Ip}:{serverInfo.HttpPort}/live/{videoSource.VideoKey}/hls.m3u8");
                        break;
                    default:

                        break;
                }
                return tdict;
            }
            else if (videoSource.VideoType == 1)
            {
                if (videoSource == null || string.IsNullOrEmpty(videoSource.NodeId))
                {
                    return tdict;
                }
                var option = _provider.GetService<IOptions<VideoOption>>();
                if (option.Value.GB28181Servers.Count == 0)
                {
                    return tdict;
                }
                if (string.IsNullOrEmpty(videoSource.NodeId))
                {
                    return tdict;
                }
                ServerInfo serverInfo = option.Value.GB28181Servers.Where(x => x.NodeId == videoSource.NodeId).FirstOrDefault();
                var channelSourceList = (await videoSourceDAL.SelectList(x => x.UserName == videoSource.UserName && x.VideoType == 2)).ToList();
                foreach (var channelSource in channelSourceList)
                {
                    switch (type)
                    {
                        case "rtmp":
                            tdict.Add(channelSource.VideoKey, $"rtmp://{serverInfo.Ip}:{serverInfo.RtmpPort}/live/{channelSource.VideoKey}");
                            break;
                        case "flv":
                            tdict.Add(channelSource.VideoKey, $"http://{serverInfo.Ip}:{serverInfo.HttpPort}/live/{channelSource.VideoKey}.live.flv");
                            break;
                        case "hls":
                            tdict.Add(channelSource.VideoKey, $"http://{serverInfo.Ip}:{serverInfo.HttpPort}/live/{videoSource.VideoKey}/hls.m3u8");
                            break;
                        default:
                            break;
                    }
                }

                return tdict;
            }
            else
            {
                return tdict;
            }
        }
        public async Task<List<PresetInfo>> GetPresetList(string sourceId)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            var videoSource = (await videoSourceDAL.SelectList(x => x.Id == sourceId && x.VideoType == 1)).FirstOrDefault();
            if (videoSource == null || string.IsNullOrEmpty(videoSource.NodeId))
            {
                return new List<PresetInfo>();
            }

            MediaPresetMessage msg = new MediaPresetMessage();
            msg.DeviceId = videoSource.Id;
            msg.ProductId = string.Empty;
            msg.MessageId = Guid.NewGuid().ToString("N");
            msg.UserName = videoSource.UserName;

            var replyMsg = await _provider.GetService<NatsScope>().PublicWait<MediaPresetMessageReply>(videoSource.NodeId, msg);
            if (replyMsg == null)
            {
                return new List<PresetInfo>();
            }
            return replyMsg.Presets;
        }


    }
}
