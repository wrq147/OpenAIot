<template>
  <div :class="className" :style="{ height: height, width: width}" :id="chartOption.bindingDiv">
    <dv-decoration-3 :key="key" :color="color"></dv-decoration-3>
  </div>
</template>

<script>

import '../../animate/animate.css'
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
      key: 1
    };
  },
  watch: {
    width() {
    },
    height() {
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
      this.key = Math.random();
    },
   
  }
};
</script>
