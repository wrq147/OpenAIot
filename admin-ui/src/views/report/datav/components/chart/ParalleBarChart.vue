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
      let dataOption = JSON.parse(JSON.stringify(this.dataOption))


      //*******************************************使用主题******************************************************/
      let color = [];
      if (dataOption.isThemeColor) {
        color = this.chartOption.theme.color;
        dataOption.linearColor0 = color[0];
        dataOption.linearColor1 = color[1];
      }

      let category = result; // 类别
      var total = dataOption.total; // 数据总数
      var datas = [];
      let totalArry = [];
      category.forEach(value => {
        datas.push(value.value);
        totalArry.push(dataOption.total);
      });

      let xAxis = dataOption.xAxis;
      xAxis.max = total;
      let option = {
        title: dataOption.title,
        tooltip: {
          trigger: "axis",
          axisPointer: {
            type: "shadow"
            //     shadowStyle: {
            //         color: 'transparent'
            // }
          },
          formatter(params) {
            for (let x in params) {
              return params[x].value;
            }
          }
        },
        xAxis: xAxis,
        grid: dataOption.grid,
        yAxis: [
          {
            type: "category",
            inverse: false,
            data: category,
            axisLine: {
              show: false
            },
            axisTick: {
              show: false
            },
            axisLabel: {
              show: false
            }
          }
        ],
        series: [
          {
            // 内
            type: "bar",
            barWidth: 18,

            legendHoverLink: false,
            silent: true,
            itemStyle: {
              normal: {
                color: function(params) {
                  return {
                    type: "linear",
                    x: 0,
                    y: 0,
                    x2: 1,
                    y2: 0,
                    colorStops: [
                      {
                        offset: 0,
                        color: dataOption.linearColor0 // 0% 处的颜色
                      },
                      {
                        offset: 1,
                        color: dataOption.linearColor1 // 100% 处的颜色
                      }
                    ]
                  };
                }
              }
            },
            label: {
              normal: {
                show: true,
                position: "left",
                formatter: "{b}",
                textStyle: {
                  color: dataOption.fontColor,
                  fontSize: dataOption.fontSize,
                  fontFamily: dataOption.fontFamily
                }
              }
            },
            data: category,
            z: 1,
            animationEasing: "elasticOut"
          },
          {
            // 分隔
            type: "pictorialBar",
            itemStyle: {
              normal: {
                color: dataOption.symbolColor
              }
            },
            symbolRepeat: "fixed",
            symbolMargin: 6,
            symbol: "rect",
            symbolClip: true,
            symbolSize: [1, 21],
            symbolPosition: "start",
            symbolOffset: [1, -1],
            symbolBoundingData: this.total,
            data: category,
            z: 2,
            animationEasing: "elasticOut"
          },
          {
            // 外边框
            type: "pictorialBar",
            symbol: "rect",
            symbolBoundingData: total,
            itemStyle: {
              normal: {
                color: "none"
              }
            },
            label: {
              normal: {
                formatter: params => {
                  var text;
                  text =
                    "{f| " + ((params.data * 100) / total).toFixed(2) + "%}";
                  return text;
                },
                rich: {
                  f: {
                    color: dataOption.fontColor,
                    fontSize: dataOption.fontSize,
                    fontFamily: dataOption.fontFamily
                  }
                },
                position: "right",
                distance: 0, // 向右偏移位置
                show: true
              }
            },
            data: datas,
            z: 0,
            animationEasing: "elasticOut"
          },
          {
            name: "外框",
            type: "bar",
            barGap: "-120%", // 设置外框粗细
            data: totalArry,
            barWidth: 25,
            itemStyle: {
              normal: {
                color: "transparent", // 填充色
                barBorderColor: dataOption.borderColor, // 边框色
                barBorderWidth: 1, // 边框宽度
                barBorderRadius: 1, //圆角半径
                label: {
                  // 标签显示位置
                  show: false,
                  position: "top" // insideTop 或者横向的 insideLeft
                }
              }
            },
            z: 0
          }
        ]
      };
     
      this.chart.setOption(option, true);

      //开启图表联动
      if (dataOption.isLink == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            seriesName: result[params.dataIndex].name,
            data: result[params.dataIndex].value
          };

          let arrs = JSON.stringify(arrObject);

          //获取绑定的图表
          let bindList = this.chartOption.bindList;

          if (bindList.length > 0) {
            getLinkChart(dataOption.arrName, arrs, bindList, this.drawingList);
          }
        });

        // this.chart.getZr().off('click');
        // this.chart.getZr().on('click',(params) => {
        //   let point=[params.offsetX,params.offsetY];
        //   if(this.chart.containPixel('grid',point)){
        //       // 使用 convertFromPixel方法 转换像素坐标值到逻辑坐标系上的点。获取点击位置对应的x轴数据的索引值，借助于索引值的获取到其它的信息
        //       let pointInGrid = this.chart.convertFromPixel({ seriesIndex: 1 }, point)
        //       // x轴数据的索引值
        //       let xIndex = pointInGrid[1]

        //       //设置参数
        //       let arrObject = {
        //         "seriesName":dataOption.staticDataValue[xIndex].name,
        //         "data":dataOption.staticDataValue[xIndex].value
        //       }

        //       let arrs = JSON.stringify(arrObject);

        //       //获取绑定的图表
        //       let bindList = this.chartOption.bindList;

        //       if(bindList.length > 0){

        //         linkChart(dataOption.arrName, arrs, bindList,this.drawingList)

        //       }

        //   }

        // })
      }
      //开启图表下钻
      else if (dataOption.isDrillDown == true) {
        this.chart.off("click");
        this.chart.on("click", params => {
          //设置参数
          let arrObject = {
            seriesName: result[params.dataIndex].name,
            data: result[params.dataIndex].value
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
