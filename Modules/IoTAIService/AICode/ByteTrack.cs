using ChannelUtility.Message;
using DeveloperService.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace IoTAIService.AICode
{
    /// <summary>
    /// 运动方向枚举（8个主方向）
    /// </summary>
    public enum MovementDirection
    {
        Unknown,    // 未知/静止
        Up,         // 上
        Down,       // 下
        Left,       // 左
        Right,      // 右
        UpLeft,     // 左上
        UpRight,    // 右上
        DownLeft,   // 左下
        DownRight   // 右下
    }
    /// <summary>
    /// 卡尔曼滤波类（ByteTrack核心依赖）
    /// </summary>
    public class KalmanFilter
    {
        private const float dt = 1.0f / 10.0f; // 帧间隔
        private readonly Matrix4x4 _F; // 状态转移矩阵
        private readonly Matrix4x4 _H; // 观测矩阵
        private readonly Matrix4x4 _Q; // 过程噪声
        private readonly Matrix4x4 _R; // 观测噪声

        private Vector4 _x; // 状态向量 [x, y, vx, vy]
        private Matrix4x4 _P; // 协方差矩阵

        public KalmanFilter()
        {
            // 修复1：用Identity属性替代CreateIdentity()
            _F = Matrix4x4.Identity;
            _F.M13 = dt;
            _F.M24 = dt;

            // 修复2：观测矩阵初始化（兼容所有版本）
            _H = Matrix4x4.Identity;
            _H.M33 = 0;
            _H.M44 = 0;

            // 修复3：噪声矩阵初始化（用Scale替代CreateScale）
            _Q = Matrix4x4.CreateScale(0.01f); // 若此方法也不存在，见下方备用方案
            _R = Matrix4x4.CreateScale(0.1f);

            _x = Vector4.Zero;
            // 修复4：协方差矩阵初始化
            _P = Matrix4x4.Identity * 1000;
        }

        /// <summary>
        /// 预测下一帧状态
        /// </summary>
        public void Predict()
        {
            // 状态预测：x = F * x
            _x = Vector4.Transform(_x, _F);
            // 协方差预测：P = F * P * F^T + Q
            _P = _F * _P * Matrix4x4.Transpose(_F) + _Q;
        }

        /// <summary>
        /// 更新观测值
        /// </summary>
        /// <param name="measurement"></param>
        public void Update(Vector2 measurement)
        {
            var z = new Vector4(measurement.X, measurement.Y, 0, 0);
            var y = z - Vector4.Transform(_x, _H);
            var S = _H * _P * Matrix4x4.Transpose(_H) + _R;

            // 修复5：矩阵求逆兼容（处理奇异矩阵）
            Matrix4x4 invS;
            if (!Matrix4x4.Invert(S, out invS))
            {
                invS = Matrix4x4.Identity; // 求逆失败时用单位矩阵兜底
            }

            var K = _P * Matrix4x4.Transpose(_H) * invS;

            _x = _x + Vector4.Transform(y, K);
            _P = (Matrix4x4.Identity - K * _H) * _P;
        }

        /// <summary>
        /// 获取预测的边界框中心
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPredictedCenter() => new Vector2(_x.X, _x.Y);

        /// <summary>
        /// 获取速度向量 [vx, vy]
        /// </summary>
        /// <returns></returns>
        public Vector2 GetVelocity() => new Vector2(_x.Z, _x.W);
    }

    // 跟踪轨迹实体（无修改）
    public class Track
    {
        private readonly Queue<Vector2> _positionHistory = new Queue<Vector2>(capacity: 5);
        private const float _minSpeedThreshold = 0.1f; // 最小速度阈值（过滤静止）

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
            var tmpcenter = detection.GetCenter();
            Kf.Update(tmpcenter);
            TimeSinceUpdate = 0;
            _positionHistory.Enqueue(tmpcenter);
        }

        // 更新轨迹
        public void Update(BoxItem detection)
        {
            CurrentDetection = detection;
            var center = detection.GetCenter();
            Kf.Update(center);

            // 更新位置历史
            _positionHistory.Enqueue(center);
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
    }

    // ByteTrack核心跟踪器（无修改）
    public class ByteTrack
    {
        private readonly float _trackThresh; // 高置信度阈值（默认0.5）
        private readonly float _trackLowThresh; // 低置信度阈值（默认0.1）
        private readonly float _matchThresh; // IoU匹配阈值（默认0.8）
        private readonly List<Track> _tracks = new List<Track>();
        private int _nextTrackId = 0;

        // 构造函数（可自定义参数）
        public ByteTrack(float trackThresh = 0.5f, float trackLowThresh = 0.1f, float matchThresh = 0.8f)
        {
            _trackThresh = trackThresh;
            _trackLowThresh = trackLowThresh;
            _matchThresh = matchThresh;
        }

        // 核心方法：处理单张图片的检测结果，返回跟踪结果
        public (List<Track>, List<Track>, List<Track>) Update(List<BoxItem> detections)
        {
            // 1. 预测所有现有轨迹的下一状态
            foreach (var track in _tracks)
            {
                track.Predict();
            }

            // 2. 拆分检测框：高置信度（用于初始匹配）、低置信度（用于补充匹配）
            var highConfDets = detections.Where(d => d.score >= _trackThresh).ToList();
            var lowConfDets = detections.Where(d => d.score >= _trackLowThresh && d.score < _trackThresh).ToList();

            // 3. 第一步匹配：高置信度框 vs 现有轨迹（IoU匹配）
            var (matchedTracks, unmatchedHighDets) = Match(highConfDets, _tracks, _matchThresh);

            // 4. 第二步匹配：低置信度框 vs 未匹配的轨迹（补充IoU匹配）
            var unmatchedTracks = _tracks.Where(t => !matchedTracks.Contains(t)).ToList();
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
            List<Track> delTracks = new List<Track>();
            for (int i = _tracks.Count - 1; i >= 0; i--)
            {
                var tmptrack = _tracks[i];
                if (!tmptrack.IsActive)
                {
                    delTracks.Add(tmptrack);
                    _tracks.RemoveAt(i);
                }
            }

            // 返回当前活跃的轨迹
            return (_tracks.Where(t => t.IsActive).ToList(), newTracks, delTracks);
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

                    var iou = CalculateIoU(det, track.CurrentDetection);
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
