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

      let seriesData = [];
      let data = {};
      data.type = "line";
      let xdata = []
      
      if (dataOption.isSmooth == true) {
          data.smooth = true;
      } else if (dataOption.isSmooth == false) {
        data.smooth = false;
      }
      //data.itemStyle = dataOption.itemStyle;
      data.lineStyle = {
        width: dataOption.lineWidth == undefined ? 3 : dataOption.lineWidth,
        shadowColor: "rgba(231,234,237,0.4)",
        shadowBlur: 10,
        shadowOffsetY: 10
      };
      //标志大小
      if (dataOption.lineSymbolSize != undefined) {
        data.symbolSize = dataOption.lineSymbolSize;
      } else {
        data.symbolSize = 8;
      }

      //是否显示平均线
      if (dataOption.isMarkLine == true) {
        data.markLine = {
          data: [{ type: "average", name: "平均值" }]
        };
      } else if (
        dataOption.isMarkLine == false ||
        dataOption.isMarkLine == undefined
      ) {
        data.markLine = {};
      }

      //是否显示最大值和最小值
      if (dataOption.isMarkPoint == true) {
        data.markPoint = {
          data: [
            { type: "max", name: "最大值" },
            { type: "min", name: "最小值" }
          ]
        };
      } else if (
        dataOption.isMarkPoint == false ||
        dataOption.isMarkPoint == undefined
      ) {
        data.markPoint = {};
      }
      data.data = []
      
      result.forEach((item, index) => {
        xdata.push(item.name);
        data.data.push(item.value);
      });
      seriesData = data
      
      let xAxisTemp = {};
      let yAxisTemp = {};
      xAxisTemp = { ...dataOption.xAxis };
      yAxisTemp = { ...dataOption.yAxis };
     
      let xAxisTick = { show: false };
      xAxisTemp.axisTick = xAxisTick;
      xAxisTemp.data = xdata;
      xAxisTemp.axisLine.show= dataOption.xAxisLineShow == undefined ? 1 : dataOption.xAxisLineShow;
      xAxisTemp.axisLine.lineStyle.color = dataOption.xAxisLineColor === undefined ? "#fff" : dataOption.xAxisLineColor === null ? "transparent" : dataOption.xAxisLineColor;
      xAxisTemp.axisLine.lineStyle.width = dataOption.xAxisLineWidth == undefined ? 1 : dataOption.xAxisLineWidth;
      xAxisTemp.axisLabel = {
        show: true,
        color: dataOption.xLabelFontColor === undefined ? "#fff" : dataOption.xLabelFontColor === null ? "transparent" : dataOption.xLabelFontColor,
        fontSize: dataOption.xLabelFontSize === undefined ? 14 : dataOption.xLabelFontSize,
      }
      xAxisTemp.nameTextStyle = {
        fontSize: dataOption.xAxisFontSize,
        color: dataOption.xLabelFontColor,
        fontFamily: dataOption.xLabelFontFamily,
        padding: [0, 0, 10, 8]
      }

      //y轴设置

      yAxisTemp.axisTick = { show: false };
      yAxisTemp.axisLine.show= dataOption.yLabelShow == undefined ? 1 : dataOption.yLabelShow;
      yAxisTemp.axisLine.lineStyle.color = dataOption.yAxisLineColor === undefined ? "#fff" : dataOption.yAxisLineColor === null ? "transparent" : dataOption.yAxisLineColor;
      yAxisTemp.axisLine.lineStyle.width = dataOption.yAxisLineWidth == undefined ? 1 : dataOption.yAxisLineWidth;
      yAxisTemp.axisLabel = {
        show: dataOption.yLabelShow == undefined ? true : dataOption.yLabelShow,
        fontSize: dataOption.yLabelFontSize,
        color: dataOption.yLabelFontColor,
        fontFamily: dataOption.yLabelFontFamily
      }
      yAxisTemp.nameTextStyle = {
        fontSize: dataOption.yAxisFontSize,
        color: dataOption.yLabelFontColor,
        fontFamily: dataOption.yLabelFontFamily,
        padding: [0, 0, 15, 0]
      }

      //图例设置
      let legendData = {
        show: dataOption.legend.show,
        x: (dataOption.legendX || 35) + "%",
        y: (dataOption.legendY || 5) + '%',
        orient: dataOption.legendOrient == undefined ? "horizontal" : dataOption.legendOrient,
        textStyle: {
          fontSize: dataOption.legendFontSize == undefined ? "14" : dataOption.legendFontSize,
          color: dataOption.legendFontColor == undefined ? "#fff" : dataOption.legendFontColor
        },
        itemWidth: dataOption.itemWidth || 14,
        itemHeight: dataOption.itemHeight || 8
      }
      
      //分隔线设置
      let splitLineStyle = {
        color:
          dataOption.splitLineColor == undefined
            ? "#ccc"
            : dataOption.splitLineColor,
        width:
          dataOption.splitLineWidth == undefined
            ? 1
            : dataOption.splitLineWidth,
        type:
          dataOption.splitLineType == undefined
            ? "solid"
            : dataOption.splitLineType
      };
      dataOption.yAxis.splitLine.lineStyle = splitLineStyle;

      let option = {
        title: dataOption.title,
        tooltip: dataOption.tooltip,
        legend: legendData,
        grid: {
          top: "15%",
          left: "2%",
          right: "7%",
          bottom: "3%",
          containLabel: true
        },
        xAxis: xAxisTemp,
        yAxis: yAxisTemp,
        series: seriesData
      };
      this.chart.setOption(option, true);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          // console.log("params =>", params);

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
      }
      //关闭图表联动，取消echart点击事件
      else {
        this.chart.off("click");
      }

    },
   
  }
};
</script>
