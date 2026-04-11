#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#ifdef _WIN32
#include <windows.h>
#define API __declspec(dllexport)
#else
#define API __attribute__((visibility("default")))
#endif

#include <libavcodec/avcodec.h>
#include <libavutil/avutil.h>
#include <libavutil/imgutils.h>
#include <libavutil/error.h>

typedef struct {
	AVCodecContext* ctx;
	AVFrame* frame;
	AVPacket* pkt;
	int width;
	int height;
	int fps;
} EncoderHandle;

// 创建硬编码器
API void* ff_h264_encoder_create(int width, int height, int fps, int bitrate) {
	EncoderHandle* h = (EncoderHandle*)malloc(sizeof(EncoderHandle));
	memset(h, 0, sizeof(EncoderHandle));

	h->width = width;
	h->height = height;
	h->fps = fps;

	// 优先硬编码器
	AVCodec* codec = NULL;
	codec = avcodec_find_encoder_by_name("h264_nvenc");
	if (!codec) codec = avcodec_find_encoder_by_name("h264_qsv");
	if (!codec) codec = avcodec_find_encoder_by_name("h264_amf");
	if (!codec) codec = avcodec_find_encoder_by_name("h264_vaapi");
	// 树莓派
	if (!codec) codec = avcodec_find_encoder_by_name("h264_omx");
	if (!codec) codec = avcodec_find_encoder_by_name("h264_mmal");
	// 瑞芯微 RK
	if (!codec) codec = avcodec_find_encoder_by_name("h264_rkmpp");
	// 海思
	if (!codec) codec = avcodec_find_encoder_by_name("h264_mpi");
	// macOS
	if (!codec) codec = avcodec_find_encoder_by_name("h264_videotoolbox");

	if (!codec) {
		free(h);
		return NULL;
	}

	printf("[FF H264] Use encoder: %s\n", codec->name);

	AVCodecContext* ctx = avcodec_alloc_context3(codec);
	ctx->codec_type = AVMEDIA_TYPE_VIDEO;
	ctx->width = width;
	ctx->height = height;
	ctx->time_base = (AVRational){ 1, fps };
	ctx->framerate = (AVRational){ fps, 1 };
	ctx->bit_rate = bitrate;
	ctx->gop_size = fps;
	ctx->max_b_frames = 0;

	av_opt_set(ctx->priv_data, "tune", "zerolatency", 0);
	av_opt_set(ctx->priv_data, "preset", "fast", 0);

	if (avcodec_open2(ctx, codec, NULL) < 0) {
		avcodec_free_context(&ctx);
		free(h);
		return NULL;
	}

	AVFrame* frame = av_frame_alloc();
	frame->width = width;
	frame->height = height;
	av_frame_get_buffer(frame, 32);

	h->ctx = ctx;
	h->frame = frame;
	h->pkt = av_packet_alloc();

	return h;
}

// 编码一帧 YUV420p
API int ff_h264_encode_frame(void* handle, const char* yuv[3], int linesize[3], int pix_fmt, int64_t pts, uint8_t** out_data, int* out_len) {
	EncoderHandle* h = (EncoderHandle*)handle;
	h->ctx->pix_fmt = pix_fmt;
	h->frame->format = pix_fmt;
	*out_data = NULL;
	*out_len = 0;

	av_image_copy(
		h->frame->data,
		h->frame->linesize,
		(const uint8_t**)yuv,
		linesize,
		h->ctx->pix_fmt,
		h->width,
		h->height
	);

	h->frame->pts = pts;

	int ret = avcodec_send_frame(h->ctx, h->frame);
	if (ret < 0 && ret != AVERROR(EAGAIN) && ret != AVERROR_EOF)
		return 0;

	int total = 0;
	uint8_t* buf = NULL;

	while (1) {
		av_packet_unref(h->pkt);
		ret = avcodec_receive_packet(h->ctx, h->pkt);
		if (ret == AVERROR(EAGAIN) || ret == AVERROR_EOF) {
			break;
		}
		else if (ret < 0) {
			if (buf != NULL) {
				free(buf);
			}
			return 0;
		}

		uint8_t* new_buf = (uint8_t*)realloc(buf, total + h->pkt->size);
		if (!new_buf) {
			free(buf);
			return 0;
		}
		buf = new_buf;
		memcpy(buf + total, h->pkt->data, h->pkt->size);
		total += h->pkt->size;
	}

	*out_data = buf;
	*out_len = total;
	return total;
}

// 释放编码后数据
API void ff_h264_free(uint8_t* data) {
	if (data) free(data);
}

// 销毁编码器
API void ff_h264_encoder_destroy(void* handle) {
	EncoderHandle* h = (EncoderHandle*)handle;
	if (!h) return;

	avcodec_send_frame(h->ctx, NULL);
	while (avcodec_receive_packet(h->ctx, h->pkt) == 0)
		av_packet_unref(h->pkt);

	av_frame_free(&h->frame);
	av_packet_free(&h->pkt);
	avcodec_free_context(&h->ctx);
	free(h);
}