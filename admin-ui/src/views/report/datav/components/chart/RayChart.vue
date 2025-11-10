<template>
  <div :class="className" :style="{ height: height, width: width}" :id="chartOption.bindingDiv">
    <dv-decoration-2 :key="key" :color="color" :style="{ 'height': height, 'width': width}" :reverse="dataOption.reverse" :dur="dataOption.dur"></dv-decoration-2>
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
