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
        dataOption.leftColorStart = color[0];
        dataOption.leftColorEnd = color[0];
        dataOption.rightColorStart = color[1];
        dataOption.rightColorEnd = color[1];
      }
      let xData = new Set();
      let legend = new Set();
      let valueData = {};
      for (let d in result) {
        //图例
        legend.add(result[d].name);
        xData.add(result[d].label);
      }
      xData = [...xData];
      legend = [...legend];
      for (let d in legend) {
        let valueArr = [];
        for (let v in result) {
          if (result[v].name === legend[d]) {
              valueArr.push(result[v].value);
            }
        }
        valueData[d] = valueArr;
      }
      let yData1 = valueData[0];
      let yData2 = valueData[1];
      let timeLineData = [1];
      
      let textColor = "#fff";
      let lineColor = "rgba(255,255,255,0.2)";
      let colors = [
        {
          borderColor: dataOption.leftColorStart,
          start: dataOption.leftColorStart,
          end: dataOption.leftColorEnd
        },
        {
          borderColor: dataOption.rightColorEnd,
          start: dataOption.rightColorStart,
          end: dataOption.rightColorEnd
        }
      ];
      let borderData = [];
      let scale = 2;
      borderData = xData.map(item => {
        return scale;
      });
      let option = {
        baseOption: {
          title: dataOption.title,
          tooltip: {
            trigger: "axis",
            axisPointer: {
              type: "none"
            },
            formatter: function(params) {
              return (
                params[0].name +
                "<br/>" +
                "<span style='display:inline-block;margin-right:5px;border-radius:10px;width:9px;height:9px;background-color:rgba(36,207,233,0.9)'></span>" +
                params[0].seriesName +
                " : " +
                params[0].value +
                "<br/>"
              );
            }
          },
          timeline: {
            show: false,
            top: 0,
            data: []
          },
          legend: {
            top: dataOption.top + "%",
            right: dataOption.right + "%",
            itemWidth: 20,
            itemHeight: 5,
            // itemGap: 343,
            icon: "horizontal",
            textStyle: {
              color: "#ffffff",
              fontSize: dataOption.legendFontSize
            },
            data: legend,
            show: dataOption.showLegend
          },
          grid: [
            {
              show: false,
              left: "5%",
              top: "11%",
              bottom: "8%",
              containLabel: true,
              width: "37%"
            },
            {
              show: false,
              left: "51%",
              top: dataOption.labelTop + "%",
              bottom: "8%",
              width: "0%"
            },
            {
              show: false,
              right: "2%",
              top: "11%",
              bottom: "8%",
              containLabel: true,
              width: "37%"
            }
          ],
          xAxis: [
            {
              type: "value",
              inverse: true,
              axisLine: {
                show: false
              },
              axisTick: {
                show: false
              },
              position: dataOption.position,
              axisLabel: {
                show: true,
                color: textColor
              },
              splitLine: {
                show: true,
                lineStyle: {
                  color: lineColor
                }
              },
              show: dataOption.showLabel
            },
            {
              gridIndex: 1,
              show: false
            },
            {
              gridIndex: 2,
              axisLine: {
                show: false
              },
              axisTick: {
                show: false
              },
              position: dataOption.position,
              axisLabel: {
                show: true,
                color: textColor
              },
              splitLine: {
                show: true,
                lineStyle: {
                  color: lineColor
                }
              },
              show: dataOption.showLabel
            }
          ],
          yAxis: [
            {
              type: "category",
              inverse: true,
              position: "right",
              axisLine: {
                show: true,
                lineStyle: {
                  color: lineColor
                }
              },

              axisTick: {
                show: false
              },
              axisLabel: {
                show: false
              },
              data: xData
            },
            {
              gridIndex: 1,
              type: "category",
              inverse: true,
              position: "left",
              axisLine: {
                show: false
              },
              axisTick: {
                show: false
              },
              axisLabel: {
                show: true,
                padding: [dataOption.letterSpacing, 0, 10, 30],
                textStyle: {
                  color: dataOption.fontColor,
                  fontSize: dataOption.fontSize
                },
                align: "center"
              },
              data: xData.map(function(value) {
                return {
                  value: value,
                  textStyle: {
                    align: "center"
                  }
                };
              })
            },
            {
              gridIndex: 2,
              type: "category",
              inverse: true,
              position: "left",
              axisLine: {
                show: true,
                lineStyle: {
                  color: lineColor
                }
              },
              axisTick: {
                show: false
              },
              axisLabel: {
                show: false
              },
              data: xData
            }
          ],
          series: []
        },
        options: []
      };

      option.baseOption.timeline.data.push(timeLineData[0]);
      option.options.push({
        series: [
          {
            name: legend[0],
            type: "bar",
            barWidth: dataOption.barWidth,
            stack: "1",
            itemStyle: {
              normal: {
                color: new echarts.graphic.LinearGradient(0, 0, 1, 0, [
                  {
                    offset: 0,
                    color: colors[0].start
                  },
                  {
                    offset: 1,
                    color: colors[0].end
                  }
                ])
              }
            },
            label: {
              normal: {
                show: false
              }
            },
            data: yData1,
            animationEasing: "elasticOut"
          },

          {
            name: legend[1],
            type: "bar",
            stack: "2",
            barWidth: dataOption.barWidth,
            xAxisIndex: 2,
            yAxisIndex: 2,
            itemStyle: {
              normal: {
                color: new echarts.graphic.LinearGradient(0, 0, 1, 0, [
                  {
                    offset: 0,
                    color: colors[1].start
                  },
                  {
                    offset: 1,
                    color: colors[1].end
                  }
                ])
              }
            },
            label: {
              normal: {
                show: false
              }
            },
            data: yData2,
            animationEasing: "elasticOut"
          }
        ]
      });
      
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
