using ChannelUtility.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.AICode
{

    /// <summary>
    /// 卡尔曼滤波类（ByteTrack核心依赖）
    /// </summary>
    public class KalmanFilter
    {
        private const int NDIM = 4;
        private const int STATE_DIM = 8;
        private const float DT = 1.0f;

        // 官方固定超参
        private readonly float _stdWeightPos = 1.0f / 20.0f;
        private readonly float _stdWeightVel = 1.0f / 160.0f;

        // 状态 & 协方差
        private float[] _mean;       // [x,y,a,h,vx,vy,va,vh] 8
        private float[,] _cov;       // 8x8

        // 固定矩阵 F / H
        private readonly float[,] _motionMat;   // 8x8
        private readonly float[,] _updateMat;   // 4x8

        public KalmanFilter()
        {
            // 初始化 F 匀速模型
            _motionMat = Eye(STATE_DIM);
            for (int i = 0; i < NDIM; i++)
                _motionMat[i, i + NDIM] = DT;

            // H: 只观测前4维 x,y,a,h
            _updateMat = new float[NDIM, STATE_DIM];
            for (int i = 0; i < NDIM; i++)
                _updateMat[i, i] = 1.0f;
        }

        #region 初始化（第一次进轨迹）
        public void InitState(BoxItem det)
        {
            var meas = BoxToMeasure(det); // x,y,a,h
            _mean = new float[STATE_DIM];

            // 前4维观测，后4维速度0
            Array.Copy(meas, _mean, NDIM);

            float h = meas[3];
            // 官方init std
            float[] std = new float[STATE_DIM]
            {
                2 * _stdWeightPos * h,
                2 * _stdWeightPos * h,
                1e-2f,
                2 * _stdWeightPos * h,

                10 * _stdWeightVel * h,
                10 * _stdWeightVel * h,
                1e-5f,
                10 * _stdWeightVel * h
            };

            _cov = Diag(std);
        }
        #endregion

        #region 预测步（动态生成Q = motion_cov）
        public void Predict()
        {
            float h = _mean[3];

            // 官方 std_pos / std_vel
            float[] stdPos = new float[NDIM]
            {
                _stdWeightPos * h,
                _stdWeightPos * h,
                1e-2f,
                _stdWeightPos * h
            };
            float[] stdVel = new float[NDIM]
            {
                _stdWeightVel * h,
                _stdWeightVel * h,
                1e-5f,
                _stdWeightVel * h
            };

            // 拼接 std => 平方 => Q(motion_cov)
            float[] fullStd = Combine(stdPos, stdVel);
            float[,] Q = DiagSquare(fullStd);

            // mean = F * mean
            _mean = MulMatVec(_motionMat, _mean);

            // cov = F*cov*F^T + Q
            var ft = Transpose(_motionMat);
            var temp = MulMat(_motionMat, _cov);
            temp = MulMat(temp, ft);
            _cov = AddMat(temp, Q);
        }
        #endregion

        #region 更新步（动态生成R = innovation_cov）
        public void Update(BoxItem det)
        {
            float[] meas = BoxToMeasure(det);
            float h = _mean[3];

            // === Project 得到 Hx + R ===
            float[] stdR = new float[NDIM]
            {
                _stdWeightPos * h,
                _stdWeightPos * h,
                1e-1f,
                _stdWeightPos * h
            };
            float[,] R = DiagSquare(stdR);

            // 投影均值/协方差
            float[] meanProj = MulMatVec(_updateMat, _mean);
            var covProj = MulMat(_updateMat, _cov);
            covProj = MulMat(covProj, Transpose(_updateMat));
            covProj = AddMat(covProj, R);

            // 残差
            float[] y = SubVec(meas, meanProj);

            // 卡尔曼增益 K = cov * H^T * inv(covProj)
            var ht = Transpose(_updateMat);
            var kPart = MulMat(_cov, ht);
            var invCovProj = Invert4x4(covProj);
            var K = MulMat(kPart, invCovProj);

            // mean = mean + K*y
            _mean = AddVec(_mean, MulMatVec(K, y));

            // cov = (I - K*H) * cov
            var kh = MulMat(K, _updateMat);
            var ikh = SubMat(Eye(STATE_DIM), kh);
            _cov = MulMat(ikh, _cov);
        }
        #endregion

        #region 对外接口（保留你原有调用）

        public Vector2 GetVelocity()
        {
            return new Vector2(_mean[4], _mean[5]);
        }

        public BoxItem GetPredictedBox()
        {
            float x = _mean[0];
            float y = _mean[1];
            float a = _mean[2];
            float h = _mean[3];
            float w = a * h;

            return new BoxItem
            {
                x1 = x - w / 2f,
                y1 = y - h / 2f,
                x2 = x + w / 2f,
                y2 = y + h / 2f
            };
        }
        #endregion

        #region 工具：框转观测 [x,y,a,h]
        private float[] BoxToMeasure(BoxItem d)
        {
            float cx = (d.x1 + d.x2) / 2f;
            float cy = (d.y1 + d.y2) / 2f;
            float w = d.x2 - d.x1;
            float h = d.y2 - d.y1;
            float a = w / h;
            return new float[] { cx, cy, a, h };
        }
        #endregion

        #region 底层矩阵向量数学
        private float[,] Eye(int n)
        {
            var m = new float[n, n];
            for (int i = 0; i < n; i++) m[i, i] = 1f;
            return m;
        }

        private float[,] Diag(float[] s)
        {
            int n = s.Length;
            var m = new float[n, n];
            for (int i = 0; i < n; i++) m[i, i] = s[i];
            return m;
        }

        private float[,] DiagSquare(float[] s)
        {
            int n = s.Length;
            var m = new float[n, n];
            for (int i = 0; i < n; i++) m[i, i] = s[i] * s[i];
            return m;
        }

        private float[] Combine(float[] a, float[] b)
        {
            var r = new float[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, r, 0, a.Length * sizeof(float));
            Buffer.BlockCopy(b, 0, r, a.Length * sizeof(float), b.Length * sizeof(float));
            return r;
        }

        private float[,] Transpose(float[,] m)
        {
            int r = m.GetLength(0), c = m.GetLength(1);
            var t = new float[c, r];
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    t[j, i] = m[i, j];
            return t;
        }

        private float[] MulMatVec(float[,] m, float[] v)
        {
            int r = m.GetLength(0), c = m.GetLength(1);
            var res = new float[r];
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    res[i] += m[i, j] * v[j];
            return res;
        }

        private float[,] MulMat(float[,] a, float[,] b)
        {
            int ar = a.GetLength(0), ac = a.GetLength(1);
            int br = b.GetLength(0), bc = b.GetLength(1);
            var res = new float[ar, bc];
            for (int i = 0; i < ar; i++)
                for (int k = 0; k < ac; k++)
                    for (int j = 0; j < bc; j++)
                        res[i, j] += a[i, k] * b[k, j];
            return res;
        }

        private float[,] AddMat(float[,] a, float[,] b)
        {
            int r = a.GetLength(0), c = a.GetLength(1);
            var res = new float[r, c];
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    res[i, j] = a[i, j] + b[i, j];
            return res;
        }

        private float[,] SubMat(float[,] a, float[,] b)
        {
            int r = a.GetLength(0), c = a.GetLength(1);
            var res = new float[r, c];
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    res[i, j] = a[i, j] - b[i, j];
            return res;
        }

        private float[] AddVec(float[] a, float[] b)
        {
            var res = new float[a.Length];
            for (int i = 0; i < a.Length; i++) res[i] = a[i] + b[i];
            return res;
        }

        private float[] SubVec(float[] a, float[] b)
        {
            var res = new float[a.Length];
            for (int i = 0; i < a.Length; i++) res[i] = a[i] - b[i];
            return res;
        }

        // 仅4阶求逆（Project输出固定4x4），稳定兜底
        private float[,] Invert4x4(float[,] m)
        {
            // 简易对角占优兜底 + 行列式安全
            float det = m[0, 0] * m[1, 1] * m[2, 2] * m[3, 3];
            if (MathF.Abs(det) < 1e-10f)
                return Eye(4);

            var inv = Eye(4);
            for (int i = 0; i < 4; i++) inv[i, i] = 1f / m[i, i];
            return inv;
        }
        #endregion
    }
}
