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
      //*******************************************使用主题******************************************************/
      let color = [];
      if (dataOption.isThemeColor) {
        color = this.chartOption.theme.color;
        dataOption.gradientsColorStart = color[0];
        dataOption.gradientsColorEnd = color[1];
      }

      let backgroundData = [];
      for (let index of result) {
        backgroundData.push(dataOption.backgroundData);
      }
      if(dataOption.subtextType&&dataOption.subtextType=='1'){//副标题展示总数
        let total=0
        for(let i=0;i<result.length;i++){
          total=total+result[i].value
        }
        dataOption.title.subtext=total
      }
      //图例设置
      let legends = [];
      dataOption.legend.height = "80%"; //图例显示区域的高度
      dataOption.legend.itemGap = dataOption.legendItemGapY || 10; //图例垂直方向间隔，默认10
      dataOption.legend.textStyle.fontSize = dataOption.legendFontSize; //图例字号
      dataOption.legend.itemStyle={}
      dataOption.legend.itemStyle.color = dataOption.legendItemColor; //图例字号
      dataOption.legend.icon = dataOption.icon; //图例形状
      dataOption.legend.itemWidth = dataOption.legend.itemWidth; //图例形状
      dataOption.legend.itemHeight = dataOption.legend.itemHeight; //图例形状
      dataOption.legend.left = dataOption.legendPositionLeft; //图例形状
      dataOption.legend.top = dataOption.legendPositionTop; //图例形状
      dataOption.legend.data =[{name:dataOption.legendText}]
      legends = JSON.parse(JSON.stringify(dataOption.legend));
      let option = {
        title: dataOption.title,
        legend:{...legends,
          formatter:(name)=>{
            if(dataOption.isHideLegendText){
              return ''; // 返回自定义的文本
            }else{
              return name; // 返回自定义的文本
            }
            
          }
        } ,
        grid: {
          left: dataOption.left + "%",
          right: dataOption.right + "%",
          bottom: dataOption.bottom + "%",
          top: dataOption.top + "%",
          containLabel: true
        },
        tooltip: {
          trigger: "axis",
          axisPointer: {
            type: "none"
          },
          formatter: function(params) {
            return (
              params[0].name +
              "：" +
              (params[0].value > 10000
                ? Number(
                    (params[0].value.toFixed(4) / 10000).toFixed(2)
                  ).toLocaleString() + " 万<br/>"
                : params[0].value)
            );
          }
        },
        xAxis: [{
          show: false,
          type: "value"
        }],
        yAxis: [
          {
            type: "category",
            inverse: true,
            axisLabel: {
              show: true,
              textStyle: {
                color: dataOption.fontColor,
                fontSize: dataOption.fontSize
              }
            },
            axisLine:{
              show:false
            },
            axisTick:{
              show:false
            },
            splitLine:{
              show:false
            },
            data: result.yAxisTwoData,
          },
          {
            type: "category",
            inverse: true,
            show: true,
            axisLabel: {
              show: dataOption.yAxis.isShowAxisLabel,
              color: dataOption.yAxis.axisLabelColor,   // 刻度标签文字的颜色
              fontStyle: dataOption.yAxis.axisLabelFontStyle,  // 字体的风格（normal无样式；italic斜体；oblique倾斜字体）         
              fontWeight: dataOption.yAxis.axisLabelFontWeight,  // 字体的粗细（normal无样式；bold加粗；bolder加粗再加粗；lighter变细；数字定义粗细也可以取值范围100至700）
              fontSize: dataOption.yAxis.axisLabelFontSize, //文字字体大小
              interval: '0',  //坐标轴刻度标签的显示间隔，在类目轴中有效.0显示所有
              inside: dataOption.yAxis.axisLabelInside, //刻度标签是否朝内，默认朝外
              margin: dataOption.yAxis.margin,
              textStyle: {
                verticalAlign: dataOption.yAxis.isSetLabelPosition?dataOption.yAxis.axisLabelVerticalAlign:null,
                align: dataOption.yAxis.isSetLabelPosition?dataOption.yAxis.axisLabelAlign:null,
                padding: dataOption.yAxis.isSetLabelPosition?[dataOption.yAxis.axisLabelPaddingTop?dataOption.yAxis.axisLabelPaddingTop:0, dataOption.yAxis.axisLabelPaddingRight?dataOption.yAxis.axisLabelPaddingRight:0, dataOption.yAxis.axisLabelPaddingBottom?dataOption.yAxis.axisLabelPaddingBottom:0,dataOption.yAxis.axisLabelPaddingLeft?dataOption.yAxis.axisLabelPaddingLeft:0]:null
              },
              formatter: function(value) {
                if (value >= 10000) {
                  return (value / 10000).toLocaleString() + "万";
                } else {
                  return value.toLocaleString();
                }
              }
            },
            axisLine: {
              show: dataOption.yAxis.isShowAxisLine,
              lineStyle: {
                color: dataOption.yAxis.axisLineColor,
                width: dataOption.yAxis.axisLineWidth,
                type: dataOption.yAxis.axisLineType// 坐标轴线线的类型（solid实线类型；dashed虚线类型；dotted点状类型）
              }
            },
            axisTick: {
              show: dataOption.yAxis.isShowAxisTick,
              length: dataOption.yAxis.axisTickInside,
              inside: dataOption.yAxis.axisTickLength,     // 坐标轴刻度是否朝内，默认朝外
              lineStyle: {
                color: dataOption.yAxis.axisTickColor,
                width: dataOption.yAxis.axisTickWidth,
                type: dataOption.yAxis.axisTickType
              }
            },
            splitLine: {
              show: dataOption.yAxis.isShowSplitLine,
              lineStyle: {
                color: dataOption.yAxis.splitLineColor,
                width: dataOption.yAxis.splitLineWidth,
                type: dataOption.yAxis.splitLineType, // 坐标轴线线的类型（solid实线类型；dashed虚线类型；dotted点状类型）
              }
            },
            data: result,
            position: dataOption.yAxis.position,
          }
        ],
        series: [
          {
            name: dataOption.name,
            type: "bar",
            zlevel: 1,
            label: {
              show: dataOption.label.isShow,
              fontFamily: "Rubik-Medium",
              fontSize: dataOption.label.fontSize,
              distance: dataOption.label.distance,//距离
              position:dataOption.label.position,
              color:dataOption.label.color,
              padding:[dataOption.label.labelPaddingTop,dataOption.label.labelPaddingRight,dataOption.label.labelPaddingBottom,dataOption.label.labelPaddingLeft],
              formatter: function(params) {
                return params.data.name;
              }
            },
            itemStyle: {
              normal: {
                barBorderRadius: dataOption.barBorderRadius,
                color: new echarts.graphic.LinearGradient(0, 0, 1, 0, [
                  {
                    offset: 0,
                    color: dataOption.gradientsColorStart
                  },
                  {
                    offset: 1,
                    color: dataOption.gradientsColorEnd
                  }
                ])
              }
            },
            barWidth: dataOption.barWidth,
            data: result
          },
          {
            name:dataOption.legendText,
            type: "bar",
            barWidth: dataOption.barWidth,
            barGap: "-100%",
            data: backgroundData,
            itemStyle: {
              normal: {
                color: dataOption.backgroundColor,
                barBorderRadius: dataOption.barBorderRadius
              }
            }
          }
        ]
      };
          
      this.chart.setOption(option, true);

      addOption(dataOption.bindingDiv, option);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            seriesName: params.name,
            data: result[params.dataIndex]
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
            data: result[params.dataIndex]
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
