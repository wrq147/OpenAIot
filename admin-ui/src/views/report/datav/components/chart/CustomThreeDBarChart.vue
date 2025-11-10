<template>
  <div :class="animate" :style="{ height: height, width: width }" :id="chartOption.bindingDiv" ref="chartDiv"></div>
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
import { forEach } from 'jszip';

const CubeLeft = echarts.graphic.extendShape({
  shape: {},
  buildPath: function (ctx, shape) {
    // const yAxisPoint = shape.yAxisPoint
    // const c0 = [shape.x, shape.y]
    // const c1 = [shape.x + threeDBarWidth, shape.y - threeDBarHeight]
    // const c2 = [yAxisPoint[0] + threeDBarWidth, yAxisPoint[1] - threeDBarHeight]
    // const c3 = [yAxisPoint[0], yAxisPoint[1]]
    ctx
      .moveTo(shape.c1[0], shape.c1[1])
      .lineTo(shape.c2[0], shape.c2[1])
      .lineTo(shape.c3[0], shape.c3[1])
      .lineTo(shape.c4[0], shape.c4[1])
      .closePath();
  }
});
const CubeRight = echarts.graphic.extendShape({
  shape: {},
  buildPath: function (ctx, shape) {
    // const yAxisPoint = shape.yAxisPoint
    // const c1 = [shape.x, shape.y]
    // const c2 = [yAxisPoint[0], yAxisPoint[1]]
    // const c3 = [yAxisPoint[0], yAxisPoint[1] + threeDBarHeight]
    // const c4 = [shape.x , shape.y + threeDBarHeight]
    ctx
      .moveTo(shape.c1[0], shape.c1[1])
      .lineTo(shape.c2[0], shape.c2[1])
      .lineTo(shape.c3[0], shape.c3[1])
      .lineTo(shape.c4[0], shape.c4[1])
      .closePath();
  }
});
const CubeTop = echarts.graphic.extendShape({
  shape: {},
  buildPath: function (ctx, shape) {
    // const c1 = [shape.x, shape.y]
    // const c2 = [shape.x, shape.y + threeDBarHeight]
    // const c3 = [shape.x + threeDBarWidth, shape.y]
    // const c4 = [shape.x + threeDBarWidth, shape.y - threeDBarHeight]
    ctx
      .moveTo(shape.c1[0], shape.c1[1])
      .lineTo(shape.c2[0], shape.c2[1])
      .lineTo(shape.c3[0], shape.c3[1])
      .lineTo(shape.c4[0], shape.c4[1])
      .closePath();
  }
});
echarts.graphic.registerShape("CubeLeft", CubeLeft);
echarts.graphic.registerShape("CubeRight", CubeRight);
echarts.graphic.registerShape("CubeTop", CubeTop);

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
      if (this.chart == null) {
        echarts.registerTheme("customTheme", this.chartOption.theme);
        this.chart = echarts.init(
          document.getElementById(this.chartOption.bindingDiv),
          "customTheme"
        );
      }
      let dataOption = JSON.parse(JSON.stringify(this.dataOption))
      this.createBar(dataOption, result);
    },
    createBar(dataOption, result) {
      //画图方法
      //坐标轴的值
      console.log(result, '=========result')
      let xdata = []
      result.forEach((item, index) => {
        xdata.push(item.name);
      });
      dataOption.yAxis.data = xdata;
     
      let maxTest = dataOption.max == undefined ? 100 : dataOption.max;

      let maxData = new Array(xdata.length).fill(
        maxTest
      );


      //显示标签的柱图的值
      dataOption.series[2].data = result[0].value;
      let labelFormatter = e => {
        return `${e.value}` + dataOption.labelText;
      };
      dataOption.series[2].label.formatter = labelFormatter;
      console.log(44444444444444444,dataOption.showAll)
      //构建占比阴影部分
      if (dataOption.showAll) {
        let shadowRenderItem = (params, api) => {
          const location = api.coord([api.value(0), api.value(1)]);
          const yAxisPoint = api.coord([0, api.value(1)]);
          return {
            type: "group",
            children: [
              {
                type: "CubeLeft",
                shape: {
                  api,
                  c1: [location[0], yAxisPoint[1]],
                  c2: [
                    location[0] + dataOption.threeDBarWidth,
                    yAxisPoint[1] - dataOption.threeDBarHeight
                  ],
                  c3: [
                    yAxisPoint[0] + dataOption.threeDBarWidth,
                    yAxisPoint[1] - dataOption.threeDBarHeight
                  ],
                  c4: [yAxisPoint[0], yAxisPoint[1]]
                },
                style: {
                  fill: dataOption.shadowColor
                },
                silent: true
              },
              {
                type: "CubeRight",
                shape: {
                  api,
                  c1: [location[0], yAxisPoint[1]],
                  c2: [yAxisPoint[0], yAxisPoint[1]],
                  c3: [
                    yAxisPoint[0],
                    yAxisPoint[1] + dataOption.threeDBarHeight
                  ],
                  c4: [location[0], yAxisPoint[1] + dataOption.threeDBarHeight]
                },
                style: {
                  fill: dataOption.shadowColor
                },
                silent: true
              },
              {
                type: "CubeTop",
                shape: {
                  api,
                  c1: [location[0], yAxisPoint[1]],
                  c2: [
                    location[0] + dataOption.threeDBarWidth,
                    yAxisPoint[1] - dataOption.threeDBarHeight
                  ],
                  c3: [location[0] + dataOption.threeDBarWidth, yAxisPoint[1]],
                  c4: [location[0], yAxisPoint[1] + dataOption.threeDBarHeight]
                },
                style: {
                  fill: dataOption.shadowColor
                },
                silent: true
              }
            ]
          };
        };
        dataOption.series[0].renderItem = shadowRenderItem();
        dataOption.series[0].data = maxData;
      }
      //构建实际数值显示部分
      const currentRenderItem = (params, api) => {
        console.log(3333333333333333333)
        const location = api.coord([api.value(0), 0]);
        const yAxisPoint = api.coord([0, api.value(1)]);
        console.log(location, yAxisPoint)
        return {
          type: "group",
          children: [
            {
              type: "CubeLeft",
              shape: {
                api,
                c1: [location[0], yAxisPoint[1]],
                c2: [
                  location[0] + dataOption.threeDBarWidth,
                  yAxisPoint[1] - dataOption.threeDBarHeight
                ],
                c3: [
                  yAxisPoint[0] + dataOption.threeDBarWidth,
                  yAxisPoint[1] - dataOption.threeDBarHeight
                ],
                c4: [yAxisPoint[0], yAxisPoint[1]]
              },
              style: {
                fill: new echarts.graphic.LinearGradient(1, 0, 0, 0, [
                  {
                    offset: 0,
                    color: dataOption.processColor[1]
                  },
                  {
                    offset: 1,
                    color: dataOption.processColor[0]
                  }
                ])
              }
            },
            {
              type: "CubeRight",
              shape: {
                api,
                c1: [location[0], yAxisPoint[1]],
                c2: [yAxisPoint[0], yAxisPoint[1]],
                c3: [yAxisPoint[0], yAxisPoint[1] + dataOption.threeDBarHeight],
                c4: [location[0], yAxisPoint[1] + dataOption.threeDBarHeight]
              },
              style: {
                fill: new echarts.graphic.LinearGradient(1, 0, 0, 0, [
                  {
                    offset: 0,
                    color: dataOption.processColor[1]
                  },
                  {
                    offset: 1,
                    color: dataOption.processColor[0]
                  }
                ])
              }
            },
            {
              type: "CubeTop",
              shape: {
                api,
                c1: [location[0], yAxisPoint[1]],
                c2: [
                  location[0] + dataOption.threeDBarWidth,
                  yAxisPoint[1] - dataOption.threeDBarHeight
                ],
                c3: [location[0] + dataOption.threeDBarWidth, yAxisPoint[1]],
                c4: [location[0], yAxisPoint[1] + dataOption.threeDBarHeight]
              },
              style: {
                fill: new echarts.graphic.LinearGradient(1, 0, 0, 0, [
                  {
                    offset: 0,
                    color: dataOption.processColor[1]
                  },
                  {
                    offset: 1,
                    color: dataOption.processColor[1]
                  }
                ])
              }
            }
          ]
        };
      };
      dataOption.series[1].data = result[0].value;
      dataOption.series[1].renderItem = currentRenderItem();
      console.log(series, '==========series')
      let grid = {
        left: dataOption.gridLeft + "%",
        right: "10%",
        bottom: "12%",
        top: "15%",
        containLabel: true
      };

      let option = {
        title: dataOption.title,
        tooltip: dataOption.tooltip,
        legend: dataOption.legend,
        grid: grid,
        series: dataOption.series,
        xAxis: dataOption.xAxis,
        yAxis: dataOption.yAxis
      };

      this.chart.setOption(option, true);
      //将setting存入map集合
      var setting = {
        bindingType: "CustomThreeDBar",
        option: option,
        // stack: stack,
        // info: info,
        showAll: dataOption.showAll,
        shadowColor: dataOption.shadowColor,
        color: dataOption.processColor,
        param1: dataOption.threeDBarWidth,
        param2: dataOption.threeDBarHeight
      };
      addOption(dataOption.bindingDiv, setting);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.on("click", params => {
          this.chart.off("click");
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

    },

  }
};
</script>
