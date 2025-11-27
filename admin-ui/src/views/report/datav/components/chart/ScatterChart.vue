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
      let dataOption = JSON.parse(JSON.stringify(this.dataOption));

      let seriesData = [];
      let data = {
        data: []
      };

      //散点大小
      if (dataOption.symbolSize == null) {
        data.symbolSize = 20;
      } 
      else {
        data.symbolSize = dataOption.symbolSize;
      }

      data.type = "scatter";

      //单组散点
      
      if(typeof result[0][0] === Number){
        let scatterData = [];
        for (const value of result) {
          scatterData.push(value);
        }
        data.data = scatterData;
        data.itemStyle = {
          color: dataOption.color
        }
        seriesData.push(data);
      }
      //多组散点
      else{
        for (const value of result) {
          
          let mulData = [value.x, value.y ];
          data.data.push(mulData);
          // if(result[0][0].length > 2){

          //   mulData.symbolSize =  data => {
          //     return data[2];
          //   }; 
          // }
          
        }
        seriesData.push(data);
      }
 
      if (dataOption.isSplitLine == false) {
        dataOption.xAxis.splitLine.show = false;
        dataOption.yAxis.splitLine.show = false;
      }

      let option = {
        tooltip:{
          show:true,
          formatter: params => {
            if(params.data.length > 2){
              return params.data[3]+"：" +params.data[2]
            }
            
          }
        },
        title: dataOption.title,
        xAxis: dataOption.xAxis,
        yAxis: dataOption.yAxis,
        // color: dataOption.color,
        series: seriesData
      };

      this.chart.setOption(option, true);

      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
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
