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
            if (videoSource == null || string.IsNullOrEmpty(videoSource.PullNode))
            {
                return BusResponse<string>.Error(111, "视频源不存在");
            }

            MediaPTZMessage msg = new MediaPTZMessage();
            msg.DeviceId = videoSource.Id;
            msg.ProductId = string.Empty;
            msg.MessageId = Guid.NewGuid().ToString("N");
            msg.UserName = videoSource.UserName;
            msg.ChannelId = videoSource.ChannelId;
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

            var replyMsg = await _provider.GetService<NatsScope>().PublicWait<MediaPTZMessageReply>(videoSource.PullNode, msg);
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
            if (videoSource == null || string.IsNullOrEmpty(videoSource.PullNode))
            {
                return new List<Out_VideoChannel>();
            }
            List<Out_VideoChannel> rtlist = new List<Out_VideoChannel>();
            if (!string.IsNullOrEmpty(videoSource.ChannelId))
            {
                rtlist.Add(new Out_VideoChannel()
                {
                    ChannelId = videoSource.ChannelId,
                    ChannelName = videoSource.Position
                });
                return rtlist;
            }
            var channelSourceList = await videoSourceDAL.SelectList(x => x.UserName == videoSource.UserName && x.VideoType == 2);
            foreach (var channel in channelSourceList)
            {
                rtlist.Add(new Out_VideoChannel()
                {
                    ChannelId = channel.ChannelId,
                    ChannelName = channel.Position
                });
            }
            return rtlist;
        }
        public async Task<string> GetPlayUrl(string sId, string cId)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            var videoSource = (await videoSourceDAL.SelectList(x => x.Id == sId && x.VideoType == 1)).FirstOrDefault();
            if (videoSource == null || string.IsNullOrEmpty(videoSource.PullNode))
            {
                return string.Empty;
            }

            if (videoSource.VideoType == 0)
            {
                var option = _provider.GetService<IOptions<VideoOption>>();
                if (option.Value.VideoServers.Count == 0)
                {
                    return string.Empty;
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
                return $"rtmp://{serverInfo.Ip}:{serverInfo.Port}/live/{videoSource.VideoKey}";
            }
            else if (videoSource.VideoType == 1)
            {
                var option = _provider.GetService<IOptions<VideoOption>>();
                if (option.Value.GB28181Servers.Count == 0)
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(videoSource.NodeId))
                {
                    return string.Empty;
                }
                ServerInfo serverInfo = option.Value.GB28181Servers.Where(x => x.NodeId == videoSource.NodeId).FirstOrDefault();
                if (videoSource.ChannelId == cId)
                {
                    return $"rtmp://{serverInfo.Ip}:{serverInfo.Port}/live/{videoSource.VideoKey}";
                }
                else
                {
                    var channelSource = (await videoSourceDAL.SelectList(x => x.UserName == videoSource.UserName && x.ChannelId == cId && x.VideoType == 2)).FirstOrDefault();
                    if (channelSource == null)
                    {
                        return string.Empty;
                    }
                    return $"rtmp://{serverInfo.Ip}:{serverInfo.Port}/live/{channelSource.VideoKey}";
                }
            }
            else
            {
                return string.Empty;
            }
        }
        public async Task<List<PresetInfo>> GetPresetList(string sourceId)
        {
            var videoSourceDAL = _provider.GetService<VideoSourceDAL>();
            var videoSource = (await videoSourceDAL.SelectList(x => x.Id == sourceId && x.VideoType == 1)).FirstOrDefault();
            if (videoSource == null || string.IsNullOrEmpty(videoSource.PullNode))
            {
                return new List<PresetInfo>();
            }

            MediaPresetMessage msg = new MediaPresetMessage();
            msg.DeviceId = videoSource.Id;
            msg.ProductId = string.Empty;
            msg.MessageId = Guid.NewGuid().ToString("N");
            msg.UserName = videoSource.UserName;

            var replyMsg = await _provider.GetService<NatsScope>().PublicWait<MediaPresetMessageReply>(videoSource.PullNode, msg);
            if (replyMsg == null)
            {
                return new List<PresetInfo>();
            }
            return replyMsg.Presets;
        }


    }
}
