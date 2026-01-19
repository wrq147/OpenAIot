<template>
  <div :class="className" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="text">
    <div id="rtPlayer"></div>
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
      animate: this.className,
      isFlv: false,
      tmpplayer: null
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
    if (FlvPlugin.isSupported()) {
      this.isFlv = true;
      this.tmpplayer = new Player({
        id: 'rtPlayer',
        isLive: true,
        plugins: [FlvPlugin]
      })
    }
    else {
      this.isFlv = false;
      this.tmpplayer = new Player({
        id: 'rtPlayer',
        isLive: true
      })
    }
    this.tmpplayer.url = this.chartOption.staticDataValue.src + "?rnd=" + new Date();
    this.tmpplayer.play()
    this.valUpdate = this.setChartVal;
  },
  methods: {
    setChartVal(result) {
      let resultData = result.length === 0 ? [{ type: '', src: '' }] : result
      this.tmpplayer.url = resultData.src + "?rnd=" + new Date();
      this.tmpplayer.play()
    },
  }
};
</script>
