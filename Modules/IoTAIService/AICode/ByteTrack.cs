using ChannelUtility.Message;
using DeveloperService.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace IoTAIService.AICode
{

    public class RegionStayInfo
    {
        public DateTime? EnterTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public bool IsSend { get; set; }
    }
    // 跟踪轨迹实体
    public class Track
    {
        private readonly Dictionary<string, RegionStatus> _regionStatusDict = new Dictionary<string, RegionStatus>();
        private readonly Dictionary<string, RegionStayInfo> _regionStayDict = new Dictionary<string, RegionStayInfo>();
        private readonly Queue<Vector2> _positionHistory = new Queue<Vector2>(capacity: 5);
        private const float _minSpeedThreshold = 0.1f; // 最小速度阈值（过滤静止）
        public DateTime CreatedOn { get; set; }
        public int Id { get; set; } // 唯一跟踪ID
        public BoxItem CurrentDetection { get; set; } // 当前检测框
        public KalmanFilter Kf { get; set; } // 卡尔曼滤波器
        public int TimeSinceUpdate { get; set; } // 未更新帧数
        public bool IsActive => TimeSinceUpdate < 30; // 轨迹是否活跃（track_buffer=30）

        public Track(BoxItem detection, int id)
        {
            Id = id;
            CurrentDetection = detection;
            Kf = new KalmanFilter();

            Kf.InitState(detection);
            Kf.Predict();
            Kf.Update(detection);

            TimeSinceUpdate = 0;
            _positionHistory.Enqueue(detection.GetCenter());
            CreatedOn = DateTime.Now;
        }

        // 更新轨迹
        public void Update(BoxItem detection)
        {
            CurrentDetection = detection;
            Kf.Update(detection);

            // 更新位置历史
            _positionHistory.Enqueue(detection.GetCenter());
            if (_positionHistory.Count > 5) // 只保留最近5帧
                _positionHistory.Dequeue();

            TimeSinceUpdate = 0;
        }

        // 预测轨迹（无新检测时）
        public void Predict()
        {
            Kf.Predict();
            TimeSinceUpdate++;
        }
        /// <summary>
        /// 获取当前运动方向
        /// </summary>
        /// <returns></returns>
        public MovementDirection GetMovementDirection()
        {
            // 1. 获取卡尔曼滤波器输出的速度向量
            var velocity = Kf.GetVelocity();

            // 2. 过滤静止状态（速度低于阈值）
            if (Math.Abs(velocity.X) < _minSpeedThreshold && Math.Abs(velocity.Y) < _minSpeedThreshold)
            {
                // 验证历史位置是否真的静止
                if (_positionHistory.Count < 2)
                    return MovementDirection.Unknown;

                var first = _positionHistory.First();
                var last = _positionHistory.Last();
                if (Vector2.Distance(first, last) < _minSpeedThreshold * 5)
                    return MovementDirection.Unknown;
            }

            // 3. 计算方向角度（弧度）
            float angle = (float)Math.Atan2(velocity.Y, velocity.X);
            // 转换为角度（0-360度）
            float degrees = (angle * 180 / (float)Math.PI + 360) % 360;

            // 4. 根据角度判断方向
            if (degrees >= 337.5 || degrees < 22.5)
                return MovementDirection.Right;
            else if (degrees >= 22.5 && degrees < 67.5)
                return MovementDirection.DownRight;
            else if (degrees >= 67.5 && degrees < 112.5)
                return MovementDirection.Down;
            else if (degrees >= 112.5 && degrees < 157.5)
                return MovementDirection.DownLeft;
            else if (degrees >= 157.5 && degrees < 202.5)
                return MovementDirection.Left;
            else if (degrees >= 202.5 && degrees < 247.5)
                return MovementDirection.UpLeft;
            else if (degrees >= 247.5 && degrees < 292.5)
                return MovementDirection.Up;
            else // 292.5-337.5
                return MovementDirection.UpRight;
        }

        /// <summary>
        /// 获取运动速度（像素/帧）
        /// </summary>
        /// <returns></returns>
        public float GetMovementSpeed()
        {
            var velocity = Kf.GetVelocity();
            return velocity.Length(); // 速度向量的模长
        }

        /// <summary>
        /// 获取历史位置列表
        /// </summary>
        public List<Vector2> GetPositionHistory() => _positionHistory.ToList();


        /// <summary>
        /// 更新目标在指定区域的状态
        /// </summary>
        /// <param name="region">监控区域</param>
        /// <returns>区域状态（是否入侵）</returns>
        public RegionStatus UpdateRegionStatus(MonitoringRegion region)
        {
            if (!region.IsActive) return RegionStatus.Outside;

            // 获取目标当前中心位置
            Vector2 currentPos = CurrentDetection.GetCenter();
            bool isInRegion = region.ContainsPoint(currentPos);

            // 获取上一帧的区域状态（默认外部）
            if (!_regionStatusDict.ContainsKey(region.Id))
                _regionStatusDict[region.Id] = RegionStatus.Outside;

            RegionStatus lastStatus = _regionStatusDict[region.Id];
            RegionStatus currentStatus;

            // 判断状态变化
            if (isInRegion)
            {
                if (lastStatus == RegionStatus.Outside || lastStatus == RegionStatus.Exited)
                {
                    // 从外部进入内部：触发入侵告警
                    currentStatus = RegionStatus.Entered;
                    _regionStayDict[region.Id] = new RegionStayInfo()
                    {
                        EnterTime = DateTime.Now,
                        ExitTime = null,
                        IsSend = false
                    };
                }
                else
                {
                    // 持续在内部
                    currentStatus = RegionStatus.Inside;
                }
            }
            else
            {
                if (lastStatus == RegionStatus.Inside || lastStatus == RegionStatus.Entered)
                {
                    // 从内部离开
                    currentStatus = RegionStatus.Exited;
                    _regionStayDict[region.Id] = new RegionStayInfo()
                    {
                        EnterTime = null,
                        ExitTime = DateTime.Now,
                        IsSend = false
                    };
                }
                else
                {
                    // 持续在外部
                    currentStatus = RegionStatus.Outside;
                }
            }

            // 更新状态字典（将Entered/Exited转换为Inside/Outside，避免状态残留）
            _regionStatusDict[region.Id] = currentStatus switch
            {
                RegionStatus.Entered => RegionStatus.Inside,
                RegionStatus.Exited => RegionStatus.Outside,
                _ => currentStatus
            };

            return currentStatus;
        }

        /// <summary>
        /// 获取目标在指定区域的当前状态
        /// </summary>
        /// <param name="regionId">区域ID</param>
        /// <returns></returns>
        public RegionStatus GetRegionStatus(string regionId)
        {
            return _regionStatusDict.TryGetValue(regionId, out var status) ? status : RegionStatus.Outside;
        }
        /// <summary>
        /// 获取目标在指定区域的停留信息
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public RegionStayInfo GetRegionStay(string regionId)
        {
            if(_regionStayDict.TryGetValue(regionId, out var info))
            {
                return info;
            }
            else
            {
                return null;
            }
        }
    }

    // ByteTrack核心跟踪器
    public class ByteTrack
    {
        private readonly float _trackThresh; // 高置信度阈值（默认0.5）
        private readonly float _matchThresh; // IoU匹配阈值（默认0.3）
        private readonly List<Track> _tracks = new List<Track>();
        private int _nextTrackId = 0;

        // 构造函数（可自定义参数）
        public ByteTrack(float trackThresh = 0.5f, float matchThresh = 0.3f)
        {
            _trackThresh = trackThresh;
            _matchThresh = matchThresh;
        }

        // 核心方法：处理单张图片的检测结果，返回跟踪结果
        public (List<Track>, List<Track>) Update(List<BoxItem> detections)
        {
            // 1. 预测所有现有轨迹的下一状态
            foreach (var track in _tracks)
            {
                track.Predict();
            }

            // 2. 拆分检测框：高置信度（用于初始匹配）、低置信度（用于补充匹配）
            var highConfDets = detections.Where(d => d.score >= _trackThresh).ToList();
            var lowConfDets = detections.Where(d => d.score < _trackThresh).ToList();
            var activeTracks = _tracks.Where(t => t.IsActive).ToList();

            // 3. 第一步匹配：高置信度框 vs 现有轨迹（IoU匹配）
            var (matchedTracks, unmatchedHighDets) = Match(highConfDets, activeTracks, _matchThresh);

            // 4. 第二步匹配：低置信度框 vs 未匹配的轨迹（补充IoU匹配）
            var unmatchedTracks = activeTracks.Where(t => !matchedTracks.Contains(t)).ToList();
            var (supplementedTracks, _) = Match(lowConfDets, unmatchedTracks, _matchThresh * 0.5f); // 低置信度匹配阈值降低

            // 5. 更新匹配到的轨迹
            foreach (var (track, det) in matchedTracks.Zip(highConfDets.Except(unmatchedHighDets), (t, d) => (t, d)))
            {
                track.Update(det);
            }
            foreach (var (track, det) in supplementedTracks.Zip(lowConfDets, (t, d) => (t, d)))
            {
                track.Update(det);
            }

            // 6. 新建轨迹：未匹配的高置信度检测框
            List<Track> newTracks = new List<Track>();
            foreach (var det in unmatchedHighDets)
            {
                var tmptrack = new Track(det, _nextTrackId++);
                _tracks.Add(tmptrack);
                newTracks.Add(tmptrack);
            }

            // 7. 清理失效轨迹（超过track_buffer未更新）
            _tracks.RemoveAll(track => !track.IsActive);

            // 返回当前活跃的轨迹
            return (_tracks.ToList(), newTracks);
        }

        // IoU匹配核心逻辑
        private (List<Track> matchedTracks, List<BoxItem> unmatchedDetections) Match(
            List<BoxItem> detections, List<Track> tracks, float iouThresh)
        {
            var matchedTracks = new List<Track>();
            var unmatchedDetections = new List<BoxItem>(detections);

            // 遍历每个检测框，找IoU最高的轨迹
            foreach (var det in detections.ToList())
            {
                Track bestTrack = null;
                float maxIoU = 0;

                foreach (var track in tracks)
                {
                    if (matchedTracks.Contains(track) || !string.Equals(track.CurrentDetection.label, det.label)) continue;

                    var predBox = track.Kf.GetPredictedBox();
                    var iou = CalculateIoU(det, predBox);
                    if (iou > maxIoU && iou >= iouThresh)
                    {
                        maxIoU = iou;
                        bestTrack = track;
                    }
                }

                if (bestTrack != null)
                {
                    matchedTracks.Add(bestTrack);
                    unmatchedDetections.Remove(det);
                }
            }

            return (matchedTracks, unmatchedDetections);
        }
    
        // 计算两个检测框的IoU（交并比）
        private float CalculateIoU(BoxItem a, BoxItem b)
        {
            var x1 = Math.Max(a.x1, b.x1);
            var y1 = Math.Max(a.y1, b.y1);
            var x2 = Math.Min(a.x2, b.x2);
            var y2 = Math.Min(a.y2, b.y2);

            var intersection = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
            if (intersection == 0) return 0;

            var union = a.GetArea() + b.GetArea() - intersection;
            return intersection / union;
        }
    }
}
