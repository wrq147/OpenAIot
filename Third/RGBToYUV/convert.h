#ifndef CONVERT_H
#define CONVERT_H

#include <stdint.h>

#if defined(_WIN32)
#   define API __declspec(dllexport)
#else
#   define API __attribute__((visibility("default")))
#endif

#ifdef __cplusplus
extern "C" {
#endif

// RGB24转NV12（最常用）
API void rgb24_to_nv12(
    const uint8_t* rgb24,
    uint8_t* nv12,
    int width,
    int height,
    int rgb_stride
);

// RGB24转YUV420P
API void rgb24_to_yuv420p(
    const uint8_t* rgb24,
    uint8_t* yuv420p,
    int width,
    int height,
    int rgb_stride
);

// RGB24转YUV422P
API void rgb24_to_yuv422p(
    const uint8_t* rgb24,
    uint8_t* yuv422p,
    int width,
    int height,
    int rgb_stride
);

// RGB24转YUV444P
API void rgb24_to_yuv444p(
    const uint8_t* rgb24,
    uint8_t* yuv444p,
    int width,
    int height,
    int rgb_stride
);

#ifdef __cplusplus
}
#endif

#endif // CONVERT_H