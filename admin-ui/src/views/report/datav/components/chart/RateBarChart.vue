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


      let xLabel = [];
      let totalData =[];
      let xValue = []
      result.forEach(element => {
        xLabel.push(element.name)
        xValue.push(element.value)
        totalData.push(element.total)
      });
      dataOption.xAxis.data = xLabel;

      let maxData = [];
      totalData.forEach(element => {
        maxData.push(dataOption.max);
      });

      let option = {
        title: dataOption.title,
        animation: false,
        grid: {
          bottom: "35%" //也可设置left和right设置距离来控制图表的大小
        },
        xAxis: dataOption.xAxis,
        yAxis: dataOption.yAxis,

        series: [
          {
            // 值
            name: "已完成",
            type: "bar",
            barWidth: dataOption.totalBar.barWidth,
            itemStyle: {
              normal: {
                color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                  {
                    offset: 0,
                    color: dataOption.totalBar.completedColor[0]
                  },
                  {
                    offset: 1,
                    color: dataOption.totalBar.completedColor[1]
                  }
                ])
              }
            },
            data: xValue,
            z: 10,
            zlevel: 2,
            label: {
              show: dataOption.label.show,
              position: "bottom",
              distance: dataOption.label.distance,
              zlevel: 4,
              z: 10,
              formatter: function(params) {
                const value = (
                  (params.value * 100) /
                  totalData[params.dataIndex]
                ).toFixed(0);
                return "{b|已完成   " + value + "%}";
              },
              rich: {
                b: {
                  fontFamily: dataOption.label.fontFamily,
                  fontSize: dataOption.label.fontSize,
                  color: dataOption.label.color,
                  fontWeight: dataOption.label.fontWeight,
                  backgroundColor: {
                    image: dataOption.label.backgroundImg
                  },
                  padding: dataOption.label.padding,
                  height: dataOption.label.imageHeight,
                  width: dataOption.label.imageWidth
                }
              }
            }
          },
          {
            // 值分隔
            type: "pictorialBar",
            itemStyle: {
              normal: {
                color: dataOption.splitBar.color
              }
            },
            symbolRepeat: "fixed",
            symbolMargin: dataOption.splitBar.symbolMargin,
            symbol: "rect",
            symbolClip: true,
            symbolSize: [dataOption.splitBar.width, dataOption.splitBar.height],
            symbolPosition: "start",
            symbolOffset: [0, -1],
            // symbolBoundingData: this.total,
            data: xValue,

            z: 0,
            zlevel: 3
          },
          {
            // 总计值
            name: "总计量",
            type: "bar",
            barWidth: dataOption.totalBar.barWidth,
            itemStyle: {
              normal: {
                color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                  {
                    offset: 0,
                    color: dataOption.totalBar.color[0]
                  },
                  {
                    offset: 1,
                    color: dataOption.totalBar.color[1]
                  }
                ])
              }
            },
            data: totalData,
            z: 10,
            zlevel: 0,
            label: {
              show: false
            }
          },
          {
            // 总计值分隔
            type: "pictorialBar",
            itemStyle: {
              normal: {
                color: dataOption.splitBar.color
              }
            },
            symbolRepeat: "fixed",
            symbolMargin: dataOption.splitBar.symbolMargin,
            symbol: "rect",
            symbolClip: true,
            symbolSize: [dataOption.splitBar.width, dataOption.splitBar.height],
            symbolPosition: "start",
            symbolOffset: [0, -1],
            // symbolBoundingData: this.total,
            data: totalData,
            z: 0,
            zlevel: 0
          },
          {
            //辅助背景图形
            name: "背景条",
            type: "bar", //pictorialBar
            barWidth: dataOption.totalBar.barWidth,
            barGap: "-100%",
            itemStyle: {
              normal: {
                borderWidth: 0,
                color: dataOption.totalBar.backgroundColor
              },
              barBorderRadius: 10
            },
            data: maxData,
            z: 0,
            zlevel: 0
          },
          {
            // 背景分隔
            type: "pictorialBar",
            itemStyle: {
              normal: {
                color: dataOption.splitBar.color
              }
            },
            symbolRepeat: "fixed",
            symbolMargin: dataOption.splitBar.symbolMargin,
            symbol: "rect",
            symbolClip: true,
            symbolSize: [dataOption.splitBar.width, dataOption.splitBar.height],
            symbolPosition: "start",
            symbolOffset: [0, -1],
            // symbolBoundingData: this.total,
            data: maxData,
            z: 0,
            zlevel: 1
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
            legendName: params.name,
            data: params.value
          };

          let arrs = JSON.stringify(arrObject);

          //获取绑定的图表
          let bindList = dataOption.bindList;

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
