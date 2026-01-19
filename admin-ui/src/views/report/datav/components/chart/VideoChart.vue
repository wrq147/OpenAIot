<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="text">
    <div id="rtccPlayer"></div>
  </div>
</template>

<script>
import '../../animate/animate.css'
import Player from 'xgplayer'
import FlvPlugin from 'xgplayer-flv'
import "xgplayer/dist/index.min.css"
import dataChart from '../mixins/dataChart.js'

export default {
  mixins: [dataChart],
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
      isFlv: false,
      tmpplayer: null,
      animate: this.className
    };
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
    if (FlvPlugin.isSupported()) {
      this.isFlv = true;
      this.tmpplayer = new Player({
        id: 'rtccPlayer',
        isLive: true,
        plugins: [FlvPlugin]
      })
    }
    else {
      this.isFlv = false;
      this.tmpplayer = new Player({
        id: 'rtccPlayer',
        isLive: true
      })
    }
    this.tmpplayer.loop = this.chartOption.replay;
    this.tmpplayer.autoplay = this.chartOption.auto;
    this.tmpplayer.url = this.chartOption.staticDataValue + "?rnd=" + new Date();
    this.tmpplayer.play()
  },
  methods: {
    setChartVal(result) {
      this.tmpplayer.loop = this.dataOption.replay;
      this.tmpplayer.autoplay = this.dataOption.auto;
      this.tmpplayer.url = typeof result === "string" ? result + "?rnd=" + new Date() : result.String() + "?rnd=" + new Date();
      this.tmpplayer.play()
    },
  }
};
</script>
