<template>
  <div :class="className" :id="customId">

     <video :id="'player'+customId" style=" height:100%; width: 100%" ref='video' align="center" :controls="true"  :loop="false"> <source :src="this.src" type="video/mp4"> </video> 
  </div>
</template>

<script>
import echarts from "echarts";
require("echarts/theme/macarons"); // echarts theme
import videojs from 'video.js'
import 'video.js/dist/video-js.css'

import 'videojs-flash'
import {parseQueryString, fIsUrL} from '../../util/urlUtil'


const animationDuration = 6000;

export default {
  components: {
   
  },
  props: {
    className: {
      type: String,
      default: "chart",
    },
    customId:{
      type:String
    },
    videoSrc:{
      type: String
    }
  },
  data() {
    return {
      
      src : ''
      
    };
  },
  watch: {
   
  },
  mounted() {
    this.$nextTick(() => {
      this.initChart();
    });
  },
  beforeDestroy() {

  },
  methods: {
    initChart() {

      this.src = this.videoSrc
    }
  }
};
</script>
