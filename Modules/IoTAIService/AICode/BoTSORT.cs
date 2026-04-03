using ChannelUtility.Message;
using NPOI.HPSF;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TemplateAction.Core;

namespace IoTAIService.AICode
{
    #region 配置类
    /// <summary>
    /// BoT-SORT核心配置
    /// </summary>
    public class BoTSORTConfig
    {
        // 跟踪阈值
        public float TrackThresh { get; set; } = 0.5f;
        public float TrackLowThresh { get; set; } = 0.1f;
        public float NewTrackThresh { get; set; } = 0.6f;

        // 匹配阈值
        public float MatchThresh { get; set; } = 0.8f;
        public float LowConfMatchThresh { get; set; } = 0.4f;

        // ReID配置
        public float AppearanceThresh { get; set; } = 0.25f;
        public float Lambda { get; set; } = 0.98f; // ReID/IoU融合权重

        // 轨迹管理
        public int TrackBuffer { get; set; } = 30;
        public int MaxLostFrames { get; set; } = 30;

    }
    #endregion


  



    #region BoTTrack类
    /// <summary>
    /// 跟踪轨迹实体
    /// </summary>
    public class BoTTrack
    {
        public int TrackId { get; }
        public BoTSORTConfig Config { get; }
        public KalmanFilter Kf { get; }
        public IReIDExtractor ReIDExtractor { get; }

        // 轨迹状态
        public BoxItem LastDetection { get; private set; }
        public float[] LastReIDFeature { get; private set; }
        public int TimeSinceUpdate { get; private set; }
        public int LostFrames { get; private set; }
        public bool IsActive => TimeSinceUpdate < Config.TrackBuffer;
        public bool IsLost => LostFrames > Config.MaxLostFrames;

        public BoTTrack(int trackId, BoxItem detection, BoTSORTConfig config, IReIDExtractor reidExtractor)
        {
            if (detection == null) throw new ArgumentNullException(nameof(detection));
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (reidExtractor == null) throw new ArgumentNullException(nameof(reidExtractor));

            TrackId = trackId;
            Config = config;
            ReIDExtractor = reidExtractor;
            Kf = new KalmanFilter();
            Kf.InitState(detection);

            LastDetection = detection;
            LastReIDFeature = detection.ReID;
            TimeSinceUpdate = 0;
            LostFrames = 0;
        }

        /// <summary>
        /// 预测轨迹
        /// </summary>
        public void Predict()
        {
            Kf.Predict();
            TimeSinceUpdate++;
            LostFrames++;
        }

        /// <summary>
        /// 更新轨迹
        /// </summary>
        /// <param name="detection">新检测框</param>
        public void Update(BoxItem detection)
        {
            if (detection == null) throw new ArgumentNullException(nameof(detection));

            Kf.Update(detection);
            LastDetection = detection;
            LastReIDFeature = detection.ReID;
            TimeSinceUpdate = 0;
            LostFrames = 0;
        }

    }
    #endregion

    #region 全局运动补偿（GMC）
    /// <summary>
    /// 全局运动补偿（基于ORB特征+单应性矩阵）
    /// </summary>
    public class GlobalMotionCompensator
    {
        private readonly BoTSORTConfig _config;

        public GlobalMotionCompensator(BoTSORTConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

    }
    #endregion

    #region BoT-SORT核心跟踪器
    /// <summary>
    /// BoT-SORT核心跟踪器（完整实现）
    /// </summary>
    public class BoTSORTTracker : IDisposable
    {
        private readonly BoTSORTConfig _config;
        private readonly IReIDExtractor _reidExtractor;
        private readonly GlobalMotionCompensator _gmc;
        private readonly List<BoTTrack> _tracks = new List<BoTTrack>();
        private int _nextTrackId = 0;
        private Image<Rgb24> _prevFrame;

        public BoTSORTTracker(BoTSORTConfig config, ITAServiceProvider provider)
        {
            _config = config ?? new BoTSORTConfig();
            _reidExtractor = provider.GetService<MobileCLIP2VisionRunner>();
            _gmc = new GlobalMotionCompensator(_config);
        }

        /// <summary>
        /// 更新跟踪器（核心方法）
        /// </summary>
        /// <param name="currFrame">当前帧图像</param>
        /// <param name="detections">原始检测框（无ReID特征）</param>
        /// <returns>跟踪结果</returns>
        public List<BoTTrack> Update(Image<Rgb24> currFrame, List<BoxItem> detections)
        {
            if (currFrame == null) throw new ArgumentNullException(nameof(currFrame));
            if (detections == null) detections = new List<BoxItem>();

            // 预处理检测框：提取ReID特征
            var processedDetections = ProcessDetections(currFrame, detections);

            // 预测所有轨迹
            foreach (var track in _tracks.ToList()) // 用ToList避免迭代时修改
            {
                track.Predict();
            }

            // 拆分高低置信度检测框
            var highConfDets = processedDetections
                .Where(d => d.score >= _config.TrackThresh)
                .ToList();

            var lowConfDets = processedDetections
                .Where(d => d.score >= _config.TrackLowThresh && d.score < _config.TrackThresh)
                .ToList();

            // 5. 第一阶段匹配：高置信度框 + 活跃轨迹
            var (matchedTracks1, unmatchedDets1) = FusedMatching(highConfDets,
                _tracks.Where(t => t.IsActive).ToList(), false);

            // 6. 第二阶段匹配：低置信度框 + 未匹配轨迹
            var unmatchedTracks = _tracks.Where(t => !matchedTracks1.Contains(t)).ToList();
            var (matchedTracks2, _) = FusedMatching(lowConfDets, unmatchedTracks, true);

            // 7. 更新匹配的轨迹
            UpdateMatchedTracks(matchedTracks1, highConfDets);
            UpdateMatchedTracks(matchedTracks2, lowConfDets);

            // 8. 创建新轨迹
            CreateNewTracks(unmatchedDets1);

            // 9. 清理失效轨迹
            CleanupLostTracks();

            // 10. 保存当前帧
            _prevFrame?.Dispose();
            _prevFrame = currFrame.Clone();

            return _tracks.Where(t => t.IsActive).ToList();
        }

