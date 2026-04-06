#include "convert.h"
#include <stdint.h>
#include <stdio.h>
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


#define CHAR_SCALE 4

#define FMT_YUV420P      0
#define FMT_YUVJ420P    12
#define FMT_YUV422P     4
#define FMT_YUV444P     5
#define FMT_NV12       23

// ==============================
// HZK16 全局句柄（只打开一次）
// ==============================
static FILE* g_hzk_fp = NULL;

// 懒加载：第一次调用自动打开
static int hzk16_get(byte q, byte w, byte out[32]) {
    // 第一次打开，只执行一次
    if (g_hzk_fp == NULL) {
        g_hzk_fp = fopen("HZK16", "rb");
    }
    if (!g_hzk_fp) return -1;

    // 直接读取
    long offset = ((q - 0xA1L) * 94 + (w - 0xA1)) * 32L;
    fseek(g_hzk_fp, offset, SEEK_SET);
    return (fread(out, 1, 32, g_hzk_fp) == 32) ? 0 : -1;
}

static void rgb2yuv(byte r, byte g, byte b, byte* Y, byte* U, byte* V) {
    int y = (66 * r + 129 * g + 25 * b + 128) / 256 + 16;
    int u = (-38 * r - 74 * g + 112 * b + 128) / 256 + 128;
    int v = (112 * r - 94 * g - 18 * b + 128) / 256 + 128;

    *Y = (y < 16) ? 16 : (y > 235) ? 235 : (byte)y;
    *U = (u < 16) ? 16 : (u > 240) ? 240 : (byte)u;
    *V = (v < 16) ? 16 : (v > 240) ? 240 : (byte)v;
}

static void utf8_to_gb(const char* utf8, byte* gb) {
    const byte* p = (const byte*)utf8;
    while (*p) {
        if ((*p & 0xF0) == 0xE0) {
            uint32_t uc = ((p[0] & 0x0F) << 12) | ((p[1] & 0x3F) << 6) | (p[2] & 0x3F);
            if (uc >= 0x4E00 && uc <= 0x9FA5) {
                uint16_t code = uc - 0x4E00 + 0xA1A1;
                *gb++ = (code >> 8) & 0xFF;
                *gb++ = code & 0xFF;
            }
            p += 3;
        }
        else {
            *gb++ = *p++;
        }
    }
    *gb = 0;
}

static void draw_pt(byte* y, byte* u, byte* v, int ys, int us, int w, int h, int fmt,
    int x, int py, byte Y, byte U, byte V) {
    if (x < 0 || py < 0 || x >= w || py >= h) return;
    y[py * ys + x] = Y;

    if (fmt == FMT_YUV420P || fmt == FMT_YUVJ420P || fmt == FMT_NV12) {
        int ux = x / 2;
        int uy = py / 2;
        if (fmt == FMT_NV12) {
            u[uy * us + ux * 2] = U;
            u[uy * us + ux * 2 + 1] = V;
        }
        else {
            u[uy * us + ux] = U;
            v[uy * us + ux] = V;
        }
    }
    else if (fmt == FMT_YUV422P) {
        u[py * us + x / 2] = U;
        v[py * us + x / 2] = V;
    }
    else if (fmt == FMT_YUV444P) {
        u[py * us + x] = U;
        v[py * us + x] = V;
    }
}

static void draw_rect(byte* y, byte* u, byte* v, int ys, int us, int w, int h, int fmt,
    int x1, int y1, int x2, int y2, byte Y, byte U, byte V) {
    int i;
    for (i = x1; i <= x2; i++) draw_pt(y, u, v, ys, us, w, h, fmt, i, y1, Y, U, V);
    for (i = x1; i <= x2; i++) draw_pt(y, u, v, ys, us, w, h, fmt, i, y2, Y, U, V);
    for (i = y1; i <= y2; i++) draw_pt(y, u, v, ys, us, w, h, fmt, x1, i, Y, U, V);
    for (i = y1; i <= y2; i++) draw_pt(y, u, v, ys, us, w, h, fmt, x2, i, Y, U, V);
}

