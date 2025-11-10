<!--仪表盘 -->
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


      dataOption.series[0].data = result;
      
      var color = [];
      if (this.chartOption.theme != null) {
        color = this.chartOption.theme.color;
      } else {
        color = dataOption.color;
      }

      //if (dataOption.istopic == true) {
      if (this.chartOption.theme != '') {
        //color = dataOption.thiscolor;
        color = this.chartOption.theme.color;
        dataOption.thiscolor = [
          [0.2, color[0]],
          [0.8, color[1]],
          [1, color[2]]
        ];
        dataOption.series[0].axisLabel.color = color[2];
      }
      dataOption.series[0].detail.color = dataOption.thiscolor[0][1];
      dataOption.series[0].axisLine.lineStyle.color = dataOption.thiscolor;
      if (dataOption.isGradients == true) {
        let thiscolor = dataOption.thiscolor;
        let thatcolor = [];
        for (let i = 0; i < thiscolor.length; i++) {
          let thecolor = {};
          thecolor.offset = thiscolor[i][0];
          thecolor.color = thiscolor[i][1];
          thatcolor.push(thecolor);
        }

        dataOption.series[0].axisLine.lineStyle.color = [
          [1, new echarts.graphic.LinearGradient(0, 0, 1, 0, thatcolor)]
        ];
      }

      if (dataOption.isClockwise == false) {
        dataOption.series[0].clockwise = false;
      } else {
        dataOption.series[0].clockwise = true;
      }

      if (dataOption.isAxisLineShow == false) {
        dataOption.series[0].axisLine.show = false;
      } else {
        dataOption.series[0].axisLine.show = true;
      }

      if (dataOption.isSplitLineShow == false) {
        dataOption.series[0].splitLine.show = false;
      } else {
        dataOption.series[0].splitLine.show = true;
      }

      if (dataOption.isAxisTickShow == false) {
        dataOption.series[0].axisTick.show = false;
      } else {
        dataOption.series[0].axisTick.show = true;
      }

      if (dataOption.isAxisLabelShow == false) {
        dataOption.series[0].axisLabel.show = false;
      } else {
        dataOption.series[0].axisLabel.show = true;
      }
      dataOption.series[0].radius = dataOption.radius;

      var option = {
        title: dataOption.title,
        tooltip: dataOption.tooltip,
        legend: dataOption.legend,
        series: dataOption.series,
        color: color,
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
