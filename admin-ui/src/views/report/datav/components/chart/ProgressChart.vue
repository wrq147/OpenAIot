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
import { addOption } from "../../codegen/codegen";
import dataChart from '../mixins/dataChart.js'
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

      const value = result;
       console.log(result, '=========result')
      // if (this.chartOption.theme != null) {
      //   var color = this.chartOption.theme.color;
      // } else {
      //   var color = dataOption.color;
      // }

      let series = [];
      dataOption.yAxis.data = [];
      dataOption.yAxis.axisLabel.textStyle.color = dataOption.labelColor;
      dataOption.legend.data = [];
      dataOption.shortRingRadiusMin =
        dataOption.shortRingRadius - dataOption.shortRingRadiusDiff;

      let minBorder = Math.min(this.chart.offsetHeight, this.chart.offsetWidth);
      dataOption.grid.top =
        ((-dataOption.pieRadiusMax / 2 -
          (dataOption.pieRadiusMax - dataOption.pieRadiusMin) /
            (value.length - 1) /
            4) *
          minBorder) /
          this.chart.offsetHeight +
        parseInt(dataOption.center[1].replace("%", "")) +
        "%";
      dataOption.grid.bottom =
        ((dataOption.pieRadiusMin / 2 -
          (dataOption.pieRadiusMax - dataOption.pieRadiusMin) /
            (value.length - 1) /
            4) *
          minBorder) /
          this.chart.offsetWidth +
        (100 - parseInt(dataOption.center[1].replace("%", ""))) +
        "%";
      dataOption.grid.left = dataOption.center[0];

      for (var i in value) {
        dataOption.legend.data.push(value[i].name );
        //防止溢出
        if (value[i].value > dataOption.maxValue) {
          dataOption.maxValue = value[i].value;
        }
      }
      if (dataOption.isShowLabel) {
        for (var i in value) {
          dataOption.yAxis.data.push(value[i].name);
        }
      }
      //限制字体大小
      dataOption.labelSizeRatio = Math.min(
        (dataOption.pieRadiusMax - dataOption.pieRadiusMin) /
          (value.length - 1),
        dataOption.labelSizeRatio
      );
      dataOption.numberSizeRatio = Math.min(
        (dataOption.pieRadiusMax - dataOption.pieRadiusMin) /
          (value.length - 1),
        dataOption.numberSizeRatio
      );

      series.push({
        name: "大环",
        type: "gauge",
        splitNumber: 15,
        z: 2,
        radius: dataOption.longRingRadius + "%",
        center: dataOption.center,
        startAngle: 90,
        endAngle: -269.9999,
        axisLine: {
          show: false,
          lineStyle: {
            color: [[1, dataOption.longRingColor]]
          }
        },
        axisTick: {
          show: false
        },
        splitLine: {
          show: true,
          length:
            dataOption.longRingRadius -
            dataOption.shortRingRadius +
            dataOption.shortRingRadiusDiff +
            1 +
            "%",
          lineStyle: {
            color: "auto",
            width: dataOption.longRingWidth
          }
        },
        axisLabel: {
          show: false
        },
        detail: {
          show: false
        },
        legend: {
          show: false
        }
      });
      series.push({
        name: "小环",
        type: "gauge",
        splitNumber: 15,
        z: 1,
        radius: dataOption.shortRingRadius + "%",
        center: dataOption.center,
        startAngle: 90,
        endAngle: -269.9999,
        axisLine: {
          show: false
        },
        legend: {
          show: false
        },
        axisTick: {
          show: true,
          lineStyle: {
            color: dataOption.shortRingColor,
            width: dataOption.shortRingWidth
          },
          length: dataOption.shortRingRadiusDiff + "%",
          splitNumber: 5
        },
        splitLine: {
          show: false
        },
        axisLabel: {
          show: false
        },
        detail: {
          show: false
        }
      });
      for (var i in value) {
        series.push({
          name: value[i].name,
          type: "pie",
          clockWise: true,
          color: color[i],
          z: 2,
          hoverAnimation: false,
          radius: [
            Math.round(
              dataOption.pieRadiusMax -
                (i / (value.length - 1)) *
                  (dataOption.pieRadiusMax - dataOption.pieRadiusMin)
            ) -
              dataOption.pieRadiusDiff +
              "%",
            Math.round(
              dataOption.pieRadiusMax -
                (i / (value.length - 1)) *
                  (dataOption.pieRadiusMax - dataOption.pieRadiusMin)
            ) +
              dataOption.pieRadiusDiff +
              "%"
          ],
          center: dataOption.center,
          label: {
            show: dataOption.isShowNumber,
            formatter: "{d}%",
            color: dataOption.numberColor,
            fontSize: Math.round(
              (dataOption.numberSizeRatio / 200) * minBorder
            ),
            position: "inside"
          },
          labelLine: {
            show: false
          },
          legend: {
            show: true
          },
          data: [
            {
              value: value[i].value,
              name: value[i].name
            },
            {
              value: dataOption.maxValue - value[i].value,
              name: "",
              itemStyle: {
                color: "rgba(0,0,0,0)",
                borderWidth: 0
              },
              tooltip: {
                show: false
              },
              label: {
                show: false
              },
              cursor: "default",
              hoverAnimation: false
            }
          ]
        });

        series.push({
          name: "背景线",
          type: "pie",
          silent: true,
          z: 1,
          clockWise: true,
          hoverAnimation: false,
          radius: [
            Math.round(
              dataOption.pieRadiusMax -
                (i / (value.length - 1)) *
                  (dataOption.pieRadiusMax - dataOption.pieRadiusMin)
            ) -
              dataOption.backgroundLineRadiusDiff +
              "%",
            Math.round(
              dataOption.pieRadiusMax -
                (i / (value.length - 1)) *
                  (dataOption.pieRadiusMax - dataOption.pieRadiusMin)
            ) +
              dataOption.backgroundLineRadiusDiff +
              "%"
          ],
          center: dataOption.center,
          cursor: "default",
          label: {
            show: false
          },
          legend: {
            show: false
          },
          itemStyle: {
            label: {
              show: false
            },
            labelLine: {
              show: false
            },
            borderWidth: 5
          },
          data: [
            {
              value: 100,
              itemStyle: {
                color: dataOption.backgroundLineColor,
                borderWidth: 0
              },
              tooltip: {
                show: false
              },
              hoverAnimation: false
            }
          ]
        });
      }

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
        grid: dataOption.grid,
        xAxis: dataOption.xAxis,
        yAxis: dataOption.yAxis,
        tooltip: dataOption.tooltip,
        legend: dataOption.legend,
        series: series
      };

      if (!dataOption.isShowLegend) {
        option.legend = { show: false };
      }

      option.yAxis.axisLabel.fontSize = Math.round(
        (dataOption.labelSizeRatio / 200) * minBorder
      );
      
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
      // else{
      //   //关闭图表联动，取消echart点击事件
      //   this.chart.off('click');
      // }

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
            linkChart(dataOption.arrName, arrs, bindList, this.drawingList);
          }
        });
      }
      // else{
      //   //关闭图表联动，取消echart点击事件
      //   this.chart.off('click');
      // }

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
