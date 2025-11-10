<template>
  <div :class="animate" :style="{ height: height, width: width,...bgImageStyle }" :id="chartOption.bindingDiv" ref="chartDiv"/>
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
  computed:{
    bgImageStyle(){
      //背景的样式
      let styleobj={}
      if(this.chartOption.bgSetting){
        if(this.chartOption.bgSetting.backgroundType=='img'&&this.chartOption.bgSetting.backgroundImg){
          styleobj.backgroundColor='transparent'
          styleobj.backgroundImage = `url(${this.chartOption.bgSetting.backgroundImg}) `;
          styleobj.backgroundSize = "100% 100%";
          styleobj.backgroundRepeat = "no-repeat";
          styleobj.borderRadius=this.chartOption.bgSetting.radius+'px'
        }else if(this.chartOption.bgSetting.backgroundType=='color'&&this.chartOption.bgSetting.backgroundColor){
          styleobj.backgroundColor=this.chartOption.bgSetting.backgroundColor
          styleobj.borderRadius=this.chartOption.bgSetting.radius+'px'
        }
      }
      return styleobj
    }
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
      try {
        if (this.chart == null) {
          echarts.registerTheme("customTheme", this.chartOption.theme);
          this.chart = echarts.init(this.$el, "customTheme");
        }
        let dataOption = JSON.parse(JSON.stringify(this.dataOption))
        let data = result;
        let yAxisData = new Set();
        let legendData = [];
        let valueData = {};

        for (let d in data) {
          //图例
          legendData.push(data[d].name);
          yAxisData.add(data[d].label);
        }
        legendData = [...new Set(legendData)]
        for (let d in legendData) {
          let valueArr = [];
          for (let v in data) {
            if (data[v].name === legendData[d]) {
                valueArr.push([data[v].value,data[v].label]);
              }
          }
          valueData[d] = valueArr;
        }
        // console.log("结果数据",valueData);
        // let top = 60;
        // let bottom = 60;
        // let left = 60;
        // let right = 60;
        // let width=88
        yAxisData = [...yAxisData];

        if (dataOption.istitle == false) {
          dataOption.title.show = false;
        } else {
          dataOption.title.show = true;
        }
        //*******************************************使用主题******************************************************/
        let color = [];
        if (dataOption.isThemeColor) {
          color = this.chartOption.theme.color;
          dataOption.leftColor = color[0];
          dataOption.rightColor = color[1];
        }
        let barRadius=0
        if(dataOption.barRadius){//柱体弧度
          barRadius=dataOption.barRadius
        }
        //图例设置
        let legends = [];
        dataOption.legend.height = "80%"; //图例显示区域的高度
        dataOption.legend.itemGap = dataOption.legendItemGapY || 10; //图例垂直方向间隔，默认10
        dataOption.legend.textStyle.fontSize = dataOption.legendFontSize; //图例字号
        dataOption.legend.textStyle.color = dataOption.legendFontColor; //图例字号
        dataOption.legend.icon = dataOption.icon; //图例形状
        dataOption.legend.itemWidth = dataOption.legend.itemWidth; //图例形状
        dataOption.legend.itemHeight = dataOption.legend.itemHeight; //图例形状
        let colNumber = dataOption.legendCols || 1; //选择图例分布的列数，默认1
        let legendCols = []; //用于装载每一列图例显示的标签
        let items = legendData; // 图例的数据

        for (let i = 0; i < colNumber; i++) {
          let legendData1 = [];
          let k = 0;
          for (let j = i; j < items.length; j += colNumber) {
            //以列数为步长
            legendData1[k] = items[j];
            
            k++;
          }
          legendCols[i] = legendData1;
        }
        for (let i = 0; i < legendCols.length; i++) {
          let legendTemp = JSON.parse(JSON.stringify(dataOption.legend));
          legendTemp.left =(dataOption.legendPositionLeft + (dataOption.legendItemGapX == undefined ? 0 : dataOption.legendItemGapX) * i || 60) + "%"; //图例距离左边框 + 间隔的距离，默认60%
          legendTemp.top = (dataOption.legendPositionTop || 30) + "%"; //图例距离上边框距离，默认30%
          legendTemp.data = legendCols[i];
          legends.push(legendTemp);
        }
        // dataOption.title.top="2%"
        // dataOption.title.textAlign="left"
        // dataOption.title.left="3%"
        let dataSeries=[]
        for(let valkey in valueData){
          let obj={
              type: "bar",
              barWidth: dataOption.barWidth,
              label: {
                show: dataOption.label.isShow,
                fontFamily: "Rubik-Medium",
                fontSize: dataOption.label.fontSize,
                distance: dataOption.label.distance,//距离
                position:dataOption.label.position,
                color:dataOption.label.color,
              },
              name: legendData[valkey],
              // label: {
              //   position: "left"
              // },
              itemStyle: {
                color: dataOption.barColorlist[valkey], //左侧柱颜色
                barBorderRadius: [0, barRadius, barRadius, 0]
              },
              data: valueData[valkey]
            }
            dataSeries.push(obj) 
        }
        let option = {
          title: dataOption.title,
          tooltip: {
            show: true,
            trigger: "axis",
            axisPointer: {
              type: "shadow"
            }
          },
          legend:legends ,
          grid: {
            //图
            left: dataOption.bgSetting.left,
            right: dataOption.bgSetting.right,
            width: dataOption.width+"%",
            containLabel: true,
            top: dataOption.bgSetting.top,
            bottom:dataOption.bgSetting.bottom
          },
          xAxis: {
            //横轴
            type: "value",
            // inverse: true,
            axisLabel: {
              show: dataOption.xAxis.isShowAxisLabel,
              color: dataOption.xAxis.axisLabelColor,   // 刻度标签文字的颜色
              fontStyle: dataOption.xAxis.axisLabelFontStyle,  // 字体的风格（normal无样式；italic斜体；oblique倾斜字体）         
              fontWeight: dataOption.xAxis.axisLabelFontWeight,  // 字体的粗细（normal无样式；bold加粗；bolder加粗再加粗；lighter变细；数字定义粗细也可以取值范围100至700）
              fontSize: dataOption.xAxis.axisLabelFontSize, //文字字体大小
              align: dataOption.xAxis.axisLabelAlign,     // 文字水平对齐方式，默认自动（left/center/right）
              verticalAlign: 'center',    // 文字垂直对齐方式，默认自动（top/middle/bottom)
              lineHeight: '20',
              interval: '0',  //坐标轴刻度标签的显示间隔，在类目轴中有效.0显示所有
              inside: dataOption.xAxis.axisLabelInside, //刻度标签是否朝内，默认朝外
              rotate: dataOption.xAxis.axisLabelRotate, //刻度标签旋转的角度，在类目轴的类目标签显示不下的时候可以通过旋转防止标签之间重叠；旋转的角度从-90度到90度
              margin: dataOption.xAxis.margin,
              
            },
            axisLine: {
              show: dataOption.xAxis.isShowAxisLine,
              lineStyle: {
                color: dataOption.xAxis.axisLineColor,
                width: dataOption.xAxis.axisLineWidth,
                type: dataOption.xAxis.axisLineType// 坐标轴线线的类型（solid实线类型；dashed虚线类型；dotted点状类型）
              }
            },
            axisTick: {
              show: dataOption.xAxis.isShowAxisTick,
              length: dataOption.xAxis.axisTickInside,
              inside: dataOption.xAxis.axisTickLength,     // 坐标轴刻度是否朝内，默认朝外
              lineStyle: {
                color: dataOption.xAxis.axisTickColor,
                width: dataOption.xAxis.axisTickWidth,
                type: dataOption.xAxis.axisTickType
              }
            },
            splitLine: {
              show: dataOption.xAxis.isShowSplitLine,
              lineStyle: {
                color: dataOption.xAxis.splitLineColor,
                width: dataOption.xAxis.splitLineWidth,
                type: dataOption.xAxis.splitLineType, // 坐标轴线线的类型（solid实线类型；dashed虚线类型；dotted点状类型）
              }
            },
            position: dataOption.position,
            show:dataOption.xAxisShow,
            
          },
          yAxis: {
            position: dataOption.yAxis.position,
            type: "category",
            // inverse: false,
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
            show:dataOption.yAxisShow,
            data: yAxisData
          },
          series: dataSeries,///
        };
        // console.log("echarts属性",option);
        this.chart.setOption(option, true);

        addOption(dataOption.bindingDiv, option);
        //开启图表联动
        if (dataOption.isLink == true) {
          this.chart.off("click");
          this.chart.on("click", params => {
            //设置参数
            let arrObject = {
              legendName: params.seriesName,
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
              legendName: params.seriesName,
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
      } catch (error) {
        console.log('出错了',error);
      }

    },
   
  }
};
</script>