static void draw_fill(byte* y, byte* u, byte* v, int ys, int us, int w, int h, int fmt,
    int x1, int y1, int x2, int y2, byte Y, byte U, byte V) {
    int yy, x;
    for (yy = y1; yy <= y2; yy++)
        for (x = x1; x <= x2; x++)
            draw_pt(y, u, v, ys, us, w, h, fmt, x, yy, Y, U, V);
}

static void draw_cn(byte* y, int ys, int x, int y_pos, byte q, byte w, byte Y) {
    if (y == NULL || ys <= 0) return;

    byte buf[32];
    if (hzk16_get(q, w, buf) != 0) return;

    // 逐行绘制 16x16 汉字（放大版）
    for (int row = 0; row < 16; row++) {
        byte b1 = buf[row * 2];
        byte b2 = buf[row * 2 + 1];

        // 左边 8 点
        for (int i = 0; i < 8; i++) {
            if (b1 & (0x80 >> i)) {
                // 放大绘制像素块
                for (int dy = 0; dy < CHAR_SCALE; dy++) {
                    for (int dx = 0; dx < CHAR_SCALE; dx++) {
                        int px = x + i * CHAR_SCALE + dx;
                        int py = y_pos + row * CHAR_SCALE + dy;
                        if (px >= 0 && px < ys && py >= 0 && py < ys * 16)
                            y[py * ys + px] = Y;
                    }
                }
            }
        }

        // 右边 8 点
        for (int i = 0; i < 8; i++) {
            if (b2 & (0x80 >> i)) {
                for (int dy = 0; dy < CHAR_SCALE; dy++) {
                    for (int dx = 0; dx < CHAR_SCALE; dx++) {
                        int px = x + (8 + i) * CHAR_SCALE + dx;
                        int py = y_pos + row * CHAR_SCALE + dy;
                        if (px >= 0 && px < ys && py >= 0 && py < ys * 16)
                            y[py * ys + px] = Y;
                    }
                }
            }
        }
    }
}

static void draw_text(byte* y, int ys, int x, int y_pos, const char* label, byte Y) {
    byte gb[256] = { 0 };
    utf8_to_gb(label, gb);
    int ox = x;
    for (int i = 0; gb[i] && gb[i + 1]; i += 2) {
        draw_cn(y, ys, ox, y_pos, gb[i], gb[i + 1], Y);
        ox += 16 * CHAR_SCALE;  // 字间距也放大
    }
}

//============================================================================
//======================== 在yuv上画检测框 ===============================
//============================================================================
void yuv_render(byte** data, int* yuvLineSizes, int w, int h, int pix_fmt,
    int x1, int y1, int x2, int y2,
    byte r, byte g, byte b, const char* label)
{
    byte* y = data[0];
    byte* u = data[1];
    byte* v = data[2];
    int ys = yuvLineSizes[0];
    int us = yuvLineSizes[1];

    byte Y, U, V;
    rgb2yuv(r, g, b, &Y, &U, &V);

    // 画目标框
    draw_rect(y, u, v, ys, us, w, h, pix_fmt, x1, y1, x2, y2, Y, U, V);

    if (!label || !*label) return;

    // 标签背景与文字
    int tx = x1 + 2;
    int ty = y1 - 22;
    if (ty < 0) ty = 0;

    int bh = 20 * CHAR_SCALE;
    int label_len = (int)strlen(label);
    int bw = 16 * CHAR_SCALE * label_len + 8 * CHAR_SCALE;

    draw_fill(y, u, v, ys, us, w, h, pix_fmt, tx, ty, tx + bw, ty + bh, Y, U, V);
    draw_rect(y, u, v, ys, us, w, h, pix_fmt, tx, ty, tx + bw, ty + bh, Y, U, V);
    draw_text(y, ys, tx + 4, ty + 3, label, 235);
}