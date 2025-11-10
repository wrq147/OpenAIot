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

import { getLinkChart } from "../../util/LinkageChart";
import { addOption } from "../../codegen/codegen";

import dataChart from '../mixins/dataChart.js';
import VueEvent from "../../VueEvent";

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
    drawingList: {
      type: Array
    }
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
      
      if (this.chartOption.theme != null) {
        dataOption.color = this.chartOption.theme.color;
      } else {
        dataOption.color = dataOption.color;
      }

      if (typeof dataOption.pieRadiusMin == "undefined") {
        dataOption.pieRadiusMin = 30;
      }
      if (typeof dataOption.pieRadiusMax == "undefined") {
        dataOption.pieRadiusMax = 70;
      }
      if (typeof dataOption.pieRadiusDiff == "undefined") {
        dataOption.pieRadiusDiff = 2;
      }
      if (typeof dataOption.maxValue == "undefined") {
        dataOption.maxValue = 800;
      }
      if (typeof dataOption.borderWidth == "undefined") {
        dataOption.borderWidth = 5;
      }

      let scaleDataArr = [];
      let seriesObj = [];
      console.log(result, '=======result')

      for (var i in result) {
        //防止溢出
        if (result[i].value > dataOption.maxValue) {
          dataOption.maxValue = result[i].value;
        }
        let scaleData = {
          name: result[i].name,
          value: result[i].value,
          radius1: [
            Math.round(
              dataOption.pieRadiusMax -
                (parseInt(i) / (result.length - 1)) *
                  (dataOption.pieRadiusMax - dataOption.pieRadiusMin)
            ) -
              dataOption.pieRadiusDiff +
              "%",
            Math.round(
              dataOption.pieRadiusMax -
                (parseInt(i) / (result.length - 1)) *
                  (dataOption.pieRadiusMax - dataOption.pieRadiusMin)
            ) +
              dataOption.pieRadiusDiff +
              "%"
          ],
          radius2:
            Math.round(
              dataOption.pieRadiusMax -
                (parseInt(i) / (result.length - 1)) *
                  (dataOption.pieRadiusMax - dataOption.pieRadiusMin)
            ) -
            dataOption.pieRadiusDiff +
            "%"
        };
        scaleDataArr.push(scaleData);
      }
      console.log(result, '===========result')
      // for (let i = 0; i < scaleDataArr.length; i++) {
      //   let data = {

      //   }
      //   seriesObj.push(
      //     {
      //       name: "",
      //       type: "pie",
      //       radius: scaleDataArr[i].radius1,
      //       hoverAnimation: false,
      //       itemStyle: {
      //         normal: {
      //           label: {
      //             show: false,
      //             color: "#ddd"
      //           }
      //         }
      //       },
      //       data: [
      //         {
      //           value: scaleDataArr[i].value,
      //           name: scaleDataArr[i].name,
      //           itemStyle: {
      //             normal: {
      //               borderWidth: dataOption.pieRadiusDiff,
      //               borderColor: dataOption.color[i]
      //             }
      //           }
      //         },
      //         {
      //           value: dataOption.maxValue - scaleDataArr[i].value,
      //           name: "",
      //           itemStyle: {
      //             normal: {
      //               label: {
      //                 show: true
      //               },
      //               labelLine: {
      //                 show: false
      //               },
      //               color: "rgba(0, 0, 0, 0)",
      //               borderColor: "rgba(0, 0, 0, 0)",
      //               borderWidth: 20
      //             }
      //           },
      //           tooltip: {
      //             show: false
      //           }
      //         }
      //       ]
      //     },
      //     {
      //       name: "",
      //       type: "gauge",
      //       detail: false,
      //       splitNumber: dataOption.splitNumber, //刻度数量
      //       radius: scaleDataArr[i].radius2, //图表尺寸
      //       center: ["50%", "50%"],
      //       startAngle: 0, //开始刻度的角度
      //       endAngle: -360, //结束刻度的角度
      //       axisLine: {
      //         show: false,
      //         lineStyle: {
      //           width: 0,
      //           shadowBlur: 0
      //         }
      //       },
      //       axisTick: {
      //         show: true,
      //         lineStyle: {
      //           color: "rgba(220, 220, 220, 0.5)",
      //           width: dataOption.borderWidth
      //         },
      //         length: dataOption.borderWidth,
      //         splitNumber: 5
      //       },
      //       splitLine: {
      //         show: false,
      //         length: 5,
      //         lineStyle: {
      //           color: "rgba(220, 220, 220, 0.1)"
      //         }
      //       },
      //       axisLabel: {
      //         show: false
      //       }
      //     }
      //   );
      //   console.log(seriesObj, '===========seriesObj')
      // }

      console.log(seriesObj, '===========seriesObj')
      let option = {
        title: {
          text: dataOption.title.text, //主标题
          textStyle: {
            color: "#fff", //颜色
            fontWeight: dataOption.title.textStyle.fontWeight, // 粗细
            fontFamily: dataOption.title.textStyle.fontFamily, // 字体
            fontSize: dataOption.title.textStyle.fontSize, // 字号大小
            align: "center" //水平对齐
          },
          subtext: dataOption.title.subtext, //副标题
          subtextStyle: {
            //对应样式
            color: "#fff",
            fontWeight: dataOption.title.textStyle.fontWeight, // 粗细
            fontFamily: dataOption.title.textStyle.fontFamily, // 字体
            fontSize: dataOption.title.textStyle.fontSize - 5,
            align: "center"
          },
          x: dataOption.title.x,
          itemGap: 7
        },
        grid: {
          left: "5%",
          // right: "2%",
          bottom: "5%",
          top: "20%"
          //containLabel: true
        },
        tooltip: {
          show: true,
          trigger: "item",
          formatter: "{b} : {c} ({d}%)"
        },
        color: dataOption.color,
        legend: dataOption.legend,
        series: seriesObj
      };
   
      this.chart.setOption(option, true);

      addOption(dataOption.bindingDiv, option);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            legendName: params.name,
            seriesName: params.seriesName,
            data: params.value
          };

          let arrs = JSON.stringify(arrObject);

          //获取绑定的图表
          let bindList = this.chartOption.bindList;

          if (bindList.length > 0) {
            getLinkChart(dataOption.arrName, arrs, bindList, this.drawingList);
          }
        });
      }
      //开启图表下钻
      else if (dataOption.isDrillDown == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            legendName: params.name,
            seriesName: params.seriesName,
            data: params.value
          };

          let arrs = JSON.stringify(arrObject);

          //获取绑定的图表
          let drillDownChartOption = this.chartOption.drillDownChartOption;

          if (
            drillDownChartOption != undefined &&
            drillDownChartOption != null
          ) {
            this.$set(
              this.chartOption.drillDownChartOption.chartOption,
              "requestParameters",
              [{"name":"drillParam","value":arrs}]
            );
            //发送下钻消息
            VueEvent.$emit(
              "drill_down_msg",
              this.chartOption.drillDownChartOption
            );
          }
        });
      } else {
        //关闭图表联动，取消echart点击事件
        this.chart.off("click");
      }

    },
    
  }
};
</script>
