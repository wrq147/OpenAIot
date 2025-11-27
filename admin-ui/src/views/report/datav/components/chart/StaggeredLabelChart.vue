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

      //*******************************************使用主题******************************************************/
      let color = [];
      if (dataOption.isThemeColor) {
        color = this.chartOption.theme.color;
        dataOption.itemPositive = color[0];
        dataOption.itemNegative = color[1];
      }

      let series = {};
      series.type = "bar";
      series.name = dataOption.name;
      //展示标签
      let label = {};
      label.show = true;
      label.formatter = "{b}";
      series.label = label;
      //标签位置
      let positiveData = {};
      positiveData.position = dataOption.labelPositive;
      let positiveItem = {};
      let positiveColor = {};
      positiveColor.color = dataOption.itemPositive;
      positiveItem.normal = positiveColor;
      let negativeData = {};
      negativeData.position = dataOption.labelNegative;
      let negativeItem = {};
      let negativeColor = {};
      negativeColor.color = dataOption.itemNegative;
      negativeItem.normal = negativeColor;
      //图表位置
      let grid = {};
      grid.top = parseFloat(dataOption.top);
      grid.bottom = parseFloat(dataOption.bottom);

      //设置x轴，y轴属性
      let xAxisData = {};
      let yAxisData = {};
      //封装数据
      let seriesData = [];
      let labelData = [];

      if (JSON.stringify(result) != "{}") {
        yAxisData.data =  [];
        for (const value of result) {
          let data = {};
          let figure = value.value;
          data.value = figure;
          yAxisData.data.push(value.name)
          //根据正负值展示标签位置
          if (parseFloat(figure) > 0) {
            data.label = positiveData;
            data.itemStyle = positiveItem;
          } else {
            data.label = negativeData;
            data.itemStyle = negativeItem;
          }
          labelData.push(data);
        }
        series.data = labelData;
        seriesData.push(series);

        seriesData[0].label.show = dataOption.showLabel;

        xAxisData.type = "value";
        let lineStyle = {};
        lineStyle.type = dataOption.lineStyle;
        let splitLine = {};
        splitLine.lineStyle = lineStyle;
        xAxisData.splitLine = splitLine;
        xAxisData.position = dataOption.position;
        xAxisData.show = dataOption.xAxisShow;

        yAxisData.type = "category";
        let axisLine = {};
        axisLine.show = JSON.parse(dataOption.axisLine);
        yAxisData.axisLine = axisLine;
        let axisLabel = {};
        axisLabel.show = JSON.parse(dataOption.axisLabel);
        yAxisData.axisLabel = axisLabel;
        let axisTick = {};
        axisTick.show = JSON.parse(dataOption.axisTick);
        yAxisData.axisTick = axisTick;
        let splitLineData = {};
        splitLineData.show = JSON.parse(dataOption.splitLine);
        yAxisData.splitLine = splitLineData;

        let xAxis = {};
        let yAxis = {};
        //正负值左右翻转
        if (dataOption.direction == "level") {
          xAxis = xAxisData;
          yAxis = yAxisData;
        }
        //正负值上下翻转
        else {
          xAxis = yAxisData;
          yAxis = xAxisData;
        }

        let option = {
          title: dataOption.title,
          tooltip: dataOption.tooltip,
          grid: grid,
          xAxis: xAxis,
          yAxis: yAxis,
          series: seriesData
        };
        this.chart.setOption(option, true);

        if (dataOption.isLink == true) {
          this.chart.off("click");
          this.chart.on("click", params => {
            //设置参数
            let arrObject = {
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

      }
    },

  }
};
</script>
