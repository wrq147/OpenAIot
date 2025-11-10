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
      let axisData = new Set();
      let resultName = new Set();
      // dataOption.xAxis.data = axisData;
      for (let d in result) {
        //图例
        axisData.add(result[d].series);
        resultName.add(result[d].name);
      }
      axisData = [...axisData]
      resultName = [...resultName]
      let xAxisTemp = {};
      let yAxisTemp = {};
      xAxisTemp = { ...dataOption.xAxis };
      yAxisTemp = { ...dataOption.yAxis };
      //是否竖显示
      if (dataOption.isVertical) {
        xAxisTemp = dataOption.yAxis;
        yAxisTemp = dataOption.xAxis;
      }
      for (let d in resultName) {
        let data = {
          data: []
        };
        result.forEach((item, index) => {
          if (item.name == resultName[d]) {
            data.name = item.name;
            data.type = "bar";
            data.barWidth = dataOption.barWidth; // ���图宽度
            data.data.push(item.data);
            data.stack = item.stack;
          }
        });
        seriesData.push(data);
      }
      xAxisTemp.data = axisData
      //图例
      let legendData = {
        //  data:legend,
        show: dataOption.legend.show,
        x: (dataOption.legendX || 35) + '%',
        y: (dataOption.legendY || 5) + '%',
        orient: dataOption.legendOrient == undefined ? "horizontal" : dataOption.legendOrient,
        textStyle: {
          fontSize: dataOption.legendFontSize == undefined ? "14" : dataOption.legendFontSize,
          color: dataOption.legendFontColor == undefined ? "#fff" : dataOption.legendFontColor
        },
        itemWidth: dataOption.itemWidth || 14,
        itemHeight: dataOption.itemHeight || 8
      };

      var option = {
        title: dataOption.title,
        tooltip: dataOption.tooltip,
        legend: legendData,
        grid: {
          top: "25%",
          left: "2%",
          right: "10%",
          bottom: "3%",
          containLabel: true
        },
        xAxis: xAxisTemp,
        yAxis: yAxisTemp,
        series: seriesData
      };

      if (!dataOption.isShowLegend) {
        option.legend = { show: false };
      }
      this.chart.setOption(option, true);

      addOption(dataOption.bindingDiv, option);

      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            legendName: params.name,
            seriesName: params.seriesName,
            data: params.data
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
  
  },
};
</script>