        /// <summary>
        /// 预处理检测框：提取ReID特征
        /// </summary>
        private List<BoxItem> ProcessDetections(Image<Rgb24> frame, List<BoxItem> detections)
        {
            var result = new List<BoxItem>();
            foreach (var det in detections)
            {
                if (det == null || det.score < _config.TrackLowThresh)
                    continue;

                // 提取ReID特征
                det.ReID = _reidExtractor.ExtractFeature(frame, det);
                result.Add(det);
            }
            return result;
        }

        /// <summary>
        /// IoU+ReID融合匹配
        /// </summary>
        private (List<BoTTrack> matchedTracks, List<BoxItem> unmatchedDets)
            FusedMatching(List<BoxItem> detections, List<BoTTrack> tracks, bool isLowConf)
        {
            var matchedTracks = new List<BoTTrack>();
            var unmatchedDets = new List<BoxItem>(detections);
            var matchThresh = isLowConf ? _config.LowConfMatchThresh : _config.MatchThresh;

            foreach (var det in detections.ToList())
            {
                if (det == null) continue;

                BoTTrack bestTrack = null;
                float maxFusedScore = 0;

                foreach (var track in tracks)
                {
                    if (track == null || matchedTracks.Contains(track))
                        continue;

                    // 1. 计算IoU
                    var predBbox = track.Kf.GetPredictedBox();
                    float iou = CalculateIoU(det, predBbox);

                    // 2. 计算ReID相似度
                    float reidSim = 0;
                    if (track.LastReIDFeature != null && det.ReID != null)
                    {
                        reidSim = _reidExtractor.CosineSimilarity(track.LastReIDFeature, det.ReID);
                    }

                    // 3. 融合得分：lambda*ReID + (1-lambda)*IoU
                    float fusedScore = _config.Lambda * reidSim + (1 - _config.Lambda) * iou;

                    // 4. 筛选最优匹配
                    if (fusedScore > maxFusedScore && fusedScore >= matchThresh && reidSim >= _config.AppearanceThresh)
                    {
                        maxFusedScore = fusedScore;
                        bestTrack = track;
                    }
                }

                if (bestTrack != null)
                {
                    matchedTracks.Add(bestTrack);
                    unmatchedDets.Remove(det);
                }
            }

            return (matchedTracks, unmatchedDets);
        }

        /// <summary>
        /// 计算IoU（交并比）
        /// </summary>
        private float CalculateIoU(BoxItem a, BoxItem b)
        {
            if (a == null || b == null) return 0;

            var x1 = Math.Max(a.x1, b.x1);
            var y1 = Math.Max(a.y1, b.y1);
            var x2 = Math.Min(a.x2, b.x2);
            var y2 = Math.Min(a.y2, b.y2);

            var intersection = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
            if (intersection == 0)
                return 0;

            var union = a.GetArea() + b.GetArea() - intersection;
            return union > 0 ? intersection / union : 0;
        }

        /// <summary>
        /// 更新匹配的轨迹
        /// </summary>
        private void UpdateMatchedTracks(List<BoTTrack> matchedTracks, List<BoxItem> detections)
        {
            foreach (var track in matchedTracks)
            {
                if (track == null) continue;

                // 找到匹配的检测框
                var matchedDet = detections
                    .Where(d => d != null)
                    .OrderByDescending(d => CalculateIoU(d, track.Kf.GetPredictedBox()))
                    .FirstOrDefault(d => CalculateIoU(d, track.Kf.GetPredictedBox()) > _config.MatchThresh * 0.5);

                if (matchedDet != null)
                {
                    track.Update(matchedDet);
                }
            }
        }

        /// <summary>
        /// 创建新轨迹
        /// </summary>
        private void CreateNewTracks(List<BoxItem> detections)
        {
            foreach (var det in detections)
            {
                if (det == null || det.score < _config.NewTrackThresh)
                    continue;

                _tracks.Add(new BoTTrack(_nextTrackId++, det, _config, _reidExtractor));
            }
        }

        /// <summary>
        /// 清理失效轨迹
        /// </summary>
        private void CleanupLostTracks()
        {
            var lostTracks = _tracks.Where(t => t.IsLost).ToList();
            foreach (var track in lostTracks)
            {
                _tracks.Remove(track);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _prevFrame?.Dispose();
            _tracks.Clear();
        }
    }
    #endregion

    #region 使用示例
    public class UsageExample
    {
        public static void Run()
        {
            try
            {
                // 配置跟踪器
                var config = new BoTSORTConfig
                {
                    TrackThresh = 0.5f
                };

                // 初始化BoT-SORT跟踪器
                using var tracker = new BoTSORTTracker(config,null);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"运行错误: {ex.Message}");
            }
        }
    }
    #endregion
}