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
import dataChart from "../mixins/dataChart.js";
import VueEvent from "../../VueEvent";

export default {
  mixins: [resize, dataChart],
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
    drawingList: {
      type: Array,
    },
  },
  data() {
    return {
      chart: null,
      animate: this.className,
    };
  },
  watch: {
    width() {
      this.$nextTick(() => {
        this.chart.resize();
      });
    },
    height() {
      this.$nextTick(() => {
        this.chart.resize();
      });
    },
    "chartOption.theme": {
      handler() {
        if (this.chart != null) {
          this.chart.dispose();
          this.chart = null;
        }
      },
    },
    className: {
      handler(value) {
        this.animate = value;
      },
    },
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
    async setChartVal(result) {
      if (this.chart == null) {
        echarts.registerTheme("customTheme", this.chartOption.theme);
        this.chart = echarts.init(this.$el, "customTheme");
      }
      let dataOption = JSON.parse(JSON.stringify(this.dataOption));

      if (dataOption.isTopic == true) {
        if (this.chartOption.theme != null) {
          dataOption.series[0].itemStyle.normal.borderColor.colorStops[0].color =
            this.chartOption.theme.color[0];
          dataOption.series[0].itemStyle.normal.borderColor.colorStops[1].color =
            this.chartOption.theme.color[2];
          dataOption.series[0].itemStyle.normal.color.colorStops[0].color =
            this.chartOption.theme.color[2];
          dataOption.series[0].itemStyle.normal.color.colorStops[1].color =
            this.chartOption.theme.color[0];
        }
      }
      let data = [];
      let xdata = []
      for (const item of result) {
        xdata.push(item.name)
        data.push([item.max, item.q1, item.min, item.medium, item.q3]);
      }
      dataOption.series[0].data = data;
      dataOption.xAxis.data = xdata;

      dataOption.yAxis.axisLabel.formatter = "{value}" + dataOption.unit;

      let xAxisTemp, yAxisTemp;
      xAxisTemp = dataOption.xAxis;
      yAxisTemp = dataOption.yAxis;
      //是否竖显示
      if (dataOption.isVertical) {
        xAxisTemp = dataOption.yAxis;
        yAxisTemp = dataOption.xAxis;
      }

      let option = {
        title: {
          text: dataOption.text, // 主标题
          textStyle: {
            color: dataOption.fontColor, // 颜色
            fontWeight: dataOption.fontWeight, // 粗细
            fontFamily: dataOption.fontFamily, // 字体
            fontSize: dataOption.fontSize, // 大小
            align: "center", // 水平对齐
          },
          subtext: dataOption.subtext, // 副标题
          subtextStyle: {
            // 对应样式
            color: dataOption.fontColor,
            fontWeight: dataOption.fontWeight, // 粗细
            fontFamily: dataOption.fontFamily, // 字体
            fontSize: dataOption.fontSize - 6,
            align: "center",
          },
          x: dataOption.x,
          y: 20,
          itemGap: 7,
        },
        tooltip: dataOption.tooltip,
        grid: {
          top: "15%",
          left: "2%",
          right: "10%",
          bottom: "3%",
          containLabel: true,
        },
        xAxis: xAxisTemp,
        yAxis: yAxisTemp,
        series: dataOption.series,
      };
      this.chart.setOption(option, true);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", (params) => {
          //设置参数
          let arrObject = {
            legendName: params.name,
            seriesName: params.seriesName,
            data: params.value,
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
        this.chart.on("click", (params) => {
          //设置参数
          let arrObject = {
            legendName: params.name,
            seriesName: params.seriesName,
            data: params.value,
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
              [{ name: "drillParam", value: arrs }]
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
  },
};
</script>
