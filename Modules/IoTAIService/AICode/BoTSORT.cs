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
    #region 基础矩阵/向量结构体
    /// <summary>
    /// 3x3矩阵（GMC单应性矩阵专用）
    /// </summary>
    public struct Matrix3x3
    {
        public float M11, M12, M13;
        public float M21, M22, M23;
        public float M31, M32, M33;

        /// <summary>
        /// 单位矩阵
        /// </summary>
        public static Matrix3x3 Identity => new Matrix3x3
        {
            M11 = 1,
            M22 = 1,
            M33 = 1,
            M12 = 0,
            M13 = 0,
            M21 = 0,
            M23 = 0,
            M31 = 0,
            M32 = 0
        };

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b)
        {
            return new Matrix3x3
            {
                M11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31,
                M12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32,
                M13 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33,

                M21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31,
                M22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32,
                M23 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33,

                M31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31,
                M32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32,
                M33 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33
            };
        }

        /// <summary>
        /// 矩阵×向量（齐次坐标）
        /// </summary>
        public static float[] operator *(Matrix3x3 m, float[] v)
        {
            if (v == null || v.Length != 3)
                throw new ArgumentException("向量必须为3维（齐次坐标）");

            return new[]
            {
                m.M11*v[0] + m.M12*v[1] + m.M13*v[2],
                m.M21*v[0] + m.M22*v[1] + m.M23*v[2],
                m.M31*v[0] + m.M32*v[1] + m.M33*v[2]
            };
        }

        /// <summary>
        /// 矩阵求逆（3x3）
        /// </summary>
        public Matrix3x3 Invert()
        {
            // 计算行列式
            float det = M11 * (M22 * M33 - M23 * M32) - M12 * (M21 * M33 - M23 * M31) + M13 * (M21 * M32 - M22 * M31);
            if (Math.Abs(det) < 1e-6) return Identity;

            float invDet = 1 / det;

            // 伴随矩阵转置
            return new Matrix3x3
            {
                M11 = invDet * (M22 * M33 - M23 * M32),
                M12 = invDet * (M13 * M32 - M12 * M33),
                M13 = invDet * (M12 * M23 - M13 * M22),

                M21 = invDet * (M23 * M31 - M21 * M33),
                M22 = invDet * (M11 * M33 - M13 * M31),
                M23 = invDet * (M13 * M21 - M11 * M23),

                M31 = invDet * (M21 * M32 - M22 * M31),
                M32 = invDet * (M12 * M31 - M11 * M32),
                M33 = invDet * (M11 * M22 - M12 * M21)
            };
        }
    }

    /// <summary>
    /// 8x8矩阵（卡尔曼滤波专用）
    /// </summary>
    public struct Matrix8x8
    {
        public float M11, M12, M13, M14, M15, M16, M17, M18;
        public float M21, M22, M23, M24, M25, M26, M27, M28;
        public float M31, M32, M33, M34, M35, M36, M37, M38;
        public float M41, M42, M43, M44, M45, M46, M47, M48;
        public float M51, M52, M53, M54, M55, M56, M57, M58;
        public float M61, M62, M63, M64, M65, M66, M67, M68;
        public float M71, M72, M73, M74, M75, M76, M77, M78;
        public float M81, M82, M83, M84, M85, M86, M87, M88;

        /// <summary>
        /// 单位矩阵
        /// </summary>
        public static Matrix8x8 Identity => new Matrix8x8
        {
            M11 = 1,
            M22 = 1,
            M33 = 1,
            M44 = 1,
            M55 = 1,
            M66 = 1,
            M77 = 1,
            M88 = 1
        };

        /// <summary>
        /// 零矩阵
        /// </summary>
        public static Matrix8x8 Zero => new Matrix8x8();

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        public static Matrix8x8 operator *(Matrix8x8 a, Matrix8x8 b)
        {
            Matrix8x8 res = new Matrix8x8();

            // 完整8x8矩阵乘法
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < 8; k++)
                    {
                        sum += GetElement(a, i, k) * GetElement(b, k, j);
                    }
                    SetElement(ref res, i, j, sum);
                }
            }

            return res;
        }

        /// <summary>
        /// 矩阵×向量
        /// </summary>
        public static float[] operator *(Matrix8x8 m, float[] v)
        {
            if (v == null || v.Length != 8)
                throw new ArgumentException("向量必须为8维");

            float[] res = new float[8];
            for (int i = 0; i < 8; i++)
            {
                float sum = 0;
                for (int j = 0; j < 8; j++)
                {
                    sum += GetElement(m, i, j) * v[j];
                }
                res[i] = sum;
            }

            return res;
        }
        /// <summary>
        /// 8x8矩阵 × 8x4矩阵 = 8x4矩阵（卡尔曼滤波核心乘法）
        /// </summary>
        public static Matrix8x4 operator *(Matrix8x8 a, Matrix8x4 b)
        {
            Matrix8x4 res = new Matrix8x4();

            // 矩阵乘法核心逻辑：8x8 × 8x4 = 8x4
            for (int i = 0; i < 8; i++)  // 结果矩阵的行（0-7）
            {
                for (int j = 0; j < 4; j++)  // 结果矩阵的列（0-3）
                {
                    float sum = 0;
                    for (int k = 0; k < 8; k++)  // 累加维度（0-7）
                    {
                        // a的第i行第k列 × b的第k行第j列
                        sum += Matrix8x8.GetElement(a, i, k) * Matrix8x4.GetElement(b, k, j);
                    }
                    // 设置结果矩阵的第i行第j列
                    Matrix8x4.SetElement(ref res, i, j, sum);
                }
            }

            return res;
        }

        /// <summary>
        /// 矩阵数乘
        /// </summary>
        public static Matrix8x8 operator *(Matrix8x8 m, float s)
        {
            Matrix8x8 res = m;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    SetElement(ref res, i, j, GetElement(res, i, j) * s);
                }
            }
            return res;
        }

        /// <summary>
        /// 矩阵加法
        /// </summary>
        public static Matrix8x8 operator +(Matrix8x8 a, Matrix8x8 b)
        {
            Matrix8x8 res = new Matrix8x8();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    SetElement(ref res, i, j, GetElement(a, i, j) + GetElement(b, i, j));
                }
            }
            return res;
        }

        /// <summary>
        /// 矩阵减法
        /// </summary>
        public static Matrix8x8 operator -(Matrix8x8 a, Matrix8x8 b)
        {
            Matrix8x8 res = new Matrix8x8();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    SetElement(ref res, i, j, GetElement(a, i, j) - GetElement(b, i, j));
                }
            }
            return res;
        }

        /// <summary>
        /// 矩阵转置
        /// </summary>
        public Matrix8x8 Transpose()
        {
            Matrix8x8 res = new Matrix8x8();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    SetElement(ref res, i, j, GetElement(this, j, i));
                }
            }
            return res;
        }

        /// <summary>
        /// 获取矩阵元素（辅助方法）
        /// </summary>
        public static float GetElement(Matrix8x8 m, int row, int col)
        {
            if (row < 0 || row >= 8 || col < 0 || col >= 8) return 0;

            return row switch
            {
                0 => col switch { 0 => m.M11, 1 => m.M12, 2 => m.M13, 3 => m.M14, 4 => m.M15, 5 => m.M16, 6 => m.M17, 7 => m.M18, _ => 0 },
                1 => col switch { 0 => m.M21, 1 => m.M22, 2 => m.M23, 3 => m.M24, 4 => m.M25, 5 => m.M26, 6 => m.M27, 7 => m.M28, _ => 0 },
                2 => col switch { 0 => m.M31, 1 => m.M32, 2 => m.M33, 3 => m.M34, 4 => m.M35, 5 => m.M36, 6 => m.M37, 7 => m.M38, _ => 0 },
                3 => col switch { 0 => m.M41, 1 => m.M42, 2 => m.M43, 3 => m.M44, 4 => m.M45, 5 => m.M46, 6 => m.M47, 7 => m.M48, _ => 0 },
                4 => col switch { 0 => m.M51, 1 => m.M52, 2 => m.M53, 3 => m.M54, 4 => m.M55, 5 => m.M56, 6 => m.M57, 7 => m.M58, _ => 0 },
                5 => col switch { 0 => m.M61, 1 => m.M62, 2 => m.M63, 3 => m.M64, 4 => m.M65, 5 => m.M66, 6 => m.M67, 7 => m.M68, _ => 0 },
                6 => col switch { 0 => m.M71, 1 => m.M72, 2 => m.M73, 3 => m.M74, 4 => m.M75, 5 => m.M76, 6 => m.M77, 7 => m.M78, _ => 0 },
                7 => col switch { 0 => m.M81, 1 => m.M82, 2 => m.M83, 3 => m.M84, 4 => m.M85, 5 => m.M86, 6 => m.M87, 7 => m.M88, _ => 0 },
                _ => 0
            };
        }

        /// <summary>
        /// 设置矩阵元素（辅助方法）- 补充缺失的核心方法
        /// </summary>
        public static void SetElement(ref Matrix8x8 m, int row, int col, float value)
        {
            if (row < 0 || row >= 8 || col < 0 || col >= 8) return;

            switch (row)
            {
                case 0:
                    switch (col)
                    {
                        case 0: m.M11 = value; break;
                        case 1: m.M12 = value; break;
                        case 2: m.M13 = value; break;
                        case 3: m.M14 = value; break;
                        case 4: m.M15 = value; break;
                        case 5: m.M16 = value; break;
                        case 6: m.M17 = value; break;
                        case 7: m.M18 = value; break;
                    }
                    break;
                case 1:
                    switch (col)
                    {
                        case 0: m.M21 = value; break;
                        case 1: m.M22 = value; break;
                        case 2: m.M23 = value; break;
                        case 3: m.M24 = value; break;
                        case 4: m.M25 = value; break;
                        case 5: m.M26 = value; break;
                        case 6: m.M27 = value; break;
                        case 7: m.M28 = value; break;
                    }
                    break;
                case 2:
                    switch (col)
                    {
                        case 0: m.M31 = value; break;
                        case 1: m.M32 = value; break;
                        case 2: m.M33 = value; break;
                        case 3: m.M34 = value; break;
                        case 4: m.M35 = value; break;
                        case 5: m.M36 = value; break;
                        case 6: m.M37 = value; break;
                        case 7: m.M38 = value; break;
                    }
                    break;
                case 3:
                    switch (col)
                    {
                        case 0: m.M41 = value; break;
                        case 1: m.M42 = value; break;
                        case 2: m.M43 = value; break;
                        case 3: m.M44 = value; break;
                        case 4: m.M45 = value; break;
                        case 5: m.M46 = value; break;
                        case 6: m.M47 = value; break;
                        case 7: m.M48 = value; break;
                    }
                    break;
                case 4:
                    switch (col)
                    {
                        case 0: m.M51 = value; break;
                        case 1: m.M52 = value; break;
                        case 2: m.M53 = value; break;
                        case 3: m.M54 = value; break;
                        case 4: m.M55 = value; break;
                        case 5: m.M56 = value; break;
                        case 6: m.M57 = value; break;
                        case 7: m.M58 = value; break;
                    }
                    break;
                case 5:
                    switch (col)
                    {
                        case 0: m.M61 = value; break;
                        case 1: m.M62 = value; break;
                        case 2: m.M63 = value; break;
                        case 3: m.M64 = value; break;
                        case 4: m.M65 = value; break;
                        case 5: m.M66 = value; break;
                        case 6: m.M67 = value; break;
                        case 7: m.M68 = value; break;
                    }
                    break;
                case 6:
                    switch (col)
                    {
                        case 0: m.M71 = value; break;
                        case 1: m.M72 = value; break;
                        case 2: m.M73 = value; break;
                        case 3: m.M74 = value; break;
                        case 4: m.M75 = value; break;
                        case 5: m.M76 = value; break;
                        case 6: m.M77 = value; break;
                        case 7: m.M78 = value; break;
                    }
                    break;
                case 7:
                    switch (col)
                    {
                        case 0: m.M81 = value; break;
                        case 1: m.M82 = value; break;
                        case 2: m.M83 = value; break;
                        case 3: m.M84 = value; break;
                        case 4: m.M85 = value; break;
                        case 5: m.M86 = value; break;
                        case 6: m.M87 = value; break;
                        case 7: m.M88 = value; break;
                    }
                    break;
            }
        }

        /// <summary>
        /// 4x4子矩阵求逆（卡尔曼滤波创新协方差求逆）
        /// </summary>
        public static float[][] Invert4x4(float[][] m)
        {
            if (m == null || m.Length != 4 || m.Any(row => row == null || row.Length != 4))
                throw new ArgumentException("矩阵必须是4x4");

            // 简化实现：仅处理对角占优矩阵（卡尔曼滤波场景适用）
            float[][] inv = new float[4][];
            for (int i = 0; i < 4; i++)
            {
                inv[i] = new float[4];
                inv[i][i] = 1 / (m[i][i] + 1e-6f); // 加小值避免除零
            }
            return inv;
        }
    }

    /// <summary>
    /// 4x8观测矩阵（卡尔曼滤波专用）
    /// </summary>
    public struct Matrix4x8
    {
        public float M11, M12, M13, M14, M15, M16, M17, M18;
        public float M21, M22, M23, M24, M25, M26, M27, M28;
        public float M31, M32, M33, M34, M35, M36, M37, M38;
        public float M41, M42, M43, M44, M45, M46, M47, M48;

        /// <summary>
        /// 单位观测矩阵（仅观测前4维）
        /// </summary>
        public static Matrix4x8 Identity => new Matrix4x8
        {
            M11 = 1,
            M22 = 1,
            M33 = 1,
            M44 = 1
        };

        /// <summary>
        /// 矩阵×向量
        /// </summary>
        public static float[] operator *(Matrix4x8 m, float[] v)
        {
            if (v == null || v.Length != 8)
                throw new ArgumentException("向量必须为8维");

            float[] res = new float[4];
            for (int i = 0; i < 4; i++)
            {
                float sum = 0;
                for (int j = 0; j < 8; j++)
                {
                    sum += GetElement(m, i, j) * v[j];
                }
                res[i] = sum;
            }
            return res;
        }

        /// <summary>
        /// 矩阵乘法（4x8 × 8x8 = 4x8）
        /// </summary>
        public static Matrix4x8 operator *(Matrix4x8 a, Matrix8x8 b)
        {
            Matrix4x8 res = new Matrix4x8();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < 8; k++)
                    {
                        sum += GetElement(a, i, k) * Matrix8x8.GetElement(b, k, j);
                    }
                    SetElement(ref res, i, j, sum);
                }
            }
            return res;
        }

        /// <summary>
        /// 矩阵乘法（8x4 × 4x8 = 8x8）- 新增：卡尔曼增益计算需要
        /// </summary>
        public static Matrix8x8 operator *(Matrix8x4 a, Matrix4x8 b)
        {
            Matrix8x8 res = new Matrix8x8();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        sum += Matrix8x4.GetElement(a, i, k) * GetElement(b, k, j);
                    }
                    Matrix8x8.SetElement(ref res, i, j, sum);
                }
            }
            return res;
        }

        /// <summary>
        /// 获取矩阵元素（辅助方法）
        /// </summary>
        public static float GetElement(Matrix4x8 m, int row, int col)
        {
            if (row < 0 || row >= 4 || col < 0 || col >= 8) return 0;

            return row switch
            {
                0 => col switch { 0 => m.M11, 1 => m.M12, 2 => m.M13, 3 => m.M14, 4 => m.M15, 5 => m.M16, 6 => m.M17, 7 => m.M18, _ => 0 },
                1 => col switch { 0 => m.M21, 1 => m.M22, 2 => m.M23, 3 => m.M24, 4 => m.M25, 5 => m.M26, 6 => m.M27, 7 => m.M28, _ => 0 },
                2 => col switch { 0 => m.M31, 1 => m.M32, 2 => m.M33, 3 => m.M34, 4 => m.M35, 5 => m.M36, 6 => m.M37, 7 => m.M38, _ => 0 },
                3 => col switch { 0 => m.M41, 1 => m.M42, 2 => m.M43, 3 => m.M44, 4 => m.M45, 5 => m.M46, 6 => m.M47, 7 => m.M48, _ => 0 },
                _ => 0
            };
        }

        /// <summary>
        /// 设置矩阵元素（辅助方法）
        /// </summary>
        public static void SetElement(ref Matrix4x8 m, int row, int col, float value)
        {
            if (row < 0 || row >= 4 || col < 0 || col >= 8) return;

            switch (row)
            {
                case 0:
                    switch (col)
                    {
                        case 0: m.M11 = value; break;
                        case 1: m.M12 = value; break;
                        case 2: m.M13 = value; break;
                        case 3: m.M14 = value; break;
                        case 4: m.M15 = value; break;
                        case 5: m.M16 = value; break;
                        case 6: m.M17 = value; break;
                        case 7: m.M18 = value; break;
                    }
                    break;
                case 1:
                    switch (col)
                    {
                        case 0: m.M21 = value; break;
                        case 1: m.M22 = value; break;
                        case 2: m.M23 = value; break;
                        case 3: m.M24 = value; break;
                        case 4: m.M25 = value; break;
                        case 5: m.M26 = value; break;
                        case 6: m.M27 = value; break;
                        case 7: m.M28 = value; break;
                    }
                    break;
                case 2:
                    switch (col)
                    {
                        case 0: m.M31 = value; break;
                        case 1: m.M32 = value; break;
                        case 2: m.M33 = value; break;
                        case 3: m.M34 = value; break;
                        case 4: m.M35 = value; break;
                        case 5: m.M36 = value; break;
                        case 6: m.M37 = value; break;
                        case 7: m.M38 = value; break;
                    }
                    break;
                case 3:
                    switch (col)
                    {
                        case 0: m.M41 = value; break;
                        case 1: m.M42 = value; break;
                        case 2: m.M43 = value; break;
                        case 3: m.M44 = value; break;
                        case 4: m.M45 = value; break;
                        case 5: m.M46 = value; break;
                        case 6: m.M47 = value; break;
                        case 7: m.M48 = value; break;
                    }
                    break;
            }
        }

        /// <summary>
        /// 转置为8x4矩阵
        /// </summary>
        public Matrix8x4 Transpose()
        {
            Matrix8x4 res = new Matrix8x4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Matrix8x4.SetElement(ref res, j, i, GetElement(this, i, j));
                }
            }
            return res;
        }
    }

    /// <summary>
    /// 8x4矩阵（卡尔曼增益专用）
    /// </summary>
    public struct Matrix8x4
    {
        public float M11, M12, M13, M14;
        public float M21, M22, M23, M24;
        public float M31, M32, M33, M34;
        public float M41, M42, M43, M44;
        public float M51, M52, M53, M54;
        public float M61, M62, M63, M64;
        public float M71, M72, M73, M74;
        public float M81, M82, M83, M84;

        /// <summary>
        /// 获取矩阵元素（辅助方法）
        /// </summary>
        public static float GetElement(Matrix8x4 m, int row, int col)
        {
            if (row < 0 || row >= 8 || col < 0 || col >= 4) return 0;

            return row switch
            {
                0 => col switch { 0 => m.M11, 1 => m.M12, 2 => m.M13, 3 => m.M14, _ => 0 },
                1 => col switch { 0 => m.M21, 1 => m.M22, 2 => m.M23, 3 => m.M24, _ => 0 },
                2 => col switch { 0 => m.M31, 1 => m.M32, 2 => m.M33, 3 => m.M34, _ => 0 },
                3 => col switch { 0 => m.M41, 1 => m.M42, 2 => m.M43, 3 => m.M44, _ => 0 },
                4 => col switch { 0 => m.M51, 1 => m.M52, 2 => m.M53, 3 => m.M54, _ => 0 },
                5 => col switch { 0 => m.M61, 1 => m.M62, 2 => m.M63, 3 => m.M64, _ => 0 },
                6 => col switch { 0 => m.M71, 1 => m.M72, 2 => m.M73, 3 => m.M74, _ => 0 },
                7 => col switch { 0 => m.M81, 1 => m.M82, 2 => m.M83, 3 => m.M84, _ => 0 },
                _ => 0
            };
        }

        /// <summary>
        /// 设置矩阵元素（辅助方法）
        /// </summary>
        public static void SetElement(ref Matrix8x4 m, int row, int col, float value)
        {
            if (row < 0 || row >= 8 || col < 0 || col >= 4) return;

            switch (row)
            {
                case 0:
                    switch (col)
                    {
                        case 0: m.M11 = value; break;
                        case 1: m.M12 = value; break;
                        case 2: m.M13 = value; break;
                        case 3: m.M14 = value; break;
                    }
                    break;
                case 1:
                    switch (col)
                    {
                        case 0: m.M21 = value; break;
                        case 1: m.M22 = value; break;
                        case 2: m.M23 = value; break;
                        case 3: m.M24 = value; break;
                    }
                    break;
                case 2:
                    switch (col)
                    {
                        case 0: m.M31 = value; break;
                        case 1: m.M32 = value; break;
                        case 2: m.M33 = value; break;
                        case 3: m.M34 = value; break;
                    }
                    break;
                case 3:
                    switch (col)
                    {
                        case 0: m.M41 = value; break;
                        case 1: m.M42 = value; break;
                        case 2: m.M43 = value; break;
                        case 3: m.M44 = value; break;
                    }
                    break;
                case 4:
                    switch (col)
                    {
                        case 0: m.M51 = value; break;
                        case 1: m.M52 = value; break;
                        case 2: m.M53 = value; break;
                        case 3: m.M54 = value; break;
                    }
                    break;
                case 5:
                    switch (col)
                    {
                        case 0: m.M61 = value; break;
                        case 1: m.M62 = value; break;
                        case 2: m.M63 = value; break;
                        case 3: m.M64 = value; break;
                    }
                    break;
                case 6:
                    switch (col)
                    {
                        case 0: m.M71 = value; break;
                        case 1: m.M72 = value; break;
                        case 2: m.M73 = value; break;
                        case 3: m.M74 = value; break;
                    }
                    break;
                case 7:
                    switch (col)
                    {
                        case 0: m.M81 = value; break;
                        case 1: m.M82 = value; break;
                        case 2: m.M83 = value; break;
                        case 3: m.M84 = value; break;
                    }
                    break;
            }
        }
    }
    #endregion

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
        public int ReIDFeatureDim { get; set; } = 512; // OSNet默认512维特征

        // 轨迹管理
        public int TrackBuffer { get; set; } = 30;
        public int MaxLostFrames { get; set; } = 30;

        // GMC配置
        public bool EnableGMC { get; set; } = true;
        public int GMCFeaturePoints { get; set; } = 500;
        public float GMCInlierThreshold { get; set; } = 5.0f;
    }
    #endregion


  

    #region 卡尔曼滤波类
    /// <summary>
    /// 8维卡尔曼滤波（BoT-SORT标准，无MathNet依赖）
    /// 状态向量：[x, y, a, h, vx, vy, va, vh]
    /// x,y: 中心坐标 | a: 宽高比 | h: 高度 | v: 对应速度
    /// </summary>
    public class BoTKalmanFilter
    {
        private readonly BoTSORTConfig _config;

        // 状态向量 (8x1)
        private float[] _x;
        // 协方差矩阵 (8x8)
        private Matrix8x8 _P;
        // 状态转移矩阵 (8x8)
        private Matrix8x8 _F;
        // 观测矩阵 (4x8)：仅观测x,y,a,h
        private Matrix4x8 _H;
        // 过程噪声协方差 (8x8)
        private Matrix8x8 _Q;
        // 观测噪声协方差 (4x4，用二维数组表示)
        private float[][] _R;

        private const float DT = 1.0f / 10.0f; // 帧间隔

        public BoTKalmanFilter(BoTSORTConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            InitializeMatrices();
        }

        /// <summary>
        /// 初始化卡尔曼滤波矩阵
        /// </summary>
        private void InitializeMatrices()
        {
            // 初始状态向量（全0）
            _x = new float[8];

            // 初始协方差矩阵（对角为1000）
            _P = Matrix8x8.Identity * 1000;

            // 状态转移矩阵F
            _F = Matrix8x8.Identity;
            Matrix8x8.SetElement(ref _F, 0, 4, DT);
            Matrix8x8.SetElement(ref _F, 1, 5, DT);
            Matrix8x8.SetElement(ref _F, 2, 6, DT);
            Matrix8x8.SetElement(ref _F, 3, 7, DT);

            // 观测矩阵H
            _H = Matrix4x8.Identity;

            // 过程噪声Q
            _Q = Matrix8x8.Identity * 0.01f;

            // 观测噪声R（4x4对角矩阵）
            _R = new float[4][];
            for (int i = 0; i < 4; i++)
            {
                _R[i] = new float[4];
                _R[i][i] = 0.1f;
            }
        }

        /// <summary>
        /// 初始化状态（从检测框）
        /// </summary>
        /// <param name="detection">检测框</param>
        public void Initialize(BoxItem detection)
        {
            if (detection == null) throw new ArgumentNullException(nameof(detection));
            var tcenter = detection.GetCenter();
            _x[0] = tcenter.X;       // x
            _x[1] = tcenter.Y;       // y
            _x[2] = detection.AspectRatio();    // a（宽高比）
            _x[3] = detection.Height();         // h（高度）
            _x[4] = 0; // vx
            _x[5] = 0; // vy
            _x[6] = 0; // va
            _x[7] = 0; // vh
        }

        /// <summary>
        /// 预测下一状态
        /// </summary>
        public void Predict()
        {
            // x = F * x
            _x = _F * _x;

            // P = F * P * F^T + Q
            var fTranspose = _F.Transpose();
            _P = _F * _P * fTranspose + _Q;
        }

        /// <summary>
        /// 更新状态（基于观测值）
        /// </summary>
        /// <param name="detection">检测框</param>
        public void Update(BoxItem detection)
        {
            if (detection == null) throw new ArgumentNullException(nameof(detection));
            var tcenter = detection.GetCenter();
            // 构建观测向量 [x, y, a, h]
            float[] z = new[]
            {
                tcenter.X,
                tcenter.Y,
                detection.AspectRatio(),
                detection.Height()
            };

            // 计算残差 y = z - H*x
            float[] hx = _H * _x;
            float[] y = new float[4];
            for (int i = 0; i < 4; i++)
            {
                y[i] = z[i] - hx[i];
            }

            // 创新协方差 S = H*P*H^T + R
            var hp = _H * _P;
            var hTranspose = _H.Transpose(); // hTranspose 是 Matrix8x4 类型（8行4列）

            // 计算 H*P*H^T
            float[][] s = new float[4][];
            for (int i = 0; i < 4; i++)
            {
                s[i] = new float[4];
                for (int j = 0; j < 4; j++)
                {
                    float sum = 0;
                    for (int x = 0; x < 8; x++)
                    {
                        // 修复：hp是Matrix4x8（4行8列），hTranspose是Matrix8x4（8行4列）
                        // 原错误：Matrix4x8.GetElement(hTranspose, x, j)
                        // 正确写法：Matrix8x4.GetElement(hTranspose, x, j)
                        sum += Matrix4x8.GetElement(hp, i, x) * Matrix8x4.GetElement(hTranspose, x, j);
                    }
                    s[i][j] = sum + _R[i][j];
                }
            }

            // 卡尔曼增益 K = P * H^T * S^-1
            var pHT = _P * hTranspose;
            float[][] invS = Matrix8x8.Invert4x4(s);

            // 计算卡尔曼增益 K (8x4)
            Matrix8x4 k = new Matrix8x4();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    float sum = 0;
                    for (int kIdx = 0; kIdx < 4; kIdx++)
                    {
                        sum += Matrix8x4.GetElement(pHT, i, kIdx) * invS[kIdx][j];
                    }
                    Matrix8x4.SetElement(ref k, i, j, sum);
                }
            }

            // 更新状态 x = x + K*y
            float[] ky = new float[8];
            for (int i = 0; i < 8; i++)
            {
                float sum = 0;
                for (int j = 0; j < 4; j++)
                {
                    sum += Matrix8x4.GetElement(k, i, j) * y[j];
                }
                ky[i] = sum;
            }

            for (int i = 0; i < 8; i++)
            {
                _x[i] += ky[i];
            }

            // 更新协方差 P = (I - K*H) * P
            var kh = k * _H;
            var ikh = Matrix8x8.Identity - kh;
            _P = ikh * _P;
        }

        /// <summary>
        /// 获取预测的检测框参数
        /// </summary>
        /// <returns>[x, y, a, h]</returns>
        public float[] GetPredictedState()
        {
            return new[] { _x[0], _x[1], _x[2], _x[3] };
        }
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
        public BoTKalmanFilter Kf { get; }
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
            Kf = new BoTKalmanFilter(config);
            Kf.Initialize(detection);

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

        /// <summary>
        /// 获取运动补偿后的预测框
        /// </summary>
        /// <returns>预测检测框</returns>
        public BoxItem GetCompensatedPrediction(Matrix3x3? homography = null)
        {
            var pred = Kf.GetPredictedState();
            float x = pred[0], y = pred[1], a = pred[2], h = pred[3];

            // 应用GMC运动补偿：使用单应性矩阵的逆矩阵
            if (homography.HasValue && Config.EnableGMC)
            {
                // 单应性矩阵变换：需要用逆矩阵补偿相机运动
                var invH = homography.Value.Invert();
                float[] point = new[] { x, y, 1.0f };
                float[] transformed = invH * point;

                // 齐次坐标归一化
                if (Math.Abs(transformed[2]) > 1e-6)
                {
                    x = transformed[0] / transformed[2];
                    y = transformed[1] / transformed[2];
                }
            }

            // 计算预测框坐标
            float w = a * h;

            return new BoxItem
            {
                x1 = x - w / 2,
                y1 = y - h / 2,
                x2 = x + w / 2,
                y2 = y + h / 2,
                ReID = LastReIDFeature
            };
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

        /// <summary>
        /// 估计相机运动（单应性矩阵）
        /// </summary>
        /// <param name="prevFrame">上一帧</param>
        /// <param name="currFrame">当前帧</param>
        /// <returns>单应性矩阵（单位矩阵表示估计失败）</returns>
        public Matrix3x3 EstimateHomography(Image<Rgb24> prevFrame, Image<Rgb24> currFrame)
        {
            if (!_config.EnableGMC || prevFrame == null || currFrame == null)
                return Matrix3x3.Identity;

            try
            {
                // 1. 提取ORB特征点（实际需对接OpenCVSharp）
                var (prevPoints, currPoints) = ExtractORBFeatures(prevFrame, currFrame);
                if (prevPoints.Count < 10 || currPoints.Count < 10)
                    return Matrix3x3.Identity;

                // 2. 计算单应性矩阵（RANSAC）
                return ComputeHomographyRANSAC(prevPoints, currPoints);
            }
            catch
            {
                return Matrix3x3.Identity;
            }
        }

        /// <summary>
        /// 提取ORB特征点（对接OpenCVSharp实现）
        /// </summary>
        private (List<Vector2>, List<Vector2>) ExtractORBFeatures(Image<Rgb24> prev, Image<Rgb24> curr)
        {
            // 占位实现：实际需对接OpenCV
            return (new List<Vector2>(), new List<Vector2>());
        }

        /// <summary>
        /// RANSAC计算单应性矩阵
        /// </summary>
        private Matrix3x3 ComputeHomographyRANSAC(List<Vector2> srcPoints, List<Vector2> dstPoints)
        {
            // 占位实现：实际需实现RANSAC算法
            return Matrix3x3.Identity;
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

            // 1. 预处理检测框：提取ReID特征
            var processedDetections = ProcessDetections(currFrame, detections);

            // 2. 估计相机运动（GMC）
            var homography = _gmc.EstimateHomography(_prevFrame, currFrame);

            // 3. 预测所有轨迹
            foreach (var track in _tracks.ToList()) // 用ToList避免迭代时修改
            {
                track.Predict();
            }

            // 4. 拆分高低置信度检测框
            var highConfDets = processedDetections
                .Where(d => d.score >= _config.TrackThresh)
                .ToList();

            var lowConfDets = processedDetections
                .Where(d => d.score >= _config.TrackLowThresh && d.score < _config.TrackThresh)
                .ToList();

            // 5. 第一阶段匹配：高置信度框 + 活跃轨迹
            var (matchedTracks1, unmatchedDets1) = FusedMatching(highConfDets,
                _tracks.Where(t => t.IsActive).ToList(), homography, false);

            // 6. 第二阶段匹配：低置信度框 + 未匹配轨迹
            var unmatchedTracks = _tracks.Where(t => !matchedTracks1.Contains(t)).ToList();
            var (matchedTracks2, _) = FusedMatching(lowConfDets, unmatchedTracks, homography, true);

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
            FusedMatching(List<BoxItem> detections, List<BoTTrack> tracks, Matrix3x3 homography, bool isLowConf)
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

                    // 1. 计算IoU（运动补偿后）
                    var predBbox = track.GetCompensatedPrediction(homography);
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
                    .OrderByDescending(d => CalculateIoU(d, track.GetCompensatedPrediction()))
                    .FirstOrDefault(d => CalculateIoU(d, track.GetCompensatedPrediction()) > _config.MatchThresh * 0.5);

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
                    TrackThresh = 0.5f,
                    ReIDFeatureDim = 512,
                    EnableGMC = false
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