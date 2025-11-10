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
      let dataOption = JSON.parse(JSON.stringify(this.dataOption));
      
      //数据值
      let seriesData = [];
      let legend = new Set();
      let xData =  new Set();
      let typeData = new Set();
      let yAxisIndex = new Set();
     
      for (let dataIndex of result) { 
        xData.add(dataIndex.name)
        legend.add(dataIndex.series);
        typeData.add(dataIndex.type);
        yAxisIndex.add(dataIndex.yAxisIndex)
      }
      xData = [...xData]
      legend = [...legend]
      typeData = [...typeData]
      yAxisIndex = [...yAxisIndex]
      for (let index in legend) {
        let data = {
          data: []
        };
        data.type = typeData[index]

        if (data.type === "line") {
          data.smooth = dataOption.isSmooth;
        }

        let axisIndex = yAxisIndex[index].yAxisIndex != undefined
            ? yAxisIndex[index].yAxisIndex : data.type === "line" ? 1 : 0;

        if (dataOption.isVertical==true) {
          data.xAxisIndex = axisIndex;
        } else {
          data.yAxisIndex = axisIndex;
        }

        data.name = legend[index];
        for (let dataIndex of result) { 
          if (legend[index] === dataIndex.series){
            data.data.push(dataIndex.value)
          }
            
        }
      
        data.barWidth = dataOption.barWidth;
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
        //是否显示最大最小值
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

        //柱状图堆叠展示
        if (dataOption.isStack) {
          if (data.type === "bar") {
            data.stack = "stack";
          }
        }
        seriesData.push(data);
      }
      //图例
      let legendData = {
        data: legend,
        show: dataOption.showLegend,
        x: (dataOption.legendX || 35) + "%",
        y: (dataOption.legendY || 5) + "%",
        orient:
          dataOption.legendOrient == undefined
            ? "horizontal"
            : dataOption.legendOrient,
        textStyle: {
          fontSize:
            dataOption.legendFontSize == undefined
              ? "14"
              : dataOption.legendFontSize,
          color:
            dataOption.legendFontColor == undefined
              ? "#fff"
              : dataOption.legendFontColor
        },
        itemWidth: dataOption.itemWidth || 14,
        itemHeight: dataOption.itemHeight || 8
      };

      //刻度单位
      let yAxisData = dataOption.yAxis;

      if (yAxisData[0].axisLabel.formatter.indexOf("{value}") == -1) {
        yAxisData[0].axisLabel.formatter =
          "{value}" + yAxisData[0].axisLabel.formatter;
      }
      if (yAxisData[1].axisLabel.formatter.indexOf("{value}") == -1) {
        yAxisData[1].axisLabel.formatter =
          "{value}" + yAxisData[1].axisLabel.formatter;
      }

      //分类名称
      let xAxisData = dataOption.xAxis;
      xAxisData[0].data = xData;

      let axisTick = {
        show: false
      };

      //设置X轴Y轴刻度不显示
      xAxisData[0].axisTick = axisTick;
      yAxisData[0].axisTick = axisTick;
      yAxisData[1].axisTick = axisTick;

      //设置分隔线样式
      if (dataOption.yAxis[0].splitLine.show) {
        let splitLineStyle = {
          color:
            dataOption.splitLineColor == undefined
              ? "#ccc"
              : dataOption.splitLineColor,
          width:
            dataOption.splitLineWidth == undefined
              ? 1
              : dataOption.splitLineWidth,
          type:
            dataOption.lineStyle == undefined ? "solid" : dataOption.lineStyle
        };
        yAxisData[0].splitLine.lineStyle = splitLineStyle;
      }

      //设置y轴颜色
      yAxisData[1].axisLine.lineStyle.color =
        yAxisData[0].axisLine.lineStyle.color;
      //设置y轴粗细
      yAxisData[0].axisLine.lineStyle.width =
        dataOption.yLineWidth == undefined ? 1 : dataOption.yLineWidth;
      yAxisData[1].axisLine.lineStyle.width =
        dataOption.yLineWidth == undefined ? 1 : dataOption.yLineWidth;

      //设置X轴标签样式
      let xAxisLabel = {
        textStyle: {
          show: true,
          color:
            typeof dataOption.xFontColor == "undefined"
              ? "#fff"
              : dataOption.xFontColor == null
              ? "transparent"
              : dataOption.xFontColor,
          fontSize:
            dataOption.xFontSize == undefined ? 14 : dataOption.xFontSize
        }
      };

      xAxisData[0].axisLabel = xAxisLabel;
      //设置x轴粗细
      xAxisData[0].axisLine.lineStyle.width =
        dataOption.xLineWidth == undefined ? 1 : dataOption.xLineWidth;

      //设置Y轴标签样式
      let textStyle = {
        show: true,
        color:
          typeof dataOption.yFontColor == "undefined"
            ? "#fff"
            : dataOption.yFontColor == null
            ? "transparent"
            : dataOption.yFontColor,
        fontSize: dataOption.yFontSize == undefined ? 14 : dataOption.yFontSize
      };

      yAxisData[0].axisLabel.textStyle = textStyle;
      yAxisData[1].axisLabel.textStyle = textStyle;

      //设置折线粗细
      let lineStyle = {
        normal: {
          width: dataOption.lineWidth == undefined ? 2 : dataOption.lineWidth
        }
      };
      //设置折线标记大小
      seriesData.forEach(element => {
        if (element.type == "line") {
          element.lineStyle = lineStyle;
          element.symbolSize =
            dataOption.lineSymbolSize == undefined
              ? 5
              : dataOption.lineSymbolSize;
        }
      });

      //设置提示框文字样式
      // let tooltipLabel = {

      //     backgroundColor :'transparent',
      //      color: "#fff",

      // }
      dataOption.tooltip.axisPointer.type = "line";

      //设置坐标轴名称样式
      let nameTextStyle = {
        color: dataOption.yFontColor,
        fontSize: dataOption.yFontSize
      };

      yAxisData[0].nameTextStyle = nameTextStyle;
      yAxisData[1].nameTextStyle = nameTextStyle;

      let option = {
        title: dataOption.title,
        tooltip: dataOption.tooltip,
        legend: legendData,
        grid: {
          top: dataOption.gridTop==undefined?"25%":dataOption.gridTop+"%",
          left: dataOption.gridLeft==undefined?"2%":dataOption.gridLeft+"%",
          right: dataOption.gridRight==undefined?"2%":dataOption.gridRight+"%",
          bottom: dataOption.gridBottom==undefined?"5%":dataOption.gridBottom+"%",
          containLabel: true
        },
        toolbox: dataOption.toolbox,
        xAxis: xAxisData,
        // yAxis: yAxisData,
        series: seriesData
      };

      if(dataOption.isVertical==true){
        option.xAxis=yAxisData;
        option.yAxis=xAxisData;
      }else{
        option.xAxis=xAxisData;
        option.yAxis=yAxisData;
      }
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

    },
   
  }
};
</script>
