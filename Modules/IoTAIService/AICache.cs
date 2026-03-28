using ChannelUtility.Message;
using Common;
using IoTAIService.AICode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TemplateAction.Core;

namespace IoTAIService
{
    public class AICache
    {
        private ITAServiceProvider _provider;
        public AICache(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public VideoCache GetVideoCache(string videoId)
        {
            var cache = _provider.GetService<CacheHelper>();
            return cache.GetCache<VideoCache>($"AICache:{videoId}");
        }
        public List<AIConfigData> GetVideoAIConfig(string videoId)
        {
            var cache = _provider.GetService<CacheHelper>();
            return cache.GetCache<List<AIConfigData>>($"AIConfig:{videoId}");
        }
        public void SetVideoAIConfig(string videoId, List<AIConfigData> configs)
        {
            var cache = _provider.GetService<CacheHelper>();
            cache.SetCache($"AIConfig:{videoId}", configs);
            cache.SetCache($"AICache:{videoId}", new VideoCache());
        }
        public void ClearVideo(string videoId)
        {
            var cache = _provider.GetService<CacheHelper>();
            cache.RemoveCache($"AIConfig:{videoId}");
            cache.RemoveCache($"AICache:{videoId}");
        }

        public class VideoCache
        {
            private Dictionary<string, object> _videoData = new Dictionary<string, object>();
            public int GetInt(string key, int def = 0)
            {
                if (_videoData.TryGetValue(key, out object videoNum))
                {
                    return Convert.ToInt32(videoNum);
                }
                else
                {
                    return def;
                }
            }
            public void SetInt(string key, int val)
            {
                _videoData[key] = val;
            }
            public DateTime GetDateTime(string key, DateTime def)
            {
                if (_videoData.TryGetValue(key, out object data))
                {
                    return Convert.ToDateTime(data);
                }
                else
                {
                    return def;
                }
            }
            public void SetDateTime(string key, DateTime val)
            {
                _videoData[key] = val;
            }
            public T GetItem<T>(string key) where T : class
            {
                if (_videoData.TryGetValue(key, out object videoNum))
                {
                    return videoNum as T;
                }
                else
                {
                    return null;
                }
            }
            public void SetItem<T>(string key, T val) where T : class
            {
                _videoData[key] = val;
            }
            public List<Track> TrackList { get; set; }
            public List<Track> AddTrackList { get; set; }
            private ByteTrack _track;
            public void UpdateByteTrack(List<BoxItem> boxes)
            {
                if (_track == null)
                {
                    _track = new ByteTrack(trackThresh: 0.5f, trackLowThresh: 0.1f, matchThresh: 0.8f);
                }
                (TrackList, AddTrackList) = _track.Update(boxes);
            }
        }
    }
}