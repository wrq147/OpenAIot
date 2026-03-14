#include "convert.h"

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
            const uint8_t b = *src++;
            const uint8_t g = *src++;
            const uint8_t r = *src++;
            
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
            const uint8_t b = *src++;
            const uint8_t g = *src++;
            const uint8_t r = *src++;
            
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
            const uint8_t b = *src++;
            const uint8_t g = *src++;
            const uint8_t r = *src++;
            
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
            const uint8_t b = *src++;
            const uint8_t g = *src++;
            const uint8_t r = *src++;
            
            int Y, U, V;
            CALC_YUV(r, g, b, Y, U, V);
            *dst_y++ = clamp_0_255(Y);
            *dst_u++ = clamp_0_255(U);
            *dst_v++ = clamp_0_255(V);
        }
    }
}