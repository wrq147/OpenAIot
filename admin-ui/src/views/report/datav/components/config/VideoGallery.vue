<template>
  <div :class="className" :id="customId">

     <video :id="'player'+customId" style=" height:100%; width: 100%" ref='video' align="center" :controls="true"  :loop="false"> <source :src="this.src" type="video/mp4"> </video> 
  </div>
</template>

<script>
require("echarts/theme/macarons"); // echarts theme
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
