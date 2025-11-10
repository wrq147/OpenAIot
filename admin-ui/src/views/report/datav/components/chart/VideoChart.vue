<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="text">
    <video-player class="video-player vjs-custom-skin" ref="videoPlayer" :playsinline="true" :options="playerOptions">
    </video-player>
  </div>
</template>

<script>
import '../../animate/animate.css'
import { videoPlayer } from 'vue-video-player';
import 'video.js/dist/video-js.css'
import 'vue-video-player/src/custom-theme.css'
import dataChart from '../mixins/dataChart.js'

export default {
  mixins: [dataChart],
  components: {
    videoPlayer
  },
  props: {
    className: {
      type: String,
      default: "chart",
    },
    width: {
      type: String,
      default: "100%",
    },
    height: {
      type: String,
      default: "100%",
    },
  },
  data() {
    return {
      controls: this.chartOption.controls,
      loop: this.chartOption.replay,
      autoplay: this.chartOption.auto,
      videoUrl: "",
      animate: this.className
    };
  },
  computed: {
    playerOptions() {
      let tmpOption = {
        playbackRates: [0.5, 1.0, 1.5, 2.0], // 可选的播放速度
        autoplay: this.autoplay, // 如果为true,浏览器准备好时开始回放。
        muted: false, // 默认情况下将会消除任何音频。
        loop: this.loop, // 是否视频一结束就重新开始。
        preload: "auto", // 建议浏览器在<video>加载元素后是否应该开始下载视频数据。auto浏览器选择最佳行为,立即开始加载视频（如果浏览器支持）
        language: "zh-CN",
        aspectRatio: "16:9", // 将播放器置于流畅模式，并在计算播放器的动态大小时使用该值。值应该代表一个比例 - 用冒号分隔的两个数字（例如"16:9"或"4:3"）
        fluid: true, // 当true时，Video.js player将拥有流体大小。换句话说，它将按比例缩放以适应其容器。
        sources: [
          {
            type: "video/mp4", // 类型
            src: this.videoUrl // url地址
          }
        ],
        poster: "", // 封面地址
        notSupportedMessage: "此视频暂无法播放，请稍后再试", // 允许覆盖Video.js无法播放媒体源时显示的默认信息。
        controlBar: {
          timeDivider: true, // 当前时间和持续时间的分隔符
          durationDisplay: true, // 显示持续时间
          remainingTimeDisplay: false, // 是否显示剩余时间功能
          fullscreenToggle: true // 是否显示全屏按钮
        },
        controls: this.controls
      };
      return tmpOption;
    }
  },
  watch: {
    width() {

    },
    height() {

    },
    className: {
      handler(value) {
        this.animate = value;
      }
    }
  },
  mounted() {
    this.valUpdate = this.setChartVal;
    this.videoUrl = this.chartOption.staticDataValue + "?rnd=" + new Date();
  },
  methods: {
    setChartVal(result) {
      this.controls = this.dataOption.controls;
      this.loop = this.dataOption.replay;
      this.autoplay = this.dataOption.auto;
      this.videoUrl =typeof result === "string" ? result + "?rnd=" + new Date() : result.String() + "?rnd=" + new Date();
    },
  }
};
</script>
