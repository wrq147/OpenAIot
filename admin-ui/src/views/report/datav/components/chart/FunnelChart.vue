<!--漏斗图 -->
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
      let dataOption = JSON.parse(JSON.stringify(this.dataOption))

      if (dataOption.animate != null) {
        //添加动画样式
        //animateUtil.addAnimate(dataOption.bindingDiv, dataOption.animate);
      }

      var option = {
        title: dataOption.title,
        tooltip: dataOption.tooltip,
        legend: dataOption.legend,
        series: []
      };

      if (dataOption.sort == "ascDes") {
        var length = Math.round(result.length / 2);
        var data1 = [];
        console.log(length,'====length')
        for (var i = 0; i < length; i++) {
          data1.push(result[i]);
        }
        var data2 = [];
        for (var i = length; i < result.length; i++) {
          data2.push(result[i]);
        }
        console.log(length, data1, data2);
        option.series = [
          {
            name: "系列名称",
            type: "funnel",
            maxSize: "120%",
            top: "10%",
            height: "40%",
            right: "50%",
            data: data1,
            sort: "descending",
            emphasis: {
              itemStyle: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: "rgba(0, 0, 0, 0.5)"
              }
            }
          },
          {
            name: "系列名称",
            type: "funnel",
            maxSize: "120%",
            top: "50%",
            height: "40%",
            right: "50%",
            sort: "ascending",
            data: data2,
            emphasis: {
              itemStyle: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: "rgba(0, 0, 0, 0.5)"
              }
            }
          }
        ];
      } else if (dataOption.sort == "desAsc") {
        var length = Math.round(result.length / 2);
        var data1 = [];
        for (var i = 0; i < length; i++) {
          data1.push(result[i]);
        }
        var data2 = [];
        for (var i = length; i < result.length; i++) {
          data2.push(result[i]);
        }
        console.log(length, data1, data2);
        option.series = [
          {
            name: "系列名称",
            type: "funnel",
            maxSize: "120%",
            top: "10%",
            height: "40%",
            right: "50%",
            data: data1,
            sort: "ascending",
            emphasis: {
              itemStyle: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: "rgba(0, 0, 0, 0.5)"
              }
            }
          },
          {
            name: "系列名称",
            type: "funnel",
            maxSize: "120%",
            top: "50%",
            height: "40%",
            right: "50%",
            sort: "descending",
            data: data2,
            emphasis: {
              itemStyle: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: "rgba(0, 0, 0, 0.5)"
              }
            }
          }
        ];
      } else {
        option.series.push(dataOption.series[0]);
        option.series[0].sort = dataOption.sort;
        option.series[0].data = result;
      }
     
      this.chart.setOption(option, true);

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
              this.chartOption.drillDownChartOption,
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
