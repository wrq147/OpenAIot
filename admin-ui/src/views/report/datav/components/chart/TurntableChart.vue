<template>
  <div :class="className" :style="{ height: height, width: width }" :id="chartOption.bindingDiv">
    <dv-decoration-9 :key="key" :dur="dataOption.dur" :color="color" :style="{ height: height, width: width, opacity:opacity}"/>
  </div>
</template>

<script>

import "../../animate/animate.css";
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
      opacity: this.chartOption.opacity,
      key: 1
    };
  },
  watch: {
    width() {
      this.setChartVal();
    },
    height() {
      this.setChartVal();
    },
  },
  mounted() {
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
  },
  computed:{
    color(){
      let corlorArr = [];
      corlorArr.push(this.dataOption.color1);
      corlorArr.push(this.dataOption.color2);
      return corlorArr
    }
  },
  methods: {
    setChartVal(result) {
      let dataOption = JSON.parse(JSON.stringify(this.dataOption))
      this.opacity = dataOption.opacity.toString();
      this.key = Math.random();
    },
  },
};
</script>

<style>
</style>