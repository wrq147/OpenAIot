<template>
  <div
    :class="animate"
    :style="{ height: height, width: width }"
    :id="chartOption.bindingDiv"
    ref="chartDiv"
  />
</template>

<script>
import echarts from "echarts";
require("echarts/theme/macarons"); // echarts theme
import resize from "@/views/dashboard/mixins/resize";
import "../../animate/animate.css";
import "echarts-liquidfill";
import { addOption } from "../../codegen/codegen";
import dataChart from '../mixins/dataChart.js'

export default {
  mixins: [resize,dataChart],
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
  },
  data() {
    return {
      chart: null,
      animate: this.className
    };
  },
  watch: {
    width() {
      this.$nextTick(() => {
        if (this.chart != null) {
          this.chart.resize();
        }
      });
    },
    height() {
      this.$nextTick(() => {
        if (this.chart != null) {
          this.chart.resize();
        }
      });
    },
    "chartOption.theme": {
      handler() {
        if (this.chart != null) {
          this.chart.dispose();
          this.chart = null;
        }
      }
    },
    className: {
      handler(value) {
        this.animate = value;
      }
    }
  },
  mounted() {
    this.valUpdate = this.setChartVal;
  },
  beforeDestroy() {
    if (!this.chart) {
      return;
    }
    this.chart.dispose();
    this.chart = null;
  },
  methods: {
    setChartVal(result) {
      
      if (this.chart == null) {
        echarts.registerTheme("customTheme", this.chartOption.theme);
        this.chart = echarts.init(this.$el, "customTheme");
      }
      let dataOption = JSON.parse(JSON.stringify(this.dataOption));
      dataOption.series[0].data = [result[0].value];
      dataOption.series[1].markLine.data[0].yAxis = [result[0].line];
      let option = {
        //title: dataOption.title,
        title: {
          text: dataOption.title.text, // 主标题
          textStyle: {
            color: "#fff", // 颜色
            fontWeight: dataOption.title.textStyle.fontWeight, // 粗细
            fontFamily: dataOption.title.textStyle.fontFamily, // 字体
            fontSize: dataOption.title.textStyle.fontSize, // 字号大小
            align: "center" // 水平对齐
          },
          subtext: dataOption.title.subtext, // 副标题
          subtextStyle: {
            // 对应样式
            color: "#fff",
            fontWeight: dataOption.title.textStyle.fontWeight, // 粗细
            fontFamily: dataOption.title.textStyle.fontFamily, // 字体
            fontSize: dataOption.title.textStyle.fontSize - 5,
            align: "center"
          },
          x: dataOption.title.x,
          itemGap: 7
        },
        xAxis: dataOption.xAxis,
        yAxis: dataOption.yAxis,
        grid: {
          top: "2.5%",
          right: "40",
          bottom: "2.5%",
          left: 0
        },
        series:  dataOption.series
      };
      this.chart.setOption(option, true);

      addOption(dataOption.bindingDiv, option);
    },
  
  }
};
</script>
