<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="chartDiv"/>
</template>

<script>
import echarts from "echarts";
require("echarts/theme/macarons"); // echarts theme
import resize from '@/views/dashboard/mixins/resize'

import '../../animate/animate.css'


import { getLinkChart} from "../../util/LinkageChart";

import VueEvent from '../../VueEvent'
import { addOption } from '../../codegen/codegen'
import dataChart from '../mixins/dataChart.js'
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
    drawingList:{
      type: Array
    }
  },
  data() {
    return {
      chart: null,
      animate:this.className,
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
      handler(value){
        this.animate = value;
      }
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
    setChartVal(result) {
      if (this.chart == null) {
        echarts.registerTheme('customTheme', this.chartOption.theme);
        this.chart = echarts.init(this.$el, 'customTheme');
      }

      let dataOption = JSON.parse(JSON.stringify(this.dataOption));
      if(dataOption.isRoseType == true){
        dataOption.series[1].roseType = true;
      }else{
        dataOption.series[1].roseType = false;
      }
      dataOption.series[0].name = dataOption.tooltipName;
      dataOption.series[1].name = dataOption.tooltipName;
      dataOption.series[1].radius[0] = dataOption.radiusI + '%';
      dataOption.series[1].radius[1] = dataOption.radiusO + '%';
      dataOption.series[1].center[0] = dataOption.centerX + '%';
      dataOption.series[1].center[1] = dataOption.centerY + '%';
      dataOption.legend.left = dataOption.legendcenterX + '%';
      dataOption.legend.top = dataOption.legendcenterY + '%';
      

      let xAxisData = [];
      let yAxisData = [];
      for( let value of result) {
        xAxisData.push(value.name);
        yAxisData.push(value.value); 
      };
      dataOption.xAxis.data     = xAxisData;
      dataOption.series[0].data = yAxisData;
      dataOption.series[1].data = result;
        
      let xAxisTemp, yAxisTemp;
      xAxisTemp = dataOption.xAxis;
      yAxisTemp = dataOption.yAxis;
      //是否竖显示
      if (dataOption.isVertical) {
          
        xAxisTemp = dataOption.yAxis;
        yAxisTemp = dataOption.xAxis;
      }
        
      //是否显示平均线
      if(dataOption.isMarkLine == true) {
        dataOption.series[0].markLine = {
            data: [
                  {type: 'average', name: '平均值'}
              ]
          }
      } else if (dataOption.isMarkLine == false || dataOption.isMarkLine == undefined) {
          dataOption.series[0].markLine = {}
      }

      //是否显示最大值和最小值
      if(dataOption.isMarkPoint == true) {
        dataOption.series[0].markPoint = {
            data: [
                {type: 'max', name: '最大值'},
                {type: 'min', name: '最小值'}
            ]
          }
      } else if (dataOption.isMarkPoint == false || dataOption.isMarkPoint == undefined) {
          dataOption.series[0].markPoint = {}
      }
      
      let option = {
          title: dataOption.title,
          tooltip: {//提示
              trigger: 'item',//饼图
              formatter: "{b} : {c}" + dataOption.unit,
          },
          legend: dataOption.legend,
          xAxis: xAxisTemp,
          yAxis: yAxisTemp,
          series: dataOption.series,
          //grid: {top: '55%'},
          grid: {
              top: 80,
              bottom: 30,
              left:150,
          },      
      };

        this.chart.setOption(option, true);

        addOption(dataOption.bindingDiv, option);

        //开启图表联动
        if(dataOption.isLink == true){
          this.chart.off("click");
          this.chart.on('click', params => {
            //设置参数
            let arrObject = {             
              "legendName":params.name,
              "data":params.value
            }

            let arrs = JSON.stringify(arrObject);
            
            //获取绑定的图表
            let bindList = dataOption.bindList;
            
            if(bindList.length > 0){
              getLinkChart(dataOption.arrName, arrs, bindList,this.drawingList)
            }

          })
      }
      //开启图表下钻
      else if(dataOption.isDrillDown == true){
          this.chart.off("click");
          this.chart.on('click', params => {
            //设置参数
            let arrObject = {             
              "legendName":params.name,
              "data":params.value
            }

            let arrs = JSON.stringify(arrObject);
            
            //获取绑定的图表
            let drillDownChartOption = this.chartOption.drillDownChartOption;
                          
            if(drillDownChartOption != undefined && drillDownChartOption != null){
              
              this.$set(this.chartOption.drillDownChartOption.chartOption, 'requestParameters',[{"name":"drillParam","value":arrs}]);
              //发送下钻消息
              VueEvent.$emit("drill_down_msg", this.chartOption.drillDownChartOption);

            }
            
          })
      }
      else{
        //关闭图表联动，取消echart点击事件
        this.chart.off('click');
      }

    },
   
  }
};
</script>
