#include <stdint.h>
#include <stdlib.h>
#include <string.h>

#if defined(_WIN32)
#define MOTION_API __declspec(dllexport)
#else
#define MOTION_API
#endif

// ============================
// 快速计算亮度 Y
// ============================
static uint8_t fast_y(uint8_t r, uint8_t g, uint8_t b) {
    return (uint8_t)((r * 2 + g * 5 + b) >> 3);
}

// ============================
// 函数1：ExtractY
// RGB24 => 亮度通道 Y
// ============================
MOTION_API void ExtractY(
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
MOTION_API int CheckBlockMotion(
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