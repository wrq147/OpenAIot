<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="chartDiv" />
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
  mixins: [resize, dataChart],
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
      try {
        if (this.chart == null) {
          echarts.registerTheme("customTheme", this.chartOption.theme);
          this.chart = echarts.init(this.$el, "customTheme",{renderer: 'canvas'});
        }

        let seriesData = [];
        let dataOption = JSON.parse(JSON.stringify(this.dataOption))
        let data = {};
        data.type = "bar";
        data.barWidth = dataOption.barWidth; // 柱图宽度
        if (dataOption.stack == true) {
          data.stack = "sum";
        } else if (dataOption.stack == false) {
          data.stack = "";
        }
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
        let raduis = dataOption.radius == undefined ? 12 : dataOption.radius
        let barBorderRadius = [];
        //设置圆角样式
        if (dataOption.itemStyle == true) {

          //非竖显示
          if (!dataOption.isVertical) {
            //半角
            if (dataOption.radiusType == "half") {
              barBorderRadius = [raduis, raduis, 0, 0]
            }else {//圆角
              barBorderRadius = raduis
            }
          } else {
              //半角
              if (dataOption.radiusType == "half") {
                barBorderRadius = [0, raduis, raduis, 0]
              }
              //圆角
              else {
                barBorderRadius = raduis
              }
          }
          data.itemStyle = {
            //柱形图圆角，初始化效果 barBorderRadius:[12, 12, 12, 12]
            barBorderRadius: barBorderRadius

          };
            
        } else if (dataOption.itemStyle == false) {
          data.itemStyle = { emphasis: {} };
        }
        if (dataOption.isLabel != undefined) {
          data.label = {
            show: dataOption.isLabel == undefined ? false : dataOption.isLabel,
            position: dataOption.labelPosition,
            distance: dataOption.labelDistance,
            align: 'left',
            verticalAlign: 'middle',
            rotate: dataOption.isVertical == true ? 0 : 90,
            formatter: '{c}  {name|{a}}',
            fontSize: dataOption.labelFontSize,
            color: dataOption.labelFontColor,
            rich: {
              name: {
                color: dataOption.labelFontColor,
                fontSize: dataOption.labelFontSize,
              }
            }
          }
        }
        data.data = []
        let xdata = []
        result.map((item, index) => {
          xdata.push(item.name);
          data.data.push(item.value);
        });
        seriesData = data
        
        let xAxisTemp = {};
        let yAxisTemp = {};
        xAxisTemp = { ...dataOption.xAxis };
        yAxisTemp = { ...dataOption.yAxis };
        //是否竖显示
        if (dataOption.isVertical) {
          xAxisTemp = dataOption.yAxis;
          yAxisTemp = dataOption.xAxis;
        }
        //设置重叠柱图
        if (dataOption.gap) {
          let z = 0;
          seriesData.forEach(element => {
            element.barGap = '-100%'
            element.z = z++
          });
        }

        //设置分隔线样式
        if (dataOption.yAxis.splitLine.show) {

          let splitLineStyle = {
            color: dataOption.splitLineColor == undefined ? '#ccc' : dataOption.splitLineColor,
            width: dataOption.splitLineWidth == undefined ? 1 : dataOption.splitLineWidth,
            type: dataOption.lineStyle == undefined ? 'solid' : dataOption.lineStyle,
          }
          //根据是否竖显示，设置x轴或者y轴分隔线
          if (yAxisTemp.splitLine != undefined) {
            yAxisTemp.splitLine.lineStyle = splitLineStyle
          } else {
            xAxisTemp.splitLine.lineStyle = splitLineStyle
          }

        }

        let axisTick = {
          show: false
        };

        //设置X轴Y轴刻度不显示
        xAxisTemp.axisTick = axisTick;
        xAxisTemp.data = [...xdata]
        xAxisTemp.axisLine.lineStyle.color = dataOption.xLineColor === undefined ? "#fff" : dataOption.xLineColor === null ? "transparent" : dataOption.xLineColor;
        xAxisTemp.axisLine.lineStyle.width = dataOption.xLineWidth == undefined ? 1 : dataOption.xLineWidth;
        xAxisTemp.axisLabel = {
            show: true,
            color: dataOption.xFontColor === undefined ? "#fff" : dataOption.xFontColor === null ? "transparent" : dataOption.xFontColor,
            fontSize: dataOption.xFontSize === undefined ? 14 : dataOption.xFontSize,
        }
        xAxisTemp.axisLine.show = dataOption.xLineShow
    
        xAxisTemp.nameTextStyle = {
          color: dataOption.xFontColor,
          fontSize: dataOption.xFontSize
        }

        //设置y轴样式
        yAxisTemp.axisTick = axisTick;
        yAxisTemp.axisLine.lineStyle.color = dataOption.yLineColor === undefined ? "#fff" : dataOption.yLineColor === null ? "transparent" : dataOption.yLineColor;
        yAxisTemp.axisLine.lineStyle.width = dataOption.yLineWidth == undefined ? 1 : dataOption.yLineWidth;
        //设置X轴标签样式
        yAxisTemp.axisLabel = {
          show: true,
          color: dataOption.yFontColor === undefined ? "#fff" : dataOption.yFontColor === null ? "transparent" : dataOption.yFontColor,
          fontSize: dataOption.yFontSize === undefined ? 14 : dataOption.yFontSize,
        }
        
        //设置坐标轴名称样式
        yAxisTemp.nameTextStyle = {
          color: dataOption.yFontColor,
          fontSize: dataOption.yFontSize
        }

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

        let dataZoom = {
          type: dataOption.dataZoomType, //  dataOption.dataZoomType,
          show: dataOption.dataZoomShow != true ? false : true,
          start: dataOption.dataZoomStart,
          end: dataOption.dataZoomEnd,
          handleSize: dataOption.handleSize,
          textStyle: {
            color: dataOption.dataZoomFontColor,
            fontSize: dataOption.dataZoomFontSize
          }
        };
        let dataZoomArr = [];
        if (dataOption.axis != undefined && dataOption.axis.length > 0) {
          dataOption.axis.forEach(element => {
            let data = JSON.parse(JSON.stringify(dataZoom))
            if (element == "x轴") {
              data.xAxisIndex = 0
              data.bottom = '0%'
            } else {
              data.yAxisIndex = 0
              data.right = '2%'
            }
            dataZoomArr.push(data)
          });
        }

        let option = {
          title: dataOption.title,
          tooltip: dataOption.tooltip,
          legend: legendData,
          grid: {
            top: "4%",
            left: "2%",
            right: "5%",
            bottom: "3%",
            containLabel: true
          },
          xAxis: xAxisTemp,
          yAxis: yAxisTemp,
          series: seriesData
        };
        if (dataOption.dataZoomShow) {
          option.dataZoom = dataZoomArr;
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
                [{ "name": "drillParam", "value": arrs }]
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
        // this.$message('柱状图报错'+error);
      }


    },

  }
};
</script>
