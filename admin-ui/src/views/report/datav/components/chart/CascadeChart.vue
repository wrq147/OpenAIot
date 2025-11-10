<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
  >
    <div class="demo-input-suffix" ref="text">
      <label class="el-form-item__label" for="" :style="normalTextStyle">{{ this.chartOption.context }}</label>
      <div class="el-form-item__content">
        <el-cascader
          :options="options"
          v-model="value"
          @change="renderChart($event)"
          clearable
          :style="cascaderStyle"
        ></el-cascader>
      </div>
    </div>
  </div>
</template>

<script>

import "../../animate/animate.css";
import { bindChart } from "../../util/LinkageChart";
import dataChart from '../mixins/dataChart.js'
import {childrenArrayDataHandle} from '../../util/commonChartChange'
export default {
  mixins: [ dataChart ],
  props: {
    className: {
      type: String,
      default: "chart"
    },
    width: {
      type: String,
      default: "100%"
    },
    height: {
      type: String,
      default: "100%"
    },
    customId: {
      type: Number
    },
    drawingList: {
      type: Array
    }
  },
  data() {
    return {
      chart: null,
      value: [],
      options: this.chartOption.staticDataValue,
      animate: this.className
    };
  },
  watch: {
    width() {},
    height() {},
    className: {
      handler(value) {
        this.animate = value;
      }
    },
    $route: {
      handler: function (route) {
        // console.log(route,);
        if(route.query){
          let pars = route.query
          if(this.chartOption.params&&pars[this.chartOption.params]){
            this.value=pars[this.chartOption.params]
          }
        }
       
      },
      immediate: true,
    },
  },
  
  beforeDestroy() {
    if (!this.chart) {
      return;
    }
    this.chart = null;
  },
  mounted() {
    if(this.chartOption.params){
      let pars = this.$route.query;
      if(pars[this.chartOption.params]){
        this.value=pars[this.chartOption.params]
      }
      
    }
    this.valUpdate = this.setChartVal;
    if (this.chartOption.getVal == null) {
      this.chartOption.getVal = () => {
        return this.value;
      };
    }
  },
  computed: {
    normalTextStyle() {
      const style = {
        color: this.chartOption.fontColor,
        fontSize: this.chartOption.fontSize + "px",
        fontFamily: this.chartOption.fontFamily,
        fontWeight: this.chartOption.fontWeight
      };
      return style;
    },
    cascaderStyle() {
      return { width: this.chartOption.width + "px" };
    }
  },
  methods: {
    renderChart(val) {
      this.$emit("onChange", val);
      //更新数据源
      if (this.chartOption.dataList != null) {
        this.chartOption.dataList.forEach(element => {
          this.refreshData(element)
        });
      }
    },
    setChartVal(result,rowGlobal) {
      // console.log(result, '=============result')
      if(this.chartOption.params){
        let pars = this.$route.query;
        if(pars[this.chartOption.params]){
          this.value=pars[this.chartOption.params]
        }
        
      }
      if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
        this.options =childrenArrayDataHandle(rowGlobal,this.chartOption)?childrenArrayDataHandle(rowGlobal,this.chartOption):[]
      }else{
        this.options = this.chartOption.staticDataValue;
      }
      // this.options = result;
    },
  }
};
</script>

<style lang="scss" scoped>
.demo-input-suffix {
  display: flex;
  float: left;
}
</style>
