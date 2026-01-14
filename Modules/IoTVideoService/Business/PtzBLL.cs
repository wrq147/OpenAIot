using ChannelUtility;
using ChannelUtility.Message;
using Common.EventBus;
using Common.Share;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using Microsoft.Extensions.Options;
using NATS.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
