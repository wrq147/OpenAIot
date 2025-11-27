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
import {heatDataHandle} from '../../util/commonChartChange'
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
    setChartVal(resData,rowGlobal) {
      // console.log("setChartValsetChartValsetChartValHeat",this.chart,this.chartOption.theme);
      let result=[]
      // let result=resData
      if(this.chartOption&&this.chartOption.dataSourceType=="gobal"&&this.chartOption.globalData&&this.chartOption.globalProcessor){
        result =heatDataHandle(rowGlobal,this.chartOption)?heatDataHandle(rowGlobal,this.chartOption):[]
      }else{
        result = this.chartOption.staticDataValue;
      }
      if (this.chart == null) {
        echarts.registerTheme("customTheme", this.chartOption.theme);
        this.chart = echarts.init(this.$el, "customTheme");
      }
      // console.log("this.chartthis.chartthis.chart",result);
      let option={}
      try {
        let dataOption = JSON.parse(JSON.stringify(this.dataOption));

        if(result&&result[0]){
          dataOption.xAxis.data = result[0].xAxisData;
          dataOption.yAxis.data = result[0].yAxisData;
          dataOption.series[0].data = result[0].data.map(
            function(item) {
              return [item[1], item[0], item[2] || "-"];
            }
          );
        }

        option = {
          title: dataOption.title,
          tooltip: dataOption.tooltip,
          grid: {
            top: "15%",
            left: "2%",
            right: "20%",
            bottom: "3%",
            containLabel: true
          },
          visualMap: dataOption.visualMap,
          xAxis: dataOption.xAxis,
          yAxis: dataOption.yAxis,
          series: dataOption.series[0]
        };
      } catch (error) {
        console.log("error报错",error);
      }
      console.log(option, '=====option')

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

<style></style>
