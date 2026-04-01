#include "convert.h"
#include <stdint.h>
#include <stdlib.h>
#include <string.h>

// 快速钳位函数（内联，无分支优化）
static inline uint8_t clamp_0_255(int v) {
    if (v < 0) return 0;
    if (v > 255) return 255;
    return (uint8_t)v;
}

// RGB转YUV核心计算（宏定义减少重复代码）
#define CALC_YUV(r, g, b, Y, U, V) \
    Y = ((19595 * (r) + 38469 * (g) + 7471 * (b)) + 32768) >> 16; \
    U = ((-11068 * (r) - 21751 * (g) + 32819 * (b)) + 32768) >> 16; \
    V = ((32819 * (r) - 27437 * (g) - 5381 * (b)) + 32768) >> 16; \
    U += 128; \
    V += 128;

// RGB24转NV12
void rgb24_to_nv12(
    const uint8_t* rgb24,
    uint8_t* nv12,
    int width,
    int height,
    int rgb_stride
) {
    const int y_size = width * height;
    uint8_t* y = nv12;
    uint8_t* uv = nv12 + y_size;

    for (int j = 0; j < height; j++) {
        const uint8_t* src = rgb24 + j * rgb_stride;
        uint8_t* dst_y = y + j * width;
        const int even_line = (j & 1) == 0;
        int uv_pos = (j / 2) * width;

        for (int i = 0; i < width; i++) {
            const uint8_t r = *src++;
            const uint8_t g = *src++;
            const uint8_t b = *src++;
            
            int Y, U, V;
            CALC_YUV(r, g, b, Y, U, V);
            *dst_y++ = clamp_0_255(Y);

            if (even_line && (i & 1) == 0) {
                uv[uv_pos++] = clamp_0_255(U);
                uv[uv_pos++] = clamp_0_255(V);
            }
        }
    }
}

// RGB24转YUV420P
void rgb24_to_yuv420p(
    const uint8_t* rgb24,
    uint8_t* yuv420p,
    int width,
    int height,
    int rgb_stride
) {
    const int y_size = width * height;
    const int uv_size = y_size / 4;
    uint8_t* y = yuv420p;
    uint8_t* u = yuv420p + y_size;
    uint8_t* v = yuv420p + y_size + uv_size;

    for (int j = 0; j < height; j++) {
        const uint8_t* src = rgb24 + j * rgb_stride;
        uint8_t* dst_y = y + j * width;
        const int even_line = (j & 1) == 0;
        int uv_pos = (j / 2) * (width / 2);

        for (int i = 0; i < width; i++) {
            const uint8_t r = *src++;
            const uint8_t g = *src++;
            const uint8_t b = *src++;
            
            int Y, U, V;
            CALC_YUV(r, g, b, Y, U, V);
            *dst_y++ = clamp_0_255(Y);

            if (even_line && (i & 1) == 0) {
                u[uv_pos] = clamp_0_255(U);
                v[uv_pos] = clamp_0_255(V);
                uv_pos++;
            }
        }
    }
}

// RGB24转YUV422P
void rgb24_to_yuv422p(
    const uint8_t* rgb24,
    uint8_t* yuv422p,
    int width,
    int height,
    int rgb_stride
) {
    const int y_size = width * height;
    const int uv_size = y_size / 2;
    uint8_t* y = yuv422p;
    uint8_t* u = yuv422p + y_size;
    uint8_t* v = yuv422p + y_size + uv_size;

    for (int j = 0; j < height; j++) {
        const uint8_t* src = rgb24 + j * rgb_stride;
        uint8_t* dst_y = y + j * width;
        int uv_pos = j * (width / 2);

        for (int i = 0; i < width; i++) {
            const uint8_t r = *src++;
            const uint8_t g = *src++;
            const uint8_t b = *src++;
            
            int Y, U, V;
            CALC_YUV(r, g, b, Y, U, V);
            *dst_y++ = clamp_0_255(Y);

            if ((i & 1) == 0) {
                u[uv_pos] = clamp_0_255(U);
                v[uv_pos] = clamp_0_255(V);
                uv_pos++;
            }
        }
    }
}

// RGB24转YUV444P
void rgb24_to_yuv444p(
    const uint8_t* rgb24,
    uint8_t* yuv444p,
    int width,
    int height,
    int rgb_stride
) {
    const int y_size = width * height;
    uint8_t* y = yuv444p;
    uint8_t* u = yuv444p + y_size;
    uint8_t* v = yuv444p + y_size * 2;

    for (int j = 0; j < height; j++) {
        const uint8_t* src = rgb24 + j * rgb_stride;
        uint8_t* dst_y = y + j * width;
        uint8_t* dst_u = u + j * width;
        uint8_t* dst_v = v + j * width;

        for (int i = 0; i < width; i++) {
            const uint8_t r = *src++;
            const uint8_t g = *src++;
            const uint8_t b = *src++;
            
            int Y, U, V;
            CALC_YUV(r, g, b, Y, U, V);
            *dst_y++ = clamp_0_255(Y);
            *dst_u++ = clamp_0_255(U);
            *dst_v++ = clamp_0_255(V);
        }
    }
}



// ============================
// 快速计算亮度 Y
// ============================
static inline uint8_t fast_y(uint8_t r, uint8_t g, uint8_t b) {
    return (uint8_t)((r * 2 + g * 5 + b) >> 3);
}

// ============================
// 函数1：ExtractY
// RGB24 => 亮度通道 Y
// ============================
void ExtractY(
    const uint8_t* rgb24,
    uint8_t* out_y,
    int width,
    int height
) {
    int total = width * height;
    for (int i = 0; i < total; i++) {
        const uint8_t* p = rgb24 + i * 3;
        out_y[i] = fast_y(p[0], p[1], p[2]);
    }
}

// ============================
// 函数2：CheckBlockMotion
// 检测画面是否变化
// ============================
int CheckBlockMotion(
    const uint8_t* last_y,
    const uint8_t* curr_rgb24,
    int width,
    int height,
    int block_size,
    int diff_thresh,
    float ratio_thresh,
    int min_blocks,
    float* out_ratio
) {
    int cols = width / block_size;
    int rows = height / block_size;
    int total_blocks = cols * rows;
    int motion_blocks = 0;

    *out_ratio = 0.0f;

    for (int by = 0; by < rows; by++) {
        for (int bx = 0; bx < cols; bx++) {
            int sum = 0;
            int cnt = 0;

            // 降采样 1/4 提速
            for (int dy = 0; dy < block_size; dy += 2) {
                for (int dx = 0; dx < block_size; dx += 2) {
                    int x = bx * block_size + dx;
                    int y = by * block_size + dy;
                    if (x >= width || y >= height) continue;

                    int i = y * width + x;
                    const uint8_t* p = curr_rgb24 + i * 3;

                    uint8_t curr = fast_y(p[0], p[1], p[2]);
                    uint8_t last = last_y[i];

                    sum += abs((int)curr - last);
                    cnt++;
                }
            }

            if (cnt == 0) continue;
            if ((sum / cnt) > diff_thresh) {
                motion_blocks++;
            }
        }
    }

    float ratio = (float)motion_blocks / total_blocks;
    *out_ratio = ratio;
    return (ratio > ratio_thresh && motion_blocks >= min_blocks) ? 1 : 0;
}