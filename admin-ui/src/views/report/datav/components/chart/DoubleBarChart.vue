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

import { getLinkChart } from "../../util/LinkageChart";
import VueEvent from "../../VueEvent";
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
      let dataOption = JSON.parse(JSON.stringify(this.dataOption))
      let data = result;
      let yAxisData = new Set();
      let legendData = [];
      let valueData = {};

      for (let d in data) {
        //图例
        legendData.push(data[d].name);
        yAxisData.add(data[d].label);
      }
      legendData = [...new Set(legendData)]
      for (let d in legendData) {
        let valueArr = [];
        for (let v in data) {
          if (data[v].name === legendData[d]) {
              valueArr.push(data[v].value);
            }
        }
        valueData[d] = valueArr;
      }
      let top = 60;
      let bottom = 60;
      yAxisData = [...yAxisData];

      if (dataOption.istitle == false) {
        dataOption.title.show = false;
      } else {
        dataOption.title.show = true;
      }
      //*******************************************使用主题******************************************************/
      let color = [];
      if (dataOption.isThemeColor) {
        color = this.chartOption.theme.color;
        dataOption.leftColor = color[0];
        dataOption.rightColor = color[1];
      }

      let option = {
        title: dataOption.title,
        tooltip: {
          show: true,
          trigger: "axis",
          axisPointer: {
            type: "shadow"
          }
        },
        legend: {
          left: "center",
          bottom: 24,
          itemWidth: 15,
          itemHeight: 11,
          itemGap: 20,
          borderRadius: 4,
          textStyle: {
            color: "#fff",
            fontSize: 14
          },
          data: legendData,
          show: dataOption.showLegend
        },
        grid: [
          {
            //图
            left: "12%",
            width: "28%",
            containLabel: true,
            top: 60,
            bottom
          },
          {
            left: "52%",
            width: "0%",
            top: 60,
            bottom: bottom + 16
          },
          {
            right: "12%",
            width: "28%",
            containLabel: true,
            top: 60,
            bottom
          }
        ],
        xAxis: [
          {
            //横轴
            type: "value",
            inverse: true,
            axisLabel: {
              show: true,
              color: "#949AA8",
              margin: 0
            },
            axisLine: {
              show: false
            },
            axisTick: {
              show: false
            },
            splitLine: {
              show: true,
              lineStyle: {
                color: "#E0E0E0",
                type: dataOption.lineStyle
              }
            },
            position: dataOption.position,
            show: dataOption.xAxisShow
          },
          {
            gridIndex: 1,
            show: true
          },
          {
            gridIndex: 2,
            type: "value",
            axisLabel: {
              show: true,
              color: "#949AA8",
              margin: 0
            },
            axisLine: {
              show: false
            },
            axisTick: {
              show: false
            },
            splitLine: {
              show: true,
              lineStyle: {
                color: "#E0E0E0",
                type: dataOption.lineStyle
              }
            },
            position: dataOption.position,
            show: dataOption.xAxisShow
          }
        ],
        yAxis: [
          {
            position: "right",
            type: "category",
            inverse: false,
            axisLabel: {
              show: false
            },
            axisLine: {
              show: true,
              lineStyle: {
                color: "#E0E0E0"
              }
            },
            axisTick: {
              show: false
            },
            data: yAxisData
          },
          {
            gridIndex: 1,
            position: "left",
            axisLabel: {
              align: "center",
              padding: [8, 0, 0, 0],
              fontSize: 12,
              color: dataOption.textColor
            },
            axisLine: {
              show: false
            },

            axisTick: {
              show: false
            },
            data: yAxisData
          },
          {
            gridIndex: 2,
            position: "left",
            type: "category",
            inverse: false,
            axisLabel: {
              show: false
            },
            axisLine: {
              show: true,
              lineStyle: {
                color: "#E0E0E0"
              }
            },
            axisTick: {
              show: false
            },
            data: yAxisData
          }
        ],
        series: [
          {
            type: "bar",
            barWidth: dataOption.barWidth,
            label: {
              show: true,
              fontFamily: "Rubik-Medium",
              fontSize: 14,
              distance: 10
            },
            name: legendData[0],
            label: {
              position: "left"
            },
            itemStyle: {
              color: dataOption.leftColor, //左侧柱颜色
              barBorderRadius: [4, 0, 0, 4]
            },
            data: valueData[0]
          },
          {
            type: "bar",
            barWidth: dataOption.barWidth,
            label: {
              show: true,
              fontFamily: "Rubik-Medium",
              fontSize: 14,
              distance: 10
            },
            xAxisIndex: 2,
            yAxisIndex: 2,
            name: legendData[1],
            label: {
              position: "right"
            },
            itemStyle: {
              color: dataOption.rightColor,
              barBorderRadius: [0, 4, 4, 0]
            },
            data: valueData[1]
          }
        ]
      };
     
      this.chart.setOption(option, true);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            legendName: params.seriesName,
            seriesName: params.name,
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
            legendName: params.seriesName,
            seriesName: params.name,
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
