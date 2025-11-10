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
      let dataOption = this.dataOption;

      if (dataOption.animate != null) {
        //添加动画样式
        //animateUtil.addAnimate(dataOption.bindingDiv, dataOption.animate);
      }

      var box = document.getElementById("keyCanvas");
      if (box != null) {
        box.remove();
      }
      //创建一个画布
      let can = document.createElement("canvas");
      //设置画布的长宽
      var width = this.width.replace("px", "");
      var height = this.height.replace("px", "");

      can.id = "keyCanvas";
      can.width = width;
      can.height = height;

      let ctx = can.getContext("2d");
      var grd = ctx.createLinearGradient(0, 0, width, 0);
      if (dataOption.isTopic == true) {
        //应用主题
        if (this.chartOption.theme != null) {
          //color = dataOption.thiscolor;
          var col = dataOption.theme.color;
          dataOption.color = [[0.2, col[0]], [0.8, col[1]], [1, col[2]]];
        }
      }

      for (var i = 0; i < dataOption.color.length; i++) {
        //console.log("嘿", dataOption.color);
        grd.addColorStop(dataOption.color[i][0], dataOption.color[i][1]);
      }
      ctx.fillStyle = grd;
      ctx.fillRect(0, 0, width, height);

      //*****************************************************
      let value = result[0].value;
      let pointAngle =
        dataOption.startAngle -
        (value / dataOption.max) *
          (dataOption.startAngle - dataOption.endAngle);
      let thisNumber = (value / dataOption.max) * dataOption.splitNumber;
      let option = {
        bindingType: "keygauge",
        title: dataOption.title,
        color: dataOption.color,
        series: [
          {
            name: "系列名称",
            type: "gauge",
            radius: dataOption.radius,
            center: dataOption.center,
            min: dataOption.min,
            max: value,
            startAngle: dataOption.startAngle,
            endAngle: pointAngle,
            splitNumber: thisNumber,
            axisLine: { show: false },
            axisTick: { show: false },
            splitLine: {
              //刻度
              show: true,
              length: dataOption.length,
              lineStyle: {
                color: {
                  image: can, //document.getElementById('keyCanvas'),
                  //color: '#893448',
                  repeat: "no-repeat"
                },
                width: dataOption.width,
                type: "solid"
              }
            },
            data: [
              {
                value: value,
                name: result[0].name
              }
            ],
            detail: {
              show: dataOption.isdetail,
              formatter: "{value}",
              show: dataOption.isdetail,
              offsetCenter: dataOption.detailOffset,
              color: dataOption.detailColor,
              fontFamily: dataOption.detailFamily,
              fontSize: dataOption.detailSize,
              fontWeight: dataOption.detailWeight
            },
            pointer: { show: false },
            title: { show: false },
            axisLabel: { show: false }
          },
          {
            name: "系列名称",
            type: "gauge",
            radius: dataOption.radius,
            center: dataOption.center,
            min: value,
            max: dataOption.max,
            startAngle: pointAngle,
            endAngle: dataOption.endAngle,
            splitNumber: dataOption.splitNumber - thisNumber,
            axisLine: { show: false },
            axisTick: { show: false },
            splitLine: {
              //刻度
              show: true,
              length: dataOption.length,
              lineStyle: {
                color: dataOption.noColor,
                width: dataOption.width,
                type: "solid"
              }
            },
            detail: { show: false },
            pointer: { show: false },
            title: { show: false },
            axisLabel: { show: false }
          }
        ]
      };
      if (dataOption.isinline == "true") {
        var inline = {
          name: "系列名称",
          type: "gauge",
          radius: dataOption.inlineRadius,
          center: dataOption.center,
          startAngle: dataOption.startAngle,
          endAngle: dataOption.endAngle,
          splitLine: { show: false },
          axisTick: { show: false },
          axisLine: {
            show: true,
            lineStyle: {
              color: [["1", dataOption.inlineColor]], //dataOption.inlineColor,
              width: dataOption.inlineWidth //dataOption.inlineWidth,
            }
          },
          detail: { show: false },
          pointer: { show: false },
          title: { show: false },
          axisLabel: { show: false }
        };
        option.series.push(inline);
      }
  
      this.chart.setOption(option, true);

      addOption(dataOption.bindingDiv, option);
    },
   
  }
};
</script>
