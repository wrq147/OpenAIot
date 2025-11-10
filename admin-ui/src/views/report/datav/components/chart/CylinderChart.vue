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
        dataOption.color2 = color[0];
        dataOption.color3 = color[0];
        dataOption.color4 = color[0];
      }

      let data = result[0].value; //百分比
      if (dataOption.istitle == false) {
        dataOption.title.show = false;
      } else {
        dataOption.title.show = true;
      }

      let option = {
        title: dataOption.title,
        backgroundColor: "rgba(0,0,0,0)",
        tooltip: {
          trigger: "none"
        },
        xAxis: {
          data: ["百分比"],
          axisTick: {
            show: false
          },
          axisLine: {
            show: false
          },
          axisLabel: {
            show: false,
            textStyle: {
              color: "#e54035"
            }
          }
        },
        yAxis: {
          splitLine: {
            show: false
          },
          axisTick: {
            show: false
          },
          axisLine: {
            show: false
          },
          axisLabel: {
            show: false
          }
        },
        series: [
          {
            name: "最上层立体圆",
            type: "pictorialBar",
            symbolSize: [dataOption.width, 45], //上椭圆的宽高
            symbolOffset: [0, -20], //左右偏移和上下偏移
            z: 12, //图层
            itemStyle: {
              normal: {
                color: dataOption.color1 //颜色
              }
            },
            data: [
              {
                value: 100, //上椭圆在立体柱中的位置（百分比）
                symbolPosition: "end" //上述位置从底部开始计算
              }
            ]
          },
          {
            name: "中间立体圆",
            type: "pictorialBar",
            symbolSize: [dataOption.width, 45],
            symbolOffset: [0, -20],
            z: 13,
            itemStyle: {
              normal: {
                color: dataOption.color2
              }
            },

            data: [
              {
                value: data,
                symbolPosition: "end"
              }
            ]
          },
          {
            name: "最底部立体圆",
            type: "pictorialBar",
            symbolSize: [dataOption.width, 45],
            symbolOffset: [0, 20],
            z: 12,
            itemStyle: {
              normal: {
                color: dataOption.color3
              }
            },
            data: [100 - data]
          },
          {
            //底部立体柱
            name: result[0].name,
            stack: "1",
            type: "bar",
            itemStyle: {
              normal: {
                color: dataOption.color4, //颜色
                opacity: 0.7
              }
            },
            z: 11,
            label: {
              show: true,
              position: "top",

              distance: 15, //文字距离下柱的位置
              color: dataOption.color5, //颜色
              fontSize: dataOption.size, //50,
              fontFamily: dataOption.family,
              fontWeight: dataOption.weight,
              formatter: "{c}" + dataOption.suffix
            },
            silent: true,
            barWidth: dataOption.width, //宽度
            barGap: "-100%", // Make series be overlap
            data: [data]
          },
          {
            //上部立体柱
            stack: "1",
            type: "bar",
            itemStyle: {
              normal: {
                color: dataOption.color6,
                opacity: 0.7
              }
            },
            z: 5,
            silent: true,
            barWidth: dataOption.width,
            barGap: "-100%", // Make series be overlap
            data: [100 - data]
          }
        ]
      };
     
      this.chart.setOption(option, true);

      addOption(dataOption.bindingDiv, option);
      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.getZr().off("click");
        this.chart.getZr().on("click", params => {
          let point = [params.offsetX, params.offsetY];
          if (this.chart.containPixel("grid", point)) {
            // 使用 convertFromPixel方法 转换像素坐标值到逻辑坐标系上的点。获取点击位置对应的x轴数据的索引值，借助于索引值的获取到其它的信息
            let pointInGrid = this.chart.convertFromPixel(
              { seriesIndex: 1 },
              point
            );
            // x轴数据的索引值
            let xIndex = pointInGrid[0];

            //设置参数
            let arrObject = {
              seriesName: result[xIndex].name,
              data: result[xIndex].value
            };

            let arrs = JSON.stringify(arrObject);

            //获取绑定的图表
            let bindList = this.chartOption.bindList;

            if (bindList.length > 0) {
              getLinkChart(dataOption.arrName, arrs, bindList, this.drawingList);
            }
          }
        });
      }
      //开启图表下钻
      else if (dataOption.isDrillDown == true) {
        this.chart.getZr().off("click");
        this.chart.getZr().on("click", params => {
          let point = [params.offsetX, params.offsetY];
          if (this.chart.containPixel("grid", point)) {
            // 使用 convertFromPixel方法 转换像素坐标值到逻辑坐标系上的点。获取点击位置对应的x轴数据的索引值，借助于索引值的获取到其它的信息
            let pointInGrid = this.chart.convertFromPixel(
              { seriesIndex: 1 },
              point
            );
            // x轴数据的索引值
            let xIndex = pointInGrid[0];

            //设置参数
            let arrObject = {
              seriesName: result[xIndex].name,
              data: result[xIndex].value
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
          }
        });
      } else {
        //关闭图表联动，取消echart点击事件
        this.chart.getZr().off("click");
      }


    },
    
  }
};
</script>
