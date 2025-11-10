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
    clearTimeout(this.timer);
  },
  methods: {
    setChartVal(result) {
      
      if (this.chart == null) {
        echarts.registerTheme("customTheme", this.chartOption.theme);
        this.chart = echarts.init(this.$el, "customTheme");
      }
      let dataOption = JSON.parse(JSON.stringify(this.dataOption))

      if (dataOption.animate != null) {
        //添加动画样式
        //animateUtil.addAnimate(dataOption.bindingDiv, dataOption.animate);
      }

      let seriesData = [];
      let data = {};
      data.type = "line";
      let xdata = []
      // let axisData = result[0].axisData;
      // dataOption.xAxis.data = axisData;
      dataOption.yAxis.splitLine.lineStyle.type = dataOption.lineStyle;
      
      if (dataOption.isSmooth == true) {
        data.smooth = true;
      } else if (dataOption.isSmooth == false) {
        data.smooth = false;
      }
      data.itemStyle = {
        normal: {
          //color: color[index],
          lineStyle: {
            //color: color[index],
            width: 2
          },
          areaStyle: {
            color: new echarts.graphic.LinearGradient(0, 1, 0, 0, [
              {
                offset: 0,
                color: "rgba(58,132,255,0)"
              },
              {
                offset: 1,
                color: "rgba(58,132,255,0.35)"
              }
            ])
          }
        }
      };
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

      let option = {
        title: dataOption.title,
        tooltip: dataOption.tooltip,
        legend: dataOption.legend,
        grid: {
          top: "20%",
          left: "2%",
          right: "5%",
          bottom: "3%",
          containLabel: true
        },
        xAxis: xAxisTemp,
        yAxis: yAxisTemp,
        series: seriesData
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
