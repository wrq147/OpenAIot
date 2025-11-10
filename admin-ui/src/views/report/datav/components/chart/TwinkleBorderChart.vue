<template>
  <div :key="key" :class="className" :style="{ height: height, width: width}" :id="chartOption.bindingDiv">
    <dv-border-box-1 v-if="dataOption.borderType=='dv-border-box-1'" :color="color" :backgroundColor="dataOption.backgroundColor"></dv-border-box-1>
    <dv-border-box-2 v-else-if="dataOption.borderType=='dv-border-box-2'" :color="color" :backgroundColor="dataOption.backgroundColor"></dv-border-box-2>
    <dv-border-box-3 v-else-if="dataOption.borderType=='dv-border-box-3'" :color="color" :backgroundColor="dataOption.backgroundColor"></dv-border-box-3>
    <dv-border-box-4 v-else-if="dataOption.borderType=='dv-border-box-4'" :color="color" :backgroundColor="dataOption.backgroundColor" :reverse="dataOption.reverse"></dv-border-box-4>
    <dv-border-box-5 v-else-if="dataOption.borderType=='dv-border-box-5'" :color="color" :backgroundColor="dataOption.backgroundColor" :reverse="dataOption.reverse"></dv-border-box-5>
    <dv-border-box-6 v-else-if="dataOption.borderType=='dv-border-box-6'" :color="color" :backgroundColor="dataOption.backgroundColor"></dv-border-box-6>
    <dv-border-box-7 v-else-if="dataOption.borderType=='dv-border-box-7'" :color="color" :backgroundColor="dataOption.backgroundColor"></dv-border-box-7>
    <dv-border-box-8 v-else-if="dataOption.borderType=='dv-border-box-8'" :color="color" :reverse="dataOption.reverse"></dv-border-box-8>
    <dv-border-box-9 v-else-if="dataOption.borderType=='dv-border-box-9'" :color="color" :backgroundColor="dataOption.backgroundColor"></dv-border-box-9>
    <dv-border-box-10 v-else-if="dataOption.borderType=='dv-border-box-10'" :color="color" :backgroundColor="dataOption.backgroundColor"></dv-border-box-10>
    <dv-border-box-11 v-else-if="dataOption.borderType=='dv-border-box-11'" :color="color" :backgroundColor="dataOption.backgroundColor" :title="dataOption.title" :titleWidth="dataOption.titleWidth"></dv-border-box-11>
    <dv-border-box-12 v-else-if="dataOption.borderType=='dv-border-box-12'" :color="color" :backgroundColor="dataOption.backgroundColor"></dv-border-box-12>
    <dv-border-box-13 v-else-if="dataOption.borderType=='dv-border-box-13'" :color="color" :backgroundColor="dataOption.backgroundColor"></dv-border-box-13>
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
      key:1,
      borderType:"dv-border-box-1",
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
      ++this.key;
    },
   
  }
};
</script>
